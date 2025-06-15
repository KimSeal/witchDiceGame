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
    private int[] adventureEventArr = new int[21]; //앞으로 남은 이벤트들에 대한 정보
    private int[] adventureEventArr_Y = new int[21]; // 이벤트 들이 위치할 곳에 대한 세로축 정보
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

    private GameObject battleBtn;

    
    public List<adventureEvent>[] adventureEventList = new List<adventureEvent>[5]; //

    public List<AdventureEventReader> adventureEventReaderList = new List<AdventureEventReader>(); // 
    public List<AdventureEventPacketReader> adventureEventPacketReaderList = new List<AdventureEventPacketReader>(); // 

    GameObject mainCamera;

    private adventureEvent curDiceEvent;
    private adventureEvent_Packet curDiceEventPacket;

    private GameObject adventureBackground;
    private GameObject adventureNPC;
    private GameObject adventureLoad;
    private GameObject[] watchNumObject = new GameObject[6];

    bool curCanvasIsAdventure = true;
    bool battleEventTrigger = false;
    bool eventWatchTrigger = false;


    //어드벤쳐 캐릭터 버튼 선택용
    private int selectDiceCharacterIdx = -1; //지금은 가장 앞에 있는 놈으로 해놨는데 추후 수정가능하게 만들기
    private GameObject nextBtnObj;
    private GameObject standObj;

    private bool eventEndClick = false; //이벤트를 넘어갈 수 있는 경우, true가 된다.

    void resetItemResult()
    {
        for (int i = 0; i < 4; i++) {
            resultItemArr[i, 0] = -99999;
            resultItemArr[i, 1] = -99999;
        }
    }
    string[] typeArr = { "consume", "dice", "equip", "passive", "destiny" };
    public void hoverInItem(int idx)
    {
        if (resultItemArr[idx, 0] != -99999 && resultItemArr[idx, 1] != -99999) //아이템이 있는 경우 해당 아이템으로 변경
        {
            if (descObj[0].activeSelf == false) descObj[0].SetActive(true);

            if (resultItemArr[idx, 0] == 4)
            {
                Destiny hoverDestiny = CharacterManager.Instance.getDestiny(resultItemArr[idx, 1]);
                descObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + hoverDestiny.getName() + "_face");
                descObj[2].GetComponent<TextMeshPro>().text = hoverDestiny.getName();
                descObj[3].GetComponent<TextMeshPro>().text = "Lets be a friend!";
            }
            else
            {
                Item hoverItem = itemManager.Instance.getItem(resultItemArr[idx, 0], resultItemArr[idx, 1]);
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
        Debug.Log("hover out!");
        descObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
        descObj[2].GetComponent<TextMeshPro>().text = "";
        descObj[3].GetComponent<TextMeshPro>().text = "";
        if (descObj[0].activeSelf == true) descObj[0].SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {

        descObj[0] = GameObject.Find("obj_ui_adventure_item_Desc_board");
        descObj[1] = GameObject.Find("obj_ui_adventure_item_Desc_logo");
        descObj[2] = GameObject.Find("obj_ui_adventure_item_Desc_name");
        descObj[3] = GameObject.Find("obj_ui_adventure_item_Desc_desc");
        descObj[0].SetActive(false);


        for (int i = 0; i < 4; i++)
        {
            diceObject[i] = GameObject.Find("adventure_dice_" + i.ToString());
        }
        mainCamera = GameObject.Find("Main Camera");
        selectInfo = GameObject.Find("adventure_selectInfo");
        eventInfo = GameObject.Find("adventure_eventInfo");
        selectImage = GameObject.Find("adventure_selectDice");

        adventureLoad = GameObject.Find("ui_adventure_loading");
        adventureBackground = GameObject.Find("ui_adventureBack_0");
        adventureNPC = GameObject.Find("ui_adventureNPC_0");

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
            while (adventureEventReaderList[eventIdx].eventIdx == adventureEventPacketReaderList[packetIdx].eventIdx) //다른 event나올때까지 업
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
        battleBtn = GameObject.Find("obj_itemUI_battleBtn");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void makeStageEventArr(int stageNum) //이번 스테이지의 나타나는 이벤트의 종류를 미리 배치한다.
    {
        // stage 순서를 랜덤으로 만든다.
        adventureEventArr = new int[adventureEventList[stageNum].Count];
        for (int i = 0; i < adventureEventList[stageNum].Count; i++)
        {
            adventureEventArr[i] = i; // 이부분 조정해서 맵 테스트 진행
        }
        for (int i = adventureEventArr.Length - 1; i > 0; i--) //나중에 보스 전은 무조건 마지막에 올수 있도록 편성한다.
        {
            int j = Random.Range(0, i + 1);

            int temp = adventureEventArr[i];
            adventureEventArr[i] = adventureEventArr[j];
            adventureEventArr[j] = temp; 
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
        CharacterManager.Instance.setTestCharacterSet();
        CameraManager.Instance.updateInitPosition(new Vector3(-500f, 0f, mainCamera.transform.position.z));
        //mainCamera.transform.position = new Vector3(-500f, 0f, mainCamera.transform.position.z);
        

        //지금은 시작 버튼 누르면 바로 시작
        StartCoroutine(phase_Manage_Coroutine());
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
            if (CharacterManager.Instance.getCharacter(characterIdx) == null || CharacterManager.Instance.getCharacter(characterIdx).getCurState() != 0)
            {
                diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_no_face");
                continue;
            }
            if (Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getCharacter(characterIdx).getName() + "_face") != null)
            {
                diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getCharacter(characterIdx).getName() + "_face");
            }
            else { diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_noImage_face"); }
        }
    }

    private IEnumerator phase_Manage_Coroutine()
    {
        gameOverChk = false;
        stageNum = 0;
        makeStageEventArr(0); //이번 스테이지의 나타나는 이벤트의 종류를 미리 배치한다.
        makeStage_placeBalpan(); // 스테이지에 맞춰 발판 생성
        stageIdx = -1;
        updateCharacterFace();

        resetItemResult();          //이전 결과물로 나온 아이템들을 얻지 못하게 초기화.
        resultObj.SetActive(false);

        SoundManager_Main.Instance.playSound(2);

        // 스테이지 끝 혹은 주사위 이벤트가 끝날때까지 유지되도록 (StartCoroutine이랑 하나 계속 돌아가게 하는 것중 뭐가 더 비용 비싼지 확인할것) 살려두는게 쌀것 같긴함.
        while (stageIdx < 20 && !gameOverChk)
        {
            eventWatchNum = -1;
            selectDiceNum = -1; // 플레이어가 주사위 던질 대상을 선택할 수 있도록

            balpanLoad.transform.position = new Vector3(balpanLoad.transform.position.x, 18, balpanLoad.transform.position.z);
            balpanLoad.GetComponent<Animator>().Play("On");
            loadEnd = false;
            yield return new WaitUntil(() => loadEnd);
            balpanScreen.transform.position = new Vector3(balpanScreen.transform.position.x, 18, balpanScreen.transform.position.z);
            
            //발판 이벤트를 위한 이펙트
            //setBalpan(stageIdx);
            for (int i = 0; i < 7; i++)
            {
                if (stageIdx + i == -1) // 스테이지 시작지점인 경우
                {
                    balpanObj[i].transform.position = new Vector3(-620 + (i * 40), 0, balpanObj[i].transform.position.z);
                    balpanObj[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/balpan/spr_balpan_start");
                    GameObject temp_0 = Instantiate(diceRollEff, balpanObj[i].transform.position, Quaternion.Euler(0, 0, 0)); //사용된 아이템에 대해 effect
                    temp_0.GetComponent<Animator>().Play("balpanCreate");
                    yield return new WaitForSeconds(0.2f);
                }
                else if (stageIdx + i >= adventureEventArr.Length) //넘어가는 경우는 출력하지 않는다
                {
                    balpanObj[i].transform.position = new Vector3(balpanObj[i].transform.position.x, -300, balpanObj[i].transform.position.z);
                }
                else
                {
                    balpanObj[i].transform.position = new Vector3(-620 + (i * 40), -1 * 10 + adventureEventArr_Y[stageIdx + i] * 10, balpanObj[i].transform.position.z); //현재 위치에 해당하는 위치로 발판 이동.
                    balpanObj[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/balpan/spr_balpan_" + adventureEventList[stageNum][adventureEventArr[stageIdx + i]].getEventType().ToString());//이벤트에 관련된 발판으로 이미지 변경

                    GameObject temp_0 = Instantiate(diceRollEff, balpanObj[i].transform.position, Quaternion.Euler(0, 0, 0)); //사용된 아이템에 대해 effect
                    temp_0.GetComponent<Animator>().Play("balpanCreate");

                    yield return new WaitForSeconds(0.2f);
                }
            }
            if (selectDiceCharacterIdx != -1 && CharacterManager.Instance.getCharacterState(selectDiceCharacterIdx) == 0)
            {
                balpanArrow.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + CharacterManager.Instance.getName_itemManager(selectDiceCharacterIdx) + "/animator_" + CharacterManager.Instance.getName_itemManager(selectDiceCharacterIdx));
            }
            balpanArrow.transform.position = balpanObj[0].transform.position + new Vector3(0, 8, 0);

            yield return new WaitUntil(() => selectDiceNum > 0);

            Instantiate(diceRollEff, nextBtnObj.transform.position, Quaternion.Euler(0, 0, Random.Range(0,4) * -90)); //사용된 아이템에 대해 effect
            loadEnd = false;
            yield return new WaitUntil(() => loadEnd);

            int moveCount = selectDiceNum;
            if(stageIdx + selectDiceNum >= adventureEventList[stageNum].Count)  // 넘어간 경우 자제한다
            {
                moveCount = adventureEventList[stageNum].Count - 1 - stageIdx;
                stageIdx = adventureEventList[stageNum].Count - 1;
            }
            else
            {
                stageIdx += selectDiceNum; //stage발판 이동
            }

            for (int i=0;i<moveCount;i++)
            {
                GameObject temp_0 = Instantiate(diceRollEff, balpanObj[i + 1].transform.position, Quaternion.Euler(0, 0, 0)); //사용된 아이템에 대해 effect
                temp_0.GetComponent<Animator>().Play("balpanTouch");
                balpanArrow.transform.position = balpanObj[i+1].transform.position + new Vector3(0,8,0);
                yield return new WaitForSeconds(0.2f);
            }
            yield return new WaitForSeconds(1.2f);

            balpanLoad.GetComponent<Animator>().Play("Off");
            loadEnd = false;
            yield return new WaitUntil(() => loadEnd);
            clearBalpan();

            yield return new WaitForSeconds(0.75f);

            //발판 이벤트 종료 

            stageInfo.GetComponent<TextMeshPro>().text = (stageIdx+1).ToString() + " / " + adventureEventList[stageNum].Count.ToString(); //초기화
            updateCharacterFace();

            resetItemResult();          //이전 결과물로 나온 아이템들을 얻지 못하게 초기화.
            resultObj.SetActive(false);

            if (true)//adventureEventArr[stageIdx] == 1)
            { //주사위 이벤트 일경우 해당 이벤트 진행. 

                adventureLoad.GetComponent<Animator>().Play("On", -1, 0f);
                loadEnd = false;
                yield return new WaitUntil(() => loadEnd);

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


                //selectImage.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + (eventWatchNum + 1).ToString());

                if (curDiceEvent.getDiceUse() == 0)//주사위 사용이 필요가 없다면, 맨 첫번째 결과가 나오게 하여 그냥 넘길수 있게 한다.
                {
                    selectDiceNum = 1;
                }
                else if (curDiceEvent.getDiceUse() == 1)//고를 수 있는 상태로 변경
                {
                    selectDiceNum = -1;
                }
                Debug.Log("selectDice Error check " + selectDiceNum);
                yield return new WaitUntil(() => selectDiceNum > 0); // 주사위 쓸 영웅 선택 대기

                adventureLoad.GetComponent<Animator>().Play("On", -1, 0f);
                loadEnd = false;
                yield return new WaitUntil(() => loadEnd);
                Debug.Log("selectDice Error check2 " + selectDiceNum);

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
                if (curDiceEventPacket.getSelectType() == 4)
                { //능력치 증가
                    for (int i = 0; i < 8; i++)
                    {
                        CharacterManager.Instance.getCharacter(selectDiceCharacterIdx).upGrade(i, curDiceEventPacket.getVal(i));
                    }

                }
                if (curDiceEventPacket.getSelectType() == 6) //전투를 진행하는 경우
                {
                    SoundManager_Main.Instance.PauseSound(2);
                    SoundManager_Main.Instance.playSound(5);
                    //nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_Stop");
                    //nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                    BattleManager.Instance.updateBattleBackground(curDiceEventPacket.getBattleBackSprite());
                    for (int i = 0; i < 4; i++)
                    {
                        if (curDiceEventPacket.getSelectType() != -99999) CharacterManager.Instance.setCharacter(i, curDiceEventPacket.getVal(i));
                        else CharacterManager.Instance.emptyEnemyCharacter(i);

                        selectDiceCharacterIdx = -1;
                        balpanArrow.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/balpan/spr_balpan_arrow_0");
                        balpanArrow.GetComponent<Animator>().Play("arrowAnim");
                    }
                    battleBtn.transform.position = nextBtnObj.transform.position;
                    battleEventTrigger = true;
                    /*CharacterManager.Instance.setCharacter(0, curDiceEventPacket.getVal(0));
                    CharacterManager.Instance.setCharacter(1, curDiceEventPacket.getVal(1));
                    CharacterManager.Instance.setCharacter(2, curDiceEventPacket.getVal(2));
                    CharacterManager.Instance.setCharacter(3, curDiceEventPacket.getVal(3));
                    */
                    yield return new WaitUntil(() => !battleEventTrigger); //돌아올때까지 대기

                    SoundManager_Main.Instance.stopSound(5);
                    SoundManager_Main.Instance.unPauseSound(2);
                    //for(int i=0;i<4;i++) CharacterManager.Instance.emptyEnemyCharacter(i); //돌아오면 적군 캐릭터 모두 없애기

                    battleBtn.transform.position += new Vector3(0, 300, 0);
                    updateCharacterFace();
                    adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                    selectInfo.GetComponent<TextMeshPro>().text = "Battle is over. Now, we need to move";
                }

                if (curDiceEventPacket.getItemExist() == 1) { //이벤트 결과로 정해진 아이템을 준다.
                    resultObj.SetActive(true);
                    for (int i = 0; i < 4; i++)   //각 칸에 대한 처리
                    {
                        resultItemArr[i, 0] = curDiceEventPacket.getItemType(i);
                        resultItemArr[i, 1] = curDiceEventPacket.getItemIdx(i);
                        //결과로 나오는 아이템에 대한 이미지 처리
                        if (resultItemArr[i, 0] == -99999 || resultItemArr[i, 1] == -99999) resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                        else if (resultItemArr[i, 0] == 4) //캐릭터를 얻는 이벤트의 경우
                        {
                            resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getDestiny(resultItemArr[i, 1]).getName() + "_face");
                        }
                        else resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(itemManager.Instance.getItemSprite(resultItemArr[i, 0], resultItemArr[i, 1]));
                    }

                    Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_no_face");
                }
                if (curDiceEventPacket.getItemExist() == 2) // 랜덤한 아이템을 준다. 이부분은 추가 구현 필요.
                {
                    resultObj.SetActive(true);
                }
                if (gameOverChk == false)
                {

                    eventEndClick = true;
                    //nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                    //nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_goAhead");
                    yield return new WaitUntil(() => !eventEndClick);
                }
            }

            
            //CameraManager.Instance.updateInitPosition(new Vector3(-500f, -500f, CameraManager.Instance.camraPointZ()));
        }
        SoundManager_Main.Instance.stopSound(2);
        //gameOverChk가 true가 되면 끝
        CharacterManager.Instance.resetCharacterManager();
        itemManager.Instance.resetItemManager();
        TownManager.Instance.backToTownUI();
        Debug.Log("end of game!");
    }

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
                if (emptyPlaceExist == -1) Debug.Log("you need Place to add character!");
                else {
                    CharacterManager.Instance.setCharacter(emptyPlaceExist, resultItemArr[idx, 1]);
                    resultObjArr[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none"); //정상종료
                    resultItemArr[idx, 0] = -99999;
                    resultItemArr[idx, 1] = -99999;
                    updateCharacterFace();
                }
            }
            else
            {
                int result = itemManager.Instance.getItemResult(resultItemArr[idx, 0], resultItemArr[idx, 1]);
                if (result == 0)
                {
                    resultObjArr[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none"); //정상종료
                    resultItemArr[idx, 0] = -99999;
                    resultItemArr[idx, 1] = -99999;

                }
                else if (result == 1) //꽉차서 못담는 경우.
                {

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
                diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_no_face");
                continue;
            }
            if (Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getCharacter(characterIdx).getName() + "_face") != null)
            {
                diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getCharacter(characterIdx).getName() + "_face");
            }
            else { diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_noImage_face"); }
        }
        for (int i = 0; i < 4; i++)
        {
            diceObject[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
        }
        standObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
    }
    public void clickDice(int characterIdx)
    {
        if (descObj[0].activeSelf == true) hoverOutItem();

        if (characterIdx == -1 && eventEndClick)
        {
            CameraManager.Instance.VibrateForeTime(0.2f);
            eventEndClick = false;
            return;
        }
        if (selectDiceNum == -1 && characterIdx == -1) { //캐릭터가 선택되었고 다음으로 가는 주사위 누를 경우
            if (selectDiceCharacterIdx <0)
            {
                return;
            }
            characterIdx = selectDiceCharacterIdx;
            CharacterManager.Instance.throwDice(characterIdx);
            //selectImage.transform.rotation = Quaternion.Euler(0, 0, CharacterManager.Instance.getDiceDir(characterIdx) * -90);
            //selectImage.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + CharacterManager.Instance.getDiceNum(characterIdx).ToString());

            nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, CharacterManager.Instance.getDiceDir(characterIdx) * -90);
            nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + CharacterManager.Instance.getDiceNum(characterIdx).ToString());
            selectDiceNum = CharacterManager.Instance.getDiceNum(characterIdx);
        }
        else if (selectDiceNum == -1 && characterIdx != -1 && CharacterManager.Instance.getCharacterState(characterIdx) == 0)
        {
            Debug.Log("charactger click Dice");
            for (int i=0;i<4;i++)
            {
                diceObject[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
            }
            diceObject[characterIdx].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.7f);
            selectDiceCharacterIdx = characterIdx;
            balpanArrow.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + CharacterManager.Instance.getName_itemManager(characterIdx) + "/animator_" + CharacterManager.Instance.getName_itemManager(characterIdx));
            standObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/backImage/spr_"+ CharacterManager.Instance.getCharacter(characterIdx).getName() + "_back" );
        }
    } 
    public void hoverInCharacterDice(int characterIdx)
    {
        diceObject[characterIdx].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.7f);
    }
    public void hoverOutCharacterDice(int characterIdx)
    {
        if (selectDiceCharacterIdx != characterIdx)
        {
            diceObject[characterIdx].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
        }
    }

    //가방, 전투 페이즈 입장을 위한 함수들
    public void enterUpgradeCanvas()
    {
        if (!itemManager.Instance.getItemBoxMove()) {
            if (itemManager.Instance.getItemBoxOpen())
            {
                itemManager.Instance.flipItemBox(1, 1);
            }
            else
            {
                itemManager.Instance.flipItemBox(0, 0);
                curCanvasIsAdventure = false;

                itemManager.Instance.click_upgradeCanvas_start();
                itemManager.Instance.updateCharacterUIBtn();
                itemManager.Instance.setUpAnimator();
                CameraManager.Instance.updateInitPosition(new Vector3(-1000f, mainCamera.transform.position.y, mainCamera.transform.position.z));
                //mainCamera.transform.position = new Vector3(-1000f, mainCamera.transform.position.y, mainCamera.transform.position.z);
            }
        }
        
    }
    public void exitUpgradeCanvas()
    {
        if (!itemManager.Instance.getItemBoxMove())
        {
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
                    if (!win) gameOverChk = true; //Adventure 반복문 탈출
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
