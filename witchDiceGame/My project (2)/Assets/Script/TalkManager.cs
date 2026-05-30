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
    [SerializeField] public GameObject nameBackground;

    [SerializeField] public GameObject curTalkIdx;

    [SerializeField] public GameObject talkClickButton;
    [SerializeField] public GameObject talkClickButtonOriginal;

    [SerializeField] public GameObject wishlistButton;
    [SerializeField] public Sprite[] wishlistSprite = new Sprite[2];

    [SerializeField] public GameObject[] tutorialArrow = new GameObject[5];
    [SerializeField] public GameObject[] tutorialArrowOrigin = new GameObject[5];
    private List<TalkReader> talkList = new List<TalkReader>();
    private List<DescReader> descList = new List<DescReader>();
    private Material[] material = new Material[4];
    private int curIdx = 0;
    private int initIdx = -1;
    //private int curLight = 0;

    private string[] descLanArr = new string[3];
    private string[] talkLanArr = new string[3];


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

    private bool libraryEntry = false;

    public int MapperLock = 0;

    [SerializeField] public GameObject skipButton;
    [SerializeField] public GameObject skipButtonOutline;
    [SerializeField] public GameObject prevButtonOutline;

    [SerializeField] public GameObject fontSizeButton;
    [SerializeField] public GameObject fontSizeButtonOutline;
    public void setMapperLock(int opt)
    {
        MapperLock = opt;
    }
    public void changeLan()
    {
        if (talkingChk)
        {
            characterTalk.GetComponent<TextMeshProUGUI>().text = talkLanArr[jsonDataManager.Instance.getLanguage()];
        }
        else if (descChk)
        {
            characterTalk.GetComponent<TextMeshProUGUI>().text = descLanArr[jsonDataManager.Instance.getLanguage()];
        }
    }
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
            clickFontSize(false);
            curTalkIdx.GetComponent<TextMeshProUGUI>().text = "";
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

    public void setDescIdx(int descIdx) // -1  : cancle
    {
        if (descIdx < 0) {
            descLanArr[0] = "";
            descLanArr[1] = "";
            descLanArr[2] = "";
            setDescString("");
        }
        else
        {
            descLanArr[0] = descList[descIdx].KR;
            descLanArr[1] = descList[descIdx].EN;
            descLanArr[2] = descList[descIdx].JP;
            setDescString(getDesc(descIdx));
        }
        
    }
    public void setDescSelectText(adventureEvent curDiceEvent)
    {
        for (int i = 0; i < curDiceEvent.selectText.Length; i++)
        {
            descLanArr[i] = curDiceEvent.selectText[i];
        }
        setDescString(curDiceEvent.getSelectText());
    }
    public void setDescChooseText(adventureEvent_Packet curDiceEventPacket)
    {
        for (int i = 0; i < curDiceEventPacket.chooseText.Length; i++)
        {
            descLanArr[i] = curDiceEventPacket.chooseText[i];
        }
        setDescString(curDiceEventPacket.getChooseText());
    }
    public void setDescResultText(adventureEvent_Packet curDiceEventPacket)
    {
        for (int i = 0; i < curDiceEventPacket.resultText.Length; i++)
        {
            descLanArr[i] = curDiceEventPacket.resultText[i];
        }
        setDescString(curDiceEventPacket.getResultText());
    }

    private void setPoint(TalkReader talkReader){
        float yDefault = -20f;
        pointArr[0] = new Vector3(talkReader.characterLeftestX, yDefault, 0);
        pointArr[1] = new Vector3(talkReader.characterLeftX, yDefault, 0);
        pointArr[2] = new Vector3(talkReader.characterRightX, yDefault, 0);
        pointArr[3] = new Vector3(talkReader.characterRightestX, yDefault, 0);
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

    private int[] lifeStartIdx = {3,  -99999, -99999, 69, -99999, 
        -99999, -99999, -99999, -99999, -99999, 
        -99999, -99999, -99999, -99999, -99999, -99999, -99999, -99999, -99999 };

    public bool loseChk = false;

    public void setLostChk(bool onOff) { 
        loseChk = onOff;
    }
    public bool stageStart(int stageStart)
    {
        if(stageStart == 0)
        {
            startTalk(lifeStartIdx[stageStart]);
            return true;
        }
        else if (jsonDataManager.Instance.getStageWatched(stageStart) || lifeStartIdx[stageStart] < 0)
        {
            return false;
        }
        else
        {
            startTalk(lifeStartIdx[stageStart]);
            jsonDataManager.Instance.setStageWatched(stageStart);
            return true;
        }

        return false;
        
    }
    public void clickDescBox()
    {
        if (!talkingChk && !descChk) return;

        if (!descClickLock)
        {
            if (MapperLock != 0)
            {
                if (MapperManager.Instance.getNotMeetChk())
                {
                    MapperManager.Instance.goNextEventIdx();
                    setMapperLock(1);
                }
                else if (MapperLock == 1)
                { //첫번째 일경우
                    MapperManager.Instance.makeSecondEvent(0);
                    setMapperLock(2);
                }
                else if (MapperLock == 2)
                {
                    if (MapperManager.Instance.getEventTalkMaxDepth() == 3)
                    {
                        setMapperLock(3);
                        MapperManager.Instance.makeThirdEvent();
                    }
                    else
                    {
                        MapperManager.Instance.goNextEventIdx();
                        setMapperLock(1);
                    }
                }
                else if (MapperLock == 3)
                {
                    MapperManager.Instance.goNextEventIdx();
                    setMapperLock(1);
                }
                return;
            }
        } 


        if (talkingChk)
        {   
            goToNextTalk();
        }
        else if (loseChk)
        {
            CameraManager.Instance.loseScreenUnActive();
        }
        else
        {
            if (AdventureManager.Instance.remainItemChk()  && AdventureManager.Instance.getTutorial() == 0 ) //튜토리얼이 아니고, 남은 게 있을 경우
            {
                AdventureManager.Instance.remainItemOnOff(true);
            }
            else
            {
                if (!descClickLock)
                {
                    AdventureManager.Instance.clickDice(-1);
                }
            }
            
        }
    }

    public void makeTutorialArrow(int idx, Vector3 position, int opt, int rotation)
    {
        this.tutorialArrow[idx].GetComponent<RectTransform>().localPosition = position;
        this.tutorialArrowOrigin[idx].GetComponent<Animator>().Play(opt.ToString() + "_" + rotation.ToString());
        tutorialArrow[idx].GetComponent<Image>().sprite = tutorialArrowOrigin[idx].GetComponent<SpriteRenderer>().sprite;
    }
    public void offTutorialArrow(int idx)
    {
        this.tutorialArrow[idx].GetComponent<RectTransform>().localPosition = new Vector3(0f, 200f, 0f);
    }
    public void resetTutorialArrow()
    {
        for(int idx=0;idx<tutorialArrow.Length;idx++) this.tutorialArrow[idx].GetComponent<RectTransform>().localPosition = new Vector3(0f, 200f, 0f);
    }
    public void setTutorialArrow(int opt)
    {
        resetTutorialArrow();
        if(opt == 1) {
            makeTutorialArrow(0, new Vector3(-77,50,0), 0, 2);
            makeTutorialArrow(1, new Vector3(80, 20, 0), 1, 2);
            makeTutorialArrow(2, new Vector3(160, 60, 0), 1, 2);
        }
        if(opt == 2)
        {
            makeTutorialArrow(0, new Vector3(-77, 50, 0), 0, 2);
            makeTutorialArrow(1, new Vector3(35, -20, 0), 1, 2);
        }
        if(opt == 3) {
            makeTutorialArrow(0, new Vector3(60, 18, 0), 0, 1);
        }
        if (opt == 4)
        {
            makeTutorialArrow(0, new Vector3(-75, 0, 0), 0, 2);
            makeTutorialArrow(1, new Vector3(-135, 0, 0), 0, 2);
        }
        if(opt == 5)
        {
            makeTutorialArrow(0, new Vector3(-72, -29, 0), 1, 2);
            makeTutorialArrow(1, new Vector3(55, -30, 0), 1, 1);
            makeTutorialArrow(2, new Vector3(105, 53, 0), 1, 1);
        }
        if (opt == 6)
        {
            makeTutorialArrow(0, new Vector3(60, 18, 0), 0, 1);
            makeTutorialArrow(1, new Vector3(150, 18, 0), 0, 3);
        }
        if (opt == 7)
        {
            makeTutorialArrow(0, new Vector3(-100, 65, 0), 0, 0);
            makeTutorialArrow(1, new Vector3(-130, 65, 0), 0, 0);
        }
        if (opt == 8)
        {
            makeTutorialArrow(0, new Vector3(121, 65, 0), 0, 0);
            makeTutorialArrow(1, new Vector3(-135, 0, 0), 0, 1);
        }
        if(opt == 9)
        {
            int tempIdx = 0;
            for (int i=0;i<4;i++)
            {
                if(BattleManager.Instance.getCharacter(i) != null && BattleManager.Instance.getCharacter(i).getCurState() == 0)
                {
                    makeTutorialArrow(tempIdx, new Vector3(-112 + (i * 64), -55, 0), 0, 2);
                    makeTutorialArrow(tempIdx + 1, new Vector3(-112 + (64 * i) + 32, -55, 0), 0, 2);
                    tempIdx += 2;
                }
            }
        }
        if(opt == 10) 
        {
            makeTutorialArrow(1, new Vector3(110, -10, 0), 1, 2);
            
        }
        if(opt == 11)
        {
            //makeTutorialArrow(0, new Vector3(-135, 20, 0), 0, 1);
            makeTutorialArrow(0, new Vector3(152, -55, 0), 0, 2);// 전투 시작 버튼 관련
        }
        if(opt == 12) makeTutorialArrow(0, new Vector3(5, -20, 0), 0, 1);
        if(opt == 13) makeTutorialArrow(0, new Vector3(-100, 65, 0), 0, 0); //주사위 획득
        if(opt == 14) makeTutorialArrow(0, new Vector3(-5, 15, 0), 0, 0);//마녀 모자 클릭
        if(opt == 15)
        {
            int tempIdx = 0;
            for (int i = 0; i < 4; i++)
            {
                if (BattleManager.Instance.getCharacter(i) != null && BattleManager.Instance.getCharacter(i).getCurState() == 0)
                {
                    makeTutorialArrow(tempIdx, new Vector3(-125 + (i * 30), 0, 0), 0, 2);
                    tempIdx += 1;
                }
            }
            for (int i=0;i<3;i++)
            {
                makeTutorialArrow(i+2, new Vector3(35 + (i * 30), 0, 0), 0, 2);
            }
        }
        if(opt == 16) makeTutorialArrow(0, new Vector3(-60, 50, 0), 0, 3);
    }

    // Start is called before the first frame update
    void Start()
    {
        wishlistButton.SetActive(false);
        resetTutorialArrow();

        loseChk = false;
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

        for (int i = 0; i < 4; i++) {
            material[i] = characterImage[i].GetComponent<Image>().material; 
        }

            talkImage[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            talkImage[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            entity.SetActive(false);

            
        }

        bool jumpFlag = false;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            clickDescBox();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
        {

            talkClickButton.GetComponent<Image>().sprite = talkClickButtonOriginal.GetComponent<SpriteRenderer>().sprite;
            for (int i = 0; i < tutorialArrow.Length; i++)
            {
                tutorialArrow[i].GetComponent<Image>().sprite = tutorialArrowOrigin[i].GetComponent<SpriteRenderer>().sprite;
            }
            float yDefault = -20f;
            
            if (entity.activeSelf)
            {
                //투명도 조정
                for (int i = 0; i < characterImage.Length; i++)
                {
                if (lightingArr[i] != '0')
                {
                    if (material[i] != null)
                    {
                        if (material[i].GetFloat("_Transparency") > 0.0f) material[i].SetFloat("_Transparency", material[i].GetFloat("_Transparency") - 0.1f);
                        else material[i].SetFloat("_Transparency", 0.0f);
                    }
                }
                else
                {
                    if (material[i] != null)
                    {
                        if (material[i].GetFloat("_Transparency") < 0.7f) material[i].SetFloat("_Transparency", material[i].GetFloat("_Transparency") + 0.1f);
                        else material[i].SetFloat("_Transparency", 0.7f);
                    }
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
        if (descList.Count == 0) {
            return "";
        }
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
            characterTalkBack.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -548f, 0f);
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
            characterTalkBack.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -548f, 0f);
            characterTalk.GetComponent<TextMeshProUGUI>().text = descString;
        }
        else //text 필요 없음. 
        {
            characterTalkBack.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -1100f, 0f);
        }
    }

    public int curTalkLastVal = 0;
    public int curTalkStartVal = 0;
    public void startTalk(int a)
    {
        if (a < 0) return;
        if (!talkingChk)
        {
            

            curTalkLastVal = listIdx[a + 1] - listIdx[a];
            curTalkStartVal = listIdx[a];
            characterTalkBack.GetComponent<Image>().color = new Color(255f, 255f, 255f);
            characterTalk.GetComponent<TextMeshProUGUI>().color = new Color(0f, 0f, 0f);

            entity.SetActive(true);
            clickFontSize(false);

            hoverOutSkipButton();
            hoverOutPrevButton();
            hoverOutFontSize();

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
            else {
                skipButton.SetActive(false); 
            }

        }
    }
    public void goToNextTalk()
    {
        if (talkingChk)
        {
            SoundManager_Sfx.Instance.playSound(0);
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
    public void goToPrevTalk()
    {
        if (talkingChk)
        {
            SoundManager_Sfx.Instance.playSound(0);
            if (talkList[curIdx].talkIdx == talkList[curIdx - 1].talkIdx)
            {
                curIdx--;
                printTalk(curIdx);
            }
        }
    }
    public void hoverInPrevButton()
    {
        ToolBarManager.Instance.setToolBar(18);
        prevButtonOutline.SetActive(true);
    }
    public void hoverOutPrevButton()
    {
        ToolBarManager.Instance.toolBarOnOff(0);
        prevButtonOutline.SetActive(false);
    }
    public void hoverInSkipButton()
    {
        ToolBarManager.Instance.setToolBar(19);
        skipButtonOutline.SetActive(true);
    }
    public void hoverOutSkipButton()
    {
        ToolBarManager.Instance.toolBarOnOff(0);
        skipButtonOutline.SetActive(false);
    }
    public void hoverInFontSize()
    {
        ToolBarManager.Instance.setToolBar(20);
        fontSizeButtonOutline.SetActive(true);
    }
    public void hoverOutFontSize()
    {
        ToolBarManager.Instance.toolBarOnOff(0);
        fontSizeButtonOutline.SetActive(false);
    }
    public void clickFontSize(bool addChk)
    {
        if(addChk) jsonDataManager.Instance.addFontSize();
        int result = jsonDataManager.Instance.getFontSize();

        fontSizeButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/talkFontSizeButton_" + result.ToString());

        characterTalk.GetComponent<TextMeshProUGUI>().fontSize = result * 5 + 25;
    }
    public void printTalk(int a)
    {
        if (talkList[a].eventType == 3){
            wishlistButton.SetActive(true);
        }

        if (talkList[a].SFX >= 0) SoundManager_Sfx.Instance.playSound(talkList[a].SFX);

        if (preSound != talkList[a].BackSnd) { //배경음 변경 타이밍

            if (preSound >= 0){
                SoundManager_Main.Instance.stopSound(preSound); //노래가 바뀌었으니 이전 노래 정지
            }
            preSound = talkList[a].BackSnd; // 노래 변경
            if (talkList[a].BackSnd >= 0) SoundManager_Main.Instance.playSound(talkList[a].BackSnd); //노래 틀어야 하는 경우 틀기.
        } // 확인


        curTalkIdx.GetComponent<TextMeshProUGUI>().text = (curIdx - curTalkStartVal + 1).ToString() +" / "+curTalkLastVal.ToString();

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

        //이미지 사용시 체크
        if (talkList[a].imagePlace == 0) {
            talkImage[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            talkImage[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
        else if (talkList[a].imagePlace == 1) {
            talkImage[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/talkImage/spr_talkImage_" + talkList[a].imageIdx.ToString());
            talkImage[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
        else if (talkList[a].imagePlace == 2) {
            talkImage[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
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
            if (nameArr[i] == "Village" || nameArr[i] == "Bard" || nameArr[i] == "Tom" || nameArr[i] == "Bob") characterImage[i].GetComponent<RectTransform>().sizeDelta = new Vector2(204f, 216f);
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
        characterName.GetComponent<TextMeshProUGUI>().text = SpecialTextChange(talkList[a].Name);

        if (talkList[a].Name == "" || talkList[a].Name == " " || talkList[a].Name == "  " || talkList[a].Name == "   ") { 
            nameBackground.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
        else
        {
            nameBackground.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/spr_ui_communicate_back");
        }
        talkLanArr[0] = talkList[a].TextKR;
        talkLanArr[1] = talkList[a].TextEN;
        talkLanArr[2] = talkList[a].TextJP;
        characterTalk.GetComponent<TextMeshProUGUI>().text = talkLanArr[jsonDataManager.Instance.getLanguage()];
        /*
        if (jsonDataManager.Instance.getLanguage() == 0) characterTalk.GetComponent<TextMeshProUGUI>().text = talkList[a].TextKR;
        else if (jsonDataManager.Instance.getLanguage() == 2) characterTalk.GetComponent<TextMeshProUGUI>().text = talkList[a].TextJP;
        else characterTalk.GetComponent<TextMeshProUGUI>().text = talkList[a].TextEN;
        */

        preBackground = talkList[a].backGround;
        setPreCharacterName();
    }
    public void stopTalk()
    {
        if (talkingChk)
        {
            wishlistButton.SetActive(false);

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
            
            talkImage[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            talkImage[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            entity.SetActive(false);
            changeTalkState(0, false);

            if (talkList[curIdx].talkIdx == 54) {
                if (TownManager.Instance.curTownIdx == 7)
                {
                    SoundManager_Main.Instance.playSound(7);
                }
                FadeUIScript.fadeIn();
            }
            ToolBarManager.Instance.toolBarOnOff(0);
        }
    }
    public bool getTalkChk()
    {
        return talkingChk;
    }

    public void hoverInWishlist()
    {
        wishlistButton.GetComponent<Image>().sprite = wishlistSprite[1];
    }
    public void hoverOutWishlist()
    {
        wishlistButton.GetComponent<Image>().sprite = wishlistSprite[0];
    }
    public void clickWishlist()
    {
        Application.OpenURL("https://store.steampowered.com/app/4022200/Destiny_Is_Dice/#game_area_purchase");
    }
}
