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
    private GameObject[] characterImage = new GameObject[2];
    private GameObject characterName;
    private GameObject characterTalk;
    private GameObject[] talkImage = new GameObject[2];

    private List<TalkReader> talkList = new List<TalkReader>();
    private Material[] material = new Material[2];
    private int curIdx = 0;
    private int initIdx = -1;
    private int curLight = 0;
    private bool talkingChk = false;

    private List<int> listIdx = new List<int>();
    private bool[] lightIngArr = new bool[2];
    string [] tempCharacter = new string[2];

    private bool libraryEntry = false;
    // Start is called before the first frame update
    void Start()
    {
        libraryEntry = false;
        talkList = CSVReader.Read<TalkReader>("Talk");
        
        initIdx = -1;
        
        for (int i = 0; i < talkList.Count; i++)
        {
            if(talkList[i].talkIdx != initIdx)
            {
                initIdx = talkList[i].talkIdx;
                listIdx.Add(i);
            }
        }

        for (int i=0;i<lightIngArr.Length;i++){ lightIngArr[i] = false; }
        entity = GameObject.Find("ui_communicate");
        characterImage[0] = GameObject.Find("ui_communicate_character_left");
        characterImage[1] = GameObject.Find("ui_communicate_character_right");
        characterName = GameObject.Find("ui_communicate_name");
        characterTalk = GameObject.Find("ui_communicate_talk");
        talkImage[0] = GameObject.Find("ui_communicate_image_front");
        talkImage[1] = GameObject.Find("ui_communicate_image_back");


        material[0] = characterImage[0].GetComponent<Image>().material;
        material[1] = characterImage[1].GetComponent<Image>().material;

        talkImage[0].SetActive(false);
        talkImage[1].SetActive(false);
        entity.SetActive(false);
    }

    
    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < tempCharacter.Length; i++)
        {
            if (lightIngArr[i])
            {
                if (material[i].GetFloat("_Transparency") > 0.0)
                {
                    material[i].SetFloat("_Transparency", material[i].GetFloat("_Transparency") - 0.01f);
                }
                else
                {
                    material[i].SetFloat("_Transparency", 0.0f);
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
            Debug.Log("talk what!" + listIdx[a]);
            curIdx = listIdx[a];
            talkingChk = true;
            tempCharacter[0] = talkList[curIdx].characterLeft;
            tempCharacter[1] = talkList[curIdx].characterRight;
            curLight = talkList[curIdx].brightCharacter;
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
        //이미지 사용시 체크
        if (talkList[a].imagePlace == 0){ talkImage[0].SetActive(false); talkImage[1].SetActive(false);}
        else if (talkList[a].imagePlace == 1){
            talkImage[0].SetActive(true);
            talkImage[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/talkImage/spr_talkImage_" + talkList[a].imageIdx.ToString());
        }
        else if (talkList[a].imagePlace == 2){
            talkImage[0].SetActive(false); talkImage[1].SetActive(true);
            talkImage[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/talkImage/spr_talkImage_" + talkList[a].imageIdx.ToString());
        }

        for (int i = 0; i < lightIngArr.Length; i++) { lightIngArr[i] = false; }

        characterImage[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterTalkStand/spr_stand_" + talkList[a].characterLeft + "_" + talkList[a].characterLeftFace);
        characterImage[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterTalkStand/spr_stand_" + talkList[a].characterRight + "_" + talkList[a].characterRightFace);

        if (talkList[a].brightCharacter == 0 || talkList[a].brightCharacter == 2) material[0].SetFloat("_Transparency", 0.7f);
        else
        {
            if (tempCharacter[0] != talkList[a].characterLeft
                ||(tempCharacter[0] == talkList[a].characterLeft && (curLight == 0 || curLight == 2))) //다른 캐릭터이거나, 같은 캐릭터지만 이전까지 검정이었던 경우
            {
                material[0].SetFloat("_Transparency", 0.7f);
                lightIngArr[0] = true;
            }
            else
            {
                material[0].SetFloat("_Transparency", 0.0f);
            }
        }

        if (talkList[a].brightCharacter == 0 || talkList[a].brightCharacter == 1) material[1].SetFloat("_Transparency", 0.7f);
        else
        {
            if (tempCharacter[1] != talkList[a].characterRight
                || (tempCharacter[1] == talkList[a].characterRight && (curLight == 0 || curLight == 1))) //다른 캐릭터이거나, 같은 캐릭터지만 이전까지 검정이었던 경우
            {
                material[1].SetFloat("_Transparency", 0.7f);
                lightIngArr[1] = true;
            }
            else
            {
                material[1].SetFloat("_Transparency", 0.0f);
            }
        }
        Debug.Log(talkList[a].Name);
        characterName.GetComponent<TextMeshProUGUI>().text = talkList[a].Name;
        characterTalk.GetComponent<TextMeshProUGUI>().text = talkList[a].Text;

        tempCharacter[0] = talkList[a].characterLeft;
        tempCharacter[1] = talkList[a].characterRight;
        curLight = talkList[a].brightCharacter;
    }
    private void stopTalk()
    {
        if (talkingChk)
        {
            characterImage[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/spr_characterEmpty");
            characterImage[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/spr_characterEmpty");

            material[0].SetFloat("_Transparency", 0.7f);
            material[1].SetFloat("_Transparency", 0.7f);

            characterName.GetComponent<TextMeshProUGUI>().text = "";
            characterTalk.GetComponent<TextMeshProUGUI>().text = "";
            entity.SetActive(false);
            talkingChk = false;
        }
    }
}
