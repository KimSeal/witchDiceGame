using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;
using AnimatedBattleText.Examples;
public class AdventureManager : MonoBehaviour
{
    public int adventureJewelMax = 0;
    public int adventureJewel = 0;
    public int getAdventureJewel()
    {
        return adventureJewel;
    }
    public void addWitchPowerCount(int val)
    {
        adventureJewel += val;
        if (adventureJewel < 0) adventureJewel = 0;
        if (adventureJewel > adventureJewelMax) adventureJewel = adventureJewelMax;
    }

    private static AdventureManager instance = null;
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

    public static AdventureManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }

    [SerializeField]
    private GameObject diceRollEff;

    [SerializeField] private GameObject balpanLoad, balpanScreen, balpanArrow, balpanArrowGoal; // obj_adventure_diceBoard_load, obj_adventure_diceBoard, obj_balpan_arrow
    [SerializeField] private GameObject[] balpanObj = new GameObject[10]; //obj_balpan_(number)
    [SerializeField] public GameObject[] balpanEffObj = new GameObject[10];
    [SerializeField] private GameObject[] balpanNewMark = new GameObject[10];
    [SerializeField] public GameObject[] balpanUpDownButton = new GameObject[2];
    [SerializeField] public GameObject balpanCurPointText;
    int[] balpanEventType = new int[10];
    int[] balpanEventIdx = new int[10];

    public int stageDepth = 0; //플레이어가 현재 위치한 스테이지 상의 깊이
    public int stageDepthMax = 0; //플레이어가 갈수 있는 최대 깊이
    private int stageNum = 0; //몇번째 스테이지인지 받는다.
    private int stageIdx = 0; //이번 스테이지에서 몇번째 맵인지(1-1 1-2의 개념) 
    private int[] witchPower = new int[2];
    [SerializeField] public GameObject witchHatButton;

    private int eventWatchNum = 0; //이벤트 선택지 볼때 쓰는 숫자
    private int selectDiceNum = -1; //현재 선택된 주사위
    private int[] adventureEventArr = new int[10001]; //앞으로 남은 이벤트들에 대한 정보
    private int[] adventureEventArr_Y = new int[10001]; // 이벤트 들이 위치할 곳에 대한 세로축 정보
    //전투 : 0  주사위 굴리기 이벤트 : 1 


    [SerializeField] private GameObject stageInfo; //현재 스테이지의 level과 스테이지 정보를 담는 text
    [SerializeField] private GameObject selectInfo; // adventure_selectInfo
    [SerializeField] private GameObject selectImage; //adventure_selectDice
    private GameObject[] textObject = new GameObject[2]; // 주사위 굴렸을때 결과를 처리하기 위해 사용한다. 
    [SerializeField] private GameObject[] diceObject = new GameObject[4]; //adventure_dice_(number)
    [SerializeField] private GameObject[] diceOutline = new GameObject[4];

    [SerializeField] private GameObject resultObj; //obj_adventureResuilt
    [SerializeField] private GameObject[] resultObjArr = new GameObject[4]; //obj_adventureResult_Item_(number)
    [SerializeField] private GameObject[] resultNewMark = new GameObject[4];
    private int[,] resultItemArr = new int[4, 2]; //결과로 주어지는 아이템들 정보.


    public List<adventureEvent>[] adventureEventList = new List<adventureEvent>[5]; //

    public List<AdventureEventReader> adventureEventReaderList = new List<AdventureEventReader>(); // 
    public List<AdventureEventPacketReader> adventureEventPacketReaderList = new List<AdventureEventPacketReader>(); // 

    [SerializeField] GameObject mainCamera; //Main Camera

    private adventureEvent curDiceEvent;
    private adventureEvent_Packet curDiceEventPacket;

    [SerializeField] private GameObject adventureBackground, adventureNPC, adventureBackBoard; //ui_adventure Back_0/ NPC_0 / backBoard
    [SerializeField] private GameObject[] watchNumObject = new GameObject[6]; //obj_adventureBtn_selectBtn_(number)
    [SerializeField] public GameObject watchNumObjectEntity; //obj_adventureBtn_selectBtn

    [SerializeField]
    public GameObject[] characterObj = new GameObject[4];
    public GameObject[] characterSmoke = new GameObject[4];
    public GameObject[] characterShadow = new GameObject[4];

    bool curCanvasIsAdventure = true;
    bool battleEventTrigger = false;
    bool eventWatchTrigger = false;


    //어드벤쳐 캐릭터 버튼 선택용
    private int selectDiceCharacterIdx = -1; //지금은 가장 앞에 있는 놈으로 해놨는데 추후 수정가능하게 만들기
    [SerializeField] private GameObject nextBtnObj, standObj; //adventure_nextBtn_0, ui_backImage_0

    [SerializeField] private GameObject diceBtnFireInit; // adventure_nextBtn_0_fire
    private ParticleSystem diceBtnFire;
    [SerializeField] private GameObject lifeObj, lifeObj_back; //obj_life, obj_life_back

    private bool eventEndClick = false; //이벤트를 넘어갈 수 있는 경우, true가 된다.

    private bool clickAble = false;

    //상점에 대한 데이터
    private int adventureGold = 0;

    private int[,] storeItemArr = new int[4, 3]; //4개의 아이템이 배치, 각각 type, index(아이템 고유번호), 가격이 저장될 예정 

    //store default 4 item
    [SerializeField] private GameObject storeEntityObj;
    [SerializeField] public GameObject[] storeImageObj = new GameObject[4];
    //[SerializeField] public GameObject[] storePriceObjInit = new GameObject[4]; //아이템 가격 관련
    public TextMeshPro[] storePriceObj = new TextMeshPro[4];

    //store Buy check ui
    [SerializeField] private GameObject storeCheckImageObjInit, storeCheckPriceObjInit; //obj_ui_adventureStore _item_price  _buy_ sprite/text
    public SpriteRenderer storeCheckImageObj;
    private TextMeshPro storeCheckPriceObj;
    [SerializeField] private GameObject storeCheckEntityObj, storeCheckButtonYes, storeCheckButtonNo; // spr_ui_adventureStore_ yes/no Btn

    [SerializeField]
    public GameObject itemRemainChk;
    public TextMeshPro itemRemainText;


    public int atkMaxVal = 30;
    public int magMaxVal = 30;
    public int spdMaxVal = 30;
    public int armorMaxVal = 3;

    public int deadEnemyCount = 0;
    public void addDeadEnemyCount()
    {
        deadEnemyCount += 1;
    }
    public int getDeadEnemyCount()
    {
        return deadEnemyCount;
    }

    public int getAtkMaxVal() { return atkMaxVal; }
    public int getMagMaxVal() { return magMaxVal; }
    public int getSpdMaxVal() { return spdMaxVal; }
    public int getArmorMaxVal() { return armorMaxVal; }
    public void initMaxVal()
    {
        atkMaxVal = 30;
        magMaxVal = 30;
        spdMaxVal = 30;
        armorMaxVal = 3;
        deadEnemyCount = 0;
        itemManager.Instance.setItemMaxNum(6);
    }
    public void addMaxVal(int opt, int val) // 0 : 공격력 최대 1 : 감응력 2 : 스피드 3: 방어도
    {
        if (opt == 0){
            atkMaxVal += val;
            if (atkMaxVal >= 100) atkMaxVal = 99;
        }
        if (opt == 1)
        {
            magMaxVal += val;
            if (magMaxVal >= 100) magMaxVal = 99;
        }
        if (opt == 2)
        {
            spdMaxVal += val;
            if (spdMaxVal >= 100) spdMaxVal = 99;
        }
        if (opt == 3) armorMaxVal += val;
        

    }


    private int[] lastCharacter = new int[4];

    private int tutorialVal = 0;

    public bool tutorialVal4ErrorChk;
    public bool getTutorialVal4ErrorChk() { return tutorialVal4ErrorChk; }
    public void setTutorialVal4ErrorChk(bool onOff) { tutorialVal4ErrorChk = onOff; }

    public void remainItemOnOff(bool onOff)
    {

        if (onOff)
        {
            itemRemainText.text = TalkManager.Instance.getDesc(67);
            itemRemainChk.transform.position = new Vector3(-500f, 0f, 0f);
            itemRemainChk.GetComponent<hoverRotate>().shakeStart();
        }
        else
        {
            itemRemainChk.transform.position = new Vector3(-500f, 300f, 0f);
        }
    }
    public void clickRemainItem()
    {
        resetItemResult();
        TalkManager.Instance.clickDescBox();
        remainItemOnOff(false);
    }
    public bool remainItemChk()
    {
        return
            resultObj.activeSelf &&
            !(resultItemArr[0, 0] == -99999 && resultItemArr[0, 1] == -99999 &&
            resultItemArr[1, 0] == -99999 && resultItemArr[1, 1] == -99999 &&
            resultItemArr[2, 0] == -99999 && resultItemArr[2, 1] == -99999 &&
            resultItemArr[3, 0] == -99999 && resultItemArr[3, 1] == -99999);
    }

    public void hoverInCharacter(int idx)
    {
        characterObj[idx].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
        if (CharacterManager.Instance.getCharacter(idx) != null)
        {
            ToolBarManager.Instance.setToolBar(CharacterManager.Instance.getCharacter(idx));
        }
    }

    public void hoverOutCharacter(int idx)
    {
        characterObj[idx].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
        ToolBarManager.Instance.toolBarOnOff(0);
    }

    private void clickAbleObjSet(GameObject gameObjectTemp, bool onOff, int opt)
    {
        if (opt == 1) { gameObjectTemp.GetComponent<hoverRotate>().expandEnd(); }
        hoverRotateAble(gameObjectTemp, opt, onOff);
    }

    public void hoverRotateAble(GameObject gameObjectTemp, int eventType, bool onOff)
    {
        if (eventType == 0) gameObjectTemp.GetComponent<hoverRotate>().shakeAble(onOff);
        else if (eventType == 1) gameObjectTemp.GetComponent<hoverRotate>().expandAble(onOff);
        else if (eventType == 2) gameObjectTemp.GetComponent<hoverRotate>().clickShakeAble(onOff); //에러있음.
    }

    public void shakeObject(GameObject gameObjectTemp)
    {
        gameObjectTemp.GetComponent<hoverRotate>().shakeStart();
    }

    public int getAdventureEventLen(int stageNum)
    {
        return adventureEventList[stageNum].Count;
    }
    public adventureEvent getAdventureEvent(int stageNum, int stageIdx)
    {
        return new adventureEvent(adventureEventList[stageNum][stageIdx]);

//        return new adventureEvent(adventureEventList[stageNum][adventureEventArr[stageIdx]]);
    }
    public int getTutorial()
    {
        return tutorialVal;
    }
    public void setTutorial(int val)
    {

        if (val == 1 || tutorialVal + 1 == val)
        {
            tutorialVal = val;
        }
        else
        {
            Debug.Log("tutorial : " + tutorialVal.ToString());
        }
    }

    public IEnumerator tutorial_Coroutine()
    {
        giveUpBtnAble(false);
        tagInit();
        resetDice();
        TalkManager.Instance.startTalk(2);
        yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
        //튜토리얼이 시작되었음을 알림.
        tutorialVal = 1;
        setTutorialVal4ErrorChk(false);
        CameraManager.Instance.updateInitPosition(new Vector3(-500f, 0f, mainCamera.transform.position.z));
        stageDepth = 0;
        stageDepthMax = 1;
        StartCoroutine(phase_Manage_Coroutine(0));
    }
    public void tutorialStart()
    {

        CharacterManager.Instance.setTutotialCharacterSet(); //캐릭터는 주인공 혼자만
        itemManager.Instance.setTutorialInitDice(); //주인공 주사위 다 1로
        useFairDice = false;

        StartCoroutine(tutorial_Coroutine());
    }
    public int getLastCharacter(int idx) {
        return lastCharacter[idx];
    }
    private void storyLineErrorChk() //챕터스토리를 보다가 끊고 다시 들어온 경우 임시 대처.(본걸로 처리)
    {
        /*
        for (int i = 0; i < 3; i++) {
            if (jsonDataManager.Instance.getChapterRead(1, i) == 1) jsonDataManager.Instance.setChapterRead(1, i);
        }
        */
    }

    [SerializeField]
    public GameObject tutorialUI;
    [SerializeField]
    public GameObject tutorialUIText;

    [SerializeField]
    public GameObject exitButtonEntity;
    [SerializeField]
    public GameObject exitButtonText;
    public void activeTutorialButton(bool onOff)
    {
        if (onOff)
        {
            tutorialUI.transform.position = new Vector3(-1500f, -500f, 0f);
            tutorialUIText.GetComponent<TextMeshPro>().text = TalkManager.Instance.getDesc(24);
            upDownManager.Instance.changeOption(3, false);
        }
        else
        {
            tutorialUI.transform.position = new Vector3(-2000f, -500f, 0f);
            upDownManager.Instance.changeOption(3, true);
        }
    }

    public void changeLanguage()
    {
        tutorialUI.transform.position = new Vector3(-2000f, -500f, 0f);
        mainExitButton(false);
        closeTryBuyItem(false);
        remainItemOnOff(false);
    }
    public void mainPlayButton(bool input)
    {
        storyLineErrorChk();
        //TalkManager.Instance.startTalk(21) ; //시작 말하기 테스트
        //Screen.SetResolution(960, 540, FullScreenMode.Windowed);
        SoundManager_Sfx.Instance.playSound(0);
        SoundManager_Main.Instance.stopSound(0);
        upDownManager.Instance.changeOption(3, false);
        //if (false) {
        if (input) {
            CameraManager.Instance.updateInitPosition(new Vector3(-1000f, -500f, mainCamera.transform.position.z));
            tutorialStart();
        }
        else
        {
            TownManager.Instance.backToTownUI();
            if (jsonDataManager.Instance.setChapterDid(0, 1))
            {
                jsonDataManager.Instance.tutorialDid();
                TalkManager.Instance.startTalk(16);
            }
            if (jsonDataManager.Instance.getChapterRead(0,2) ==2 && jsonDataManager.Instance.setChapterDid(1, 1)) //업데이트 시 반영되도록
            {
                SoundManager_Main.Instance.stopSound(7);
                TalkManager.Instance.startTalk(54);
            }
            //CameraManager.Instance.updateInitPosition(new Vector3(-500f, -500f, mainCamera.transform.position.z));
            //SoundManager_Main.Instance.playSound(7);
        }
        TalkManager.Instance.setDescIdx(-1);

        tutorialUI.transform.position = new Vector3(-1500f, -250f, 0f);
    }
    public void mainExitButton(bool onOff)
    {
        if (onOff) Application.Quit();
        else exitButtonEntity.transform.position = new Vector3(-2000f, -500f, 0f);
    }

    public void activeExitButton()
    {
        exitButtonEntity.transform.position = new Vector3(-1500f, -500f, 0f);
        exitButtonText.GetComponent<TextMeshPro>().text = TalkManager.Instance.getDesc(169);
        activeTutorialButton(false);
    }

    public void resetItemResult()
    {
        for (int i = 0; i < 4; i++)
        {
            resultItemArr[i, 0] = -99999;
            resultItemArr[i, 1] = -99999;
            storeItemArr[i, 0] = -99999;
            storeItemArr[i, 1] = -99999;
            storeItemArr[i, 2] = -99999;
        }

    }
    #region
    public int getAdventureGold()
    {
        return this.adventureGold;
    }
    /*
    public void addAdventureMoney(int money) {
        
        moneyText.text = "$  " + adventureMoney.ToString();
    }
    */
    public void buyItem()
    {
        int storeIdx = storeLastClickIdx;
        if (storeItemArr[storeIdx, 0] == -99999 || storeItemArr[storeIdx, 1] == -99999 || storeItemArr[storeIdx, 2] == -99999) return; //비어있을 경우 아예 아무것도 없게
        if (adventureGold >= storeItemArr[storeIdx, 2]) {
            int buyResult = itemManager.Instance.getItemResult(storeItemArr[storeIdx, 0], storeItemArr[storeIdx, 1]);
            if (buyResult == 0) //정상작동의 경우
            {
                SoundManager_Sfx.Instance.playSound(4);
                addMoney(0, storeItemArr[storeIdx, 2] * -1);
                storeItemArr[storeIdx, 0] = -99999;
                storeItemArr[storeIdx, 1] = -99999;
                storeItemArr[storeIdx, 2] = -99999;

                closeTryBuyItem(true);
                shakeObject(storeImageObj[storeLastClickIdx]);
                updateStore();
            }
            else if (buyResult == 1) //인벤토리가 가득 찬 경우
            {
                SoundManager_Sfx.Instance.playSound(7);
                shakeObject(storeCheckEntityObj);
                storeCheckPriceObj.text = TalkManager.Instance.getDesc(19);
            }
        }
        else
        {
            shakeObject(storeCheckEntityObj);
            SoundManager_Sfx.Instance.playSound(7);
            storeCheckPriceObj.text = TalkManager.Instance.getDesc(20);
        }
    }
    public void updateStore() //가게 이미지 업데이트
    {
        for (int storeIdx =0;storeIdx<4;storeIdx++) {
            if (storeItemArr[storeIdx, 0] == -99999 || storeItemArr[storeIdx, 1] == -99999 || storeItemArr[storeIdx, 2] == -99999) {
                storeImageObj[storeIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                storePriceObj[storeIdx].text = "";//"No Item Here!";
            }
            else
            {
                Item hoverItem = itemManager.Instance.getItem(storeItemArr[storeIdx, 0], storeItemArr[storeIdx, 1]);
                storeImageObj[storeIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/" + typeArr[storeItemArr[storeIdx, 0]] + "ItemSprite/spr_item_" + typeArr[storeItemArr[storeIdx, 0]] + "_" + hoverItem.getItemName());
                storePriceObj[storeIdx].text = storeItemArr[storeIdx, 2].ToString();
            }
        }
    }
    public int storeLastClickIdx = 0;
    public void tryBuyItem(int idx)
    {
        
        int storeIdx = idx;
        //아이템이 비어있는 경우 불가능하도록
        if (storeItemArr[storeIdx, 0] == -99999 || storeItemArr[storeIdx, 1] == -99999 || storeItemArr[storeIdx, 2] == -99999)
        {
            SoundManager_Sfx.Instance.playSound(7);
            return;
        }
        else
        {
            activeGiveUpBoard(false);
            SoundManager_Sfx.Instance.playSound(0);

            storeCheckEntityObj.SetActive(true);
            clickAbleObjSet(storeCheckButtonYes, true, 1);
            clickAbleObjSet(storeCheckButtonNo, true, 1);
            shakeObject(storeCheckEntityObj);

            Item hoverItem = itemManager.Instance.getItem(storeItemArr[storeIdx, 0], storeItemArr[storeIdx, 1]);
            storeCheckImageObj.sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/" + typeArr[storeItemArr[storeIdx, 0]] + "ItemSprite/spr_item_" + typeArr[storeItemArr[storeIdx, 0]] + "_" + hoverItem.getItemName());
            storeCheckPriceObj.text = TalkManager.Instance.getDesc(10) + " : " + storeItemArr[storeIdx, 2].ToString() +
                    "\n" + TalkManager.Instance.getDesc(11) + adventureGold.ToString() + " -> " + (adventureGold - storeItemArr[storeIdx, 2]).ToString();

            storeLastClickIdx = idx;
        }
    }
    public void closeTryBuyItem(bool soundOnOff)
    {
        storeCheckButtonYes.GetComponent<hoverRotate>().expandEnd();
        storeCheckButtonNo.GetComponent<hoverRotate>().expandEnd();
        storeCheckEntityObj.SetActive(false);
        if (soundOnOff) SoundManager_Sfx.Instance.playSound(7);
    }

    public Character[] resultCharacter = new Character[4];
    public void hoverInItem_store(int storeIdx)
    {
        if (storeItemArr[storeIdx, 0] != -99999 && storeItemArr[storeIdx, 1] != -99999 && storeItemArr[storeIdx, 2] != -99999) //아이템이 있는 경우 해당 아이템으로 변경
        {

            if (storeItemArr[storeIdx, 0] == 4)
            {
                ToolBarManager.Instance.setToolBar(resultCharacter[storeIdx]);
            }
            else
            {
                Item hoverItem = itemManager.Instance.getItem(storeItemArr[storeIdx, 0], storeItemArr[storeIdx, 1]);
                ToolBarManager.Instance.setToolBar(hoverItem);
            }
        }

    }
    #endregion

    string[] typeArr = { "consume", "dice", "equip", "passive", "destiny" };
    int[] typeArr2 = { 78, 79, 80, 81, 82 };
    public void hoverInItem(int idx)
    {
        if (resultItemArr[idx, 0] != -99999 && resultItemArr[idx, 1] != -99999) //아이템이 있는 경우 해당 아이템으로 변경
        {
            //if (descObj[0].activeSelf == false) descObj[0].SetActive(true);

            if (resultItemArr[idx, 0] == 4)
            {
                ToolBarManager.Instance.setToolBar(resultCharacter[idx]);
            }
            else
            {
                Item hoverItem = itemManager.Instance.getItem(resultItemArr[idx, 0], resultItemArr[idx, 1]);
                ToolBarManager.Instance.setToolBar(hoverItem);
            }
        }
    }
    public void hoverOutItem()
    {
        ToolBarManager.Instance.toolBarOnOff(0);
    }
    // Start is called before the first frame update
    void Start()
    {
        initMaxVal();
        remainItemOnOff(false);
        adventureGold = jsonDataManager.Instance.getMoney();
        addMoney(0, 0);


        mainExitButton(false);
        tutorialUI.transform.position = new Vector3(-1500f, -250f, 0f);

        lastCharacter[0] = -99999;
        lastCharacter[1] = -99999;
        lastCharacter[2] = -99999;
        lastCharacter[3] = -99999;

        lifeObj.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0f);
        lifeObj_back.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0f);

        lifeObj.SetActive(false);

        diceBtnFire = diceBtnFireInit.GetComponent<ParticleSystem>();
        diceBtnFire.Stop();

        standObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");

        for (int i = 0; i < 4; i++)
        {
            resetItemResult();
        }
        resultObj.SetActive(false);
        stageNum = 1;
        stageIdx = 1;
        //stageInfo.GetComponent<TextMeshPro>().text = "Stage : " + stageNum + "  Level : " + stageIdx;
        //selectInfo.GetComponent<TextMeshPro>().text = "Stage : " + stageNum + "  Level : " + stageIdx;

        AdventureEventPacketReader[] tempList = new AdventureEventPacketReader[6];

        adventureEventReaderList = CSVReader.Read<AdventureEventReader>("Event");
        adventureEventPacketReaderList = CSVReader.Read<AdventureEventPacketReader>("EventPacket");

        for (int Idx = 0; Idx < adventureEventList.Length; Idx++) //스테이지 갯수만큼 adventure 리스트 만들기
        {
            adventureEventList[Idx] = new List<adventureEvent>();
        }

        int packetIdx = 0; //전체 packet 배열을 위한 변수
        int packetArrIdx = 0; //event내 packet 배열에 대응하는 변수
        for (int eventIdx = 0; eventIdx < adventureEventReaderList.Count; eventIdx++) //Reader 2개를 병합 시켜 하나의 event를 만들어 list에 추가
        {
            for (int i = 0; i < 6; i++) { // 배열 초기화
                tempList[i] = null;
            }
            packetArrIdx = 0;
            while (eventIdx < adventureEventReaderList.Count && packetIdx < adventureEventPacketReaderList.Count
                && adventureEventReaderList[eventIdx].eventIdx == adventureEventPacketReaderList[packetIdx].eventIdx) //다른 event나올때까지 업
            {
                tempList[packetArrIdx++] = adventureEventPacketReaderList[packetIdx++];
            }
            adventureEventList[adventureEventReaderList[eventIdx].stageIdx].Add(new adventureEvent(adventureEventReaderList[eventIdx], tempList)); //packet과 event 내용을 받은 event 리스트 생성
        }


        //상점 관련
        //for(int i=0;i<4;i++) storePriceObj[i] = storePriceObjInit[i].GetComponent<TextMeshPro>();

        storeEntityObj.SetActive(false);
        storeCheckImageObj = storeCheckImageObjInit.GetComponent<SpriteRenderer>();
        storeCheckPriceObj = storeCheckPriceObjInit.GetComponent<TextMeshPro>();
        storeCheckEntityObj.SetActive(false);

        resetItemResult();
        giveUpBtnAble(true);
        adventureStartChk = false;

        activeGiveUpBoard(false);
        //activeTutorialButton(false);

    }

    private float[] characterMoveSize = new float[4];
    private float[] characterMoveVal = new float[4];
    private float[] characterLandVal = new float[4];
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            //jumpTest();
        }
    }

    void FixedUpdate()
    {
        
        for (int i = 0; i < 4; i++)
        {
            if (CharacterManager.Instance.getCharacter(i) != null && CharacterManager.Instance.getCharacter(i).getCurState() == 0) {
                if (characterMoveSize[i] > 0f)
                {
                    characterMoveSize[i] -= jumpValMove;
                    if (characterMoveSize[i] <= 0f || characterMoveVal[i] >= 50f)
                    {
                        characterMoveVal[i] -= 50f;
                        characterLandVal[i] += 0.5f;
                        if (!getBattleEventChk()) { //전투 들어갔을땐 소리 안나도록
                            SoundManager_Sfx.Instance.playSound(1);
                        }
                    }
                    characterMoveVal[i] += 1.0f + characterLandVal[i];
                    characterObj[i].GetComponent<Transform>().position = new Vector3(characterObj[i].transform.position.x, -74 + characterMoveSize[i] * Mathf.Abs(Mathf.Sin((characterMoveVal[i] / 50) * Mathf.PI)), 0f);
                    
                }
                else
                {
                    characterMoveSize[i] = 0.0f;
                    characterMoveVal[i] = 0.0f;
                    characterLandVal[i] = 0.0f;
                    characterObj[i].GetComponent<Transform>().position = new Vector3(characterObj[i].transform.position.x, -74, 0f);
                }
            }
            else
            {
                characterMoveSize[i] = 0.0f;
                characterMoveVal[i] = 0.0f;
                characterLandVal[i] = 0.0f;
                characterObj[i].GetComponent<Transform>().position = new Vector3(characterObj[i].transform.position.x, -74, 0f);
            }
        }
    }
    [SerializeField]
    public float jumpPower = 50f;
    public float jumpValMove = 0.5f;
    public float jumpDelay = 0.1f;
    [SerializeField]
    public GameObject boardObj;

    public bool adventureStart = false;
    public IEnumerator jumpCharacter(){ 
        for(int i=3; i>=0; i--){
            if (CharacterManager.Instance.getCharacter(i) != null && CharacterManager.Instance.getCharacter(i).getCurState() == 0)
            {
                characterMoveSize[i] = jumpPower;
                SoundManager_Sfx.Instance.playSound(19);
                characterMoveVal[i] = 0f;
                yield return new WaitForSeconds(0.05f);
            }
        }
        
    }

    public void randomMake(int start, int end) //이 중간에 있는 stage를 섞는다
    {
        if (start == end) return;
        for (int i = end; i > start; i--) //나중에 보스 전은 무조건 마지막에 올수 있도록 편성한다.
        {
            int j = Random.Range(start, i + 1);

            int temp = adventureEventArr[i];
            adventureEventArr[i] = adventureEventArr[j];
            adventureEventArr[j] = temp;
        }
    }
    public void makeStageEventArr(int stageNum) //이번 스테이지의 나타나는 이벤트의 종류를 미리 배치한다.
    {
        /*
        if (tutorialVal > 0)
        {
            adventureEventArr = new int[adventureEventList[stageNum].Count];
            for (int i = 0; i < adventureEventList[stageNum].Count; i++)
            {
                adventureEventArr[i] = i; //i;이부분 조정해서 맵 테스트 진행
            }
            return;
        }
        */
        // stage 순서를 랜덤으로 만든다.
        adventureEventArr = new int[adventureEventList[stageNum].Count];
        for (int i = 0; i < adventureEventList[stageNum].Count; i++)
        {
            adventureEventArr[i] = i; //i;이부분 조정해서 맵 테스트 진행
        }

        int EndPoint = adventureEventArr.Length - 1;

        for (int i = adventureEventArr.Length - 1; i >= 0; i--) //나중에 보스 전은 무조건 마지막에 올수 있도록 편성한다.
        {
            //레벨이 달리지는경우 혹은 
            if (i == 0 || adventureEventList[stageNum][i].getLevel() != adventureEventList[stageNum][i - 1].getLevel())
            {
                //if (i != 1) 
                    randomMake(i, EndPoint);
                //else randomMake(0, EndPoint);

                
                EndPoint = i - 1;
            }
            /*
            int j = Random.Range(0, i + 1);

            int temp = adventureEventArr[i];
            adventureEventArr[i] = adventureEventArr[j];
            adventureEventArr[j] = temp; 
            */
        }
    }
    private void makeStage_placeBalpan()
    {
        adventureEventArr_Y = new int[adventureEventList[stageNum].Count];
        for (int i = 0; i < adventureEventArr_Y.Length; i++)
        {
            int j = Random.Range(0, 2);
            if (i == 0) //처음은 이전거 탐지 못하므로 0 아니면 2로 나올 수 있게 한다.
            {
                if(j == 0) adventureEventArr_Y[0] = 0;
                else adventureEventArr_Y[0] = 2;
            }
            else if (j == 0){
                if (adventureEventArr_Y[i - 1] > 0) adventureEventArr_Y[i] = 0;
                else adventureEventArr_Y[i] = 1;
            }
            else{
                if (adventureEventArr_Y[i - 1] == 1) adventureEventArr_Y[i] = 2;
                else adventureEventArr_Y[i] = 1;
            }
        }
    }
    public bool loadEnd = true;
    public void adventure_load() //adventure로딩 끝난 경우 확인
    {
        loadEnd = true;
    }
    public bool adventureStartChk = false;
    public bool getAdventureStartChk()
    {
        return adventureStartChk;
    }
    public void startAdventure()
    {
        stageDepth = 0;
        adventureStart = true;
        giveUpBtnAble(false);
        tagInit();
        adventureGold = jsonDataManager.Instance.getMoney();
        addMoney(0,0);
        adventureJewelMax = 9999;
        adventureJewel = 5;
        addMoney(1, 0);
        initMaxVal();
        stageDepthMax = 1;
        if (jsonDataManager.Instance.getChapterRead(0, 2) == 2) {
            stageDepthMax = 2;
        }

        curCanvasIsAdventure = true;
        battleEventTrigger = false;

        CharacterManager.Instance.setTestCharacterSet();
        CameraManager.Instance.updateInitPosition(new Vector3(-500f, 0f, mainCamera.transform.position.z));
        //mainCamera.transform.position = new Vector3(-500f, 0f, mainCamera.transform.position.z);
        
        resetDice();
        //지금은 시작 버튼 누르면 바로 시작
        if (jsonDataManager.Instance.getChapterRead(0, 2) == 2)
        {
            //StartCoroutine(phase_Manage_Coroutine(3));

            StartCoroutine(phase_Manage_Coroutine(Random.Range(1,3)));
        }
        else
        {
            StartCoroutine(phase_Manage_Coroutine(2));
        }
    }

    public void clearBalpan()// 발판 이벤트 끝나고 발판 화면 치우기
    {
        balpanScreen.transform.position = new Vector3(balpanScreen.transform.position.x, 300, balpanScreen.transform.position.z);
        balpanArrow.transform.position = new Vector3(balpanArrow.transform.position.x, 300, balpanArrow.transform.position.z);
        //balpanArrow.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/balpan/spr_balpan_arrow_0");
    }

    private int eventIndexReturn() //어떤 이벤트가 나올지 이후 지정할 필요가 있다. 현재는 0번째 이벤트밖에 나오지 않지만, 나중에는 해당 스테이지에 해당된 랜덤한 이벤트가 나오도록 해야함.
    {
        return 1;
    }
  
    
    private void updateCharacterFace()
    {
        for (int characterIdx = 0; characterIdx < 4; characterIdx++) //캐릭터 얼굴 업로드
        {
            diceObject[characterIdx].transform.rotation = Quaternion.Euler(0, 0, 0);
            if (CharacterManager.Instance.getCharacter(characterIdx) == null || CharacterManager.Instance.getCharacter(characterIdx).getCurState() != 0) {
                lastCharacter[characterIdx] = -99999;
                characterObj[characterIdx].GetComponent<Animator>().runtimeAnimatorController =
                    Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/animator_noneCharacter");

                //diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_no_face");
                continue;
            }
            if (Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getCharacter(characterIdx).getName() + "_face") != null){
                lastCharacter[characterIdx] = CharacterManager.Instance.getCharacter(characterIdx).getDestiny().DestinyIdx;
                characterObj[characterIdx].GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + CharacterManager.Instance.getName_itemManager(characterIdx) + "/animator_" + CharacterManager.Instance.getName_itemManager(characterIdx));
                //diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getCharacter(characterIdx).getName() + "_face");
            }
            else {
                characterObj[characterIdx].GetComponent<Animator>().runtimeAnimatorController =
                Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/animator_noneCharacter");
                //diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_noImage_face");
                }
        }



        hoverOutCharacterDice(0);
    }

    private bool giveUpAble = false;
    [SerializeField]
    public GameObject giveUpBtn;
    [SerializeField]
    public GameObject giveUpBoard;
    [SerializeField]
    public GameObject giveUpText;

    public bool getAdventureSter()
    {
        return adventureStart;
    }

    public void giveUpBtnAble(bool onOff)
    {

        if (!onOff)
        {
            giveUpAble = false;
            activeGiveUpBoard(false);
            giveUpBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/spr_ui_exitButton_lock");
        }
        else if (tutorialVal == 0)
        {
            giveUpText.GetComponent<TextMeshProUGUI>().text = TalkManager.Instance.getDesc(13);
            giveUpAble = true;
            giveUpBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/spr_ui_exitButton_off");
        }
    }

    public void hoverInExitButton()
    {
        if (giveUpAble)
        {
            giveUpBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/spr_ui_exitButton_on");
        }
        else
        {
            giveUpBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/spr_ui_exitButton_lock");
        }
    }
    public void hoverOutExitButton()
    {
        if (giveUpAble)
        {
            giveUpBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/spr_ui_exitButton_off");
        }
        else
        {
            giveUpBtn.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/spr_ui_exitButton_lock");
        }
    }

    


    public bool getGameOverChk()
    {
        return gameOverChk;
    }
    public void clickGiveUpButton()
    {
        closeTryBuyItem(true);

        if (tutorialVal != 0)
        { //튜토리얼 중에는 항복 불가능
            fullUI.showFull(14);
            return;
        }
        activeGiveUpBoard(true);
        optionManager.Instance.unactiveOptionBoard();
    }
    public void activeGiveUpBoard(bool onOff)
    {
        if (giveUpAble && onOff)
        {
            giveUpBoard.SetActive(true);
            giveUpText.GetComponent<TextMeshProUGUI>().text = TalkManager.Instance.getDesc(13);
        }
        if (!onOff) giveUpBoard.SetActive(false);
    }
    private void meetDiceEvent(bool onOff)
    {
        
        eventWatchTrigger = onOff;
        changeSelectNum(0);
        TalkManager.Instance.setDescClickLock(onOff);
        if (onOff) {
            watchNumObjectEntity.transform.position = new Vector3(9f - 500f, -43f, 0f);
        }
        else
        {
            watchNumObjectEntity.transform.position = new Vector3(9f - 500f, 134f, 0f);
        }
    }

    public int adventureBalpanPointTemp = 0;

    public void hoverInBalpanUpDownButton(int idx) {
        hoverOutBalpanUpDownButton();
        ToolBarManager.Instance.setToolBar(14);
        balpanUpDownButton[idx].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
    }
    public void hoverOutBalpanUpDownButton() {
        ToolBarManager.Instance.toolBarOnOff(0);
        balpanUpDownButton[0].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
        balpanUpDownButton[1].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
    }
    public void clickBalpanUpDownButton( int dir)
    {
        if (stageIdx + adventureBalpanPointTemp + dir >= -1 &&
            stageIdx + adventureBalpanPointTemp + dir < adventureEventArr.Length)
        {
            adventureBalpanPointTemp += dir;
        }


        for (int i = 0; i < balpanObj.Length; i++)
        {
            if (stageIdx + adventureBalpanPointTemp + i - 2 < -1)
            {
                setBalpan(i, -1);
            }
            else if (stageIdx + adventureBalpanPointTemp + i - 2 == -1) // 스테이지 시작지점인 경우
            {
                setBalpan(i, -2);
                SoundManager_Sfx.Instance.playSound(3);
            }
            else if (stageIdx + adventureBalpanPointTemp + i - 2 >= adventureEventArr.Length) //넘어가는 경우는 출력하지 않는다
            {
                setBalpan(i, -1);
            }
            else
            {
                setBalpan(i, adventureEventList[stageNum][adventureEventArr[stageIdx + adventureBalpanPointTemp + i - 2]]);
                SoundManager_Sfx.Instance.playSound(3);
            }
        }
        if (adventureBalpanPointTemp * -1 + 2 >= 0 && adventureBalpanPointTemp * -1 + 2 < 10)
        {
            balpanArrow.transform.position = balpanObj[adventureBalpanPointTemp * -1 + 2].transform.position;
        }
        else
        {
            balpanArrow.transform.position = new Vector3(0f, -300f, 0f);
        }
    }
    public void clickBalpanUpDownButton(bool soundOnOff, int dir)
    {
        if (stageIdx + adventureBalpanPointTemp + dir >= -1 &&
            stageIdx + adventureBalpanPointTemp + dir < adventureEventArr.Length) {
            adventureBalpanPointTemp += dir;
        }


        for (int i = 0; i < balpanObj.Length; i++)
        {
            //shakeObject(balpanObj[i]);
            if (stageIdx + adventureBalpanPointTemp + i - 2 < -1)
            {
                setBalpan(i, -1);
            }
            else if (stageIdx + adventureBalpanPointTemp + i - 2 == -1) // 스테이지 시작지점인 경우
            {
                setBalpan(i, -2);
                if (soundOnOff) SoundManager_Sfx.Instance.playSound(3);
            }
            else if (stageIdx + adventureBalpanPointTemp + i - 2 >= adventureEventArr.Length) //넘어가는 경우는 출력하지 않는다
            {
                setBalpan(i, -1);
            }
            else
            {
                setBalpan(i, adventureEventList[stageNum][adventureEventArr[stageIdx + adventureBalpanPointTemp + i - 2]]);
                if (soundOnOff) SoundManager_Sfx.Instance.playSound(3);
            }
        }
        if (adventureBalpanPointTemp * -1 + 2 >= 0 && adventureBalpanPointTemp * -1 + 2 < 10){
            balpanArrow.transform.position = balpanObj[adventureBalpanPointTemp * -1 + 2].transform.position;
        }
        else
        {
            balpanArrow.transform.position = new Vector3(0f, -300f, 0f);
        }
    }

    private void diceSelectInit()
    {
        
        selectDiceCharacterIdx = -1;
        for (int diceIdx = 0; diceIdx < 4; diceIdx++)
        {
            hoverOutCharacterDice(diceIdx);
            hoverInChangeSelectNumByDice(-1);
            diceObject[diceIdx].GetComponent<hoverRotate>().expandEnd();
        }
    }
    [SerializeField]
    public GameObject[] upgradeTagEntity = new GameObject[4];
    public GameObject[] upgradeTagType = new GameObject[4];
    public GameObject[] upgradeTagText = new GameObject[4];
    
    private float[] upgradeTagAmount = new float[4];
    private int[] upgradeTagTypeVal = new int[4];

    public void tagInit()
    {
        for (int i=0;i<4;i++)
        {
            upgradeTagEntity[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            upgradeTagType[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            upgradeTagAmount[i] = 0f;
            upgradeTagTypeVal[i] = -1;
            upgradeTagText[i].GetComponent<TextMeshPro>().text = "";
        }
    }
    public void hoverInTagType(int idx)
    {
        if(upgradeTagTypeVal[idx] >= 0){
            itemManager.Instance.hoverInInfo(upgradeTagTypeVal[idx]);
        }
    }
    public void hoverOutTagType()
    {
        ToolBarManager.Instance.toolBarOnOff(0);
    }
    public void setTag(int idx, int opt, int val)
    {
        //0 : 체력 / 1: 최대체력 / 2:마나 / 3:최대 마나 / 4:방어도 / 5:공격력 / 6:마법 감응력/ 7 : 스피드 -> 캐릭터 전투 기준
        //설명에서의 능력치는 아래함수에 의해 변경됨.
        switch (opt)
        {
            case 0: 
                opt = 4;
                break;
            case 1:
                opt = 5;
                break;
            case 4:
                opt = 3;
                break;
            case 5:
                opt = 0;
                break;
            case 6:
                opt = 1;
                break;
            case 7:
                opt = 2;
                break;
        }

        upgradeTagTypeVal[idx] = opt;

        switch (upgradeTagTypeVal[idx])
        {
            case 0:
                upgradeTagType[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_info_atk_1212_2");
                break;
            case 1:
                upgradeTagType[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_info_mag_1212_2");
                break;
            case 2:
                upgradeTagType[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_info_speed_1212_2");
                break;
            case 3:
                upgradeTagType[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_info_armor_1212_2");
                break;
            case 4:
                upgradeTagType[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_info_hp_1212_2");
                break;
            case 5:
                upgradeTagType[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_info_maxhp_1212_2");
                break;
            default:
                upgradeTagType[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                break;
        }   

        if (val < 0) {
            upgradeTagText[idx].GetComponent<TextMeshPro>().text = val.ToString();
            upgradeTagEntity[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/loading/spr_subStatePost");
        }
        else if(val > 0)
        {
            upgradeTagText[idx].GetComponent<TextMeshPro>().text = "+" + val.ToString();
            upgradeTagEntity[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/loading/spr_addStatePost");
        }
        else
        {
            upgradeTagText[idx].GetComponent<TextMeshPro>().text = "";
            upgradeTagEntity[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
    }

    public void downgradeEff(int opt, int val, int characterIdx, int height)
    {
        if (opt == 0)
        {
            SoundManager_Sfx.Instance.playSound(Random.Range(8, 11));
        }
        specialTextManager.GetComponent<ExampleTextManager>().printAdventureUpgrade(opt, val, characterIdx, height);
        characterObj[characterIdx].GetComponent<Animator>().Play("Hit");

    }
    public void upgradeEff(int opt, int val, int characterIdx, int height)
    {
        specialTextManager.GetComponent<ExampleTextManager>().printAdventureUpgrade(opt, val, characterIdx, height);
    }

    public GameObject specialTextManager;

    public void setBalpan(int balpanIdx, int type)
    {
        balpanNewMark[balpanIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        balpanEventIdx[balpanIdx] = -1; balpanEventType[balpanIdx] = -1;
        if (type == -1){ // balpan이 빈경우
            balpanObj[balpanIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");            
        }
        if (type == -2) { // 시작 발판인 경우
            balpanObj[balpanIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/balpan/newBalpan/spr_balpanNew_empty");
        }
        
        
    }
    public void setBalpan(int balpanIdx, adventureEvent adventureEventTemp)
    {
       
        balpanEventIdx[balpanIdx] = adventureEventTemp.getEventIdx();
        balpanEventType[balpanIdx] = adventureEventTemp.getEventType();
        if (tutorialVal == 0 && !jsonDataManager.Instance.getEventMeet(balpanEventIdx[balpanIdx]) && adventureEventTemp.getEventType() < 98)
        {
            balpanObj[balpanIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/balpan/newBalpan/spr_balpanNew_new" );//이벤트에 관련된 발판으로 이미지 변경
            balpanNewMark[balpanIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_newMark");
        }
        else
        {
            if(adventureEventTemp.getEventType() < 100) balpanObj[balpanIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/balpan/newBalpan/spr_balpanNew_" + adventureEventTemp.getEventType().ToString());//이벤트에 관련된 발판으로 이미지 변경
            else if (adventureEventTemp.getEventType() >= 100) {
                balpanObj[balpanIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/balpan/newBalpan/spr_balpanNew_100");//이벤트에 관련된 발판으로 이미지 변경
            }
            balpanNewMark[balpanIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
    }
    public bool giveUpChk = false;

    private IEnumerator phase_Manage_Coroutine(int stageNumTemp)
    {
        giveUpChk = false;
        adventureStartChk = true;
        resetItemResult();

        upDownManager.Instance.setInit(adventureGold, adventureJewel);

        meetDiceEvent(false);
        diceEntity.SetActive(false);
        rerollBtn.SetActive(false);
        gameOverAtBattle = false;
        
        gameOverChk = false;
        stageNum = stageNumTemp;
        addMoney(0,0);

        //시작시 이미지 없애기
        selectDiceCharacterIdx = -1;
        adventureBackground.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/loading/adventureBoard_2");
        adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        standObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");

        TalkManager.Instance.setDescIdx(-1);
        //selectInfo.GetComponent<TextMeshPro>().text = "";

        makeStageEventArr(stageNum); //이번 스테이지의 나타나는 이벤트의 종류를 미리 배치한다.
        makeStage_placeBalpan(); // 스테이지에 맞춰 발판 생성
        stageIdx = -1;
        updateCharacterFace();
        

        resetItemResult();          //이전 결과물로 나온 아이템들을 얻지 못하게 초기화.
        resultObj.SetActive(false);

        SoundManager_Main.Instance.playSound(2);

        //스테이지 시작시 나오는 인생 이미지 종료
        #region
        lifeObj.SetActive(true);
        lifeObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/lifeImage/spr_life_" + stageNum.ToString());
        float lifeAlpha = 0.0f;

        while (lifeAlpha < 1.0f)
        {
            lifeAlpha += 0.1f;
            lifeObj.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, lifeAlpha);
            lifeObj_back.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, lifeAlpha);
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(2.0f);
        if (TalkManager.Instance.stageStart(stageNum))
        {
            yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
        }
        while (lifeAlpha > 0.0f)
        {
            lifeAlpha -= 0.1f;
            lifeObj.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, lifeAlpha);
            lifeObj_back.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, lifeAlpha);
            yield return new WaitForSeconds(0.05f);
        }
        lifeObj.SetActive(false);
        #endregion
        // 스테이지 시작시 나오는 인생 이미지 종료

        diceBtnFire.Stop();
        // 스테이지 끝 혹은 주사위 이벤트가 끝날때까지 유지되도록 (StartCoroutine이랑 하나 계속 돌아가게 하는 것중 뭐가 더 비용 비싼지 확인할것) 살려두는게 쌀것 같긴함.
        while (//stageIdx < 20 &&
               !gameOverChk)
        {
            giveUpBtnAble(false);
            closeTryBuyItem(true);
            eventWatchNum = -1;
            selectDiceNum = -1; // 플레이어가 주사위 던질 대상을 선택할 수 있도록

            for (int i = 0; i < balpanObj.Length; i++) { setBalpan(i, -1); }
            diceSelectInit();
            
            balpanScreen.GetComponent<Animator>().Play("Open");
            hoverOutBalpanUpDownButton();
            balpanUpDownButton[0].SetActive(false);
            balpanUpDownButton[1].SetActive(false);
            balpanScreen.transform.position = new Vector3(balpanScreen.transform.position.x, 0, balpanScreen.transform.position.z);
            loadEnd = false;
            clickAble = false; // 주사위 클릭 못하게
            clickAbleObjSet(nextBtnObj, false, 1);

            yield return new WaitUntil(() => loadEnd);
            resetItemResult();
            tagInit();
            adventureBalpanPointTemp = 0;
            //발판 이벤트를 위한 이펙트
            //setBalpan(stageIdx);
            for (int i = 0; i < balpanObj.Length; i++)
            {
                shakeObject(balpanObj[i]);
                if (stageIdx + i - 2 < -1) {
                    setBalpan(i, -1);
                }
                else if (stageIdx + i - 2 == -1) // 스테이지 시작지점인 경우
                {
                    setBalpan(i, -2);
                    SoundManager_Sfx.Instance.playSound(3);
                    yield return new WaitForSeconds(0.1f);
                }
                else if (stageIdx + i - 2 >= adventureEventArr.Length) //넘어가는 경우는 출력하지 않는다
                {
                    setBalpan(i, -1);
                }
                else
                {
                    setBalpan(i, adventureEventList[stageNum][adventureEventArr[stageIdx + i - 2]]);
                    SoundManager_Sfx.Instance.playSound(3);
                    yield return new WaitForSeconds(0.08f);
                }
            }
            if (gameOverChk) { continue; }

            for (int i = 0; i < 4; i++)
            {
                if (CharacterManager.Instance.getCharacter(i) != null && CharacterManager.Instance.getCharacter(i).getCurState() == 0)
                { shakeObject(diceObject[i]); }
            }

            //error 발생 지점.
            balpanUpDownButton[0].SetActive(true);
            balpanUpDownButton[1].SetActive(true);
            balpanCurPointText.GetComponent<TextMeshPro>().text = (stageIdx+1).ToString() + " / " + (adventureEventArr.Length ).ToString();

            if (stageIdx == -1) balpanCurPointText.GetComponent<TextMeshPro>().text = "START!";
            balpanArrow.GetComponent<Animator>().Play("Idle");
            balpanArrow.transform.position = balpanObj[2].transform.position; //+ new Vector3(0, 8, 0);
            BattleManager.Instance.makeCoin(1, balpanArrow.transform.position);
            clickAble = true;
            clickAbleObjSet(nextBtnObj, true, 1);
            //나아갈수 있다는 것을 주사위에 표시
            nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_goAhead");
            nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, 0);

            TalkManager.Instance.setDescClickLock(true);

            TalkManager.Instance.setDescIdx(1);
            

            diceBtnFire.Play();
            if (tutorialVal == 1) {
                
                TalkManager.Instance.startTalk(4); //4
                yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
            } //튜토리얼에서 주사위 굴리기를 알려주기 위한 대화

            if (tutorialVal != 0 && tutorialVal < 17) { witchHatButton.SetActive(false); }
            else witchHatButton.SetActive(true);

            makeAdventureDice(0);

            if (tutorialVal == 1)
            {
                //TalkManager.Instance.startTalk(34);
                //yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                TalkManager.Instance.setTutorialArrow(1);
                setTutorial(2);
                setTutorialVal4ErrorChk(false);
            }
            if (tutorialVal == 18) {
                tutorialVal = 19;
                TalkManager.Instance.setDescClickLock(true);
                TalkManager.Instance.setDescIdx(62);
                TalkManager.Instance.startTalk(47);
                yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                TalkManager.Instance.setTutorialArrow(16);
                yield return new WaitUntil(() => tutorialVal == 20);
                TalkManager.Instance.resetTutorialArrow();
                TalkManager.Instance.setDescClickLock(true);
                TalkManager.Instance.setDescIdx(1);
            }

            giveUpBtnAble(true);
            yield return new WaitUntil(() => selectDiceNum > 0 || gameOverChk);
            TalkManager.Instance.resetTutorialArrow();

            if (gameOverChk) { continue; }
            giveUpBtnAble(false);

            hoverOutBalpanUpDownButton();
            balpanUpDownButton[0].SetActive(false);
            balpanUpDownButton[1].SetActive(false);

            TalkManager.Instance.setDescClickLock(false);
            TalkManager.Instance.setDescIdx(-1);

            diceEntity.SetActive(false);

            diceBtnFire.Stop();


            int moveCount = selectDiceNum;
            if(stageIdx + selectDiceNum >= adventureEventList[stageNum].Count)  // 넘어간 경우 자제한다
            {
                moveCount = adventureEventList[stageNum].Count - 1 - stageIdx;
                //stageIdx = adventureEventList[stageNum].Count - 1;
            }
            else
            {
                //stageIdx += selectDiceNum; //stage발판 이동
            }

            while (adventureBalpanPointTemp != 0)
            {
                if (adventureBalpanPointTemp > 0) clickBalpanUpDownButton(true ,-1);
                if (adventureBalpanPointTemp < 0) clickBalpanUpDownButton(true, 1);
                yield return new WaitForSeconds(0.01f);
            }
            balpanArrowGoal.transform.position = new Vector3(-330, 247, 0);
            for (int i=0;i<moveCount;i++)
            {
                SoundManager_Sfx.Instance.playSound(4);
                shakeObject(balpanObj[i+1 + 2]);
                

                balpanArrow.transform.position = balpanObj[i + 1 + 2].transform.position;// + new Vector3(0,8,0);
                stageIdx++;
                balpanCurPointText.GetComponent<TextMeshPro>().text = (stageIdx+1).ToString() + " / " + (adventureEventArr.Length).ToString();
                
                if (adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType() >= 98) //만약 무조건 멈춰야 하는 곳인 경우 정지시킨다.
                {
                    balpanEffObj[i + 1 + 2].GetComponent<Animator>().Play("Update");
                    jsonDataManager.Instance.setEventMeet(adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventIdx()); //이벤트 만난거 처리
                    setBalpan(i+1+2, adventureEventList[stageNum][adventureEventArr[stageIdx]]);

                    balpanArrow.GetComponent<Animator>().Play("Stop");
                    SoundManager_Sfx.Instance.playSound(11);
                    break;
                }
                else if(i == moveCount - 1)
                {
                    balpanEffObj[i + 1 + 2].GetComponent<Animator>().Play("Update");
                    jsonDataManager.Instance.setEventMeet(adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventIdx()); //이벤트 만난거 처리
                    setBalpan(i+1+2, adventureEventList[stageNum][adventureEventArr[stageIdx]]);
                    
                    balpanArrow.GetComponent<Animator>().Play("Do");
                    SoundManager_Sfx.Instance.playSound(34);
                    break;
                }
                else
                {
                    BattleManager.Instance.makeCoin(1, balpanArrow.transform.position); //운명조각 얻기
                }
                yield return new WaitForSeconds((0.5f / moveCount));
                
            }
            yield return new WaitForSeconds(1.2f);


            //balpanLoad.GetComponent<Animator>().Play("Off");

            
            balpanArrow.transform.position = new Vector3(balpanArrow.transform.position.x, 300, balpanArrow.transform.position.z);
            for (int i = 0; i < balpanObj.Length; i++) { setBalpan(i, -1); }
            balpanScreen.GetComponent<Animator>().Play("Close");
            loadEnd = false;
            yield return new WaitUntil(() => loadEnd);
            clearBalpan();

            //발판 이벤트 종료 
            stageInfo.GetComponent<TextMeshPro>().text = "";//(stageIdx+1).ToString() + " / " + adventureEventList[stageNum].Count.ToString(); //초기화
            updateCharacterFace();

            resetItemResult();          //이전 결과물로 나온 아이템들을 얻지 못하게 초기화.
            resultObj.SetActive(false);
            storeEntityObj.SetActive(false);
            tagInit();

            if (true)//adventureEventArr[stageIdx] == 1)
            { //주사위 이벤트 일경우 해당 이벤트 진행. 
                nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_goAhead");
                nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, 0);

                //float tempMoveVal = 1.0f;
                float timeVal = 0.0f;
                StartCoroutine(jumpCharacter());
                boardObj.GetComponent<Animator>().Play("Move");
                while (timeVal < 1.0f) {
                    adventureBackground.transform.localPosition = new Vector3(32f + 0.0f - (4 * Mathf.Sin(timeVal * Mathf.PI)), 7.0f + (4 * Mathf.Sin(timeVal * Mathf.PI)), 0f);
                    adventureBackBoard.transform.localPosition = new Vector3(32f + 8.0f + (4 * Mathf.Sin(timeVal * Mathf.PI)), -1.0f - (4 * Mathf.Sin(timeVal * Mathf.PI)), 0f);
                    timeVal += 0.05f;
                    yield return new WaitForSeconds(0.01f);
                }


                eventWatchNum = 0;

                curDiceEvent = new adventureEvent(adventureEventList[stageNum][adventureEventArr[stageIdx]]); //랜덤한 이벤트를 받아온다. -> 현재는 그냥 보드 이벤트 따라가게 함.
                

                if (curDiceEvent.getEventType() == 6) { //이벤트에서 숫자가 의미 있을 경우, 주사위 별 선택지를 확인. 아닌 경우 확인 불가능하도록

                    meetDiceEvent(true);
                }
                else
                {
                    meetDiceEvent(false);
                }


                TalkManager.Instance.setDescSelectText(curDiceEvent);
                //TalkManager.Instance.setDescString(curDiceEvent.getSelectText());

                adventureBackground.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/background/spr_ui_adventureBack_" + curDiceEvent.getBackgroundSprite());

                adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/NPC/spr_ui_NPC_" + curDiceEvent.getNPCSprite());

                selectImage.transform.rotation = Quaternion.Euler(0, 0, 0);


                timeVal = 0.0f;
                float amountTemp = 2.0f;
                while (timeVal < 1.0f)
                {
                    adventureBackground.transform.localPosition = new Vector3(32f + 0.0f + (amountTemp * Mathf.Sin(timeVal * Mathf.PI)), 7.0f - (amountTemp * Mathf.Sin(timeVal * Mathf.PI)), 0f);
                    adventureBackBoard.transform.localPosition = new Vector3(32f + 8.0f - (amountTemp * Mathf.Sin(timeVal * Mathf.PI)), -1.0f + (amountTemp * Mathf.Sin(timeVal * Mathf.PI)), 0f);
                    timeVal += 0.02f;
                    amountTemp -= 0.04f;
                    yield return new WaitForSeconds(0.01f);
                }

                //selectImage.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + (eventWatchNum + 1).ToString());

                if (curDiceEvent.getEventType() != 6)//(curDiceEvent.getDiceUse() == 0)//주사위 사용이 필요가 없다면, 맨 첫번째 결과가 나오게 하여 그냥 넘길수 있게 한다.
                {
                    selectDiceNum = -1;
                }
                else if (curDiceEvent.getEventType() == 6)//고를 수 있는 상태로 변경
                {
                    
                    diceBtnFire.Play();
                    diceSelectInit();
                    makeAdventureDice(0);
                    selectDiceNum = -1;

                    if (tutorialVal == 2) //아이템 칸 설명을 위한 대화로 넘어가기.
                    {

                        //TalkManager.Instance.startTalk(49);
                        //yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                        TalkManager.Instance.setTutorialArrow(2);
                        setTutorialVal4ErrorChk(false);
                    }

                }
                giveUpBtnAble(true);
                yield return new WaitUntil(() => selectDiceNum > 0 || gameOverChk); // 주사위 쓸 영웅 선택 대기
                TalkManager.Instance.resetTutorialArrow();
                if (gameOverChk) continue;
                giveUpBtnAble(false);

                diceEntity.SetActive(false);
                if (gameOverChk) { break; }

                diceBtnFire.Stop();
                if (curDiceEvent.getEventType() == 6) 
                {
                    diceBtnFire.Play();
                    //Instantiate(diceRollEff, nextBtnObj.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 4) * -90));
                    //SoundManager_Sfx.Instance.playSound(0);
                }

                adventureBackground.GetComponent<hoverRotate>().shakeStart(10.0f);
                meetDiceEvent(false);
                for (int i = 0; i < 6; i++)
                {
                    watchNumObject[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
                }

                eventWatchNum = selectDiceNum - 1;

                //selectInfo.GetComponent<TextMeshPro>().text = curDiceEventPacket.getChooseText();//선택지 텍스트 변경
                //eventInfo.GetComponent<TextMeshPro>().text = curDiceEventPacket.getResultText();



                if (curDiceEvent.getEventType() == 6) //주사위를 굴리는 이벤트일 경우, 주사위 결과 반영해 NPC 스프라이트 변경
                {
                    
                    curDiceEventPacket = curDiceEvent.getPacket(eventWatchNum);
                    adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/" + curDiceEvent.getEventName() + "/spr_ui_NPC_" + curDiceEvent.getEventName() + "_" + curDiceEventPacket.getSpriteIndex());

                    TalkManager.Instance.setDescResultText(curDiceEventPacket);
                    //TalkManager.Instance.setDescString(curDiceEventPacket.getResultText());
                }
                else
                {
                    nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_goAhead");
                    nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                    curDiceEventPacket = curDiceEvent.getPacket(0); // 주사위 결과가 의미 없는 경우 0번째 packet으로 변경
                    adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/NPC/spr_ui_NPC_" + curDiceEventPacket.getSpriteIndex()); //적힌 sprite받아오기
                    TalkManager.Instance.setDescResultText(curDiceEventPacket);
                    //TalkManager.Instance.setDescString(curDiceEventPacket.getResultText());
                }

                if (curDiceEventPacket.getSelectType() == 3 || curDiceEventPacket.getSelectType() == 4 || curDiceEventPacket.getSelectType() == 5) { //능력치 변화
                    tagInit();
                    int tagIdxTemp = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        if (CharacterManager.Instance.getCharacter(selectDiceCharacterIdx) == null || CharacterManager.Instance.getCharacter(selectDiceCharacterIdx).getCurState() != 0)
                        {
                            break;
                        }

                        if (curDiceEventPacket.getVal(i) < 0)
                        {
                            downgradeEff(i, curDiceEventPacket.getVal(i) , selectDiceCharacterIdx, tagIdxTemp);
                            setTag(tagIdxTemp++, i, curDiceEventPacket.getVal(i));
                            if (CharacterManager.Instance.getCharacter(selectDiceCharacterIdx).downGrade(i, -1 * curDiceEventPacket.getVal(i)) == 1)
                            { //약화 효과로 인해 죽어버릴 경우
                                selectDiceCharacterIdx = -1;
                                resetDice();
                                break;
                            }
                        }
                        else if (curDiceEventPacket.getVal(i) > 0)
                        {
                            upgradeEff(i, curDiceEventPacket.getVal(i), selectDiceCharacterIdx, tagIdxTemp);
                            setTag(tagIdxTemp++, i, curDiceEventPacket.getVal(i));
                            CharacterManager.Instance.getCharacter(selectDiceCharacterIdx).upGrade(i, curDiceEventPacket.getVal(i));
                        }
                        
                    }
                }

                if (curDiceEventPacket.getSelectType() == 8) { //상점 시스템
                    
                    storeEntityObj.SetActive(true);
                    for (int tempIdx=0;tempIdx<4;tempIdx++)
                    {
                        Item itemTemp = itemManager.Instance.getRandomItemByChapter(-1, -1);
                        storeItemArr[tempIdx, 0] = itemTemp.getType();
                        storeItemArr[tempIdx, 1] = itemTemp.getIdx();
                        storeItemArr[tempIdx, 2] = itemTemp.getRare() * 10 + 5;
                    }
                    updateStore();
                }
                if (curDiceEventPacket.getSelectType() == 4)
                { //능력치 증가
                    for (int i = 0; i < 8; i++)
                    {
                        CharacterManager.Instance.getCharacter(selectDiceCharacterIdx).upGrade(i, curDiceEventPacket.getVal(i));
                    }

                }
                if (curDiceEventPacket.getSelectType() == 6) //전투를 진행하는 경우
                {
                    SoundManager_Main.Instance.stopSound(2);
                    int battleSoundTemp = 5;
                    if (adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType() == 100)
                    { // 올빼미 선배
                        if (jsonDataManager.Instance.setChapterDid(0, 4)) { 
                            TalkManager.Instance.startTalk(21);
                            yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                        }
                        battleSoundTemp = 17;
                    }
                    else if (adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType() == 102)
                    { // Bard
                        if (jsonDataManager.Instance.setChapterDid(1, 4))
                        {
                            TalkManager.Instance.startTalk(64);
                            yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                        }
                        battleSoundTemp = 26;
                    }
                    
                    else if (adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType() == 98 ||
                        adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType() == 99 ||
                        adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType() == 101) {
                        battleSoundTemp = 18;
                    }

                    SoundManager_Main.Instance.playSound(battleSoundTemp);
                    //nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_Stop");
                    //nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                    BattleManager.Instance.updateBattleBackground(curDiceEventPacket.getBattleBackSprite());

                    BattleManager.Instance.changeBossPhase(adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType());
                    updateCharacterFace();
                    for (int i = 0; i < 4; i++)
                    {
                        //마지막 전투에서의 캐릭터 정보를 확인
                        CharacterManager.Instance.emptyEnemyCharacter(i);
                        if (curDiceEventPacket.getSelectType() != -99999) CharacterManager.Instance.setCharacter(i, curDiceEventPacket.getVal(i));
                    }
                    /*
                    hoverRotateAble(battleBtn, 2, true);
                    hoverRotateAble(battleBtn, 1, true);
                    shakeObject(battleBtn);
                    battleBtn.transform.position = nextBtnObj.transform.position;
                    */
                    nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_Battle");
                    battleEventTrigger = true;

                    giveUpBtnAble(true);
                    yield return new WaitUntil(() => !battleEventTrigger || gameOverChk); //돌아올때까지 대기

                    SoundManager_Main.Instance.stopSound(battleSoundTemp);
                    SoundManager_Main.Instance.playSound(2);
                    updateCharacterFace();
                    if (gameOverChk) { break; }
                    giveUpBtnAble(false);

                    nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_goAhead");
                    nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                    if (selectDiceCharacterIdx == -1 || CharacterManager.Instance.getCharacter(selectDiceCharacterIdx) == null || CharacterManager.Instance.getCharacter(selectDiceCharacterIdx).getCurState() != 0)
                    {
                        selectDiceCharacterIdx = -1; //전투 후 돌아오면 해당 캐릭터가 생존했는지 확인한 다음 돌아올 수 있게 바꿀것. 
                        standObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                    }
                    
                    //for(int i=0;i<4;i++) CharacterManager.Instance.emptyEnemyCharacter(i); //돌아오면 적군 캐릭터 모두 없애기

                    adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                    TalkManager.Instance.setDescIdx(16);
                    //selectInfo.GetComponent<TextMeshPro>().text = TalkManager.Instance.getDesc(16);
                }

                

                if (curDiceEventPacket.getItemExist() == 1) { //이벤트 결과로 정해진 아이템을 준다.
                    resultObj.SetActive(true);
                    for (int i = 0; i < 4; i++)   //각 칸에 대한 처리
                    {
                        resultItemArr[i, 0] = curDiceEventPacket.getItemType(i);
                        resultItemArr[i, 1] = curDiceEventPacket.getItemIdx(i);
                        resultNewMark[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                        //결과로 나오는 아이템에 대한 이미지 처리
                        if (resultItemArr[i, 0] == -99999 || resultItemArr[i, 1] == -99999)
                        {
                            
                            clickAbleObjSet(resultObjArr[i], false, 1);
                            clickAbleObjSet(resultObjArr[i], false, 2);
                            resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                        }
                        else  //캐릭터를 얻는 이벤트의 경우
                        {
                            clickAbleObjSet(resultObjArr[i], true, 1);
                            clickAbleObjSet(resultObjArr[i], true, 2);
                            if (resultItemArr[i, 0] == 4) {
                                CharacterManager.Instance.setCharacter_destinyBase(ref resultCharacter[i], resultItemArr[i, 1]); //getCharacter(resultItemArr[i, 1]);
                                for (int j = 0; j < 6; j++) resultCharacter[i].changeDiceNum(j, Random.Range(1, 7));
                                
                                if (tutorialVal != 0) //튜토리얼에서는 1만 넣는다.
                                {
                                    for (int j = 0; j < 6; j++) resultCharacter[i].changeDiceNum(j, 1);
                                }
                                if (!jsonDataManager.Instance.getPlayerCharacterAble(resultItemArr[i, 1])) { 
                                    resultNewMark[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_newMark"); 
                                }
                                resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getDestiny(resultItemArr[i, 1]).getName() + "_face");
                            }
                            else resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(itemManager.Instance.getItemSprite(resultItemArr[i, 0], resultItemArr[i, 1]));
                        }
                    }
                    if (tutorialVal == 2) //아이템 칸 설명을 위한 대화로 넘어가기.
                    {
                        setTutorialVal4ErrorChk(false);
                        TalkManager.Instance.setDescClickLock(true);
                        TalkManager.Instance.setDescIdx(51);
                        TalkManager.Instance.setTutorialArrow(3);
                        //TalkManager.Instance.startTalk(35);
                        //yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                        eventEndClick = true;
                        clickAble = false;
                        clickAbleObjSet(nextBtnObj, false, 1);


                        yield return new WaitUntil(() => (resultItemArr[0, 0] == -99999 && resultItemArr[1, 0] == -99999 && resultItemArr[2, 0] == -99999 && resultItemArr[3, 0] == -99999 &&
                                                        resultItemArr[0, 1] == -99999 && resultItemArr[1, 1] == -99999 && resultItemArr[2, 1] == -99999 && resultItemArr[3, 1] == -99999 ));
                        TalkManager.Instance.resetTutorialArrow();
                        TalkManager.Instance.setDescClickLock(true);
                        TalkManager.Instance.setDescIdx(52);
                        TalkManager.Instance.setTutorialArrow(4);
                        //TalkManager.Instance.startTalk(36);
                        //yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                        setTutorialVal4ErrorChk(true);
                        yield return new WaitUntil(() => tutorialVal == 3);
                        TalkManager.Instance.resetTutorialArrow();
                        setTutorialVal4ErrorChk(false);
                        
                        TalkManager.Instance.startTalk(37);
                        yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                        TalkManager.Instance.setTutorialArrow(5);
                        
                        setTutorialVal4ErrorChk(true);
                        TalkManager.Instance.setDescClickLock(true);
                        TalkManager.Instance.setDescIdx(53);
                        yield return new WaitUntil(() => tutorialVal == 4);
                        TalkManager.Instance.resetTutorialArrow();
                        setTutorialVal4ErrorChk(false);
                        TalkManager.Instance.setDescClickLock(false);
                        TalkManager.Instance.setDescIdx(54);
                        clickAble = true;
                        clickAbleObjSet(nextBtnObj, true, 1);
                    }
                    else if (tutorialVal == 4)
                    {
                        TalkManager.Instance.setTutorialArrow(6);
                        TalkManager.Instance.setDescClickLock(true);
                        TalkManager.Instance.setDescIdx(55);
                        eventEndClick = true;
                        clickAble = false;
                        clickAbleObjSet(nextBtnObj, false, 1);
                        yield return new WaitUntil(() => (resultItemArr[0, 0] == -99999 && resultItemArr[1, 0] == -99999 && resultItemArr[2, 0] == -99999 && resultItemArr[3, 0] == -99999 &&
                                                        resultItemArr[0, 1] == -99999 && resultItemArr[1, 1] == -99999 && resultItemArr[2, 1] == -99999 && resultItemArr[3, 1] == -99999));
                        TalkManager.Instance.setTutorialArrow(7);

                        TalkManager.Instance.setDescClickLock(true);
                        TalkManager.Instance.setDescIdx(56);
                        //TalkManager.Instance.startTalk(38);
                        setTutorialVal4ErrorChk(true);
                        //yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                        yield return new WaitUntil(() => tutorialVal == 5);
                        TalkManager.Instance.setTutorialArrow(8);
                        TalkManager.Instance.setDescClickLock(true);
                        TalkManager.Instance.setDescIdx(57);
                        yield return new WaitUntil(() => tutorialVal == 6);
                        TalkManager.Instance.resetTutorialArrow();
                        TalkManager.Instance.setDescIdx(53);
                        yield return new WaitUntil(() => tutorialVal == 7);
                        TalkManager.Instance.setDescClickLock(false);
                        TalkManager.Instance.setDescIdx(54);
                        clickAble = true;
                        clickAbleObjSet(nextBtnObj, true, 1);
                    }
                }
 
                if (curDiceEventPacket.getItemExist() >= 11 && curDiceEventPacket.getItemExist() <= 14 ) // 랜덤한 아이템을 준다.
                {
                    resultObj.SetActive(true);
                    for (int i = 0; i < 4; i++)   // 보상 수만큼 해주기
                    {
                        resultNewMark[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                        if (curDiceEventPacket.getItemExist() % 10 > i) //아이템 수 만큼만 지급.
                        {
                            Item itemTemp = itemManager.Instance.getRandomItemByChapter(-1, -1);

                            resultItemArr[i, 0] = itemTemp.getType();
                            resultItemArr[i, 1] = itemTemp.getIdx();
                        }
                        else
                        {
                            resultItemArr[i, 0] = -99999;
                            resultItemArr[i, 1] = -99999;
                        }
                        //결과로 나오는 아이템에 대한 이미지 처리
                        if (resultItemArr[i, 0] == -99999 || resultItemArr[i, 1] == -99999)
                        {
                            clickAbleObjSet(resultObjArr[i], false, 1);
                            clickAbleObjSet(resultObjArr[i], false, 2);
                            resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                        }
                        else
                        {
                            clickAbleObjSet(resultObjArr[i], true, 1);
                            clickAbleObjSet(resultObjArr[i], true, 2);
                            resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(itemManager.Instance.getItemSprite(resultItemArr[i, 0], resultItemArr[i, 1]));
                        }
                    }
                }
                if (curDiceEventPacket.getItemExist() >= 21 && curDiceEventPacket.getItemExist() <= 24) // 랜덤한 캐릭터를 준다.
                {
                    resultObj.SetActive(true);
                    for (int i = 0; i < 4; i++)   
                    {
                        resultNewMark[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                        if (curDiceEventPacket.getItemExist() % 10 > i) // 지정된 캐릭터 보상 수만큼 해주기
                        {
                            resultItemArr[i, 0] = 4;
                            resultItemArr[i, 1] = CharacterManager.Instance.getRandomCharacterDestinyIdx();
                            CharacterManager.Instance.setCharacter_destinyBase(ref resultCharacter[i], resultItemArr[i, 1]);
                            for (int j=0;j<6;j++ )resultCharacter[i].changeDiceNum(j, Random.Range(1, 7));
                        }
                        else
                        {
                            resultItemArr[i, 0] = -99999;
                            resultItemArr[i, 1] = -99999;
                        }
                        //결과로 나오는 아이템에 대한 이미지 처리
                        if (resultItemArr[i, 0] == -99999 || resultItemArr[i, 1] == -99999)
                        {
                            resultNewMark[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                            clickAbleObjSet(resultObjArr[i], false, 1);
                            clickAbleObjSet(resultObjArr[i], false, 2);
                            resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                        }
                        else if (resultItemArr[i, 0] == 4) //캐릭터를 얻는 이벤트의 경우
                        {
                            if (!jsonDataManager.Instance.getPlayerCharacterAble(resultItemArr[i, 1]))
                            {
                                resultNewMark[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_newMark");
                            }
                            else
                            {
                                resultNewMark[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                            }
                            clickAbleObjSet(resultObjArr[i], true, 1);
                            clickAbleObjSet(resultObjArr[i], true, 2);
                            resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getDestiny(resultItemArr[i, 1]).getName() + "_face");
                        }
                    }
                }
                if (adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType() == 98 && !gameOverChk && jsonDataManager.Instance.setChapterDid(stageDepth, 2)){ // 1스테이지 중간 보스 클리어
                    if(jsonDataManager.Instance.getChapterRead(stageDepth,0) == 0) jsonDataManager.Instance.setChapterRead(stageDepth,0);
                    giveUpBtnAble(false);

                    if (stageDepth == 0) TalkManager.Instance.startTalk(33);
                    else TalkManager.Instance.startTalk(66);
                    
                    yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                    giveUpBtnAble(true);
                }
                if (adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType() == 99 && !gameOverChk && jsonDataManager.Instance.setChapterDid(stageDepth, 3))
                { // 1스테이지 최종 보스 클리어
                    if (jsonDataManager.Instance.getChapterRead(stageDepth, 1) == 0) jsonDataManager.Instance.setChapterRead(stageDepth, 1);
                    giveUpBtnAble(false);
                    TalkManager.Instance.startTalk(32);
                    yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                    giveUpBtnAble(true);
                }
                //데모 보스 클리어 확인
                if (adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType() == 100 && !gameOverChk ) { // 올빼미 선배 클리어
                    if (jsonDataManager.Instance.getChapterRead(0, 2) == 0) jsonDataManager.Instance.setChapterRead(0, 2);
                    giveUpBtnAble(false);
                    demoEndChk = 1;
                    gameOverChk = true;
                }
                if (adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType() == 102 && !gameOverChk)
                { // 바르드 클리어
                    if (jsonDataManager.Instance.getChapterRead(1, 2) == 0) jsonDataManager.Instance.setChapterRead(1, 2);
                    giveUpBtnAble(false);
                    demoEndChk = 3;
                    gameOverChk = true;
                }
                //튜토리얼 보스 클리어 확인
                if (adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType() == 101 && !gameOverChk)
                {
                    giveUpBtnAble(false);
                    demoEndChk = 2;
                    gameOverChk = true;
                    jsonDataManager.Instance.tutorialDid();
                }

                if (gameOverChk == false)
                {
                    eventEndClick = true;
                    //nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                    //nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_goAhead");
                    giveUpBtnAble(true);
                    yield return new WaitUntil(() => !eventEndClick || gameOverChk);
                    if (gameOverChk) continue;
                    giveUpBtnAble(false);
                }
                storeEntityObj.SetActive(false);
                resultObj.SetActive(false);

            }
        }
        giveUpBtnAble(false);
        adventureStart = true;

        //종료시 기본적으로 해줘야할 처리.
        balpanArrow.transform.position = new Vector3(balpanArrow.transform.position.x, 300, balpanArrow.transform.position.z);
        for (int i = 0; i < balpanObj.Length; i++) { setBalpan(i, -1); }
        balpanScreen.GetComponent<Animator>().Play("Close");
        loadEnd = false;
        storeEntityObj.SetActive(false);
        resultObj.SetActive(false);
        closeTryBuyItem(true);
        clearBalpan();

        bool nextStageGo = false; //다음 단계로 나아갈지 말지 정하는 변수.

        if (gameOverChk) //게임오버로 왔을 경우.
        {
            
            SoundManager_Main.Instance.stopSound(2); //기본 브금 제거
            SoundManager_Main.Instance.playSound(3); //기본 브금 제거
            TalkManager.Instance.setDescClickLock(false);
            TalkManager.Instance.setLostChk(true);
            TalkManager.Instance.setDescIdx(126);
            //selectInfo.GetComponent<TextMeshPro>().text = "";
            if (demoEndChk != 0 || (giveUpChk == true && !battleEventTrigger))
            { //스테이지 보스 잡은 경우 스테이지 클리어 띄우기
                if (demoEndChk == 1) //올빼미 선배 전투 종료시
                {
                    if (!jsonDataManager.Instance.getOwlBattleWin())
                    {
                        jsonDataManager.Instance.owlBattleWin();
                        TalkManager.Instance.startTalk(18);
                        yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                    }
                    stageDepth = 1;
                    //yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                }
                if(demoEndChk == 2) { stageDepth = 1; stageDepthMax = 1; }
                if(demoEndChk == 3)
                {
                    stageDepth = 2;
                }


                if (giveUpChk && !battleEventTrigger)
                { //항복한 경우
                    CameraManager.Instance.resultScreenActive(1);
                    yield return new WaitUntil(() => !(CameraManager.Instance.getLoseScreenActive()));
                }
                else if (stageDepth == stageDepthMax)    //스테이지가 Max까지 도달한 경우
                {
                    CameraManager.Instance.resultScreenActive(2);
                    if (demoEndChk == 2)
                    { //튜토리얼 종료
                        TalkManager.Instance.startTalk(50);
                        yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                    }
                    yield return new WaitUntil(() => !(CameraManager.Instance.getLoseScreenActive()));
                }
                else { // 스토리 보스 클리어 & 아직 최대 깊이 도달 X 인경우
                    SoundManager_Main.Instance.stopSound(3); //기본 브금 제거
                    nextStageGo = true;
                }


                if (demoEndChk == 2 ) //튜토리얼 종료시
                {
                    if (jsonDataManager.Instance.setChapterDid(0, 1))
                    {
                        SoundManager_Main.Instance.stopSound(3); //기본 브금 제거
                        TalkManager.Instance.startTalk(13);
                        yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                        TalkManager.Instance.startTalk(16);
                    }
                    tutorialVal = 0;
                }
                demoEndChk = 0;
            }
            else if(!gameOverAtBattle){
                CameraManager.Instance.resultScreenActive(0);

                if (!jsonDataManager.Instance.getFirstGetCharacterPart())
                {
                    bool newDestinyChk = false;
                    for (int i = 0; i < 4; i++)
                    {
                        int destinyIdx = AdventureManager.Instance.getLastCharacter(i); //마지막으로 전투했던 캐릭터들 정보 얻기.
                        if (destinyIdx != -99999 && destinyIdx != 0) newDestinyChk = true;
                    }
                    if (newDestinyChk)
                    {
                        jsonDataManager.Instance.firstGetCharacterPart();
                        //TalkManager.Instance.startTalk(20);
                        //yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                    }
                }

                yield return new WaitUntil(() => !(CameraManager.Instance.getLoseScreenActive()));
            }

            if (!nextStageGo)
            {
                SoundManager_Main.Instance.stopSound(3); //기본 브금 제거
                                                         //gameOverChk가 true가 되면 끝
                CharacterManager.Instance.resetCharacterManager();
                selectDiceCharacterIdx = -1;
                updateCharacterFace();
                itemManager.Instance.resetItemManager();
                //시작시 이상하지 않도록
                selectDiceCharacterIdx = -1;
                adventureBackground.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/loading/adventureBoard_2");
                adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                standObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");

                TownManager.Instance.backToTownUI();

                adventureJewel = 0;
                addMoney(1, 0);

                TalkManager.Instance.setLostChk(false);
                TalkManager.Instance.setDescString("");

                initMaxVal();
            }
            
        }
        adventureStartChk = false;
        giveUpBtnAble(true);
        if (nextStageGo)
        {
            if (stageDepth == 1)
            {
                if (jsonDataManager.Instance.getChapterRead(1, 2) != 2)
                {// 2챕터 클리어전 depth 1
                    StartCoroutine(phase_Manage_Coroutine(3));
                }
                else
                {// 2챕터 클리어후 depth 1
                    StartCoroutine(phase_Manage_Coroutine(3));
                }
            }

            TalkManager.Instance.setLostChk(false);
            TalkManager.Instance.setDescString("");
        }
        
    }

    [SerializeField]
    public GameObject coinEff;

    public void addMoney(int opt, int val) { //opt  0: gold 1: jewel
        if (opt == 0) {
            adventureGold += val;
            if (val < 0) upDownManager.Instance.addGold(val);
        }
        else if(opt == 1) {
            adventureJewel += val;
            if (val < 0) upDownManager.Instance.addJewel(val);
        }


    }

    public void giveUpAdventure()
    {
        if (!giveUpAble) return;

        if (battleEventTrigger) {
            BattleManager.Instance.useGiveUpBtn();
        }

        if (selectDiceNum <= 0)
        {
            selectDiceNum = 1;
        }
        if(eventEndClick){ 
            eventEndClick = false;
        }
        giveUpChk = true;
        gameOverChk = true;
        activeGiveUpBoard(false);
        //CameraManager.Instance.updateInitPosition(new Vector3(-1500f, 500f, CameraManager.Instance.cameraPointZ()));
        //CameraManager.Instance.resultScreenActive(0);
    }

    private int demoEndChk = 0;
    public void clickResultItem(int idx)
    {
        //이벤트가 종료된 상태이고, 해당 아이템들이 유효할때
        if (eventEndClick && resultItemArr[idx, 0] != -99999 && resultItemArr[idx, 1] != -99999)
        {
            ToolBarManager.Instance.toolBarOnOff(0);
            if (resultItemArr[idx, 0] == 4) //캐릭터 습득일 경우
            {
                int emptyPlaceExist = -1;
                for (int i = 0; i < 4; i++)
                {
                    if (CharacterManager.Instance.getCharacter(i) == null || CharacterManager.Instance.getCharacter(i).getCurState() != 0)
                    {
                        emptyPlaceExist = i;
                        break;
                    }
                }
                if (emptyPlaceExist == -1)
                {
                    SoundManager_Sfx.Instance.playSound(7);
                    fullUI.showFull(0);
                }
                else
                {
                    SoundManager_Sfx.Instance.playSound(3);
                    CharacterManager.Instance.setCharacter(emptyPlaceExist, resultItemArr[idx, 1]);
                    resetDice();
                    smokeCharacter(emptyPlaceExist);
                    SoundManager_Sfx.Instance.playSound(72);
                    for (int i = 0; i < 6; i++)
                    {
                        CharacterManager.Instance.getCharacter(emptyPlaceExist).changeDiceNum(i, resultCharacter[idx].getDice(i)); // 주사위 랜덤으로 변경
                    }
                    if (AdventureManager.Instance.getTutorial() != 0)
                    {
                        for (int i = 0; i < 6; i++) CharacterManager.Instance.getCharacter(emptyPlaceExist).changeDiceNum(i, 1); // 주사위 랜덤으로 변경
                    }
                    resultObjArr[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none"); //정상종료

                    resultNewMark[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");


                    resultItemArr[idx, 0] = -99999;
                    resultItemArr[idx, 1] = -99999;
                    clickAbleObjSet(resultObjArr[idx], false, 1);
                    clickAbleObjSet(resultObjArr[idx], false, 2);
                    updateCharacterFace();
                }
            }
            else
            {
                int result = itemManager.Instance.getItemResult(resultItemArr[idx, 0], resultItemArr[idx, 1]);
                if (result == 0)
                {
                    SoundManager_Sfx.Instance.playSound(3);
                    resultObjArr[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none"); //정상종료
                    resultNewMark[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                    resultItemArr[idx, 0] = -99999;
                    resultItemArr[idx, 1] = -99999;
                    clickAbleObjSet(resultObjArr[idx], false, 1);
                    clickAbleObjSet(resultObjArr[idx], false, 2);

                }
                else if (result == 1) //꽉차서 못담는 경우.
                {
                    SoundManager_Sfx.Instance.playSound(7);
                    fullUI.showFull(0);
                }
                else if (result == 2)
                {
                    Debug.Log("Error, this is not exist item");
                }
            }
        }

    }
    public void changeSelectNum(bool upEvent)
    { //현재 아래 방향이 상승
        if (eventWatchTrigger) {
            if (eventWatchNum < 6 && eventWatchNum >= 0)
            {
                if (!upEvent)
                {
                    if (eventWatchNum == 5) eventWatchNum = 0;
                    else eventWatchNum++;
                }
                else
                {
                    if (eventWatchNum == 0) eventWatchNum = 5;
                    else eventWatchNum--;
                }
                //회전도 적용(나중에 마녀 능력을 위해서)
                TalkManager.Instance.setDescChooseText(curDiceEvent.getPacket(eventWatchNum));
                //TalkManager.Instance.setDescString(curDiceEvent.getPacket(eventWatchNum).getChooseText());
            }
        }
    }

    public void hoverInChangeSelectNumByDice(int idx)
    {
        if (idx == -1) changeSelectNum(0);
        else
        {
            if (CharacterManager.Instance.getCharacter(idx) != null && CharacterManager.Instance.getCharacterState(idx) == 0)
            {
                changeSelectNum(CharacterManager.Instance.getDiceNum(idx));
            }
        }
        
    }
    public void changeSelectNum(int inputNum)
    { //현재 아래 방향이 상승
        if (eventWatchTrigger)
        {

            if (inputNum == 0) {

                for (int i = 0; i < 6; i++)
                {
                    watchNumObject[i].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
                }
                ToolBarManager.Instance.toolBarOnOff(0);
                TalkManager.Instance.setDescSelectText(curDiceEvent);
                //TalkManager.Instance.setDescString(curDiceEvent.getSelectText());
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    if (i + 1 == inputNum)
                    {
                        watchNumObject[i].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
                        //watchNumObject[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.7f);
                    }
                    else {
                        watchNumObject[i].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
                        //watchNumObject[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
                    }
                }
                ToolBarManager.Instance.setToolBar(15);
                eventWatchNum = inputNum - 1;
                TalkManager.Instance.setDescChooseText(curDiceEvent.getPacket(eventWatchNum));
                //TalkManager.Instance.setDescString(curDiceEvent.getPacket(eventWatchNum).getChooseText());//선택지 텍스트 변경    
            }
        }
    }
    public void smokeCharacter(int idx)
    {
        characterSmoke[idx].GetComponent<Animator>().Play("Smoke");
    }
    public void resetDice()
    {

        resetCharacterObjSet();

        selectDiceCharacterIdx = -2; //의미 없는 캐릭터 idx로 변경
        standObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        for (int characterIdx = 0; characterIdx < 4; characterIdx++) //캐릭터 얼굴 업로드
        {
            if (CharacterManager.Instance.getCharacter(characterIdx) != null && CharacterManager.Instance.getCharacter(characterIdx).getCurState() == 0)
            {
                selectDiceCharacterIdx = characterIdx;
                standObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/backImage/spr_" + CharacterManager.Instance.getCharacter(characterIdx).getName() + "_back");
                break;
                
            }
        }
    }
    public void resetDice(int idx)
    {
        if (idx < 0) {resetDice(); return; }
        resetCharacterObjSet();
        selectDiceCharacterIdx = idx;
        standObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/backImage/spr_" + CharacterManager.Instance.getCharacter(idx).getName() + "_back");
    }
    public void resetCharacterObjSet()
    {
        for (int characterIdx = 0; characterIdx < 4; characterIdx++) //캐릭터 얼굴 업로드
        {

            diceObject[characterIdx].transform.rotation = Quaternion.Euler(0, 0, 0);
            if (CharacterManager.Instance.getCharacter(characterIdx) == null || CharacterManager.Instance.getCharacter(characterIdx).getCurState() != 0)
            {
                clickAbleObjSet(diceObject[characterIdx], false, 1);
                clickAbleObjSet(diceObject[characterIdx], false, 2);

                characterObj[characterIdx].GetComponent<Animator>().runtimeAnimatorController =
                Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/animator_noneCharacter");
                characterShadow[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                //diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_no_face");
                continue;
            }

            clickAbleObjSet(diceObject[characterIdx], true, 1);
            clickAbleObjSet(diceObject[characterIdx], true, 2);
            if (Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getCharacter(characterIdx).getName() + "_face") != null)
            {
                characterObj[characterIdx].GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + CharacterManager.Instance.getName_itemManager(characterIdx) + "/animator_" + CharacterManager.Instance.getName_itemManager(characterIdx));
                characterShadow[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/spr_character_shadow_0");
                //diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getCharacter(characterIdx).getName() + "_face");
            }
            else
            {
                characterObj[characterIdx].GetComponent<Animator>().runtimeAnimatorController =
                Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/animator_noneCharacter");
                characterShadow[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                //diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_noImage_face"); 
            }
        }
        hoverOutCharacterDice(0);
    }
    public int getSelectDiceCharacterIdx()
    {
        return selectDiceCharacterIdx;
    }
    public void setSelectDiceCharacterIdx(int input)
    {
        selectDiceCharacterIdx = input;
    }
    public bool rerollChk = false;
    [SerializeField]
    public GameObject rerollBtn;
    public void clickRerollBtn()
    {
        if (selectDiceCharacterIdx >= 0)
        {
            shakeObject(diceObject[selectDiceCharacterIdx]);
            int characterIdx = selectDiceCharacterIdx;
            CharacterManager.Instance.throwDice(characterIdx);
            //selectImage.transform.rotation = Quaternion.Euler(0, 0, CharacterManager.Instance.getDiceDir(characterIdx) * -90);
            //selectImage.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + CharacterManager.Instance.getDiceNum(characterIdx).ToString());
            Instantiate(diceRollEff, nextBtnObj.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 4) * -90));
            SoundManager_Sfx.Instance.playSound(0);

            if (tutorialVal >= 1 && tutorialVal <= 4) { selectDiceNum = 1; }
            else {selectDiceNum = CharacterManager.Instance.getDiceNum(characterIdx); }
            nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, CharacterManager.Instance.getDiceDir(characterIdx) * -90);
            nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/"+ selectDiceNum.ToString());
            
            rerollChk = false;
        }
        else
        {
            for (int i = 0; i < 4; i++) if (CharacterManager.Instance.getCharacter(i) != null && CharacterManager.Instance.getCharacter(i).getCurState() == 0) { shakeObject(diceObject[i]); }
            fullUI.showFull(1);
        }
    }

    public GameObject diceEntity;
    public void makeAdventureDice(int cost)
    {
        if (adventureJewel < cost) {
            fullUI.showFull(70);
            return;
        }
        else
        {
            addMoney(1, -1 * cost);
        }
        if (cost != 0) {
            if (getTutorial() == 19)
            {
                setTutorial(20);
            }
        }
        diceEntity.SetActive(true);
        SoundManager_Sfx.Instance.playSound(2);
        for (int i = 0; i < 4; i++)
        {
            if (CharacterManager.Instance.getCharacter(i) != null && CharacterManager.Instance.getCharacterState(i) == 0)
            {
                //if (cost == 0) CharacterManager.Instance.throwDice(i);
                //else 
                if (cost != 0)
                {
                    CharacterManager.Instance.throwDiceExcept(i);
                }
                else CharacterManager.Instance.throwDice(i);
                int temp = Random.Range(0, 4) * 90;
                Instantiate(diceRollEff, diceObject[i].transform.position, Quaternion.Euler(0, 0, temp)); //사용된 아이템에 대해 effect
                //Instantiate(diceRollEff, nextBtnObj.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 4) * -90));
                diceObject[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + CharacterManager.Instance.getDiceNum(i).ToString());
            }
            else
            {
                diceObject[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");//Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            }
        }
    }
    public void updateAdventureDice()
    {
        if (!diceEntity.activeSelf) return;

        for (int i = 0; i < 4; i++)
        {
            if (CharacterManager.Instance.getCharacter(i) != null && CharacterManager.Instance.getCharacterState(i) == 0)
            {
                diceObject[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + CharacterManager.Instance.getDiceNum(i).ToString());
            }
            else
            {
                diceObject[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            }
        }
    }

    public void clickDice(int characterIdx)
    {
        if (!clickAble) return;
        
        if (tutorialVal == 19) { // 운명 마법사용때문인듯?
            fullUI.showFull(65);
            return;
        }

        if (characterIdx == -1 && battleEventTrigger && selectDiceCharacterIdx >= 0) {
            TalkManager.Instance.setDescIdx(-1);
            enterBattleCanvas();
        }
        
        if (characterIdx == -1 && eventEndClick )
        {
            SoundManager_Sfx.Instance.playSound(7);
            CameraManager.Instance.VibrateForeTime(0.2f);
            eventEndClick = false;
            return;
        }
        //클릭 부분 확인
        diceBtnFire.Play(true);
        if (characterIdx == -1) { //캐릭터가 선택되었고 다음으로 가는 주사위 누를 경우
            if (selectDiceNum == -1) { //아직 주사위를 한번도 안굴렸을 경우.
                if (selectDiceCharacterIdx >= 0)
                {
                    shakeObject(diceObject[selectDiceCharacterIdx]);
                    characterIdx = selectDiceCharacterIdx;
                    
                    if (tutorialVal >= 1 && tutorialVal <= 4) { selectDiceNum = 1; }
                    else selectDiceNum = CharacterManager.Instance.getDiceNum(characterIdx);

                    //nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, CharacterManager.Instance.getDiceDir(characterIdx) * -90);
                    //nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + selectDiceNum.ToString());
                    rerollChk = true;
                    
                    
                    
                }
                else {
                    for (int i = 0; i < 4; i++) if (CharacterManager.Instance.getCharacter(i) != null && CharacterManager.Instance.getCharacter(i).getCurState() == 0) { shakeObject(diceObject[i]); }
                    fullUI.showFull(1);
                }
            }
            /*
            else if (selectDiceNum > 0 && rerollChk) {
                if (selectDiceCharacterIdx >= 0) {
                    rerollChk = false;
                }
                else {
                    for (int i = 0; i < 4; i++) if (CharacterManager.Instance.getCharacter(i) != null && CharacterManager.Instance.getCharacter(i).getCurState() == 0) { shakeObject(diceObject[i]); }
                    fullUI.showFull(1);
                }
            }
            */
        }
        else if (selectDiceNum == -1 || rerollChk) {
            if (characterIdx != -1 && CharacterManager.Instance.getCharacterState(characterIdx) == 0)
            {
                SoundManager_Sfx.Instance.playSound(0);
                selectDiceCharacterIdx = characterIdx;
                hoverOutCharacterDice(selectDiceCharacterIdx);
                //balpanArrow.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + CharacterManager.Instance.getName_itemManager(characterIdx) + "/animator_" + CharacterManager.Instance.getName_itemManager(characterIdx));
                standObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/backImage/spr_" + CharacterManager.Instance.getCharacter(characterIdx).getName() + "_back");
                clickDice(-1);

            }
            else if ( characterIdx != -1 && CharacterManager.Instance.getCharacterState(characterIdx) != 0)
            {
                SoundManager_Sfx.Instance.playSound(7);
            }
        }
    } 

    public void hoverInBalpan(int idx)
    {

        if (balpanEventType[idx] == -1) return;
        balpanObj[idx].GetComponent<SpriteRenderer>().material.SetFloat("_Radius", 1);
        if (!jsonDataManager.Instance.getEventMeet(balpanEventIdx[idx]) && balpanEventType[idx] < 98 && tutorialVal == 0) //boss 가 아니고, 아직 만난적 없는 이벤트인 경우
        {
            ToolBarManager.Instance.setToolBar(8);
            return;
        }
        
        switch (balpanEventType[idx]) {
            case 0:
                ToolBarManager.Instance.setToolBar(0);
                break;
            case 2:
                ToolBarManager.Instance.setToolBar(1);
                break;
            case 3:
                ToolBarManager.Instance.setToolBar(2);
                break;
            case 4:
                ToolBarManager.Instance.setToolBar(3);
                break;
            case 5:
                ToolBarManager.Instance.setToolBar(10);
                break;
            case 6:
                ToolBarManager.Instance.setToolBar(4);
                break;
            case 7:
                ToolBarManager.Instance.setToolBar(9);
                break;
            case 8:
                ToolBarManager.Instance.setToolBar(5);
                break;
            case 98:
                ToolBarManager.Instance.setToolBar(6);
                break;
            case 99:
                ToolBarManager.Instance.setToolBar(6);
                break;
        }
        if (balpanEventType[idx] >= 100) ToolBarManager.Instance.setToolBar(7);
    }
    public void hoverOutBalpan()
    {
        for (int i = 0; i < balpanObj.Length; i++)
        {
            balpanObj[i].GetComponent<SpriteRenderer>().material.SetFloat("_Radius", 0);
        }
        ToolBarManager.Instance.toolBarOnOff(0);
    }
    public void hoverInCharacterDice(int characterIdx)
    {
        for(int i = 0; i < 4; i++) {
            if (characterIdx == i && CharacterManager.Instance.getCharacter(i) != null && CharacterManager.Instance.getCharacterState(i) == 0) {
                diceOutline[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");

                clickBalpanUpDownButton(false, adventureBalpanPointTemp * -1);
                balpanArrowGoal.transform.position = balpanObj[CharacterManager.Instance.getDiceNum(characterIdx) + 2].transform.position;
 
            }
            else
            {
                diceOutline[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            }
            /*
            if (characterIdx != i && selectDiceCharacterIdx != i ) diceOutline[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            else diceOutline[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
            */
            /*
                if(characterIdx != i && selectDiceCharacterIdx != i) diceObject[i].GetComponent<SpriteRenderer>().material.SetFloat("_Radius", 0);
                else diceObject[i].GetComponent<SpriteRenderer>().material.SetFloat("_Radius", 1);
            */
        }
    }
    public void hoverOutCharacterDice(int characterIdx)
    {
        for (int i = 0; i < 4; i++)
        {
            //if (selectDiceCharacterIdx == i) diceOutline[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
            //else
            //{
                diceOutline[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            //}
        }
        balpanArrowGoal.transform.position = new Vector3(-330, 247, 0);
    }

    //가방, 전투 페이즈 입장을 위한 함수들

    public bool curCanvasItemCanvas = false;
    public bool useFairDice = false;
    public void setUseFairDice(bool input) {
        useFairDice=input;
    }

    public void enterBattleCanvas()
    {
        if (battleEventTrigger) //battle event가 발생해 배틀 canvas로 넘어가야 하는 경우
        {
            giveUpBtnAble(false);
            upDownManager.Instance.changeOption(1, true);
            gameOverChk = false;
            curCanvasIsAdventure = false;
            upDownManager.Instance.resetUI();
            BattleManager.Instance.startBattle_fromAdventure();
            CameraManager.Instance.updateInitPosition(new Vector3(0f, mainCamera.transform.position.y, mainCamera.transform.position.z));
            
        }
        
    }
    private bool gameOverChk = false;
    private bool gameOverAtBattle = false;
    public bool exitBattleCanvas(bool win)
    {
        if (battleEventTrigger) //battle event가 발생해 배틀 canvas로 넘어가야 하는 경우
        {
            upDownManager.Instance.changeOption(1, false);
            curCanvasIsAdventure = true;

            upDownManager.Instance.deleteOtherLock(0);
            upDownManager.Instance.resetUI();

            battleEventTrigger = false;
            if (!win)
            {
                gameOverAtBattle = true;
                gameOverChk = true; //Adventure 반복문 탈출
            }
            CameraManager.Instance.updateInitPosition(new Vector3(-500f, mainCamera.transform.position.y, mainCamera.transform.position.z));
            //mainCamera.transform.position = new Vector3(-500f, mainCamera.transform.position.y, mainCamera.transform.position.z);
            return true;
        }
        return false;
    }

    public bool getBattleEventChk()
    {
        return !(curCanvasIsAdventure);// battleEventTrigger;
    }


    public int getWitchPower(int idx)
    {
        return witchPower[idx];
    }

    public void stageClear()
    {
        stageIdx++;
        if (stageIdx == 10)
        {
            stageIdx = 0;
            stageNum++;
        }
    }

}
