using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TalkManager : MonoBehaviour
{

    //싱클톤
    private static TalkManager instance = null;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static TalkManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }
    private GameObject entity;
    private GameObject[] characterImage = new GameObject[4];
    private GameObject characterName;
    private GameObject characterTalk;
    private GameObject[] talkImage = new GameObject[2];

    private List<TalkReader> talkList = new List<TalkReader>();
    private Material[] material = new Material[4];
    private int curIdx = 0;
    private int initIdx = -1;
    private int curLight = 0;
    private bool talkingChk = false;

    private List<int> listIdx = new List<int>();
    private int[] lightingArr = new int[4];
    private int[] preLightingArr = new int[4];
    string [] tempCharacter = new string[4];

    private float[] characterMoveVal = { 0.0f , 0.0f,0.0f,0.0f};
    private Vector3[] pointArr = new Vector3[4];
    private string[] preNameArr = { "", "", "", "" };
    private string[] nameArr = { "", "", "", "" };
    private string[] faceArr = {"","","",""};

    private bool libraryEntry = false;
    private void setPoint(TalkReader talkReader){
        pointArr[0] = new Vector3(talkReader.characterLeftestX, 0, 0);
        pointArr[1] = new Vector3(talkReader.characterLeftX, 0, 0);
        pointArr[2] = new Vector3(talkReader.characterRightX, 0, 0);
        pointArr[3] = new Vector3(talkReader.characterRightestX, 0, 0);
    }
    private void setCharacterName(TalkReader talkReader) {
        nameArr[0] = talkReader.characterLeftest;
        nameArr[1] = talkReader.characterLeft;
        nameArr[2] = talkReader.characterRight;
        nameArr[3] = talkReader.characterRightest;
    }
    private void setPreCharacterName()
    {
        for(int i=0;i<4; i++) preNameArr[i] = nameArr[i];
    }
    private void setCharacterFace(TalkReader talkReader)
    {
        faceArr[0] = talkReader.characterLeftestFace;
        faceArr[1] = talkReader.characterLeftFace;
        faceArr[2] = talkReader.characterRightFace;
        faceArr[3] = talkReader.characterRightestFace;
    }
    // Start is called before the first frame update
    void Start()
    {
        
        libraryEntry = false;
        talkList = CSVReader.Read<TalkReader>("Talk_2");
        
        initIdx = -1;
        
        for (int i = 0; i < talkList.Count; i++)
        {
            if(talkList[i].talkIdx != initIdx)
            {
                initIdx = talkList[i].talkIdx;
                listIdx.Add(i);
            }
        }

        for (int i=0;i<lightingArr.Length;i++){ lightingArr[i] = 0; preLightingArr[i] = 0; characterMoveVal[i] = 0.0f; }
        entity = GameObject.Find("ui_communicate");

        for (int i = 0; i < 4; i++)
        {
            characterImage[i] = GameObject.Find("ui_communicate_character_" + i.ToString());
            material[i] = characterImage[i].GetComponent<Image>().material;
        }
        characterName = GameObject.Find("ui_communicate_name");
        characterTalk = GameObject.Find("ui_communicate_talk");
        talkImage[0] = GameObject.Find("ui_communicate_image_front");
        talkImage[1] = GameObject.Find("ui_communicate_image_back");

        talkImage[0].SetActive(false);
        talkImage[1].SetActive(false);
        entity.SetActive(false);
    }

    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (entity.activeSelf)
        {
            //투명도 조정
            for (int i = 0; i < characterImage.Length; i++)
            {
                if (lightingArr[i] == '1')
                {
                    if (material[i].GetFloat("_Transparency") > 0.0f) material[i].SetFloat("_Transparency", material[i].GetFloat("_Transparency") - 0.1f);
                    else material[i].SetFloat("_Transparency", 0.0f);
                }
                else
                {
                    if (material[i].GetFloat("_Transparency") < 0.7f) material[i].SetFloat("_Transparency", material[i].GetFloat("_Transparency") + 0.1f);
                    else material[i].SetFloat("_Transparency", 0.7f);
                }
            }
            //움직임 조정
            for (int i = 0; i < 4; i++)
            {
                if (characterImage[i].activeSelf && characterMoveVal[i] > 0.1f) {
                    characterMoveVal[i] -= 0.1f;
                    if (characterMoveVal[i] > 0.0f)
                    {
                        characterImage[i].GetComponent<RectTransform>().localPosition = Vector3.Lerp(characterImage[i].GetComponent<RectTransform>().localPosition, pointArr[i], 0.1f);
                    }
                }
            }
        }
    }
    public void startTalk(int a)
    {
        if (a == 1) {
            if (libraryEntry) return;
            else libraryEntry = true;
        }
        if (!talkingChk)
        {
            
            entity.SetActive(true);

            curIdx = listIdx[a];
            setCharacterName(talkList[a]);
            setPreCharacterName();

            talkingChk = true;
            for (int i = 0; i < lightingArr.Length; i++) { lightingArr[i] = 0; preLightingArr[i] = 0; }

            setPoint(talkList[curIdx]);
            for (int i = 0; i < 4; i++)
            {
                characterMoveVal[i] = 0.0f;
                if (characterImage[i].activeSelf) characterImage[i].GetComponent<RectTransform>().localPosition = pointArr[i];
            }
            printTalk(curIdx);
        }
    }
    public void goToNextTalk()
    {
        if (talkingChk)
        {
            if (talkList[curIdx].talkIdx != talkList[curIdx + 1].talkIdx)
            {
                stopTalk();
            }
            else
            {
                curIdx++;
                printTalk(curIdx);
            }
        }
    }
    public void printTalk(int a)
    {
        setCharacterName(talkList[a]);
        setCharacterFace(talkList[a]);
        setPoint(talkList[a]);
        for (int i = 0; i < 4; i++)
        {
            if (nameArr[i] == preNameArr[i]) characterMoveVal[i] = 10.0f;
            else
            {
                characterImage[i].GetComponent<RectTransform>().localPosition = pointArr[i];
                characterMoveVal[i] = 0.0f;
            }
        }
        //이미지 사용시 체크
        if (talkList[a].imagePlace == 0) { talkImage[0].SetActive(false); talkImage[1].SetActive(false); }
        else if (talkList[a].imagePlace == 1) {
            talkImage[0].SetActive(true);
            talkImage[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/talkImage/spr_talkImage_" + talkList[a].imageIdx.ToString());
        }
        else if (talkList[a].imagePlace == 2) {
            talkImage[0].SetActive(false); talkImage[1].SetActive(true);
            talkImage[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/talkImage/spr_talkImage_" + talkList[a].imageIdx.ToString());
        }

        //캐릭터 밝기 조정
        for (int i = 0; i < lightingArr.Length; i++) {
            preLightingArr[i] = lightingArr[i];
            if (nameArr[i] != preNameArr[i]) material[i].SetFloat("_Transparency", 0.7f);
            lightingArr[i] = talkList[a].brightCharacter[i + 1];
        }

        //캐릭터 스프라이트 업데이트
        for (int i = 0; i < lightingArr.Length; i++)
        {
            Debug.Log(("sprite/TestSprite/CharacterTalkStand/spr_stand_" + nameArr[i] + "_" + faceArr[i]));
            characterImage[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterTalkStand/spr_stand_" + nameArr[i] + "_" + faceArr[i]);
        }
        characterName.GetComponent<TextMeshProUGUI>().text = talkList[a].Name;
        characterTalk.GetComponent<TextMeshProUGUI>().text = talkList[a].Text;
        setPreCharacterName();
    }
    private void stopTalk()
    {
        if (talkingChk)
        {
            for (int i = 0; i < characterImage.Length; i++) {
                characterImage[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/spr_characterEmpty");
                material[i].SetFloat("_Transparency", 0.7f);
            }

            characterName.GetComponent<TextMeshProUGUI>().text = "";
            characterTalk.GetComponent<TextMeshProUGUI>().text = "";
            entity.SetActive(false);
            talkingChk = false;
        }
    }
}
