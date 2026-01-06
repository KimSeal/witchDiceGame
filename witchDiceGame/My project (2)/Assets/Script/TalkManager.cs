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
    [SerializeField] private GameObject entity;//ui_communicate
    [SerializeField] private GameObject[] characterImage = new GameObject[4]; //ui_communicate_character_(number)
    [SerializeField] private GameObject characterName; //ui_communicate_name
    [SerializeField] private GameObject characterTalk;//ui_communicate_talk
    [SerializeField] private GameObject characterTalkBack; 
    [SerializeField] private GameObject[] talkImage = new GameObject[2]; //ui_communicate_image_front/back
    [SerializeField] private GameObject background;

    [SerializeField] public GameObject talkClickButton;
    [SerializeField] public GameObject talkClickButtonOriginal;
    private List<TalkReader> talkList = new List<TalkReader>();
    private List<DescReader> descList = new List<DescReader>();
    private Material[] material = new Material[4];
    private int curIdx = 0;
    private int initIdx = -1;
    //private int curLight = 0;

    private bool talkingChk = false;
    private bool descChk = false;
    private string descString = "";

    private bool descClickLock = false;

    private List<int> listIdx = new List<int>();
    private int[] lightingArr = new int[4];
    private int[] preLightingArr = new int[4];
    string [] tempCharacter = new string[4];

    private float[] characterMoveVal = { 0.0f , 0.0f,0.0f,0.0f};
    private Vector3[] pointArr = new Vector3[4];
    private string[] preNameArr = { "", "", "", "" };
    private string[] nameArr = { "", "", "", "" };
    private string[] faceArr = {"","","",""};
    private string preBackground = "";

    private int[] jumpChk = { 0, 0, 0, 0 };
    private float[] jumpSpd = { 0, 0, 0, 0 };

    private bool titleScreen = true;
    

    private bool libraryEntry = false;

    [SerializeField] public GameObject skipButton;

    public void setDescClickLock(bool hello)
    {
        descClickLock = hello;
    }

    public void setDescString(string str) {

        if (str == "")
        {
            descString = "";
            changeTalkState(1, false);
        }
        else
        {
            if (descClickLock)
            {
                characterTalkBack.GetComponent<Image>().color = new Color(48f, 38f, 38f);
                characterTalk.GetComponent<TextMeshProUGUI>().color = new Color(255f, 255f, 255f);
            }
            else {
                characterTalkBack.GetComponent<Image>().color = new Color(255f, 255f, 255f);
                characterTalk.GetComponent<TextMeshProUGUI>().color = new Color(0f, 0f, 0f);
            }
            descString = str;
            changeTalkState(1, true);
        }
    }

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
    public string SpecialTextChange(string inputStr)
    {
        return inputStr.Replace("\\n", "\n").Replace("+o", ",").Replace("？", "?")/*.Replace("。", ".").Replace("、", ", ")*/.Replace("！", "!").Replace("）",")").Replace("（", "(");
    }

    private int[] lifeStartIdx = {3,  -99999};
    public bool stageStart(int stageStart)
    {
        //스테이지가 0이 아니면서(다시 튜토리얼 시도 할때 대사 보여줘야 하니까) 기존에 방문했던 stage가 아니면
        if (stageStart != 0 && (lifeStartIdx[stageStart] == -99999 || jsonDataManager.Instance.getStageWatched(stageStart))) {
            return false;
        }
        else
        {
            startTalk(lifeStartIdx[stageStart]);
            return true;
        }
        
    }
    public void titleClick()
    {
        if(titleScreen) TalkManager.Instance.setDescString(TalkManager.Instance.getDesc(39));
    }

    public void clickDescBox()
    {
        Debug.Log("click talk Box!");
        
        if (talkingChk)
        {   
            goToNextTalk();
        }
        else if (titleScreen) {
            AdventureManager.Instance.clickPlay();
            titleScreen = false;
            setDescString("");
        }
        else
        {
            if (!descClickLock)
            {
                AdventureManager.Instance.clickDice(-1);
            }
        }
    }

        // Start is called before the first frame update
        void Start()
        {
            characterTalkBack.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -1100f, 0f);
            libraryEntry = false;
            talkList = CSVReader.Read<TalkReader>("Talk_2");
            for (int i = 0; i < talkList.Count; i++)
            {
                talkList[i].TextKR = SpecialTextChange(talkList[i].TextKR);
                talkList[i].TextEN = SpecialTextChange(talkList[i].TextEN);
                talkList[i].TextJP = SpecialTextChange(talkList[i].TextJP);
            }
            descList = CSVReader.Read<DescReader>("Desc");
            for (int i = 0; i < descList.Count; i++)
            {
                descList[i].KR = SpecialTextChange(descList[i].KR);
                descList[i].EN = SpecialTextChange(descList[i].EN);
                descList[i].JP = SpecialTextChange(descList[i].JP);
            }

            initIdx = -1;

            for (int i = 0; i < talkList.Count; i++)
            {
                if (talkList[i].talkIdx != initIdx)
                {
                    initIdx = talkList[i].talkIdx;
                    listIdx.Add(i);
                }
            }

            for (int i = 0; i < lightingArr.Length; i++) { lightingArr[i] = 0; preLightingArr[i] = 0; characterMoveVal[i] = 0.0f; }

            for (int i = 0; i < 4; i++) material[i] = characterImage[i].GetComponent<Image>().material;

            talkImage[0].SetActive(false);
            talkImage[1].SetActive(false);
            entity.SetActive(false);

            titleScreen = true;
            
        }

        bool jumpFlag = false;
        // Update is called once per frame
        void FixedUpdate()
        {

            talkClickButton.GetComponent<Image>().sprite = talkClickButtonOriginal.GetComponent<SpriteRenderer>().sprite;
            float yDefault = -20f;
            if (entity.activeSelf)
            {
                //투명도 조정
                for (int i = 0; i < characterImage.Length; i++)
                {
                    if (lightingArr[i] != '0')
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
                jumpFlag = false;
                //움직임 조정
                for (int i = 0; i < 4; i++)
                {
                    if (characterImage[i].activeSelf && characterMoveVal[i] > 0.1f)
                    {
                        characterMoveVal[i] -= 0.1f;
                        if (characterMoveVal[i] > 0.0f)
                        {
                            characterImage[i].GetComponent<RectTransform>().localPosition = new Vector3(Vector3.Lerp(characterImage[i].GetComponent<RectTransform>().localPosition, pointArr[i], 0.1f).x,
                                characterImage[i].GetComponent<RectTransform>().localPosition.y, characterImage[i].GetComponent<RectTransform>().localPosition.z);
                        }
                    }
                    if (jumpChk[i] > 0)
                    {
                        if (jumpChk[i] % 2 == 0)
                        {
                            jumpSpd[i] = 4;
                            jumpChk[i]--;

                            if (!jumpFlag) { SoundManager_Sfx.Instance.playSound(19); jumpFlag = true; }

                        }

                        characterImage[i].GetComponent<RectTransform>().localPosition += new Vector3(0, jumpSpd[i], 0);
                        jumpSpd[i] -= 0.5f;
                        if (characterImage[i].GetComponent<RectTransform>().localPosition.y < yDefault)
                        {
                            characterImage[i].GetComponent<RectTransform>().localPosition = new Vector3(characterImage[i].GetComponent<RectTransform>().localPosition.x, yDefault, characterImage[i].GetComponent<RectTransform>().localPosition.z);
                            jumpChk[i]--;
                            jumpSpd[i] = 0;
                        }
                    }
                    else characterImage[i].GetComponent<RectTransform>().localPosition = new Vector3(characterImage[i].GetComponent<RectTransform>().localPosition.x, yDefault, characterImage[i].GetComponent<RectTransform>().localPosition.z);
                }
            }
        }
    

    private int preSound = 0;

    public string getDesc(int idx)
    {
        if(jsonDataManager.Instance.getLanguage() == 0) return descList[idx].KR;
        if (jsonDataManager.Instance.getLanguage() == 1) return descList[idx].EN;
        if (jsonDataManager.Instance.getLanguage() == 2) return descList[idx].JP;
        return descList[idx].EN;
    }

    public void changeTalkState(int opt, bool onOff)
    {
        if (opt == 0) {
            talkingChk = onOff;
        }
        if (opt == 1) {
            descChk = onOff;
        }

        if (talkingChk) { //대화 필요
            

            characterTalkBack.GetComponent<Image>().color = new Color(255f, 255f, 255f);
            characterTalk.GetComponent<TextMeshProUGUI>().color = new Color(0f, 0f, 0f);
            characterTalkBack.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -550f, 0f);
        }
        else if(descChk) // 설명만
        {
            if (descClickLock)
            {
                characterTalkBack.GetComponent<Image>().color = new Color(48f/255f, 38f/255f, 38f/255f);
                characterTalk.GetComponent<TextMeshProUGUI>().color = new Color(255f, 255f, 255f);
            }
            else
            {
                characterTalkBack.GetComponent<Image>().color = new Color(255f, 255f, 255f);
                characterTalk.GetComponent<TextMeshProUGUI>().color = new Color(0f, 0f, 0f);
            }
            characterTalkBack.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -550f, 0f);
            characterTalk.GetComponent<TextMeshProUGUI>().text = descString;
        }
        else //text 필요 없음. 
        {
            characterTalkBack.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -1100f, 0f);
        }
    }
    public void startTalk(int a)
    {

        if (!talkingChk)
        {
            characterTalkBack.GetComponent<Image>().color = new Color(255f, 255f, 255f);
            characterTalk.GetComponent<TextMeshProUGUI>().color = new Color(0f, 0f, 0f);

            entity.SetActive(true);

            curIdx = listIdx[a];
            setCharacterName(talkList[a]);
            setPreCharacterName();
            changeTalkState(0, true);


            for (int i = 0; i < lightingArr.Length; i++) { lightingArr[i] = 0; preLightingArr[i] = 0; }

            setPoint(talkList[curIdx]);
            for (int i = 0; i < 4; i++)
            {
                characterMoveVal[i] = 0.0f;
                if (characterImage[i].activeSelf) characterImage[i].GetComponent<RectTransform>().localPosition = pointArr[i];
            }

            preSound = talkList[curIdx].BackSnd;
            if (preSound >= 0) SoundManager_Main.Instance.playSound(preSound);
            preBackground = talkList[a].backGround;
            printTalk(curIdx);

            
            if (talkList[curIdx].skipAble == 1)
            {
                skipButton.SetActive(true);
            }
            else { skipButton.SetActive(false); }
            
        }
    }
    public void goToNextTalk()
    {
        if (talkingChk)
        {
            Debug.Log("cur idx : " + curIdx.ToString());
            SoundManager_Sfx.Instance.playSound(0);
            if (talkList[curIdx].talkIdx != talkList[curIdx + 1].talkIdx)
            {
                Debug.Log("case 1");
                stopTalk();
            }
            else
            {
                Debug.Log("case 2");
                curIdx++;
                printTalk(curIdx);
            }
        }
    }
    public void goToPrevTalk()
    {
        if (talkingChk)
        {
            Debug.Log("cur idx : " + curIdx.ToString());
            SoundManager_Sfx.Instance.playSound(0);
            if (talkList[curIdx].talkIdx == talkList[curIdx - 1].talkIdx)
            {
                Debug.Log("case 2");
                curIdx--;
                printTalk(curIdx);
            }
        }
    }
    public void printTalk(int a)
    {
        if (talkList[a].SFX >= 0) SoundManager_Sfx.Instance.playSound(talkList[a].SFX);

        if (preSound != talkList[a].BackSnd) { //배경음 변경 타이밍

            if (preSound >= 0){
                SoundManager_Main.Instance.stopSound(preSound); //노래가 바뀌었으니 이전 노래 정지
            }
            preSound = talkList[a].BackSnd; // 노래 변경
            if (talkList[a].BackSnd >= 0) SoundManager_Main.Instance.playSound(talkList[a].BackSnd); //노래 틀어야 하는 경우 틀기.
        } // 확인
    
        

        if (talkList[a].eventType == 1) FadeUIScript.fadeIn();
        if (talkList[a].eventType == 2) CameraManager.Instance.VibrateForeTime(0.2f, 0.5f);

        /*
        if (a == listIdx[2] + 36)
        {
            SoundManager_Main.Instance.playSound(8);
        }
        if (a == listIdx[2] + 78)
        {
            SoundManager_Main.Instance.stopSound(8);
        }
        */
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

        Debug.Log("sprite/talkImage/spr_talkImage_" + talkList[a].imageIdx.ToString());
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

        for (int i = 0; i < lightingArr.Length; i++)    //점프 애니메이션 관련
        {
            if (jumpChk[i] == 0){
                if (talkList[a].brightCharacter[i + 1] == '2'){jumpChk[i] = 2;}
                if (talkList[a].brightCharacter[i + 1] == '3'){ jumpChk[i] = 4; }
            }
        }
        //캐릭터 스프라이트 업데이트
        for (int i = 0; i < lightingArr.Length; i++)
        {
            if (nameArr[i] == "People" || nameArr[i] == "Bard" || nameArr[i] == "Tom" || nameArr[i] == "Bob") characterImage[i].GetComponent<RectTransform>().sizeDelta = new Vector2(204f, 216f);
            else characterImage[i].GetComponent<RectTransform>().sizeDelta = new Vector2(102f, 216f);

            if (nameArr[i] == ".") characterImage[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterTalkStand/empty/spr_stand_" + nameArr[i] + "_" + faceArr[i]);
            else
            {
                if (Resources.Load<Sprite>("sprite/TestSprite/CharacterTalkStand/" + nameArr[i] + "/spr_stand_" + nameArr[i] + "_" + faceArr[i]) != null) {
                    characterImage[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterTalkStand/" + nameArr[i] + "/spr_stand_" + nameArr[i] + "_" + faceArr[i]);
                }
                else
                {
                    characterImage[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterTalkStand/" + nameArr[i] + "/spr_" + nameArr[i] + "_" + faceArr[i]);
                }
            }
        }

        //배경 이미지 업데이트
        background.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/backgroundImage/spr_background_" + talkList[a].backGround);

        //언어따라 다른 text
        characterName.GetComponent<TextMeshProUGUI>().text = talkList[a].Name;
        if(jsonDataManager.Instance.getLanguage() == 0) characterTalk.GetComponent<TextMeshProUGUI>().text = talkList[a].TextKR;
        else if (jsonDataManager.Instance.getLanguage() == 2) characterTalk.GetComponent<TextMeshProUGUI>().text = talkList[a].TextJP;
        else characterTalk.GetComponent<TextMeshProUGUI>().text = talkList[a].TextEN;

        Debug.Log(talkList[a].TextKR);
        preBackground = talkList[a].backGround;
        setPreCharacterName();
    }
    public void stopTalk()
    {
        if (talkingChk)
        {
            Debug.Log("case 123");
            changeTalkState(0, true);
            if (preSound >= 0) SoundManager_Main.Instance.stopSound(preSound);
            for (int i = 0; i < characterImage.Length; i++)
            {
                characterImage[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/spr_characterEmpty");
                material[i].SetFloat("_Transparency", 0.7f);
            }
            background.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/backgroundImage/spr_background_" + "empty");
            characterName.GetComponent<TextMeshProUGUI>().text = "";
            characterTalk.GetComponent<TextMeshProUGUI>().text = "";
            entity.SetActive(false);
            changeTalkState(0, false);
        }
    }
    public bool getTalkChk()
    {
        return talkingChk;
    }
}
