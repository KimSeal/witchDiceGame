using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class upDownManager : MonoBehaviour
{
    [SerializeField]
    public GameObject goldEntity;
    public TextMeshProUGUI goldText;
    public GameObject goldLogo;
    public GameObject jewelEntity;
    public TextMeshProUGUI jewelText;
    public GameObject jewelLogo;
    public int gold;
    public int jewel;
    
    [SerializeField]
    public GameObject[] underHoverBar = new GameObject[6]; //entity, picture, dice 0-3
    public TextMeshProUGUI underHoverBarTitle, underHoverBarDesc; // Title, desc

    [SerializeField]
    public GameObject[] upperHoverBar = new GameObject[6]; //entity, picture, dice 0-3
    public TextMeshProUGUI upperHoverBarTitle, upperHoverBarDesc; // Title, desc

    [SerializeField]
    public GameObject underSkillEntity;
    public GameObject[] underSkillButton = new GameObject[8];
    public GameObject[] underSkillOutline = new GameObject[8];
    public GameObject underBattleOutline;
    public GameObject[] underSkillDiceDescImage = new GameObject[4];
    public TextMeshProUGUI[] underSkillDiceDescText = new TextMeshProUGUI[4];

    [SerializeField]
    public GameObject underTownEntity;
    public GameObject[] underTownButton = new GameObject[8];
    public GameObject[] underTownOutline = new GameObject[8];
    public GameObject[] underTownNewMark = new GameObject[8];
    public int curTownIdx = 0;

    [SerializeField]
    public GameObject underTitleEntity;
    public GameObject[] underTitleButton = new GameObject[8];
    public GameObject[] underTitleOutline = new GameObject[8];
    public GameObject[] underTitleNewMark = new GameObject[8];

    [SerializeField]
    public GameObject backBlackItem;
    public GameObject[] upperItemEff = new GameObject[12];
    public GameObject[] upperItemEffOrigin = new GameObject[12];
    public GameObject[] upperItemButton = new GameObject[12];
    public GameObject[] upperItemOutline = new GameObject[12];

    public GameObject bigDiceItemCharacterEntity;
    public GameObject[] bigDiceItemCharacterButton = new GameObject[4];
    public GameObject[] bigDiceItemCharacterOutline = new GameObject[4];
    public GameObject bigDiceItemCharacterInfo;
    public TextMeshProUGUI bigDiceItemCharacterHpText;
    public TextMeshProUGUI bigDiceItemCharacterArmorText;
    public TextMeshProUGUI bigDiceItemCharacterAtkText;
    public TextMeshProUGUI bigDiceItemCharacterVal0Text;
    public TextMeshProUGUI bigDiceItemCharacterVal1Text;

    public GameObject bigDiceItemCharacterDiceEntity;
    public GameObject[] bigDiceItemCharacterDiceButton = new GameObject[6];
    public GameObject[] bigDiceItemCharacterDiceOutline = new GameObject[6];

    public GameObject bigDiceItemCharacterEquipEntity;
    public GameObject[] bigDiceItemCharacterEquipButton = new GameObject[2];
    public GameObject[] bigDiceItemCharacterEquipOutline = new GameObject[2];

    private int itemSelectDepth = 0;

    [SerializeField]
    public GameObject upperItemTypeInitButton;
    public GameObject upperItemTypeInitOutline;
    public GameObject upperItemTypeEntity;
    public GameObject[] upperItemTypeButton = new GameObject[4];
    public GameObject[] upperItemTypeOutline = new GameObject[4];
    public bool itemTypeOnOff = false;
    public bool itemTypeButtonLock = false;


    [SerializeField]
    public GameObject backBlackSkill;
    public GameObject bigDiceSkillEntity;
    public GameObject[] bigDiceSkillButton = new GameObject[4];
    public GameObject[] bigDiceSkillOutline = new GameObject[4];
    public GameObject[] bigDiceSkillState = new GameObject[4];
    public GameObject[] bigDiceSkillChain = new GameObject[3];

    [SerializeField]
    public GameObject bigDicePowerEntity;
    public GameObject[] bigDicePowerButton = new GameObject[8];
    public GameObject[] bigDicePowerOutline = new GameObject[8];
    public GameObject[] bigDicePowerState = new GameObject[8];
    public GameObject[] bigDicePowerChain = new GameObject[8];
    public GameObject[] bigDicePowerButtonEff = new GameObject[8];
    public GameObject[] bigDicePowerButtonEffOrigin = new GameObject[8];
    public GameObject bigDicePowerCancleObj;

    [SerializeField]
    public GameObject characterEntity;
    public GameObject[] underCharacterButton = new GameObject[4];
    public GameObject[] underCharacterOutline = new GameObject[4];

    [SerializeField]
    public GameObject deleteChkEntity;
    public TextMeshProUGUI deleteChkText;
    public GameObject deleteBtnInit;
    public GameObject deleteBtnInitOutline;
    public GameObject[] deleteBtn = new GameObject[2];
    public GameObject[] deleteOutline = new GameObject[2];
    public TextMeshProUGUI deleteText;

    [SerializeField]
    public GameObject changeChkEntity;
    public GameObject changeBtnInit;
    public GameObject changeBtnInitOutline;
    public GameObject[] changeBtn = new GameObject[4];
    public GameObject[] changeOutline = new GameObject[4];

    private int lockState = 0; //0 : free  1: underbar hover  2: upperbar hover 3: battleMode  4: witchPower 
    private int curSkill = -1;
    private int curItemIdx = -1;
    private int curItemType = 0;
    private int curCharacterIdx = -1;

    private int curUnderBarOption = 0;
    private float goldRotateSize;
    private float jewelRotateSize;
    private float goldRotateVal;
    private float jewelRotateVal;

    private static upDownManager instance = null;


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

    public static upDownManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; i++) optionOnOff[i] = false;
         backBlackSkill.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
        backBlackItem.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
        bigDiceSkillEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 327f, 0f);
        bigDicePowerCancleObj.SetActive(false);
        hoverInItemTypeButton(curItemType);
        cancleChangeBtn();
        cancleDeleteBtn();
        changeOption(3, true);
        itemTypeButtonLock = false;
        curTownIdx = 7;

        goldRotateSize = 0f;
        jewelRotateSize = 0f;
        goldRotateVal =0f;
        jewelRotateVal=0f;

        townName[0] = "Tower";
        townName[1] = "Home";
        townName[2] = "Library";
        townName[3] = "Market Street";
        townName[4] = "???";
        townName[5] = "???";
        townName[6] = "???";
        townName[7] = "Hill";
        initSet = false;
    }

    private bool initSet = false;
    void Update()
    {
        if (!initSet) {
            initSet = true;
            hoverOutUnderTitleButton();
        }
        for (int i = 0; i < 12; i++)
        {
            upperItemEff[i].GetComponent<Image>().sprite = upperItemEffOrigin[i].GetComponent<SpriteRenderer>().sprite;
        }
        for (int i=0;i<8;i++)
        {
            bigDicePowerButtonEff[i].GetComponent<Image>().sprite =
               bigDicePowerButtonEffOrigin[i].GetComponent<SpriteRenderer>().sprite;
        }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        moveBattleUI(moveArrY[0], underHoverBar[0]);
        moveBattleUI(moveArrY[1], upperHoverBar[0]);

        if (goldRotateSize > 0f) {
            goldRotateSize -= 1f;
            goldRotateVal += 0.5f;
            if (goldRotateVal > 100 * Mathf.PI) goldRotateVal -= 100.0f * Mathf.PI;
            goldLogo.GetComponent<RectTransform>().rotation = Quaternion.Euler(goldLogo.transform.rotation.x, goldLogo.transform.rotation.y, goldRotateSize * Mathf.Sin(goldRotateVal));
        }
        if(jewelRotateSize > 0f)
        {
            jewelRotateSize -= 1f;
            jewelRotateVal += 0.5f;
            if (jewelRotateVal > 100 * Mathf.PI) jewelRotateVal -= 100.0f * Mathf.PI;
            jewelLogo.GetComponent<RectTransform>().rotation = Quaternion.Euler(jewelLogo.transform.rotation.x, jewelLogo.transform.rotation.y, jewelRotateSize * Mathf.Sin(jewelRotateVal));
        }
            
    }

    
    public void activeWitchPowerDice(int powerIdx, int diceIdx)
    {
        bigDicePowerButtonEffOrigin[diceIdx].GetComponent<Animator>().Play(powerIdx.ToString());
        bigDicePowerButtonEff[diceIdx].GetComponent<Image>().sprite =
               bigDicePowerButtonEffOrigin[diceIdx].GetComponent<SpriteRenderer>().sprite;
    }
    public void activePassiveItem(int itemIdx)
    {
        upperItemEffOrigin[itemIdx].GetComponent<Animator>().Play("Active");
    }
    public void resetUI()
    {
        clickItem(-1);
        clickItem(-1);
        clickCharacterButton(-1);
    }

    //underBar, upperBar
    private float[] moveArrY = { -2f, 225f, };

    public void onOffUI(int idx, int opt) {
        moveArrY[idx] = moveConstY[opt, idx];
    }
    //off, on
    private float[,] moveConstY = {
        {-2f, 225f},
        { 55f, 168f}
    };

    public void rotateGold()
    {
        goldRotateSize = 30f;
        goldRotateVal = 0f;
    }

    public void rotateJewel()
    {
        jewelRotateSize = 30f;
        jewelRotateVal = 0f;
    }

    public void hoverInWitchHatButton()
    {
        ToolBarManager.Instance.setToolBar(TalkManager.Instance.getDesc(91), TalkManager.Instance.getDesc(40),
           Resources.Load<Sprite>("sprite/townImage/spr_Destiny Change"));
    }
    public void hoverOutWitchHatButton()
    {
        ToolBarManager.Instance.toolBarOnOff(0);
    }

    public void hoverInAdventureWitchPowerButton()
    {
        ToolBarManager.Instance.setToolBar(TalkManager.Instance.getDesc(91), TalkManager.Instance.getDesc(48),
           Resources.Load<Sprite>("sprite/townImage/spr_Destiny Change"));
    }
    public void hoverOutAdventureWitchPowerButton()
    {
        ToolBarManager.Instance.toolBarOnOff(0);
    }
    public void hoverInWitchPowerButton()
    {
        int powerIdx = BattleManager.Instance.getCurWitchPower();
        ToolBarManager.Instance.setToolBar(powerName[powerIdx], TalkManager.Instance.getDesc(41 + powerIdx), Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPowerSmall/spr_witchPowerSmall_" + powerName[powerIdx]));
    }
    public void hoverOutWitchPowerButton()
    {
        ToolBarManager.Instance.toolBarOnOff(0);
    }

    public string[] townName = {"Tower", "Home", "Library", "Market Street", "???", "???", "???", "Hill"};
    
    public void hoverInUnderTownButton(int idx)
    {
        if (idx == 0 || idx == 1 || idx == 7 ||
            (idx == 2 && townCondition(idx)) // chapter1 clear
            ) {
            
            skillDescUpdate("none", 0, 0, 0, 0, townName[idx], TalkManager.Instance.getDesc(30 + idx));
            underHoverBar[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/townImage/spr_town_" + townName[idx]);
        }
        else
        {
            skillDescUpdate("none", 0, 0, 0, 0,"???", TalkManager.Instance.getDesc(38));
            underHoverBar[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/townImage/spr_town_" + "lock");
        }
        underTownOutline[idx].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
        TownManager.Instance.hoverInUIBtn(idx);
        onOffUI(0, 1);
    }
    public void hoverOutUnderTownButton()
    {
        for (int i=0;i<underTownButton.Length;i++) {
            underTownOutline[i].GetComponent<Image>().sprite
                = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        underTownOutline[curTownIdx].GetComponent<Image>().sprite
                = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");

        TownManager.Instance.hoverOutUIBtn();
        skillDescUpdate("none", 0, 0, 0, 0, "", "");
        onOffUI(0, 0);
        for (int i=0;i<8;i++) // new mark 관련
        {
            if (TownManager.Instance.getTownNewMark(i)) {
                underTownNewMark[i].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_newMark");
            }
            else
            {
                underTownNewMark[i].GetComponent<Image>().sprite
                = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            }
        }
       
    }
    public void clickUnderTitleButton(int idx)
    {
        if (idx == 0) {
            if (jsonDataManager.Instance.getTutorialDid()) {
                AdventureManager.Instance.activeTutorialButton(true);
            }
        }
        if (idx == 7) {
            if (jsonDataManager.Instance.getTutorialDid()) {
                AdventureManager.Instance.mainPlayButton(false);
            }
            else
            {
                AdventureManager.Instance.mainPlayButton(true);
            }
        }
    } 
    public void hoverInUnderTitleButton(int idx)
    {
        if (idx == 0 && jsonDataManager.Instance.getTutorialDid())
        {
            skillDescUpdate("none", 0, 0, 0, 0, "Tutorial", TalkManager.Instance.getDesc(123));
        }
        else if (idx == 7 && !jsonDataManager.Instance.getTutorialDid()) {
            skillDescUpdate("none", 0, 0, 0, 0, "Tutorial", TalkManager.Instance.getDesc(123));
        }
        else if (idx == 7 && jsonDataManager.Instance.getTutorialDid())
        {
            skillDescUpdate("none", 0, 0, 0, 0, townName[idx], TalkManager.Instance.getDesc(30 + idx));
        }
        else
        {
            skillDescUpdate("none", 0, 0, 0, 0, "???", TalkManager.Instance.getDesc(38));
        }
        underHoverBar[1].GetComponent<Image>().sprite = underTitleButton[idx].GetComponent<Image>().sprite;


        underTitleOutline[idx].GetComponent<Image>().sprite
             = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
        onOffUI(0, 1);
    }
    public void hoverOutUnderTitleButton()
    {
       
        for (int i = 0; i < 8; i++) //new mark 다 초기화
        {
            underTitleOutline[i].GetComponent<Image>().sprite
               = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            underTitleNewMark[i].GetComponent<Image>().sprite
                = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            underTitleButton[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/townImage/spr_town_" + "lock");
        }

        if (!jsonDataManager.Instance.getTutorialDid()) { // 아직 아무것도 안된 경우 튜토리얼만 열어두기.
            underTitleNewMark[7].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_newMark");
            underTitleButton[7].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_deleteInitBtn");
        }
        else
        {
            underTitleButton[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_deleteInitBtn");
            underTitleButton[7].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/townImage/spr_town_Hill");
        }
        skillDescUpdate("none", 0, 0, 0, 0, "", "");
        onOffUI(0, 0);
    }
    public void activeTownUI(bool input) {
        
        if (input) {
            hoverOutUnderTownButton();
            underTownEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 18f, 0f);
        }
        else
        {
            underTownEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -50f, 0f);
        }
    }

    public bool townCondition(int idx)
    {
        if (idx == 0 || idx == 1 || idx == 7) {
            return true;
        }
        if (idx == 2 && jsonDataManager.Instance.getChapterRead(0, 2) == 2) {
            return true;
        }
        if (idx == 4) {
            return true;
        }
        return false;
    }
    public void clickUnderTownButton(int idx)
    {
        if (townCondition(idx))
        {
            curTownIdx = idx;
            TownManager.Instance.clickTownUI(idx);
            hoverOutUnderTownButton();
        }
        else
        {
            fullUI.showFull(6);
        }
    }

    public Character getCharacter(int idx)
    {
        if (!AdventureManager.Instance.getBattleEventChk()) return CharacterManager.Instance.getCharacter(idx);
        else return BattleManager.Instance.getCharacter(idx);
    }
    public bool getCharacterExist(int idx)
    {
        return (getCharacter(idx) != null && getCharacter(idx).getCurState() == 0);
    }
    #region
    public void cancleChangeBtn()
    {
        changeChkEntity.SetActive(false);
    }

    public void setItemTypeButtonLock(bool onOff)
    {
        itemTypeButtonLock = onOff;
    }

    public void hoverInUpperItemTypeInit()
    {
        upperItemTypeInitOutline.GetComponent<Image>().sprite
                = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
    }
    public void hoverOutUpperItemTypeInit() {
        upperItemTypeInitOutline.GetComponent<Image>().sprite
                = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
    }

    public void addGold(int a)
    {
        rotateGold();
        gold += a;
        int goldTemp = gold;
        if (gold < 0) goldTemp = 0;
        goldText.text = goldTemp.ToString();
    }

    public void addJewel(int a)
    {
        rotateJewel();
        jewel += a;
        int jewelTemp = jewel;
        jewelText.text = jewelTemp.ToString();
    }

    public void setInit(int goldVal, int jewelVal)
    {
        gold = goldVal;
        jewel = jewelVal;
        goldText.text = gold.ToString();
        jewelText.text = jewel.ToString();
    }

    public void clickUpperItemTypeInit()
    {
        if (itemTypeButtonLock)
        {
            Debug.Log("why?");
            return;
        }

        itemTypeOnOff = !itemTypeOnOff;
        if (itemTypeOnOff)
        {
            upperItemTypeEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(-108f, 0f, 0f);
        }
        else
        {
            upperItemTypeEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(-108f, 200f, 0f);
        }

            

    }

    public void hoverInChangeBtn(int i)
    {
        changeOutline[i].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
       
    }

    public void hoverOutChangeBtn(int input)
    {
        for (int i = 0; i < 4; i++)
        {
            if (curCharacterIdx != i)
            {
                changeOutline[i].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            }
            else
            {
                changeOutline[i].GetComponent<Image>().sprite
                = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
            }
        }
    }

    public void clickChangeBtn(int i)
    {
        

        changeChkEntity.SetActive(false);
        if (i != -1)
        {
            itemManager.Instance.swapCharacter(curCharacterIdx, i);
            clickCharacterButton(i);
            clickChangeInitBtn();
        }
    }

    public void hoverInChangeInitBtn()
    {
        if (AdventureManager.Instance.getBattleEventChk())
        {
            return;
        }
        changeBtnInitOutline.GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
        itemManager.Instance.hoverInSwapOrDelete(0);
    }

    public void hoverOutChangeInitBtn()
    {
        changeBtnInitOutline.GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        itemManager.Instance.hoverOutDesc();
    }
    public void clickChangeInitBtn()
    {
        if (AdventureManager.Instance.getBattleEventChk())
        {
            clickCharacterButton(-1);
            return;
        }
        changeChkEntity.SetActive(true);
        for (int characterSelectIdx = 0; characterSelectIdx < 4; characterSelectIdx++)
        {
            Character tempCharacter = CharacterManager.Instance.getCharacter(characterSelectIdx);
            if (tempCharacter == null || tempCharacter.getCurState() != 0)
            {
                changeBtn[characterSelectIdx].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_no_face");
            }
            else
            {
                if (Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + tempCharacter.getName() + "_face") != null)
                {
                    changeBtn[characterSelectIdx].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + tempCharacter.getName() + "_face");
                }
                else { changeBtn[characterSelectIdx].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_noImage_face"); }
            }
        }
        hoverOutChangeBtn(0);
    }


    public void hoverInDeleteBtn(int i)
    {
        if (AdventureManager.Instance.getBattleEventChk())
        {
            return;
        }
        deleteOutline[i].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPowerUI/spr_ui_library_yesBtn_outline");
        
    }
    public void hoverOutDeleteBtn()
    {
        deleteOutline[0].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        deleteOutline[1].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
    }

    public void clickDeleteBtn()
    {
        itemManager.Instance.deleteCharacter();
        hoverOutDeleteBtn();
        cancleDeleteBtn();
        clickCharacterButton(-1);
    }
    public void cancleDeleteBtn()
    {
        deleteChkEntity.SetActive(false);
    }

    public void hoverInDeleteInitBtn()
    {
        if (AdventureManager.Instance.getBattleEventChk())
        {
            return;
        }
        deleteBtnInitOutline.GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
        itemManager.Instance.hoverInSwapOrDelete(1);
    }
    public void hoverOutDeleteInitBtn()
    {
        deleteBtnInitOutline.GetComponent<Image>().sprite
             = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        itemManager.Instance.hoverOutDesc();
    }

    public void clickDeleteInitBtn()
    {
        if (AdventureManager.Instance.getBattleEventChk())
        {
            clickCharacterButton(-1);
            return;
        }
        deleteChkText.text = TalkManager.Instance.getDesc(18);
        deleteChkEntity.SetActive(true);
        hoverOutDeleteBtn();
    }
    #endregion

    private bool[] optionOnOff = new bool[4];
    // 0 : non
    // 1 : skillUI
    // 2 : characterUI
    // 3 : titleUI

    public void changeOption(int i, bool onOff)
    {
        optionOnOff[i] = onOff;

        underTitleEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -50f, 0f);
        underSkillEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -50f, 0f);
        characterEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -50f, 0f);

        if (optionOnOff[3]) { //title SelectUI
            underTitleEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 18f, 0f);
            return;
        }
        if (optionOnOff[2]) { //characterSelect UI
            characterEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 18f, 0f);
            return;
        }
        if(optionOnOff[1]){ //skillSelectUI
            underSkillEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 18f, 0f);
        }
    }

    public void hoverInBigDiceItemCharacter(int idx)
    {
        if (getCharacterExist(idx))
        {
            bigDiceItemCharacterOutline[idx].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
            updateBigDiceItemCharacter(idx);
        }

    }
    public void hoverOutBigDiceItemCharacter()
    {
        for (int idx = 0; idx < 4; idx++) {
            if (curCharacterIdx != idx)
            {
                bigDiceItemCharacterOutline[idx].GetComponent<Image>().sprite
                 = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            }
            else
            {
                bigDiceItemCharacterOutline[idx].GetComponent<Image>().sprite
                = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
            }
        }
        updateBigDiceItemCharacter(curCharacterIdx);
    }
    public void hoverInBigDiceItemDice(int idx)
    {
        bigDiceItemCharacterDiceOutline[idx].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
    }
    public void hoverOutBigDiceItemDice(int idx)
    {
        bigDiceItemCharacterDiceOutline[idx].GetComponent<Image>().sprite
           = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
    }

    public void hoverInBigDiceItemEquip(int idx)
    {
        bigDiceItemCharacterEquipOutline[idx].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
    }
    public void hoverOutBigDiceItemEquip(int idx)
    {
        bigDiceItemCharacterEquipOutline[idx].GetComponent<Image>().sprite
           = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
    }

    public void hoverInBigDiceSkill(int idx)
    {
        if (BattleManager.Instance.getCharacter(idx) != null)
        {
            bigDiceSkillOutline[idx].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
        }
    }
    public void hoverOutBigDiceSkill(int idx)
    {
        bigDiceSkillOutline[idx].GetComponent<Image>().sprite
             = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
    }

    string[] typeArr = { "consume", "dice", "equip", "passive", "destiny" }; //item type string 
    string[] typeArr2 = { "- CONSUME -", "- DICE -", "- EQUIP -", "- PASSIVE -", "- DESTINY -" };
    public void updateUpperItem(bool deleteChk, int idx, int typeIdx, string name)
    {
        if (deleteChk)
        {
            upperItemButton[idx].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
        }
        else
        {
            upperItemButton[idx].GetComponent<Image>().sprite =
                Resources.Load<Sprite>("sprite/TestSprite/itemSprite/" + typeArr[typeIdx] + "ItemSprite/spr_item_" + typeArr[typeIdx] + "_" + name);
        }
    }
    public void updateUpperItemOutline(int idx, bool onOff)
    {

    }

    public void hoverInUpperBar(int idx) {
        if(true)
        //if (idx == 11 || itemManager.Instance.getCurItem(idx) != null)
        {
            upperItemOutline[idx].GetComponent<Image>().sprite
                = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
            itemManager.Instance.hoverInItem(idx);

            if (lockState != 2)
            {
                onOffUI(1, 1);
                if (lockState == 0)
                {
                    BattleManager.Instance.updateMoveUI(1);
                }
            }
        }
    }
    public void hoverOutUpperBar(int idx)
    {
        if (curItemIdx != idx)
        {
            upperItemOutline[idx].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            if (curItemIdx != -1) itemManager.Instance.hoverInItem(curItemIdx);
            else
            {
                updateUpperHoverBar(0, null);
            }

        }
        if (lockState != 2)
        {
            onOffUI(1, 0);

            if (lockState == 0)
            {
                BattleManager.Instance.updateMoveUI(0);
            }
        }
    }

    public void hoverInCharacterButton(int i)
    {
        Character tempCharacter = getCharacter(i);
        if (!(tempCharacter == null || tempCharacter.getCurState() != 0))
        {
            underCharacterOutline[i].GetComponent<Image>().sprite
                = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
        }
    }
    public void hoverOutCharacterButton()
    {
        for (int idx=0;idx<4;idx++) {
            if (curCharacterIdx != idx)
            {
                underCharacterOutline[idx].GetComponent<Image>().sprite
               = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            }
        }
    }

    public void clickCharacterButton(int input)
    {
        if (AdventureManager.Instance.getTutorial() == 1) {
            return;
        }
        else if (AdventureManager.Instance.getTutorial() == 2 && !AdventureManager.Instance.getTutorialVal4ErrorChk())            
        {
            return;
        }
        for (int i = 0; i < 4; i++)
        {
            Character tempCharacter = getCharacter(i);
            if (tempCharacter == null || tempCharacter.getCurState() != 0)
            {
                underCharacterButton[i].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_no_face");
            }
            else
            {
                underCharacterButton[i].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + tempCharacter.getName() + "_face");
            }
        }

        if (AdventureManager.Instance.getBattleEventChk())
        {
            changeBtnInit.GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            deleteBtnInit.GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            /*
            changeBtnInit.GetComponent<Image>().sprite
                = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_changeInitBtn_off");
            deleteBtnInit.GetComponent<Image>().sprite
                = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_deleteInitBtn_off");
            */
            curCharacterIdx = itemManager.Instance.click_Character_battle(input);
            BattleManager.Instance.hoverOutCharacter(0);

        }
        else
        {
            changeBtnInit.GetComponent<Image>().sprite
               = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_changeInitBtn");
            deleteBtnInit.GetComponent<Image>().sprite
                = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_deleteInitBtn");

            curCharacterIdx = itemManager.Instance.click_Character(input);
           
        }
        AdventureManager.Instance.updateAdventureDice();

        if (curCharacterIdx == -1)
        {
            changeOption(2, false);
            if (AdventureManager.Instance.getTutorial() == 3 && AdventureManager.Instance.getTutorialVal4ErrorChk())
            {
                AdventureManager.Instance.setTutorialVal4ErrorChk(false);
                AdventureManager.Instance.setTutorial(4);
            }
        }
        else
        {
            if (AdventureManager.Instance.getTutorial() == 2 && AdventureManager.Instance.getTutorialVal4ErrorChk())
            {
                AdventureManager.Instance.setTutorialVal4ErrorChk(false);
                AdventureManager.Instance.setTutorial(3);
            }
            changeOption(2, true);
            hoverOutCharacterButton();
        }
    }

    public void hoverInItemTypeButton(int idx)
    {
        upperItemTypeOutline[idx].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");

    }
    public void hoverOutItemTypeButton(int idx)
    {
        if (curItemType != idx)
        {
            upperItemTypeOutline[idx].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        
    }

    public void clickBattleStart()
    {
        if (false)
        {
            
        }
        else
        {
            battleStart();
        }
    }

    public void battleStart()
    {
        if (BattleManager.Instance.skillEmptyChk()) {
            fullUI.showFull(68);
            BattleManager.Instance.skillEmptyChkEnd();
            return;
        }
        if (AdventureManager.Instance.getTutorial() == 7 || AdventureManager.Instance.getTutorial() == 8 ||
            AdventureManager.Instance.getTutorial() == 11 || AdventureManager.Instance.getTutorial() == 12 ||
                AdventureManager.Instance.getTutorial() == 13 || AdventureManager.Instance.getTutorial() == 14) {
            return;
        }
        clickItem(-1);
        clickItem(-1);
        BattleManager.Instance.setCurClickSkill(-1);
        lockState = 3;
        BattleManager.Instance.updateMoveUI(3);
        BattleManager.Instance.moveToBattlePhase();
    }

    public void hoverInUnderBarBattle()
    {
        underBattleOutline.GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
    }
    public void hoverOutUnderBarBattle()
    {
        underBattleOutline.GetComponent<Image>().sprite
                     = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
    }
    public void hoverInUnderBarSkill(int idx)
    {

        Character tempCharacter = getCharacter(idx/2);
        if (!(tempCharacter == null || tempCharacter.getCurState() != 0))
        {
            underSkillOutline[idx].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
            BattleManager.Instance.makeSkillCommand(idx / 2, idx % 2);

            if (lockState != 1)
            {
                onOffUI(0, 1);

                if (lockState == 0)
                {
                    BattleManager.Instance.updateMoveUI(2);

                }
            }
        }
    }

    public void hoverInDiceSkill(int idx)
    {
        if (BattleManager.Instance.getDiceTake(idx) != -999)
        {
            ToolBarManager.Instance.setToolBar(BattleManager.Instance.getSkillTake(idx));

        }
    }
    public void hoverOutDiceSkill()
    {
        ToolBarManager.Instance.toolBarOnOff(0);
    }
    public void hoverOutUnderBarSkill(int idx)
    {
        if (curSkill != idx)
        {
            underSkillOutline[idx].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            if (curSkill != -1) BattleManager.Instance.makeSkillCommand(curSkill / 2, curSkill % 2);
        }
        if (lockState != 1) {
            onOffUI(0, 0);

            if (lockState == 0) //고정 상태가 아니라면, 뒤에거도 움직여줄 것.
            {
                BattleManager.Instance.updateMoveUI(0);
            }
        }
    }

    public void clickSkill(int input)
    {
        //전투 중에는 추가 잠금 불가능하게
        if (input != -1 && lockState == 3) return;

        deleteOtherLock(1);
        for (int idx = 0; idx < 8; idx++) {
            underSkillOutline[idx].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        curSkill = (input / 10) * 2 + input % 10;
        if (input != -1)
        {
            //deleteOtherLock(1);
            backBlackSkill.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 108f, 0f);
            bigDiceSkillEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 130f, 0f);
            hoverInUnderBarSkill(curSkill);
            lockState = 1; //클릭시 현재 스킬에 대한 설명으로 고정.
            updateBigDiceSkill();
        }
        else {
            if (AdventureManager.Instance.getTutorial() == 8) AdventureManager.Instance.setTutorial(9);
            backBlackSkill.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
            bigDiceSkillEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 327f, 0f);
            updateBigDiceSkill();
            lockState = 0;
            hoverOutUnderBarSkill(0);
        }

    }


    public void deleteOtherLock(int tryLock)
    {
        if (tryLock == lockState) return;

        if (lockState == 1) {
            BattleManager.Instance.setCurClickSkill(-1);
        }
        if (lockState == 2) {
            clickItem(-1);
        }
        if (lockState == 4) {
            
            clickEnterPower(-1);
        }
    }

    public void clickTrashButton()
    {
        if (AdventureManager.Instance.getTutorial() == 12 || AdventureManager.Instance.getTutorial() == 11) {
            fullUI.showFull(65);
            return;
        }
        if (curItemIdx >= 0) {
            if (AdventureManager.Instance.getTutorial() == 5 && curItemType == 0 && curItemIdx == 0) AdventureManager.Instance.setTutorial(6);
            itemManager.Instance.useItem(curItemType, curItemIdx);
        }
    }

    public void clickItem(int input)
    {
        //clickCharacterButton(-1);
        //전투 중에는 추가 잠금 불가능하게
        if (input >= 0 && !AdventureManager.Instance.getAdventureStartChk()) {
            fullUI.showFull(69);
            return; 
        }
        if (input != -1 && lockState == 3) return;
        if (AdventureManager.Instance.getTutorial() != 0 && (AdventureManager.Instance.getTutorial() != 4 && AdventureManager.Instance.getTutorial() != 5 && AdventureManager.Instance.getTutorial() != 6 &&  AdventureManager.Instance.getTutorial() <= 9))
        {
            //fullUI.showFull(65);
            return;
        }/*
        if (curItemType == 3) {
            return;
        }
        */
        clickCharacterButton(-1);
        deleteOtherLock(2);
        for (int idx = 0; idx < upperItemOutline.Length; idx++)
        {
            upperItemOutline[idx].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        
        if (input != -1)
        {
            curItemIdx = input;

            if (itemSelectDepth == 0)
            {
                curCharacterIdx = -1;
            }
            hoverOutBigDiceItemCharacter();
            
            //deleteOtherLock(2);
            // curItemIdx = input;
            backBlackItem.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 108f, 0f);
            if (itemSelectDepth == 0) bigDiceItemUpdateByDepth(1);
            //bigDiceSkillEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 120f, 0f);
            hoverInUpperBar(input);
            lockState = 2; //클릭시 현재 스킬에 대한 설명으로 고정.
            if (AdventureManager.Instance.getTutorial() == 4 && input == 0 && curItemType == 0 && AdventureManager.Instance.getTutorialVal4ErrorChk()) { AdventureManager.Instance.setTutorial(5); }
            if (AdventureManager.Instance.getTutorial() == 11 && input == 0 && curItemType == 1) AdventureManager.Instance.setTutorial(12);
            //updatebigDiceSkill();
        }
        else {
            if (itemSelectDepth == 1) {
                curItemIdx = -1;
                backBlackItem.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
                bigDiceItemUpdateByDepth(0);
                lockState = 0;
                hoverOutUpperBar(0);
                if (AdventureManager.Instance.getTutorial() == 6) AdventureManager.Instance.setTutorial(7);
            }
            else if(itemSelectDepth == 2) {
                bigDiceItemUpdateByDepth(1);
            }
            //itemManager.Instance.hoverOutItem(input);

        }
    }

    public void clickBigDiceItemCharacter(int idx)
    {
        if (AdventureManager.Instance.getTutorial() == 4)
        {
            return;
        }
        if (getCharacterExist(idx))
        {
            curCharacterIdx = idx;
            if (curItemType == 3) {
                return;
            }
            if (curItemType == 0)
            {
                itemManager.Instance.useConsumeItem(curCharacterIdx, curItemType, curItemIdx);//click_info_useItem();

                bigDiceItemUpdateByDepth(1);
                curItemIdx = -1;
                hoverOutUpperBar(0);
            }
            else if (curItemType == 1)
            {
                
                bigDiceItemUpdateByDepth(2);
            }
            else if (curItemType == 2)
            {
                bigDiceItemUpdateByDepth(2);
            }
            hoverOutBigDiceItemCharacter();
            BattleManager.Instance.updateHp();
        }
    }
    public void clickBigDiceItemCharacterDice(int idx)
    {
        if (curItemIdx == -1 || curCharacterIdx == -1 || !getCharacterExist(curCharacterIdx)) return; // 선택된게 없으면 캔슬.

        for (int i = 0; i < upperItemOutline.Length; i++)
        {
            upperItemOutline[i].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        itemManager.Instance.click_dice_changeNum(curCharacterIdx, idx, curItemIdx); // 아이템 사용.
        
        updateBigDiceItemCharacter(curCharacterIdx);
        curItemIdx = -1;
        hoverOutUpperBar(0);
        //if (curItemIdx != -1) clickItem(curItemIdx);
    }

    public void clickBigDiceItemCharacterEquip(int idx)
    {
        if (curItemIdx == -1 || curCharacterIdx == -1 || !getCharacterExist(curCharacterIdx)) return; // 선택된게 없으면 캔슬.

        for (int i = 0; i < upperItemOutline.Length; i++)
        {
            upperItemOutline[i].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        itemManager.Instance.click_equip_changeNum(curCharacterIdx, curItemIdx, idx);
        updateBigDiceItemCharacter(curCharacterIdx);
        curItemIdx = -1;
        hoverOutUpperBar(0);
        //if (curItemIdx != -1) clickItem(curItemIdx);
    }

    public void updateBigDiceItemCharacter(int characterIdx)
    {
        if (characterIdx == -1)
        {
            bigDiceItemCharacterHpText.text = "";
            bigDiceItemCharacterArmorText.text = "";
            bigDiceItemCharacterAtkText.text = "";
            bigDiceItemCharacterVal0Text.text = "";
            bigDiceItemCharacterVal1Text.text = "";
            //character Face
            for (int i = 0; i < 4; i++)
            {
                Character tempCharacter = getCharacter(i);
                if (tempCharacter == null || tempCharacter.getCurState() != 0)
                {
                    bigDiceItemCharacterButton[i].GetComponent<Image>().sprite 
                        = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_no_face");
                }
                else
                {
                    bigDiceItemCharacterButton[i].GetComponent<Image>().sprite
                        = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + tempCharacter.getName() + "_face");
                }
            }

            //character dice
            for (int i = 0; i < 6; i++)
            {
                bigDiceItemCharacterDiceButton[i].GetComponent<Image>().sprite =
                Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            }

            //character Equip
            for (int i = 0; i < 2; i++)
            {
                bigDiceItemCharacterEquipButton[i].GetComponent<Image>().sprite =
                     Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_no_face");
            }
            return;
        }

        if (getCharacterExist(characterIdx))
        {
            //character info
            bigDiceItemCharacterHpText.text = getCharacter(characterIdx).getHp().ToString() + " / "
                + getCharacter(characterIdx).getMaxHp().ToString();
            bigDiceItemCharacterArmorText.text = "0";
            bigDiceItemCharacterAtkText.text = getCharacter(characterIdx).getPhyAtk().ToString();

            //character dice
            for (int i = 0; i < 6; i++)
            {

                bigDiceItemCharacterDiceButton[i].GetComponent<Image>().sprite =
                Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + getCharacter(characterIdx).getDice(i).ToString());
            }

            //character Equip
            Item tempItem;
            for (int i = 0; i < 2; i++)
            {
                tempItem = getCharacter(characterIdx).getItem(i);
                bigDiceItemCharacterEquipButton[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/equipItemSprite/spr_item_equip_" + tempItem.getItemName());
            }
        }
        else
        {
            bigDiceItemCharacterHpText.text = "";
            bigDiceItemCharacterArmorText.text = "";
            bigDiceItemCharacterAtkText.text = "";
            bigDiceItemCharacterVal0Text.text = "";
            bigDiceItemCharacterVal1Text.text = "";
            //character Face
            for (int i = 0; i < 4; i++)
            {
                if (getCharacterExist(i))
                {
                    bigDiceItemCharacterButton[i].GetComponent<Image>().sprite
                        = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + getCharacter(i).getName() + "_face");
                }
                else
                {
                    bigDiceItemCharacterButton[i].GetComponent<Image>().sprite
                         = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_no_face");
                }
            }

            //character dice
            for (int i = 0; i < 6; i++)
            {
                bigDiceItemCharacterDiceButton[i].GetComponent<Image>().sprite =
                Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            }

            //character Equip
            for (int i = 0; i < 2; i++)
            {
                bigDiceItemCharacterEquipButton[i].GetComponent<Image>().sprite =
                     Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_no_face");
            }
        }

    }
    public void bigDiceItemUpdateByDepth(int depth)
    {
        itemSelectDepth = depth;
        if (depth == 0)
        {
            if (AdventureManager.Instance.getTutorial() == 13) AdventureManager.Instance.setTutorial(14);
            backBlackItem.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
            bigDiceItemCharacterEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
            bigDiceItemCharacterDiceEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
            bigDiceItemCharacterEquipEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
            bigDiceItemCharacterInfo.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
        }
        else if (depth == 1)
        {
            backBlackItem.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 108f, 0f);
            bigDiceItemCharacterEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 108f, 0f);
            bigDiceItemCharacterDiceEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
            bigDiceItemCharacterEquipEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
            bigDiceItemCharacterInfo.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 100f, 0f);
        }
        else if (depth == 2)
        {
            backBlackItem.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 108f, 0f);
            bigDiceItemCharacterEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 108f, 0f);
            bigDiceItemCharacterInfo.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
            if (curItemType == 1)
            {
                bigDiceItemCharacterDiceEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 100f, 0f);
                bigDiceItemCharacterEquipEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
                
            }
            if (curItemType == 2)
            {
                bigDiceItemCharacterDiceEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
                bigDiceItemCharacterEquipEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 100f, 0f);
                
            }
        }
        updateBigDiceItemCharacter(curCharacterIdx);
    }

    public void clickBigDicePower(int idx)
    {
        BattleManager.Instance.select_witchPower_Dice(idx);
    }
    public void clickEnterPower(int input)
    {
        //전투 중에는 추가 잠금 불가능하게
        if (input != -1 && lockState == 3) return;

        if (AdventureManager.Instance.getTutorial() != 0 && AdventureManager.Instance.getTutorial() < 13) return;
        deleteOtherLock(4);

        if (input != -1)
        {
            if (AdventureManager.Instance.getTutorial() == 14) AdventureManager.Instance.setTutorial(15);
            //backBlack.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 108f, 0f);
            hoverOutBigDicePowerButton();
            BattleManager.Instance.updateMoveUI(4);
            BattleManager.Instance.witchPowerPhase();
            bigDicePowerEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 60f, 0f);
            bigDicePowerCancleObj.SetActive(true);
            lockState = 4; //클릭시 현재 스킬에 대한 설명으로 고정.
            updateBigDicePower();
        }
        else
        {
           
            if (AdventureManager.Instance.getTutorial() == 16) AdventureManager.Instance.setTutorial(17);
            bigDicePowerCancleObj.SetActive(false);
            BattleManager.Instance.deleteWitchPowerUI();
            //backBlack.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
            bigDicePowerEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
            lockState = 0;
            BattleManager.Instance.updateMoveUI(0);
        }
    }


    public void clickItemTypeButton(int idx)
    {
        /*
        if (idx == 3) {
            clickItem(-1);
            clickItem(-1);
        }
        */
        for (int i = 0; i < upperItemTypeOutline.Length; i++)
        {
            upperItemTypeOutline[i].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        for (int i = 0; i < upperItemOutline.Length; i++)
        {
            upperItemOutline[i].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        hoverInItemTypeButton(idx);

        if (curItemType != idx) {
            updateUpperHoverBar(0, null);
        }

        curItemType = idx;
        curItemIdx = -1;

        if (curItemType == 0) upperItemTypeInitButton.GetComponent<Image>().sprite
                 = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_itemType_consume");
        if (curItemType == 1) upperItemTypeInitButton.GetComponent<Image>().sprite
                 = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_itemType_dice");
        if (curItemType == 2) upperItemTypeInitButton.GetComponent<Image>().sprite
                 = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_itemType_equip");
        if (curItemType == 3) upperItemTypeInitButton.GetComponent<Image>().sprite
                 = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_itemType_passive");

        itemTypeOnOff = false;
        upperItemTypeEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(-108f, 200f, 0f);

        //deleteOtherLock(0);

        //item use ui랑 호환 맞추기.
        /*
        if (curItemType == 3) //passive
        {
            bigDiceItemUpdateByDepth(0);
        }
        else */if (itemSelectDepth == 0 || itemSelectDepth == 1)
        {
            bigDiceItemUpdateByDepth(itemSelectDepth);
        }
        else if(itemSelectDepth == 2){
            if (curItemType == 0 || curItemType == 3) { //consume or passive
                bigDiceItemUpdateByDepth(1);
            }
            else //dice or equip
            {
                bigDiceItemUpdateByDepth(itemSelectDepth);
            }
        }

        itemManager.Instance.click_itemType_selectButton(idx);
    }

    public void skillDescUpdate(string pictureStr, int needDice0, int needDice1, int needDice2, int needDice3 ,string skillName, string skillCommand) {
        int[] arrNum = new int[4];
        arrNum[0] = needDice0; arrNum[1] = needDice1; arrNum[2] = needDice2; arrNum[3] = needDice3;

        underHoverBar[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + pictureStr);


        
        for (int i=0;i<4;i++)
        {
            underHoverBar[2 + i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_" + arrNum[i].ToString());

            if (lockState == 1)
            {
                underSkillDiceDescImage[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_" + arrNum[i].ToString());
                if (arrNum[i] <= 6 && arrNum[i] >= 1) underSkillDiceDescText[i].text = arrNum[i].ToString() + " "+TalkManager.Instance.getDesc(99);
                else if (arrNum[i] == 7) underSkillDiceDescText[i].text = TalkManager.Instance.getDesc(27);
                else if (arrNum[i] == 8) underSkillDiceDescText[i].text = TalkManager.Instance.getDesc(28);
                else if (arrNum[i] == 9) underSkillDiceDescText[i].text = TalkManager.Instance.getDesc(29);
                else if (arrNum[i] >= 11 && arrNum[i] <= 16) underSkillDiceDescText[i].text = (arrNum[i] % 10).ToString() + " " + TalkManager.Instance.getDesc(25);
                else if (arrNum[i] >= 21 && arrNum[i] <= 26) underSkillDiceDescText[i].text = (arrNum[i] % 10).ToString() + " " + TalkManager.Instance.getDesc(26);
                else underSkillDiceDescText[i].text = "";
            }
            else {
                underSkillDiceDescImage[i].GetComponent<Image>().sprite =
                Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                underSkillDiceDescText[i].text = "";
            }
        }

        underHoverBarTitle.text = skillName;
        underHoverBarDesc.text = skillCommand;
    }
    public void skillIconUpdate(int idx, string str)
    {
        underSkillButton[idx].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + str);
    }
    public string[] powerName = { "Reroll Origin", "Reroll", "Add", "Sub"};
    
    public void updateUpperHoverBar(int option, Item item)
    {
        if(option == 1) // delete Bar
        {
            upperHoverBar[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            upperHoverBarTitle.text = "Delete Box";
            upperHoverBarDesc.text = TalkManager.Instance.getDesc(8);//"아이템을 선택 후 이곳을 클릭하면\n아이템을 버릴 수 있습니다.";
        }
        else if (item == null)
        {
            upperHoverBar[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            upperHoverBarTitle.text = "";
            upperHoverBarDesc.text = "";//"아이템을 선택 후 이곳을 클릭하면\n아이템을 버릴 수 있습니다.";
        }
        else
        {
            //Item hoverItem = ItemArr[curSelectItemType, idx];
            //descObj[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/battleResultUI/spr_selectUI_board_" + hoverItem.getRare() + "_90");
            upperHoverBar[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/" + typeArr[item.getType()] + "ItemSprite/spr_item_" + typeArr[item.getType()] + "_" + item.getItemName());
            upperHoverBarTitle.text = item.getItemName();
            upperHoverBarDesc.text = item.getContent();
        }
    }
    public void updateBigDiceSkill()
    {

        for (int i = 0; i < 4; i++)
        {
            bigDiceSkillButton[i].GetComponent<Image>().sprite = BattleManager.Instance.getDiceSprite(0, i);
            bigDiceSkillState[i].GetComponent<Image>().sprite = BattleManager.Instance.getDiceSprite(1, i);
        }
        for (int i = 0; i < 3; i++)
        {
            bigDiceSkillChain[i].GetComponent<Image>().sprite = BattleManager.Instance.getDiceSprite(2, i);
        }
        
    }

    public void hoverInBigDicePowerButton(int i)
    {
        
        if (BattleManager.Instance.getCharacter(i) != null && BattleManager.Instance.getCharacter(i).getCurState() == 0)
        {
            BattleManager.Instance.witchPowerLookUpdate(i);
            hoverInWitchPowerButton();
            
            bigDicePowerOutline[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
        }
    }
    public void hoverOutBigDicePowerButton()
    {
        for (int i = 0; i < 8; i++)
        {
            hoverOutWitchPowerButton();
            bigDicePowerOutline[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
    }
    public void hoverOutBigDicePowerButton(int i)
    {
        if (BattleManager.Instance.getCharacter(i) != null && BattleManager.Instance.getCharacter(i).getCurState() == 0)
        {
            BattleManager.Instance.witchPowerLookUpdate(-1);
        }
        hoverOutBigDicePowerButton();
    }
    public void updateBigDicePower()
    {
        for (int i = 0; i < 8; i++) {
            bigDicePowerButton[i].GetComponent<Image>().sprite = BattleManager.Instance.getDiceSprite(0,i);
            bigDicePowerState[i].GetComponent<Image>().sprite = BattleManager.Instance.getDiceSprite(1, i);
        }
        for (int i = 0; i < 3; i++){
            bigDicePowerChain[i].GetComponent<Image>().sprite = BattleManager.Instance.getDiceSprite(2, i); 
        }
        for (int i = 4; i < 7; i++)
        {
            bigDicePowerChain[i].GetComponent<Image>().sprite = BattleManager.Instance.getDiceSprite(2, i-1);
        }
        
    }

    private void moveBattleUI(float inputY, GameObject gameObjTemp)
    {
        Vector3 destination = new Vector3(gameObjTemp.GetComponent<RectTransform>().anchoredPosition.x, inputY, 0);

        float termY = 0.2f;
        if (gameObjTemp.GetComponent<RectTransform>().anchoredPosition.y < inputY)
        {
            termY *= -1;

            if (gameObjTemp.GetComponent<RectTransform>().anchoredPosition.y < inputY + termY)
            {
                gameObjTemp.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(gameObjTemp.GetComponent<RectTransform>().anchoredPosition, destination, 0.1f);
            }
            else
            {
                gameObjTemp.GetComponent<RectTransform>().anchoredPosition = destination;
            }
        }
        else
        {
            if (gameObjTemp.GetComponent<RectTransform>().anchoredPosition.y > inputY + termY)
            {
                gameObjTemp.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(gameObjTemp.GetComponent<RectTransform>().anchoredPosition, destination, 0.1f);
            }
            else
            {
                gameObjTemp.GetComponent<RectTransform>().anchoredPosition = destination;
            }
        }
    }



}
