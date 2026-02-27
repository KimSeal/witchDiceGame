using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MapperManager : MonoBehaviour
{

    private static MapperManager instance = null;
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

    public static MapperManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }

    [SerializeField] public GameObject[] stageNumButton = new GameObject[2];
    [SerializeField] public Sprite[] stageNumButtonOn = new Sprite[2];
    [SerializeField] public Sprite[] stageNumButtonOff = new Sprite[2];
    [SerializeField] public Sprite stageNumButtonLock;

    [SerializeField] private GameObject adventureBackground, adventureNPC, adventureBackBoard; //ui_adventure Back_0/ NPC_0 / backBoard
    [SerializeField] private GameObject[] watchNumObject = new GameObject[6]; //obj_adventureBtn_selectBtn_(number)
    [SerializeField] public GameObject watchNumObjectEntity; //obj_adventureBtn_selectBtn

    [SerializeField] private GameObject resultObj; //obj_adventureResuilt
    [SerializeField] private GameObject[] resultObjArr = new GameObject[4]; //obj_adventureResult_Item_(number)
    private int[,] resultItemArr = new int[4, 2]; //결과로 주어지는 아이템들 정보.

    public Character[] resultCharacter = new Character[4];

    [SerializeField] private GameObject storeEntityObj;
    [SerializeField] public GameObject[] storeImageObj = new GameObject[4];
    //[SerializeField] public GameObject[] storePriceObjInit = new GameObject[4]; //아이템 가격 관련
    [SerializeField] public TextMeshPro[] storePriceObj = new TextMeshPro[4];

    [SerializeField]
    public GameObject[] upgradeTagEntity = new GameObject[4];
    public GameObject[] upgradeTagType = new GameObject[4];
    public GameObject[] upgradeTagText = new GameObject[4];

    private float[] upgradeTagAmount = new float[4];
    private int[] upgradeTagTypeVal = new int[4];

    private adventureEvent curDiceEvent;
    private adventureEvent_Packet curDiceEventPacket;

    private int stageNum;
    private int stageIdx;
    private int selectDepth;
    [SerializeField] public TextMeshPro stageIdxText;
    [SerializeField] public TextMeshPro stagePercentText;
    [SerializeField] public TextMeshPro noWatchText;

    // Start is called before the first frame update
    void Start()
    {
        initMapper();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setEventIdxText(float curX)
    {
        curX -= (-1075f);

        int curPoint = (int)(curX / (215f / (float)(AdventureManager.Instance.getAdventureEventLen(stageNum))));
        if(curPoint == AdventureManager.Instance.getAdventureEventLen(stageNum))
        {
            curPoint -= 1;
        }
        stageIdx = curPoint;
        makeFirstEvent();
        stageIdxText.text = (stageIdx+1).ToString() + " / " + AdventureManager.Instance.getAdventureEventLen(stageNum).ToString();
    }
    public int getEventTalkMaxDepth()
    {
        if (curDiceEventPacket.getItemExist() > 0 && curDiceEventPacket.getSelectType() == 6) {
            return 3; // 전투 결과에 대한 처리
        }
        return 2; //다른 경우에는 결말이 심플.
    }
    public void tagInit()
    {
        for (int i = 0; i < 4; i++)
        {
            upgradeTagEntity[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            upgradeTagType[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            upgradeTagAmount[i] = 0f;
            upgradeTagTypeVal[i] = -1;
            upgradeTagText[i].GetComponent<TextMeshPro>().text = "";
        }
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

        if (val < 0)
        {
            upgradeTagText[idx].GetComponent<TextMeshPro>().text = val.ToString();
            upgradeTagEntity[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/loading/spr_subStatePost");
        }
        else if (val > 0)
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

    public void clickStageNumButton(int idx)
    {
        if (idx > 2) return;
        stageNum = idx;


        int clearStageNum = 0;
        for (int i = 0; i < AdventureManager.Instance.getAdventureEventLen(stageNum); i++)
        {
            if (jsonDataManager.Instance.getEventMeet(AdventureManager.Instance.getAdventureEvent(stageNum, i).getEventIdx()))
            {
                clearStageNum += 1;
            }
        }

        if (clearStageNum == 0) stagePercentText.text = "";
        else stagePercentText.text = ((float)clearStageNum * 100f / (float)AdventureManager.Instance.getAdventureEventLen(stageNum)).ToString() + " %";
        initMapper();

        for (int i=0;i<stageNumButton.Length;i++)
        {
            if (clearStageNum == 0)
            {
                stageNumButton[i].GetComponent<SpriteRenderer>().sprite = stageNumButtonLock;
            }
            else if (i == stageNum)
            {
                stageNumButton[i].GetComponent<SpriteRenderer>().sprite = stageNumButtonOn[i];
            }
            else
            {
                stageNumButton[i].GetComponent<SpriteRenderer>().sprite = stageNumButtonOff[i];
            }
        }
        
    }

    public void hoverDice(int inputNum)
    {
        if (curDiceEvent.getEventType() == 6)
        {
            if (inputNum == 0)
            {

                for (int i = 0; i < 6; i++)
                {
                    watchNumObject[i].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
                }
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
                    else
                    {
                        watchNumObject[i].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
                        //watchNumObject[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
                    }
                }
                TalkManager.Instance.setDescChooseText(curDiceEvent.getPacket(inputNum - 1));
                //TalkManager.Instance.setDescString(curDiceEvent.getPacket(eventWatchNum).getChooseText());//선택지 텍스트 변경    
            }

        }
    }
    public void initMapper()
    {
        storeEntityObj.SetActive(false);
        resultObj.SetActive(false);
        watchNumObjectEntity.SetActive(false);

        adventureBackground.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/loading/adventureBoard_2");
        adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");

        TalkManager.Instance.setDescString("");
        TalkManager.Instance.setDescClickLock(false);
        noWatchText.text = "";

        for (int i = 0; i < 4; i++)
        {
            resultCharacter[i] = null;
            resultItemArr[i, 0] = -99999;
            resultItemArr[i, 1] = -99999;

            upgradeTagEntity[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            upgradeTagType[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            upgradeTagAmount[i] = 0f;
            upgradeTagTypeVal[i] = -1;
            upgradeTagText[i].GetComponent<TextMeshPro>().text = "";
        }
    }
    public void makeFirstEvent()
    {
        //TalkManager.Instance.setDescString("");
        initMapper();


        curDiceEvent = AdventureManager.Instance.getAdventureEvent(stageNum, stageIdx);
        selectDepth = 1;
        TalkManager.Instance.setMapperLock(1);

        if (!jsonDataManager.Instance.getEventMeet(curDiceEvent.getEventIdx()))
        {
            noWatchText.text = TalkManager.Instance.getDesc(132);
            return;
        }

        

        
        if (curDiceEvent.getEventType() == 6)
        { //이벤트에서 숫자가 의미 있을 경우, 주사위 별 선택지를 확인. 아닌 경우 확인 불가능하도록

            TalkManager.Instance.setDescClickLock(true);
            watchNumObjectEntity.SetActive(true);
            for (int i = 0; i < 6; i++)
            {
                watchNumObject[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
            }
        }
        else
        {
            TalkManager.Instance.setDescClickLock(false);
            watchNumObjectEntity.SetActive(false);
        }


        TalkManager.Instance.setDescSelectText(curDiceEvent);
        //TalkManager.Instance.setDescString(curDiceEvent.getSelectText());

        adventureBackground.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/background/spr_ui_adventureBack_" + curDiceEvent.getBackgroundSprite());
        adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/NPC/spr_ui_NPC_" + curDiceEvent.getNPCSprite());
    }
    public void clickMapperDice(int num)
    {
        makeSecondEvent(num);
    }

    public void makeSecondEvent(int selectDiceNum)
    {
        if (selectDepth != 1) return;

        TalkManager.Instance.setMapperLock(2);
        
        selectDepth = 2;
        adventureBackground.GetComponent<hoverRotate>().shakeStart(10.0f);
        watchNumObjectEntity.SetActive(false);

        int eventWatchNum = selectDiceNum - 1;
        TalkManager.Instance.setDescClickLock(false);
        if (curDiceEvent.getEventType() == 6) //주사위를 굴리는 이벤트일 경우, 주사위 결과 반영해 NPC 스프라이트 변경
        {
            curDiceEventPacket = curDiceEvent.getPacket(eventWatchNum);
            adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/" + curDiceEvent.getEventName() + "/spr_ui_NPC_" + curDiceEvent.getEventName() + "_" + curDiceEventPacket.getSpriteIndex());

            TalkManager.Instance.setDescResultText(curDiceEventPacket);
            //TalkManager.Instance.setDescString(curDiceEventPacket.getResultText());
        }
        else
        {
            curDiceEventPacket = curDiceEvent.getPacket(0); // 주사위 결과가 의미 없는 경우 0번째 packet으로 변경
            adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/adventureUI/NPC/spr_ui_NPC_" + curDiceEventPacket.getSpriteIndex()); //적힌 sprite받아오기
            TalkManager.Instance.setDescResultText(curDiceEventPacket);
            //TalkManager.Instance.setDescString(curDiceEventPacket.getResultText());
        }

        if (curDiceEventPacket.getSelectType() == 3 || curDiceEventPacket.getSelectType() == 4 || curDiceEventPacket.getSelectType() == 5)
        { //능력치 변화
            tagInit();
            int tagIdxTemp = 0;
            for (int i = 0; i < 8; i++)
            {
                if (curDiceEventPacket.getVal(i) < 0)
                {
                    setTag(tagIdxTemp++, i, curDiceEventPacket.getVal(i));
                }
                else if (curDiceEventPacket.getVal(i) > 0)
                {
                    setTag(tagIdxTemp++, i, curDiceEventPacket.getVal(i));
                }

            }
        }

        if (curDiceEventPacket.getSelectType() == 8)
        { //상점 시스템
            storeEntityObj.SetActive(true);
        }

        if (curDiceEventPacket.getSelectType() == 6) //전투를 진행하는 경우
        {
            /*
            for (int i = 0; i < 4; i++)
            {
                //마지막 전투에서의 캐릭터 정보를 확인
                CharacterManager.Instance.emptyEnemyCharacter(i);
                if (curDiceEventPacket.getSelectType() != -99999) CharacterManager.Instance.setCharacter(i, curDiceEventPacket.getVal(i));
            }
            */
            return;
        }

        if (curDiceEventPacket.getItemExist() == 1)
        { //이벤트 결과로 정해진 아이템을 준다.
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
                    if (resultItemArr[i, 0] == 4)
                    {
                        CharacterManager.Instance.setCharacter_destinyBase(ref resultCharacter[i], resultItemArr[i, 1]); //getCharacter(resultItemArr[i, 1]);
                        for (int j = 0; j < 6; j++) resultCharacter[i].changeDiceNum(j, Random.Range(1, 7));

                        resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getDestiny(resultItemArr[i, 1]).getName() + "_face");
                    }
                    else resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(itemManager.Instance.getItemSprite(resultItemArr[i, 0], resultItemArr[i, 1]));
                }
            }
        }

        if (curDiceEventPacket.getItemExist() >= 11 && curDiceEventPacket.getItemExist() <= 14) // 랜덤한 아이템을 준다.
        {
            resultObj.SetActive(true);
            for (int i = 0; i < 4; i++)   // 보상 수만큼 해주기
            {
                if (curDiceEventPacket.getItemExist() % 10 > i) //아이템 수 만큼만 지급.
                {
                    resultItemArr[i, 0] = -1;
                    resultItemArr[i, 1] = -1;
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
                    resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_noImage");
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
                    resultItemArr[i, 1] = -1;
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
                    resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_noImage");
                }
            }
        }
    }

    public void makeThirdEvent()
    {
        TalkManager.Instance.setMapperLock(3);
        TalkManager.Instance.setDescClickLock(false);
        TalkManager.Instance.setDescIdx(127);

        adventureNPC.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        if (curDiceEventPacket.getItemExist() == 1)
        { //이벤트 결과로 정해진 아이템을 준다.
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
                    if (resultItemArr[i, 0] == 4)
                    {
                        CharacterManager.Instance.setCharacter_destinyBase(ref resultCharacter[i], resultItemArr[i, 1]); //getCharacter(resultItemArr[i, 1]);
                        for (int j = 0; j < 6; j++) resultCharacter[i].changeDiceNum(j, Random.Range(1, 7));

                        resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getDestiny(resultItemArr[i, 1]).getName() + "_face");
                    }
                    else resultObjArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(itemManager.Instance.getItemSprite(resultItemArr[i, 0], resultItemArr[i, 1]));
                }
            }
        }

        if (curDiceEventPacket.getItemExist() >= 11 && curDiceEventPacket.getItemExist() <= 14) // 랜덤한 아이템을 준다.
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
                    CharacterManager.Instance.setCharacter_destinyBase(ref resultCharacter[i], resultItemArr[i, 1]);
                    for (int j = 0; j < 6; j++) resultCharacter[i].changeDiceNum(j, Random.Range(1, 7));
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
    }

    public void hoverInTagType(int idx)
    {
        if (upgradeTagTypeVal[idx] >= 0)
        {
            itemManager.Instance.hoverInInfo(upgradeTagTypeVal[idx]);
        }
    }
    public void hoverOutTagType()
    {
        ToolBarManager.Instance.toolBarOnOff(0);
    }

    public void hoverInStore(int idx)
    {
    }
    public void hoverInItem(int idx)
    {
        if (resultItemArr[idx, 0] != -99999 && resultItemArr[idx, 1] != -99999) //아이템이 있는 경우 해당 아이템으로 변경
        {
            //if (descObj[0].activeSelf == false) descObj[0].SetActive(true);

            if (resultItemArr[idx, 0] == 4)
            {
                if (resultItemArr[idx, 1] == -1)
                {
                    ToolBarManager.Instance.setToolBarRandom(1);
                }
                else
                {
                    ToolBarManager.Instance.setToolBar(resultCharacter[idx]);
                }
            }
            else
            {
                if (resultItemArr[idx, 1] == -1)
                {
                    ToolBarManager.Instance.setToolBarRandom(0);
                }
                else
                {
                    Item hoverItem = itemManager.Instance.getItem(resultItemArr[idx, 0], resultItemArr[idx, 1]);
                    ToolBarManager.Instance.setToolBar(hoverItem);
                }
            }
        }
    }

    public void hoverOutItem()
    {
        ToolBarManager.Instance.toolBarOnOff(0);
    }
}
