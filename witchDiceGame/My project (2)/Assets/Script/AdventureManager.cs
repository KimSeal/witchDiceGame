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


    private int stageNum = 0; //몇번째 스테이지인지 받는다.
    private int stageIdx = 0; //이번 스테이지에서 몇번째 맵인지(1-1 1-2의 개념) 
    private int[] witchPower = new int[2];

    private int eventWatchNum = 0; //이벤트 선택지 볼때 쓰는 숫자
    private int selectDiceNum = -1; //현재 선택된 주사위
    private int[] adventureEventArr = new int[21]; //앞으로 남은 이벤트들에 대한 정보
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

    
    public List<adventureEvent> adventureEventList = new List<adventureEvent>(); //

    public List<AdventureEventReader> adventureEventReaderList = new List<AdventureEventReader>(); // 
    public List<AdventureEventPacketReader> adventureEventPacketReaderList = new List<AdventureEventPacketReader>(); // 

    GameObject mainCamera;

    private adventureEvent curDiceEvent;
    private adventureEvent_Packet curDiceEventPacket;

    private GameObject adventureBackground;
    private GameObject adventureNPC;
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
    // Start is called before the first frame update
    void Start()
    {
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
        stageInfo.GetComponent<TextMeshPro>().text = "Stage : " + stageNum + "  Level : " + stageIdx;
        selectInfo.GetComponent<TextMeshPro>().text = "Stage : " + stageNum + "  Level : " + stageIdx;

        AdventureEventPacketReader[] tempList = new AdventureEventPacketReader[6];

        adventureEventReaderList = CSVReader.Read<AdventureEventReader>("Event");
        adventureEventPacketReaderList = CSVReader.Read<AdventureEventPacketReader>("EventPacket");

        for (int eventIdx = 0; eventIdx < adventureEventReaderList.Count; eventIdx++) //Reader 2개를 병합 시켜 하나의 event를 만들어 list에 추가
        {
            for (int packetIdx = 0; packetIdx < 6; packetIdx++) //각 이벤트 당 6개의 packet을 받는다.
            {
                tempList[packetIdx] = adventureEventPacketReaderList[eventIdx * 6 + packetIdx];
            }
            adventureEventList.Add(new adventureEvent(adventureEventReaderList[eventIdx], tempList)); //packet과 event 내용을 받은 event 리스트 생성
        }

        for (int i = 0; i < 6; i++)
        {
            watchNumObject[i] = GameObject.Find("obj_adventureBtn_selectBtn_" + (i + 1).ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void makeStageEventArr() //이번 스테이지의 나타나는 이벤트의 종류를 미리 배치한다.
    {
        for (int i = 0; i < 20; i++)
        {
            adventureEventArr[i] = 1; //지금은 모두 주사위 이벤트가 나오도록 설정. 나중에는 전투, 주사위 이벤트 등 어떤게 나올지 랜덤하게(단 전투가 많이) 나오게 수정해야한다.
        }
    }
    public void startAdventure()
    {
        mainCamera.transform.position = new Vector3(-500f, 0f, mainCamera.transform.position.z);
        //지금은 시작 버튼 누르면 바로 시작
        StartCoroutine(phase_Manage_Coroutine());
    }

    private int eventIndexReturn() //어떤 이벤트가 나올지 이후 지정할 필요가 있다. 현재는 0번째 이벤트밖에 나오지 않지만, 나중에는 해당 스테이지에 해당된 랜덤한 이벤트가 나오도록 해야함.
    {
        return 1;
    }
    private IEnumerator phase_Manage_Coroutine()
    {
        makeStageEventArr(); //이번 스테이지의 나타나는 이벤트의 종류를 미리 배치한다.
        stageIdx = 0;
        // 스테이지 끝 혹은 주사위 이벤트가 끝날때까지 유지되도록 (StartCoroutine이랑 하나 계속 돌아가게 하는 것중 뭐가 더 비용 비싼지 확인할것) 살려두는게 쌀것 같긴함.
        while (stageIdx < 20)
        {
            stageInfo.GetComponent<TextMeshPro>().text = "Stage : " + stageNum + "  Level : " + stageIdx; //초기화
            for (int characterIdx = 0; characterIdx < 4; characterIdx++) //캐릭터 얼굴 업로드
            {
                diceObject[characterIdx].transform.rotation = Quaternion.Euler(0, 0, 0);
                if (CharacterManager.Instance.getCharacter(characterIdx) == null || CharacterManager.Instance.getCharacter(characterIdx).getCurState() != 0) {
                    diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_no_face");
                    continue;
                }
                if (Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getCharacter(characterIdx).getName() + "_face") != null)
                {
                    diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getCharacter(characterIdx).getName() + "_face");
                }
                else { diceObject[characterIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_noImage_face"); }
            }

            resetItemResult();          //이전 결과물로 나온 아이템들을 얻지 못하게 초기화.
            resultObj.SetActive(false);

            if (adventureEventArr[stageIdx] == 1)
            { //주사위 이벤트 일경우 해당 이벤트 진행. 

                eventWatchNum = 0;
                curDiceEvent = new adventureEvent(adventureEventList[6]); //랜덤한 이벤트를 받아온다. -> 현재는 그냥 보드 이벤트 따라가게 함.
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
                
                yield return new WaitUntil(() => selectDiceNum > 0); // 주사위 쓸 영웅 선택 대기


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
                    for (int i=0;i<8;i++)
                    {
                        if (CharacterManager.Instance.getCharacter(selectDiceCharacterIdx) == null || CharacterManager.Instance.getCharacter(selectDiceCharacterIdx).getCurState() != 0)
                        {
                            break;
                        }
 
                        if (CharacterManager.Instance.getCharacter(selectDiceCharacterIdx).downGrade(i, curDiceEventPacket.getVal(i)) == 1)
                        { //약화 효과로 인해 죽어버릴 경우
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
                    //nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_Stop");
                    //nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                    for (int i = 0; i < 4; i++)
                    {
                        if (curDiceEventPacket.getSelectType() != -99999) CharacterManager.Instance.setCharacter(i, curDiceEventPacket.getVal(i));
                        else CharacterManager.Instance.emptyEnemyCharacter(i);
                    }
                    battleEventTrigger = true;
                    /*CharacterManager.Instance.setCharacter(0, curDiceEventPacket.getVal(0));
                    CharacterManager.Instance.setCharacter(1, curDiceEventPacket.getVal(1));
                    CharacterManager.Instance.setCharacter(2, curDiceEventPacket.getVal(2));
                    CharacterManager.Instance.setCharacter(3, curDiceEventPacket.getVal(3));
                    */
                    yield return new WaitUntil(() => !battleEventTrigger); //돌아올때까지 대기
                    adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                    selectInfo.GetComponent<TextMeshPro>().text = "Battle is over. Now, we need to move";
                }

                if (curDiceEventPacket.getItemExist() == 1) { //이벤트 결과로 정해진 아이템을 준다.
                    resultObj.SetActive(true);
                    for (int i=0;i<4;i++)   //각 칸에 대한 처리
                    {
                        resultItemArr[i, 0] = curDiceEventPacket.getItemType(i);
                        resultItemArr[i, 1] = curDiceEventPacket.getItemIdx(i);
                        //결과로 나오는 아이템에 대한 이미지 처리
                        if (resultItemArr[i, 0] == -99999 || resultItemArr[i, 1] == -99999) resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                        else resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(itemManager.Instance.getItemSprite(resultItemArr[i,0], resultItemArr[i, 1]));
                    }
                    
                }
                if (curDiceEventPacket.getItemExist() == 2) // 랜덤한 아이템을 준다. 이부분은 추가 구현 필요.
                {
                    resultObj.SetActive(true);
                }

                eventEndClick = true;
                //nextBtnObj.transform.rotation = Quaternion.Euler(0, 0, 0);
                //nextBtnObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_goAhead");

                yield return new WaitUntil(() => !eventEndClick);
                selectDiceNum = -1; // 선택 못하게 변경
                eventWatchNum = -1;

            }

            stageIdx++;
        }
    }

    public void clickResultItem(int idx)
    {
        //이벤트가 종료된 상태이고, 해당 아이템들이 유효할때
        if (eventEndClick && resultItemArr[idx, 0] != -99999 && resultItemArr[idx, 1] != -99999)
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
            else if (result == 2) Debug.Log("Error, this is not exist item");
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
        if (characterIdx == -1 && eventEndClick)
        {
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
            for (int i=0;i<4;i++)
            {
                diceObject[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
            }
            diceObject[characterIdx].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.7f);
            selectDiceCharacterIdx = characterIdx;
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
                mainCamera.transform.position = new Vector3(-1000f, mainCamera.transform.position.y, mainCamera.transform.position.z);
            }
        }
        
    }
    public void exitUpgradeCanvas()
    {
        if (!itemManager.Instance.getItemBoxMove())
        {
                curCanvasIsAdventure = true;
                mainCamera.transform.position = new Vector3(-500f, mainCamera.transform.position.y, mainCamera.transform.position.z);
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
                    curCanvasIsAdventure = false;
                    BattleManager.Instance.startBattle_fromAdventure();
                    mainCamera.transform.position = new Vector3(0f, mainCamera.transform.position.y, mainCamera.transform.position.z);
                }
            }
            
        }
        
    }
    public void exitBattleCanvas()
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
                    mainCamera.transform.position = new Vector3(-500f, mainCamera.transform.position.y, mainCamera.transform.position.z);
                }
            }
        }
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

    //test for movie maker
    public void MoveToTestCharacterSetCanvas()
    {
        mainCamera.transform.position = new Vector3(-500f, -500f, mainCamera.transform.position.z);
    
    }

    public void clickBtn(int a)
    {
        GameObject temp0 = GameObject.Find("testbtn_" + a.ToString());
        temp0.GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.7f);
        GameObject temp = GameObject.Find("testStand_one");
        if (a == 0)
        {
            temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/standImage/spr_" + "Yongsa" + "_stand");
        }
        if (a == 1)
        {
            temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/standImage/spr_" + "Neaco" + "_stand");
        }
        if (a == 2)
        {
            
            temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/standImage/spr_" + "Druid" + "_stand");
        }
    }
    public void clickStand(int a) {
        GameObject temp = GameObject.Find("testStand_" + a.ToString());
        if (a == 0) { 
            temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/Yongsa/spr_juingong_left_idle_0");
        }
        if (a == 1)
        {
            temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/Druid/spr_Druid_left_idle_0");
        }
        if (a == 2)
        {
            temp.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/Neaco/spr_Neaco_left_idle_0");
        }
    }
}
