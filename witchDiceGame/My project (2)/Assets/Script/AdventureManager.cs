using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;

public class AdventureManager : MonoBehaviour
{


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

    private GameObject[] descObj = new GameObject[4];
    private GameObject balpanLoad;
    private GameObject balpanScreen;
    private GameObject balpanArrow;
    private GameObject[] balpanObj = new GameObject[7];

    private int stageNum = 0; //몇번째 스테이지인지 받는다.
    private int stageIdx = 0; //이번 스테이지에서 몇번째 맵인지(1-1 1-2의 개념) 
    private int[] witchPower = new int[2];

    private int eventWatchNum = 0; //이벤트 선택지 볼때 쓰는 숫자
    private int selectDiceNum = -1; //현재 선택된 주사위
    private int[] adventureEventArr = new int[10001]; //앞으로 남은 이벤트들에 대한 정보
    private int[] adventureEventArr_Y = new int[10001]; // 이벤트 들이 위치할 곳에 대한 세로축 정보
    //전투 : 0  주사위 굴리기 이벤트 : 1 


    private GameObject stageInfo; //현재 스테이지의 level과 스테이지 정보를 담는 text
    private GameObject selectInfo;
    private GameObject eventInfo;
    private GameObject selectImage;
    private GameObject[] textObject = new GameObject[2]; // 주사위 굴렸을때 결과를 처리하기 위해 사용한다. 
    private GameObject[] diceObject = new GameObject[4];

    private GameObject resultObj;
    private GameObject[] resultObjArr = new GameObject[4];
    private int[,] resultItemArr = new int[4, 2]; //결과로 주어지는 아이템들 정보.

    
    public List<adventureEvent>[] adventureEventList = new List<adventureEvent>[5]; //

    public List<AdventureEventReader> adventureEventReaderList = new List<AdventureEventReader>(); // 
    public List<AdventureEventPacketReader> adventureEventPacketReaderList = new List<AdventureEventPacketReader>(); // 

    GameObject mainCamera;

    private adventureEvent curDiceEvent;
    private adventureEvent_Packet curDiceEventPacket;

    private GameObject adventureBackground;
    private GameObject adventureNPC;
    private GameObject adventureBackBoard;
    private GameObject[] watchNumObject = new GameObject[6];

    bool curCanvasIsAdventure = true;
    bool battleEventTrigger = false;
    bool eventWatchTrigger = false;


    //어드벤쳐 캐릭터 버튼 선택용
    private int selectDiceCharacterIdx = -1; //지금은 가장 앞에 있는 놈으로 해놨는데 추후 수정가능하게 만들기
    private GameObject nextBtnObj;
    private GameObject standObj;

    private ParticleSystem diceBtnFire;
    private GameObject lifeObj, lifeObj_back;

    private bool eventEndClick = false; //이벤트를 넘어갈 수 있는 경우, true가 된다.

    private bool clickAble = false;

    //상점에 대한 데이터
    private int adventureMoney = 0;
    private TextMeshPro moneyText;
    private int[,] storeItemArr = new int[4, 3]; //4개의 아이템이 배치, 각각 type, index(아이템 고유번호), 가격이 저장될 예정 
    private GameObject storeEntityObj;
    private GameObject storeImageObj;
    private TextMeshPro storePriceObj;

    private GameObject storeCheckEntityObj;
    private SpriteRenderer storeCheckImageObj;
    private TextMeshPro storeCheckPriceObj;
    private GameObject storeCheckButtonYes;
    private GameObject storeCheckButtonNo;

    private int storeIdx = 0;

    private int[] lastCharacter = new int[4];

    private int tutorialVal = 0;

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

    public int getTutorial()
    {
        return tutorialVal;
    }
    public void setTutorial(int val)
    {
        tutorialVal = val;
    }

    public IEnumerator tutorial_Coroutine()
    {
        resetDice();
        TalkManager.Instance.startTalk(2);
        yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
        //튜토리얼이 시작되었음을 알림.
        tutorialVal = 1;
        CameraManager.Instance.updateInitPosition(new Vector3(-500f, 0f, mainCamera.transform.position.z));
        StartCoroutine(phase_Manage_Coroutine(0));
    }
    public void tutorialStart()
    {
        adventureMoney = 0;
        addAdventureMoney(0);
        CharacterManager.Instance.setTurotialCharacterSet(); //캐릭터는 주인공 혼자만
        itemManager.Instance.setTutorialInitDice(); //주인공 주사위 다 1로

        StartCoroutine(tutorial_Coroutine());
    }
    public int getLastCharacter(int idx) {
        return lastCharacter[idx];
    }
    private void storyLineErrorChk() //챕터스토리를 보다가 끊고 다시 들어온 경우 임시 대처.(본걸로 처리)
    {
        for (int i = 0; i < 3; i++) {
            if (jsonDataManager.Instance.getChapterRead(1, i) == 1) jsonDataManager.Instance.setChapterRead(1, i);
        }
    }

    public void mainPlayButton()
    {
        storyLineErrorChk();
        //TalkManager.Instance.startTalk(21) ; //시작 말하기 테스트
        //Screen.SetResolution(960, 540, FullScreenMode.Windowed);
        SoundManager_Sfx.Instance.playSound(0);
        SoundManager_Main.Instance.stopSound(0);
        if (false) {
        //if (!jsonDataManager.Instance.getTutorialDid()) {
            CameraManager.Instance.updateInitPosition(new Vector3(-1000f, -500f, mainCamera.transform.position.z));
            tutorialStart();            
        }
        else
        {
            TownManager.Instance.backToTownUI();
            //CameraManager.Instance.updateInitPosition(new Vector3(-500f, -500f, mainCamera.transform.position.z));
            //SoundManager_Main.Instance.playSound(7);
        }
    }
    public void mainExitButton()
    {

        Application.Quit();
    }
    void resetItemResult()
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
    public int getAdventureMoney()
    {
        return this.adventureMoney;
    }
    public void addAdventureMoney(int money) {
        adventureMoney += money;
        moneyText.text = "$  " + adventureMoney.ToString();
    }
    public void buyItem()
    {
        if (storeItemArr[storeIdx, 0] == -99999 || storeItemArr[storeIdx, 1] == -99999 || storeItemArr[storeIdx, 2] == -99999) return; //비어있을 경우 아예 아무것도 없게
        if (adventureMoney >= storeItemArr[storeIdx, 2]) {
            int buyResult = itemManager.Instance.getItemResult(storeItemArr[storeIdx, 0], storeItemArr[storeIdx, 1]);
            if (buyResult == 0) //정상작동의 경우
            {
                SoundManager_Sfx.Instance.playSound(4);
                addAdventureMoney(storeItemArr[storeIdx, 2] * -1);
                storeItemArr[storeIdx, 0] = -99999;
                storeItemArr[storeIdx, 1] = -99999;
                storeItemArr[storeIdx, 2] = -99999;

                closeTryBuyItem();
                shakeObject(storeImageObj);
                updateStore();
            }
            else if (buyResult == 1) //인벤토리가 가득 찬 경우
            {
                
                SoundManager_Sfx.Instance.playSound(7);
                shakeObject(storeCheckEntityObj);
                storeCheckPriceObj.text = "더 넣을 공간이 없어요!";
            }
        }
        else
        {
            shakeObject(storeCheckEntityObj);
            SoundManager_Sfx.Instance.playSound(7);
            storeCheckPriceObj.text = "돈이 부족해요!";
        }
    }
    public void updateStore() //가게 이미지 업데이트
    {
        if (storeItemArr[storeIdx, 0] == -99999 || storeItemArr[storeIdx, 1] == -99999 || storeItemArr[storeIdx, 2] == -99999) { 
            storeImageObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            storePriceObj.text = "";//"No Item Here!";
        }
        else
        {
            Item hoverItem = itemManager.Instance.getItem(storeItemArr[storeIdx, 0], storeItemArr[storeIdx, 1]);
            storeImageObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/" + typeArr[storeItemArr[storeIdx, 0]] + "ItemSprite/spr_item_" + typeArr[storeItemArr[storeIdx, 0]] + "_" + hoverItem.getItemName());
            storePriceObj.text = storeItemArr[storeIdx, 2].ToString();
        }
    }
    public void storeArrow(int dir)
    {
        shakeObject(storeImageObj);
        SoundManager_Sfx.Instance.playSound(0);
        if (dir == 1){
            storeIdx++;
            if (storeIdx > 3) storeIdx = 0;
        }
        else if (dir == -1){
            storeIdx--;
            if (storeIdx < 0) storeIdx = 3;
        }
        updateStore();
    }
    public void tryBuyItem()
    {
        //아이템이 비어있는 경우 불가능하도록
        if (storeItemArr[storeIdx, 0] == -99999 || storeItemArr[storeIdx, 1] == -99999 || storeItemArr[storeIdx, 2] == -99999)
        {
            SoundManager_Sfx.Instance.playSound(7);
            return;
        }
        else
        {
            SoundManager_Sfx.Instance.playSound(0);
            
            storeCheckEntityObj.SetActive(true);
            clickAbleObjSet(storeCheckButtonYes, true, 1);
            clickAbleObjSet(storeCheckButtonNo, true, 1);
            shakeObject(storeCheckEntityObj);

            Item hoverItem = itemManager.Instance.getItem(storeItemArr[storeIdx, 0], storeItemArr[storeIdx, 1]);
            storeCheckImageObj.sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/" + typeArr[storeItemArr[storeIdx, 0]] + "ItemSprite/spr_item_" + typeArr[storeItemArr[storeIdx, 0]] + "_" + hoverItem.getItemName());
            storeCheckPriceObj.text = "가격 : " + storeItemArr[storeIdx, 2].ToString() +
                    "\n현재 금액" + adventureMoney.ToString() + " -> " + (adventureMoney - storeItemArr[storeIdx, 2]).ToString();
        }
    }
    public void closeTryBuyItem()
    {
        storeCheckButtonYes.GetComponent<hoverRotate>().expandEnd();
        storeCheckButtonNo.GetComponent<hoverRotate>().expandEnd();
        storeCheckEntityObj.SetActive(false);
        SoundManager_Sfx.Instance.playSound(7);
    }
    
    public void hoverInItem_store()
    {
        if (storeItemArr[storeIdx, 0] != -99999 && storeItemArr[storeIdx, 1] != -99999 && storeItemArr[storeIdx, 2] != -99999) //아이템이 있는 경우 해당 아이템으로 변경
        {
            if (descObj[0].activeSelf == false) descObj[0].SetActive(true);

            if (storeItemArr[storeIdx, 0] == 4)
            {
                
                Destiny hoverDestiny = CharacterManager.Instance.getDestiny(storeItemArr[storeIdx, 1]);
                descObj[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/battleResultUI/spr_selectUI_board_90");
                descObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + hoverDestiny.getName() + "_face");
                descObj[2].GetComponent<TextMeshPro>().text = hoverDestiny.getName();
                descObj[3].GetComponent<TextMeshPro>().text = TalkManager.Instance.getDesc(9);//"Lets be a friend!";
            }
            else
            {
                
                Item hoverItem = itemManager.Instance.getItem(storeItemArr[storeIdx, 0], storeItemArr[storeIdx, 1]);
                descObj[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/battleResultUI/spr_selectUI_board_" + hoverItem.getRare() + "_90");
                descObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/" + typeArr[storeItemArr[storeIdx, 0]] + "ItemSprite/spr_item_" + typeArr[storeItemArr[storeIdx, 0]] + "_" + hoverItem.getItemName());
                descObj[2].GetComponent<TextMeshPro>().text = hoverItem.getItemName();
                descObj[3].GetComponent<TextMeshPro>().text = typeArr[storeItemArr[storeIdx, 0]] + "\n\n" + hoverItem.getContent();
            }
        }
        else
        {
            descObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            descObj[2].GetComponent<TextMeshPro>().text = "";
            descObj[3].GetComponent<TextMeshPro>().text = "";
            if (descObj[0].activeSelf == true) descObj[0].SetActive(false);

        }
    }
    #endregion

    string[] typeArr = { "consume", "dice", "equip", "passive", "destiny" };
    public void hoverInItem(int idx)
    {
        if (resultItemArr[idx, 0] != -99999 && resultItemArr[idx, 1] != -99999) //아이템이 있는 경우 해당 아이템으로 변경
        {
            if (descObj[0].activeSelf == false) descObj[0].SetActive(true);

            if (resultItemArr[idx, 0] == 4)
            {
                Destiny hoverDestiny = CharacterManager.Instance.getDestiny(resultItemArr[idx, 1]);
                descObj[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/battleResultUI/spr_selectUI_board_90");
                descObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + hoverDestiny.getName() + "_face");
                descObj[2].GetComponent<TextMeshPro>().text = hoverDestiny.getName();
                descObj[3].GetComponent<TextMeshPro>().text = TalkManager.Instance.getDesc(9);
            }
            else
            {
                Item hoverItem = itemManager.Instance.getItem(resultItemArr[idx, 0], resultItemArr[idx, 1]);
                descObj[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/battleResultUI/spr_selectUI_board_" + hoverItem.getRare() + "_90");
                descObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/" + typeArr[resultItemArr[idx, 0]] + "ItemSprite/spr_item_" + typeArr[resultItemArr[idx, 0]] + "_" + hoverItem.getItemName());
                descObj[2].GetComponent<TextMeshPro>().text = hoverItem.getItemName();
                descObj[3].GetComponent<TextMeshPro>().text = typeArr[resultItemArr[idx, 0]] + "\n\n" + hoverItem.getContent();
            }
        }
        else
        {
            descObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            descObj[2].GetComponent<TextMeshPro>().text = "";
            descObj[3].GetComponent<TextMeshPro>().text = "";
            if (descObj[0].activeSelf == true) descObj[0].SetActive(false);

        }
    }
    public void hoverOutItem()
    {
        descObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
        descObj[2].GetComponent<TextMeshPro>().text = "";
        descObj[3].GetComponent<TextMeshPro>().text = "";
        if (descObj[0].activeSelf == true) descObj[0].SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        lastCharacter[0] = -99999;
        lastCharacter[1] = -99999;
        lastCharacter[2] = -99999;
        lastCharacter[3] = -99999;

        lifeObj = GameObject.Find("obj_life");
        lifeObj_back = GameObject.Find("obj_life_back");

        lifeObj.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0f);
        lifeObj_back.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0f);

        lifeObj.SetActive(false);

        descObj[0] = GameObject.Find("obj_ui_adventure_item_Desc_board");
        descObj[1] = GameObject.Find("obj_ui_adventure_item_Desc_logo");
        descObj[2] = GameObject.Find("obj_ui_adventure_item_Desc_name");
        descObj[3] = GameObject.Find("obj_ui_adventure_item_Desc_desc");
        descObj[0].SetActive(false);

        diceBtnFire = GameObject.Find("adventure_nextBtn_0_fire").GetComponent<ParticleSystem>();
        diceBtnFire.Stop();
        for (int i = 0; i < 4; i++)
        {
            diceObject[i] = GameObject.Find("adventure_dice_" + i.ToString());
        }
        mainCamera = GameObject.Find("Main Camera");
        selectInfo = GameObject.Find("adventure_selectInfo");
        eventInfo = GameObject.Find("adventure_eventInfo");
        selectImage = GameObject.Find("adventure_selectDice");

        adventureBackground = GameObject.Find("ui_adventureBack_0");
        adventureNPC = GameObject.Find("ui_adventureNPC_0");
        adventureBackBoard = GameObject.Find("ui_adventureBack_backBoard");

        nextBtnObj = GameObject.Find("adventure_nextBtn_0");
        standObj = GameObject.Find("ui_backImage_0");
        standObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");

        resultObj = GameObject.Find("obj_adventureResult");
        for (int i = 0; i < 4; i++)
        {
            resultObjArr[i] = GameObject.Find("obj_adventureResult_Item_" + i.ToString());
            resetItemResult();
        }
        resultObj.SetActive(false);


        stageNum = 1;
        stageIdx = 1;
        stageInfo = GameObject.Find("adventure_stageInfo");
        //stageInfo.GetComponent<TextMeshPro>().text = "Stage : " + stageNum + "  Level : " + stageIdx;
        //selectInfo.GetComponent<TextMeshPro>().text = "Stage : " + stageNum + "  Level : " + stageIdx;

        AdventureEventPacketReader[] tempList = new AdventureEventPacketReader[6];

        adventureEventReaderList = CSVReader.Read<AdventureEventReader>("Event");
        adventureEventPacketReaderList = CSVReader.Read<AdventureEventPacketReader>("EventPacket");

        for (int Idx = 0; Idx < adventureEventList.Length; Idx++) //스테이지 갯수만큼 adventure 리스트 만들기
        {
            adventureEventList[Idx] = new List<adventureEvent>();
        }

        int packetIdx=0; //전체 packet 배열을 위한 변수
        int packetArrIdx = 0; //event내 packet 배열에 대응하는 변수
        for (int eventIdx = 0; eventIdx < adventureEventReaderList.Count; eventIdx++) //Reader 2개를 병합 시켜 하나의 event를 만들어 list에 추가
        {
            for (int i =0;i<6;i++) { // 배열 초기화
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

        for (int i = 0; i < 6; i++)
        {
            watchNumObject[i] = GameObject.Find("obj_adventureBtn_selectBtn_" + (i + 1).ToString());
        }

        balpanLoad = GameObject.Find("obj_Adventure_diceBoard_load");
        balpanScreen = GameObject.Find("obj_Adventure_diceBoard");//"obj_balpan");
        balpanArrow = GameObject.Find("obj_balpan_arrow");
        for (int i=0;i<7;i++){  //발판 오브젝트 담기
            balpanObj[i] = GameObject.Find("obj_balpan_" + i.ToString()); 
        }

        //상점 관련
        moneyText = GameObject.Find("obj_adventure_money").GetComponent<TextMeshPro>();
        storeEntityObj = GameObject.Find("obj_adventureStore");
        storeImageObj = GameObject.Find("obj_adventureStore_Item_image");
        storePriceObj = GameObject.Find("obj_adventureStore_Item_price").GetComponent<TextMeshPro>();
        storeEntityObj.SetActive(false);
        storeCheckEntityObj = GameObject.Find("obj_ui_adventureStore_buy");
        storeCheckImageObj = GameObject.Find("obj_ui_adventureStore_buy_sprite").GetComponent<SpriteRenderer>();
        storeCheckPriceObj = GameObject.Find("obj_ui_adventureStore_buy_text").GetComponent<TextMeshPro>();
        storeCheckButtonYes = GameObject.Find("spr_ui_adventureStore_yesBtn");
        storeCheckButtonNo = GameObject.Find("spr_ui_adventureStore_noBtn");
        storeCheckEntityObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void randomMake(int start, int end) //이 중간에 있는 stage를 섞는다
    {
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
        // stage 순서를 랜덤으로 만든다.
        adventureEventArr = new int[adventureEventList[stageNum].Count];
        for (int i = 0; i < adventureEventList[stageNum].Count; i++)
        {
            adventureEventArr[i] = i; //i;이부분 조정해서 맵 테스트 진행
        }
        int EndPoint = adventureEventArr.Length - 1;

        for (int i = adventureEventArr.Length - 1; i > 0; i--) //나중에 보스 전은 무조건 마지막에 올수 있도록 편성한다.
        {
            //레벨이 달리지는경우 혹은 
            if (i == 1 || adventureEventList[stageNum][i].getLevel() != adventureEventList[stageNum][i - 1].getLevel())
            {
                randomMake(i, EndPoint);
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
    public void startAdventure()
    {
        
        adventureMoney = 0;
        addAdventureMoney(0);
        CharacterManager.Instance.setTestCharacterSet();
        CameraManager.Instance.updateInitPosition(new Vector3(-500f, 0f, mainCamera.transform.position.z));
        //mainCamera.transform.position = new Vector3(-500f, 0f, mainCamera.transform.position.z);

        resetDice();
        //지금은 시작 버튼 누르면 바로 시작
        StartCoroutine(phase_Manage_Coroutine(1));
    }
    public void setBalpan(int stageIdx) //이벤트 끝나고 발판 나올수 있도록 하는거
    {
        for (int i=0;i<7;i++) {
            if(stageIdx + i == -1) // 스테이지 시작지점인 경우
            {
                balpanObj[i].transform.position = new Vector3(-620 + (i * 40), 300, balpanObj[i].transform.position.z);
                balpanObj[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/balpan/spr_balpan_start");
            } 
            else if (stageIdx + i >= adventureEventArr.Length) //넘어가는 경우는 출력하지 않는다
            {
                balpanObj[i].transform.position = new Vector3(balpanObj[i].transform.position.x, -300, balpanObj[i].transform.position.z);
            }
            else
            {
                balpanObj[i].transform.position = new Vector3(-620 + (i * 40), 290 + adventureEventArr_Y[stageIdx + i] * 10, balpanObj[i].transform.position.z); //현재 위치에 해당하는 위치로 발판 이동.
                balpanObj[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/balpan/spr_balpan_" + adventureEventList[stageNum][adventureEventArr[stageIdx + i]].getEventType().ToString());//이벤트에 관련된 발판으로 이미지 변경
            }
        }
        balpanScreen.transform.position = new Vector3(balpanScreen.transform.position.x, 0, balpanScreen.transform.position.z);
    }
    public void clearBalpan()// 발판 이벤트 끝나고 발판 화면 치우기
    {
        balpanScreen.transform.position = new Vector3(balpanScreen.transform.position.x, 300, balpanScreen.transform.position.z);
        balpanArrow.transform.position = new Vector3(balpanArrow.transform.position.x, 300, balpanArrow.transform.position.z);
        balpanArrow.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/balpan/spr_balpan_arrow_0");
        for (int i = 0; i < 7; i++)
        {
            balpanObj[i].transform.position = new Vector3(balpanObj[i].transform.position.x, -300, balpanObj[i].transform.position.z);
        }
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
                diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_no_face");
                continue;
            }
            if (Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getCharacter(characterIdx).getName() + "_face") != null){
                lastCharacter[characterIdx] = CharacterManager.Instance.getCharacter(characterIdx).getDestiny().DestinyIdx;
                diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getCharacter(characterIdx).getName() + "_face");
            }
            else { diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_noImage_face"); }
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

    private void giveUpBtnAble(bool onOff)
    {

        if (!onOff)
        {
            giveUpAble = false;
            activeGiveUpBoard(false);
            giveUpBtn.GetComponent<Animator>().Play("unactive");
        }
        else if (tutorialVal == 0)
        {
            giveUpText.GetComponent<TextMeshPro>().text = TalkManager.Instance.getDesc(13);
            giveUpAble = true;
            giveUpBtn.GetComponent<Animator>().Play("active");
        }
        

    }
    public bool getGameOverChk()
    {
        return gameOverChk;
    }
    public void activeGiveUpBoard(bool onOff)
    {
        if (tutorialVal != 0) { //튜토리얼 중에는 항복 불가능
            fullUI.showFull(14);
            return;
        }
        if (onOff && itemManager.Instance.getItemBoxOpen() || itemManager.Instance.getItemBoxMove()) return;//아이템 창 열려있으면 못키게


        if (giveUpAble && onOff)
        {
            giveUpBoard.SetActive(true);
            giveUpBoard.GetComponent<hoverRotate>().shakeStart();
        }
        if(!onOff) giveUpBoard.SetActive(false);
    }
    private IEnumerator phase_Manage_Coroutine(int stageNumTemp)
    {
        rerollBtn.SetActive(false);
        gameOverAtBattle = false;
        giveUpBtnAble(false);
        gameOverChk = false;
        stageNum = stageNumTemp;
        addAdventureMoney(0);

        //시작시 이미지 없애기
        selectDiceCharacterIdx = -1;
        adventureBackground.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/loading/adventureBoard_2");
        adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        standObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        selectInfo.GetComponent<TextMeshPro>().text = "";

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
            if (descObj[0].activeSelf == true) hoverOutItem();
            giveUpBtnAble(false);
            eventWatchNum = -1;
            selectDiceNum = -1; // 플레이어가 주사위 던질 대상을 선택할 수 있도록

            balpanLoad.transform.position = new Vector3(balpanLoad.transform.position.x, 18, balpanLoad.transform.position.z);
            balpanLoad.GetComponent<Animator>().Play("On");
            loadEnd = false;
            clickAble = false; // 주사위 클릭 못하게
            clickAbleObjSet(nextBtnObj, false, 1);


            yield return new WaitUntil(() => loadEnd);
            balpanScreen.transform.position = new Vector3(balpanScreen.transform.position.x, 18, balpanScreen.transform.position.z);

            //발판 이벤트를 위한 이펙트
            //setBalpan(stageIdx);
            for (int i = 0; i < 7; i++)
            {
                shakeObject(balpanObj[i]);
                if (stageIdx + i == -1) // 스테이지 시작지점인 경우
                {
                    balpanObj[i].transform.position = new Vector3(-620 + (i * 40), 8.2f, balpanObj[i].transform.position.z);
                    balpanObj[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/balpan/spr_balpan_start");
                    GameObject temp_0 = Instantiate(diceRollEff, balpanObj[i].transform.position, Quaternion.Euler(0, 0, 0)); //사용된 아이템에 대해 effect
                    temp_0.GetComponent<Animator>().Play("balpanCreate");
                    shakeObject(temp_0);
                    SoundManager_Sfx.Instance.playSound(3);
                    yield return new WaitForSeconds(0.2f);
                }
                else if (stageIdx + i >= adventureEventArr.Length) //넘어가는 경우는 출력하지 않는다
                {
                    balpanObj[i].transform.position = new Vector3(balpanObj[i].transform.position.x, -300, balpanObj[i].transform.position.z);
                }
                else
                {
                    balpanObj[i].transform.position = new Vector3(-620 + (i * 40), -1 * 10 + adventureEventArr_Y[stageIdx + i] * 10 + 8.2f, balpanObj[i].transform.position.z); //현재 위치에 해당하는 위치로 발판 이동.

                    balpanObj[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/balpan/spr_balpan_" + adventureEventList[stageNum][adventureEventArr[stageIdx + i]].getEventType().ToString());//이벤트에 관련된 발판으로 이미지 변경

                    GameObject temp_0 = Instantiate(diceRollEff, balpanObj[i].transform.position, Quaternion.Euler(0, 0, 0)); //사용된 아이템에 대해 effect
                    temp_0.GetComponent<Animator>().Play("balpanCreate");
                    shakeObject(temp_0);
                    SoundManager_Sfx.Instance.playSound(3);
                    yield return new WaitForSeconds(0.2f);
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (CharacterManager.Instance.getCharacter(i) != null && CharacterManager.Instance.getCharacter(i).getCurState() == 0)
                { shakeObject(diceObject[i]); }
            }
            if (selectDiceCharacterIdx != -1 && CharacterManager.Instance.getCharacterState(selectDiceCharacterIdx) == 0)
            {
                balpanArrow.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + CharacterManager.Instance.getName_itemManager(selectDiceCharacterIdx) + "/animator_" + CharacterManager.Instance.getName_itemManager(selectDiceCharacterIdx));
            }
            balpanArrow.transform.position = balpanObj[0].transform.position; //+ new Vector3(0, 8, 0);
            clickAble = true;
            clickAbleObjSet(nextBtnObj, true, 1);
            //나아갈수 있다는 것을 주사위에 표시
            nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_goAhead");
            nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, 0);

            diceBtnFire.Play();
            if (tutorialVal == 1) { 
                TalkManager.Instance.startTalk(4);
                yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
            } //튜토리얼에서 주사위 굴리기를 알려주기 위한 대화
            yield return new WaitUntil(() => selectDiceNum > 0);
            rerollBtn.SetActive(true);
            rerollBtn.GetComponent<hoverRotate>().expandEnd();
            rerollBtn.GetComponent<hoverDark>().changeAlpha(0.0f);
            rerollChk = true;
            yield return new WaitUntil(() => !rerollChk);
            rerollBtn.SetActive(false);
            diceBtnFire.Stop();

            Debug.Log("hello~");
            //Instantiate(diceRollEff, nextBtnObj.transform.position, Quaternion.Euler(0, 0, Random.Range(0,4) * -90)); //사용된 아이템에 대해 effect
            //SoundManager_Sfx.Instance.playSound(2);
            /*
            loadEnd = false;
            yield return new WaitUntil(() => loadEnd);
            */

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

            for (int i=0;i<moveCount;i++)
            {
                GameObject temp_0 = Instantiate(diceRollEff, balpanObj[i + 1].transform.position, Quaternion.Euler(0, 0, 0)); //사용된 아이템에 대해 effect
                temp_0.GetComponent<Animator>().Play("balpanTouch");
                SoundManager_Sfx.Instance.playSound(4);
                shakeObject(temp_0);
                shakeObject(balpanObj[i+1]);
                balpanArrow.transform.position = balpanObj[i + 1].transform.position;// + new Vector3(0,8,0);
                stageIdx++;
                if (adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType() >= 98) //만약 무조건 멈춰야 하는 곳인 경우 정지시킨다.
                {
                    balpanArrow.GetComponent<Animator>().Play("Hit");
                    break;
                }
                yield return new WaitForSeconds(0.2f);
                
            }
            yield return new WaitForSeconds(1.2f);

            balpanLoad.GetComponent<Animator>().Play("Off");
            loadEnd = false;
            yield return new WaitUntil(() => loadEnd);
            clearBalpan();

            yield return new WaitForSeconds(0.75f);

            //발판 이벤트 종료 

            stageInfo.GetComponent<TextMeshPro>().text = "";//(stageIdx+1).ToString() + " / " + adventureEventList[stageNum].Count.ToString(); //초기화
            updateCharacterFace();

            if (descObj[0].activeSelf == true) hoverOutItem();
            resetItemResult();          //이전 결과물로 나온 아이템들을 얻지 못하게 초기화.
            resultObj.SetActive(false);
            storeEntityObj.SetActive(false);

            if (true)//adventureEventArr[stageIdx] == 1)
            { //주사위 이벤트 일경우 해당 이벤트 진행. 
                nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_goAhead");
                nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, 0);

                //float tempMoveVal = 1.0f;
                float timeVal = 0.0f;
                while(timeVal < 1.0f) {
                    adventureBackground.transform.localPosition = new Vector3(0.0f - (4 * Mathf.Sin(timeVal * Mathf.PI)), 16.0f + (4 * Mathf.Sin(timeVal * Mathf.PI)), 0f);
                    adventureBackBoard.transform.localPosition = new Vector3(8.0f + (4 * Mathf.Sin(timeVal * Mathf.PI)), 8.0f - (4 * Mathf.Sin(timeVal * Mathf.PI)), 0f);
                    timeVal += 0.05f;
                    yield return new WaitForSeconds(0.01f);
                }


                eventWatchNum = 0;

                curDiceEvent = new adventureEvent(adventureEventList[stageNum][adventureEventArr[stageIdx]]); //랜덤한 이벤트를 받아온다. -> 현재는 그냥 보드 이벤트 따라가게 함.
                if (curDiceEvent.getEventType() == 6) { //이벤트에서 숫자가 의미 있을 경우, 주사위 별 선택지를 확인. 아닌 경우 확인 불가능하도록
                    eventWatchTrigger = true;
                }
                else
                {
                    eventWatchTrigger = false;
                }
                //selectInfo.GetComponent<TextMeshPro>().text = curDiceEvent.getPacket(eventWatchNum).getChooseText(); //선택지 텍스트 변경
                //eventInfo.GetComponent<TextMeshPro>().text = curDiceEvent.getSelectText(); // 이벤트 텍스트 내용 변경

                selectInfo.GetComponent<TextMeshPro>().text = curDiceEvent.getSelectText(); // 이벤트 텍스트 내용 변경

                adventureBackground.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/background/spr_ui_adventureBack_" + curDiceEvent.getBackgroundSprite());
                //adventureBackground.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/" + curDiceEvent.getEventName() + "/spr_ui_adventureBack_" + curDiceEvent.getEventName() + "_0");

                adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/NPC/spr_ui_NPC_" + curDiceEvent.getNPCSprite());
                //adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/" + curDiceEvent.getEventName() + "/spr_ui_NPC_" + curDiceEvent.getEventName() + "_0");

                selectImage.transform.rotation = Quaternion.Euler(0, 0, 0);


                timeVal = 0.0f;
                float amountTemp = 2.0f;
                while (timeVal < 1.0f)
                {
                    adventureBackground.transform.localPosition = new Vector3(0.0f + (amountTemp * Mathf.Sin(timeVal * Mathf.PI)), 16.0f - (amountTemp * Mathf.Sin(timeVal * Mathf.PI)), 0f);
                    adventureBackBoard.transform.localPosition = new Vector3(8.0f - (amountTemp * Mathf.Sin(timeVal * Mathf.PI)), 8.0f + (amountTemp * Mathf.Sin(timeVal * Mathf.PI)), 0f);
                    timeVal += 0.02f;
                    amountTemp -= 0.04f;
                    yield return new WaitForSeconds(0.01f);
                }
                //여기까지가 보드 변경!
                giveUpBtnAble(true);

                //selectImage.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + (eventWatchNum + 1).ToString());

                if (curDiceEvent.getDiceUse() == 0)//주사위 사용이 필요가 없다면, 맨 첫번째 결과가 나오게 하여 그냥 넘길수 있게 한다.
                {
                    selectDiceNum = 1;
                }
                else if (curDiceEvent.getDiceUse() == 1)//고를 수 있는 상태로 변경
                {
                    diceBtnFire.Play();
                    selectDiceNum = -1;
                }
                yield return new WaitUntil(() => selectDiceNum > 0); // 주사위 쓸 영웅 선택 대기
                if (gameOverChk) { break; }

                diceBtnFire.Stop();
                if (curDiceEvent.getEventType() == 6) 
                {
                    diceBtnFire.Play();
                    //Instantiate(diceRollEff, nextBtnObj.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 4) * -90));
                    //SoundManager_Sfx.Instance.playSound(0);
                }

                adventureBackground.GetComponent<hoverRotate>().shakeStart(10.0f);
                eventWatchTrigger = false;
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
                    selectInfo.GetComponent<TextMeshPro>().text = curDiceEventPacket.getResultText();//선택지 텍스트 변경
                }
                else
                {
                    nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_goAhead");
                    nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                    curDiceEventPacket = curDiceEvent.getPacket(0); // 주사위 결과가 의미 없는 경우 0번째 packet으로 변경
                    adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/NPC/spr_ui_NPC_" + curDiceEventPacket.getSpriteIndex()); //적힌 sprite받아오기
                    selectInfo.GetComponent<TextMeshPro>().text = curDiceEventPacket.getResultText();//선택지 텍스트 변경
                }

                if (curDiceEventPacket.getSelectType() == 3) { //능력치 감소
                    for (int i = 0; i < 8; i++)
                    {
                        if (CharacterManager.Instance.getCharacter(selectDiceCharacterIdx) == null || CharacterManager.Instance.getCharacter(selectDiceCharacterIdx).getCurState() != 0)
                        {
                            break;
                        }

                        if (CharacterManager.Instance.getCharacter(selectDiceCharacterIdx).downGrade(i, curDiceEventPacket.getVal(i)) == 1)
                        { //약화 효과로 인해 죽어버릴 경우
                            balpanArrow.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/balpan/spr_balpan_arrow_0");
                            balpanArrow.GetComponent<Animator>().Play("arrowAnim");

                            selectDiceCharacterIdx = -1;
                            resetDice();
                            break;
                        }
                    }

                }
                if (curDiceEventPacket.getSelectType() == 8) { //상점 시스템
                    
                    storeEntityObj.SetActive(true);
                    for (int tempIdx=0;tempIdx<4;tempIdx++)
                    {
                        storeItemArr[tempIdx, 0] = Random.Range(0,4);

                        if (storeItemArr[tempIdx, 0] == 2) { // 장비인경우 일단 데모에서는 제거.
                            storeItemArr[tempIdx, 0] = Random.Range(0, 3);
                            if (storeItemArr[tempIdx, 0] == 2) storeItemArr[tempIdx, 0]++;
                        }
                        
                        
                        storeItemArr[tempIdx, 1] = Random.Range(1,itemManager.Instance.getItemListCount(storeItemArr[tempIdx, 0]));
                        
                        storeItemArr[tempIdx, 2] = itemManager.Instance.getItem(storeItemArr[tempIdx, 0], storeItemArr[tempIdx, 1]).getRare() * 10 + 5;
                    }
                    storeIdx = 0;
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
                    if (adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType() == 100 && jsonDataManager.Instance.setChapterDid(0, 4))
                    { // 올빼미 선배
                        giveUpBtnAble(false);
                        TalkManager.Instance.startTalk(21);
                        yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                        giveUpBtnAble(true);
                    }

                    SoundManager_Main.Instance.stopSound(2);
                    SoundManager_Main.Instance.playSound(5);
                    //nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_Stop");
                    //nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                    BattleManager.Instance.updateBattleBackground(curDiceEventPacket.getBattleBackSprite());

                    BattleManager.Instance.changeBossPhase(adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType());
                    updateCharacterFace();
                    for (int i = 0; i < 4; i++)
                    {
                        //마지막 전투에서의 캐릭터 정보를 확인
                        

                        if (curDiceEventPacket.getSelectType() != -99999) CharacterManager.Instance.setCharacter(i, curDiceEventPacket.getVal(i));
                        else CharacterManager.Instance.emptyEnemyCharacter(i);

                        
                        balpanArrow.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/balpan/spr_balpan_arrow_0");
                        balpanArrow.GetComponent<Animator>().Play("arrowAnim");
                    }
                    /*
                    hoverRotateAble(battleBtn, 2, true);
                    hoverRotateAble(battleBtn, 1, true);
                    shakeObject(battleBtn);
                    battleBtn.transform.position = nextBtnObj.transform.position;
                    */
                    nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_Battle");
                    battleEventTrigger = true;
                    
                    yield return new WaitUntil(() => !battleEventTrigger); //돌아올때까지 대기
                    SoundManager_Main.Instance.stopSound(5);
                    SoundManager_Main.Instance.playSound(2);
                    updateCharacterFace();
                    if (gameOverChk) { break; }

                    nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_goAhead");
                    nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                    if (selectDiceCharacterIdx == -1 || CharacterManager.Instance.getCharacter(selectDiceCharacterIdx) == null || CharacterManager.Instance.getCharacter(selectDiceCharacterIdx).getCurState() != 0)
                    {
                        selectDiceCharacterIdx = -1; //전투 후 돌아오면 해당 캐릭터가 생존했는지 확인한 다음 돌아올 수 있게 바꿀것. 
                        standObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                    }
                    
                    //for(int i=0;i<4;i++) CharacterManager.Instance.emptyEnemyCharacter(i); //돌아오면 적군 캐릭터 모두 없애기

                    adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                    selectInfo.GetComponent<TextMeshPro>().text = "전투 종료! 다음 이벤트로 넘어가자!";
                }

                

                if (curDiceEventPacket.getItemExist() == 1) { //이벤트 결과로 정해진 아이템을 준다.
                    resultObj.SetActive(true);
                    for (int i = 0; i < 4; i++)   //각 칸에 대한 처리
                    {
                        resultItemArr[i, 0] = curDiceEventPacket.getItemType(i);
                        resultItemArr[i, 1] = curDiceEventPacket.getItemIdx(i);
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
                                resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getDestiny(resultItemArr[i, 1]).getName() + "_face");
                            }
                            else resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(itemManager.Instance.getItemSprite(resultItemArr[i, 0], resultItemArr[i, 1]));
                        }
                    }
                    if (tutorialVal == 4) //아이템 칸 설명을 위한 대화로 넘어가기.
                    {
                        giveUpBtnAble(false);
                        TalkManager.Instance.startTalk(10);
                        yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                        eventEndClick = true;
                        clickAble = false;
                        clickAbleObjSet(nextBtnObj, false, 1);
                        yield return new WaitUntil(() => tutorialVal == 5);
                        clickAble = true;
                        clickAbleObjSet(nextBtnObj, true, 1);
                        giveUpBtnAble(true);
                    }
                }
 
                if (curDiceEventPacket.getItemExist() >= 11 && curDiceEventPacket.getItemExist() <= 14 ) // 랜덤한 아이템을 준다.
                {
                    resultObj.SetActive(true);
                    for (int i = 0; i < 4; i++)   // 보상 수만큼 해주기
                    {
                        if (curDiceEventPacket.getItemExist() % 10 > i) //아이템 수 만큼만 지급.
                        {
                            int j = Random.Range(0, 3);
                            if (j == 2) j++; //데모버젼이니까 장비 아이템은 안나오도록
                            int k = Random.Range(1, itemManager.Instance.getItemListCount(j));

                            resultItemArr[i, 0] = j;
                            resultItemArr[i, 1] = k;
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
                        if (curDiceEventPacket.getItemExist() % 10 > i) // 지정된 캐릭터 보상 수만큼 해주기
                        {
                            resultItemArr[i, 0] = 4;
                            resultItemArr[i, 1] = CharacterManager.Instance.getRandomCharacterDestinyIdx();
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
                        else if (resultItemArr[i, 0] == 4) //캐릭터를 얻는 이벤트의 경우
                        {
                            clickAbleObjSet(resultObjArr[i], true, 1);
                            clickAbleObjSet(resultObjArr[i], true, 2);
                            resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getDestiny(resultItemArr[i, 1]).getName() + "_face");
                        }
                    }
                }
                if (adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType() == 98 && !gameOverChk && jsonDataManager.Instance.setChapterDid(0, 2)){ // 1스테이지 중간 보스 클리어
                    if(jsonDataManager.Instance.getChapterRead(1,0) == 0) jsonDataManager.Instance.setChapterRead(1,0);
                    giveUpBtnAble(false);
                    TalkManager.Instance.startTalk(33);
                    yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                    giveUpBtnAble(true);
                }
                if (adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType() == 99 && !gameOverChk && jsonDataManager.Instance.setChapterDid(0, 3))
                { // 1스테이지 최종 보스 클리어
                    if (jsonDataManager.Instance.getChapterRead(1, 1) == 0) jsonDataManager.Instance.setChapterRead(1, 1);
                    giveUpBtnAble(false);
                    TalkManager.Instance.startTalk(32);
                    yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                    giveUpBtnAble(true);
                }
                //데모 보스 클리어 확인
                if (adventureEventList[stageNum][adventureEventArr[stageIdx]].getEventType() == 100 && !gameOverChk && jsonDataManager.Instance.setChapterDid(0, 5)) { // 올빼미 선배 클리어
                    if (jsonDataManager.Instance.getChapterRead(1, 2) == 0) jsonDataManager.Instance.setChapterRead(1, 2);
                    giveUpBtnAble(false);
                    TalkManager.Instance.startTalk(18);
                    yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                    demoEndChk = 1;
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
                    yield return new WaitUntil(() => !eventEndClick);
                    storeEntityObj.SetActive(false);
                }
            }
            //CameraManager.Instance.updateInitPosition(new Vector3(-500f, -500f, CameraManager.Instance.camraPointZ()));
        }
        if (gameOverChk) //게임오버로 왔을 경우.
        {
            SoundManager_Main.Instance.stopSound(2); //기본 브금 제거
            SoundManager_Main.Instance.playSound(3); //기본 브금 제거
            selectInfo.GetComponent<TextMeshPro>().text = "";
            if (demoEndChk != 0)
            { //스테이지 보스 잡은 경우 스테이지 클리어 띄우기
                if (demoEndChk == 1) //튜토리얼 종료시
                {
                    if (!jsonDataManager.Instance.getOwlBattleWin())
                    {
                        jsonDataManager.Instance.owlBattleWin();
                        // TalkManager.Instance.startTalk(18);
                        //yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                    }
                    //yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                }

                CameraManager.Instance.resultScreenActive(2);



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
                        TalkManager.Instance.startTalk(20);
                        yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                    }
                }
                yield return new WaitUntil(() => !(CameraManager.Instance.getLoseScreenActive()));


                if (demoEndChk == 2) //튜토리얼 종료시
                {
                    TalkManager.Instance.startTalk(13);
                    yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                    TalkManager.Instance.startTalk(16);
                    tutorialVal = 0;
                }
                demoEndChk = 0;
            }
            else if(!gameOverAtBattle){
                CameraManager.Instance.resultScreenActive(0);
                yield return new WaitUntil(() => !(CameraManager.Instance.getLoseScreenActive()));
            }


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

            if(jsonDataManager.Instance.getChapterRead(1,0) == 1) //만약 1스테이지 중간 보스를 무찔렀는 경우.
            {
                TalkManager.Instance.startTalk(23); yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                TalkManager.Instance.startTalk(24); yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                TalkManager.Instance.startTalk(25); yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                jsonDataManager.Instance.setChapterRead(1, 0);
            }
            if (jsonDataManager.Instance.getChapterRead(1, 1) == 1) //만약 1스테이지 최종 보스를 무찔렀는 경우.
            {
                TalkManager.Instance.startTalk(26); yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                TalkManager.Instance.startTalk(27); yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                TalkManager.Instance.startTalk(28); yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                jsonDataManager.Instance.setChapterRead(1, 1);
            }
            if (jsonDataManager.Instance.getChapterRead(1, 2) == 1) //만약 부엉이 선배를 무찔렀는 경우.
            {
                TalkManager.Instance.startTalk(29); yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                TalkManager.Instance.startTalk(30); yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                TalkManager.Instance.startTalk(31); yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                TalkManager.Instance.startTalk(19); yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                jsonDataManager.Instance.setChapterRead(1, 2);
            }

            TownManager.Instance.backToTownUI();
            
            adventureMoney = 0;
            addAdventureMoney(0);
        }
        
    }

    public void giveUpAdventure()
    {
        if (!giveUpAble) return;

        if (battleEventTrigger) {
            battleEventTrigger = false;
        }
        if (selectDiceNum <= 0)
        {
            selectDiceNum = 1;
        }
        if(eventEndClick){
            
            eventEndClick = false;
        }
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
                    Debug.Log("you need Place to add character!");
                }
                else
                {
                    SoundManager_Sfx.Instance.playSound(3);
                    CharacterManager.Instance.setCharacter(emptyPlaceExist, resultItemArr[idx, 1]);
                    for(int i=0;i<6;i++) CharacterManager.Instance.getCharacter(emptyPlaceExist).changeDiceNum(i, Random.Range(1, 7)); // 주사위 랜덤으로 변경

                    resultObjArr[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none"); //정상종료
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
                selectInfo.GetComponent<TextMeshPro>().text = curDiceEvent.getPacket(eventWatchNum).getChooseText();//선택지 텍스트 변경
                                                                                                                    //selectImage.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + (eventWatchNum+1).ToString());
            }
        }
    }

    public void changeSelectNum(int inputNum)
    { //현재 아래 방향이 상승
        if (eventWatchTrigger)
        {

            if (inputNum == 0) {
                selectInfo.GetComponent<TextMeshPro>().text = curDiceEvent.getSelectText();
            }
            else
            {
                for (int i = 0; i < 6; i++)
                {
                    if (i + 1 == inputNum) watchNumObject[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.7f);
                    else watchNumObject[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
                }
                eventWatchNum = inputNum - 1;
                selectInfo.GetComponent<TextMeshPro>().text = curDiceEvent.getPacket(eventWatchNum).getChooseText();//선택지 텍스트 변경                                                                                                       //selectImage.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + (eventWatchNum+1).ToString());
            }
        }
    }

    public void resetDice()
    {
        selectDiceCharacterIdx = -2; //의미 없는 캐릭터 idx로 변경
        for (int characterIdx = 0; characterIdx < 4; characterIdx++) //캐릭터 얼굴 업로드
        {
            
            diceObject[characterIdx].transform.rotation = Quaternion.Euler(0, 0, 0);
            if (CharacterManager.Instance.getCharacter(characterIdx) == null || CharacterManager.Instance.getCharacter(characterIdx).getCurState() != 0)
            {
                clickAbleObjSet(diceObject[characterIdx], false, 1);
                clickAbleObjSet(diceObject[characterIdx], false, 2);
                diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_no_face");
                continue;
            }

            clickAbleObjSet(diceObject[characterIdx], true, 1);
            clickAbleObjSet(diceObject[characterIdx], true, 2);
            if (Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getCharacter(characterIdx).getName() + "_face") != null)
            {
                diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getCharacter(characterIdx).getName() + "_face");
            }
            else { diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_noImage_face"); }
        }
        hoverOutCharacterDice(0);
 
        balpanArrow.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/balpan/spr_balpan_arrow_0");
        standObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
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
            nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, CharacterManager.Instance.getDiceDir(characterIdx) * -90);
            nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + CharacterManager.Instance.getDiceNum(characterIdx).ToString());
            selectDiceNum = CharacterManager.Instance.getDiceNum(characterIdx);
            rerollChk = false;
        }
        else
        {
            for (int i = 0; i < 4; i++) if (CharacterManager.Instance.getCharacter(i) != null && CharacterManager.Instance.getCharacter(i).getCurState() == 0) { shakeObject(diceObject[i]); }
            fullUI.showFull(1);
        }
    }
    public void clickDice(int characterIdx)
    {
        if (!clickAble) return;
        
        if (descObj[0].activeSelf == true) hoverOutItem();

        if (battleEventTrigger) {
            enterBattleCanvas();
        }

        if (characterIdx == -1 && eventEndClick)
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
                    CharacterManager.Instance.throwDice(characterIdx);
                    //selectImage.transform.rotation = Quaternion.Euler(0, 0, CharacterManager.Instance.getDiceDir(characterIdx) * -90);
                    //selectImage.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + CharacterManager.Instance.getDiceNum(characterIdx).ToString());
                    Instantiate(diceRollEff, nextBtnObj.transform.position, Quaternion.Euler(0, 0, Random.Range(0, 4) * -90));
                    SoundManager_Sfx.Instance.playSound(0);
                    nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, CharacterManager.Instance.getDiceDir(characterIdx) * -90);
                    nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + CharacterManager.Instance.getDiceNum(characterIdx).ToString());
                    selectDiceNum = CharacterManager.Instance.getDiceNum(characterIdx);
                    rerollChk = true;
                }
                else {
                    for (int i = 0; i < 4; i++) if (CharacterManager.Instance.getCharacter(i) != null && CharacterManager.Instance.getCharacter(i).getCurState() == 0) { shakeObject(diceObject[i]); }
                    fullUI.showFull(1);
                }
            }
            else if (selectDiceNum > 0 && rerollChk) {
                if (selectDiceCharacterIdx >= 0) {
                    rerollChk = false;
                }
                else {
                    for (int i = 0; i < 4; i++) if (CharacterManager.Instance.getCharacter(i) != null && CharacterManager.Instance.getCharacter(i).getCurState() == 0) { shakeObject(diceObject[i]); }
                    fullUI.showFull(1);
                }
            }
        }
        else if (selectDiceNum == -1 || rerollChk) {
            if (characterIdx != -1 && CharacterManager.Instance.getCharacterState(characterIdx) == 0)
            {
                SoundManager_Sfx.Instance.playSound(0);
                Debug.Log("charactger click Dice");
                selectDiceCharacterIdx = characterIdx;
                hoverOutCharacterDice(selectDiceCharacterIdx);
                balpanArrow.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + CharacterManager.Instance.getName_itemManager(characterIdx) + "/animator_" + CharacterManager.Instance.getName_itemManager(characterIdx));
                standObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/backImage/spr_" + CharacterManager.Instance.getCharacter(characterIdx).getName() + "_back");
            }
            else if ( characterIdx != -1 && CharacterManager.Instance.getCharacterState(characterIdx) != 0)
            {
                SoundManager_Sfx.Instance.playSound(7);
            }
        }
    } 
    public void hoverInCharacterDice(int characterIdx)
    {
        for(int i = 0; i < 4; i++) {
            if(characterIdx != i && selectDiceCharacterIdx != i) diceObject[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.7f);
            else diceObject[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
        }
    }
    public void hoverOutCharacterDice(int characterIdx)
    {
        for (int i = 0; i < 4; i++)
        {
            if(selectDiceCharacterIdx == i) diceObject[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
            else diceObject[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.7f);
        }
    }

    //가방, 전투 페이즈 입장을 위한 함수들
    public void enterUpgradeCanvas()
    {
        if (gameOverChk) return;
        bool tutorialChk = true;
        SoundManager_Sfx.Instance.playSound(0);
        if (tutorialVal == 1 || tutorialVal == 2 || tutorialVal == 3) tutorialChk = false;
        if (tutorialVal == 4)
        {
            if (resultObj.activeSelf == true &&
                resultItemArr[0, 0] == -99999 && resultItemArr[0, 1] == -99999 &&
                resultItemArr[1, 0] == -99999 && resultItemArr[1, 1] == -99999 &&
                resultItemArr[2, 0] == -99999 && resultItemArr[2, 1] == -99999 &&
                resultItemArr[3, 0] == -99999 && resultItemArr[3, 1] == -99999 ) {
                
                
            }
            else {
                tutorialChk = false;
                fullUI.showFull(2);
            } 
        }
        if (tutorialChk) //튜토리얼에서 문제 없는 경우.
        {
            if (!itemManager.Instance.getItemBoxMove())
            {
                if (itemManager.Instance.getItemBoxOpen())
                {
                    itemManager.Instance.flipItemBox(1, 1);
                }
                else
                {
                    if (tutorialVal == 4) {
                        tutorialVal = 5;
                        TalkManager.Instance.startTalk(11);
                    }
                    itemManager.Instance.flipItemBox(0, 0);
                    curCanvasIsAdventure = false;

                    itemManager.Instance.click_upgradeCanvas_start();
                    itemManager.Instance.updateCharacterUIBtn();
                    itemManager.Instance.setUpAnimator();
                    curCanvasItemCanvas = true;
                    CameraManager.Instance.updateInitPosition(new Vector3(-1000f, mainCamera.transform.position.y, mainCamera.transform.position.z));
                    //mainCamera.transform.position = new Vector3(-1000f, mainCamera.transform.position.y, mainCamera.transform.position.z);
                }
            }
        }
    }
    public bool curCanvasItemCanvas = false;
    public void exitUpgradeCanvas()
    {
        
        if (!itemManager.Instance.getItemBoxMove())
        {
            curCanvasItemCanvas = false;
            SoundManager_Sfx.Instance.playSound(0); 
                curCanvasIsAdventure = true;
                CameraManager.Instance.updateInitPosition(new Vector3(-500f, mainCamera.transform.position.y, mainCamera.transform.position.z));
                //mainCamera.transform.position = new Vector3(-500f, mainCamera.transform.position.y, mainCamera.transform.position.z);
                itemManager.Instance.flipItemBox(0, 1);

        }
    }
    public void enterBattleCanvas()
    {
        if (battleEventTrigger) //battle event가 발생해 배틀 canvas로 넘어가야 하는 경우
        {
            if (!itemManager.Instance.getItemBoxMove() ) //상자 동작중에는 넘어가기 불가능.
            {
                if (itemManager.Instance.getItemBoxOpen())
                { //열려있는 경우 상자 끄기
                    itemManager.Instance.flipItemBox(1, 1);
                }
                else
                {
                   
                    gameOverChk = false;
                    curCanvasIsAdventure = false;
                    BattleManager.Instance.startBattle_fromAdventure();
                    CameraManager.Instance.updateInitPosition(new Vector3(0f, mainCamera.transform.position.y, mainCamera.transform.position.z));
                   // mainCamera.transform.position = new Vector3(0f, mainCamera.transform.position.y, mainCamera.transform.position.z);
                }
            }
            
        }
        
    }
    private bool gameOverChk = false;
    private bool gameOverAtBattle = false;
    public bool exitBattleCanvas(bool win)
    {
        if (battleEventTrigger) //battle event가 발생해 배틀 canvas로 넘어가야 하는 경우
        {
            if (!itemManager.Instance.getItemBoxMove()) //상자 동작중에는 넘어가기 불가능.
            {
                if (itemManager.Instance.getItemBoxOpen())
                {   //열려있는 경우 상자 끄기
                    itemManager.Instance.flipItemBox(2, 1);
                }
                else
                {
                    curCanvasIsAdventure = true;
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
            }
        }
        return false;
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
