using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class upDownManager : MonoBehaviour
{
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
    public GameObject[] underSkillDiceDescImage = new GameObject[4];
    public TextMeshProUGUI[] underSkillDiceDescText = new TextMeshProUGUI[4];

    [SerializeField]
    public GameObject backBlackItem;
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
    public GameObject[] bigDicePowerChain = new GameObject[3];
    public GameObject bigDicePowerCancleObj;

    [SerializeField]
    public GameObject characterEntity;
    public GameObject[] underCharacterButton = new GameObject[4];
    public GameObject[] underCharacterOutline = new GameObject[4];

    [SerializeField]
    public GameObject deleteChkEntity;
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
        backBlackSkill.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
        backBlackItem.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
        bigDiceSkillEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 327f, 0f);
        bigDicePowerCancleObj.SetActive(false);
        hoverInItemTypeButton(curItemType);
        cancleChangeBtn();
        cancleDeleteBtn();
        changeOption(0, false);
        itemTypeButtonLock = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        moveBattleUI(moveArrY[0], underHoverBar[0]);
        moveBattleUI(moveArrY[1], upperHoverBar[0]);
        // characterSprite.GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<SpriteRenderer>().bounds.size.x, GetComponent<SpriteRenderer>().bounds.size.y);
    }

    public void resetUI()
    {
        clickItem(-1);
        clickItem(-1);
        clickCharacterButton(-1);
    }

    //underBar, upperBar
    private float[] moveArrY = { -2f, 225f, };

    private void onOffUI(int idx, int opt) {
        moveArrY[idx] = moveConstY[opt, idx];
    }
    //off, on
    private float[,] moveConstY = {
        {-2f, 225f},
        { 55f, 168f}
    };

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
    }

    public void hoverOutChangeInitBtn()
    {
        changeBtnInitOutline.GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
    }
    public void clickChangeInitBtn()
    {
        if (AdventureManager.Instance.getBattleEventChk()) {
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
            = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPowerUI");

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
    }
    public void hoverOutDeleteInitBtn()
    {
        deleteBtnInitOutline.GetComponent<Image>().sprite
             = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
    }

    public void clickDeleteInitBtn()
    {
        if (AdventureManager.Instance.getBattleEventChk())
        {
            return;
        }
        deleteChkEntity.SetActive(true);
        hoverOutDeleteBtn();
    }
    #endregion

    public bool[] optionOnOff = { false, false, false }; 
    // 0 : non
    // 1 : skillUI
    // 2 : characterUI

    public void changeOption(int i, bool onOff)
    {
        optionOnOff[i] = onOff;

        underSkillEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -50f, 0f);
        characterEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, -50f, 0f);

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

    public void hoverInCharacterButton(int idx)
    {
        underCharacterOutline[idx].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
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
        Debug.Log("clickCharacter");
        Debug.Log(input);
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
                = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_changeInitBtn_off");
            deleteBtnInit.GetComponent<Image>().sprite
                = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_deleteInitBtn_off");

            Debug.Log("click character acting battle!");
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
        }
        else {
            changeOption(2, true);
            hoverOutCharacterButton();
        }
        Debug.Log(curCharacterIdx);
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

    public void battleStart()
    {
        clickItem(-1);
        clickItem(-1);
        BattleManager.Instance.setCurClickSkill(-1);
        lockState = 3;
        BattleManager.Instance.updateMoveUI(3);
        BattleManager.Instance.moveToBattlePhase();
    }

    public void hoverInUnderBarSkill(int idx)
    {
        if (BattleManager.Instance.getCharacter(idx / 2) != null)
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
            bigDiceSkillEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 120f, 0f);
            hoverInUnderBarSkill(curSkill);
            lockState = 1; //클릭시 현재 스킬에 대한 설명으로 고정.
            updateBigDiceSkill();
        }
        else {
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
        if (curItemIdx > 0) {
            itemManager.Instance.useItem(curItemType, curItemIdx);
        }
    }

    public void clickItem(int input)
    {
        //전투 중에는 추가 잠금 불가능하게
        if (input != -1 && lockState == 3) return;

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
            //updatebigDiceSkill();
        }
        else {
            if (itemSelectDepth == 1) {
                curItemIdx = -1;
                backBlackItem.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
                bigDiceItemUpdateByDepth(0);
                lockState = 0;
                curItemIdx = input;
                hoverOutUpperBar(0);
            }
            else if(itemSelectDepth == 2) {
                bigDiceItemUpdateByDepth(1);
            }
            //itemManager.Instance.hoverOutItem(input);

        }
        Debug.Log(curItemIdx);
    }

    public void clickBigDiceItemCharacter(int idx)
    {
        if (getCharacterExist(idx))
        {
            curCharacterIdx = idx;
            if (curItemType == 0)
            {
                itemManager.Instance.useConsumeItem(curCharacterIdx, curItemType, curItemIdx);//click_info_useItem();

                bigDiceItemUpdateByDepth(1);
                clickItem(curItemIdx);
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
        }
    }
    public void clickBigDiceItemCharacterDice(int idx)
    {
        if (curItemIdx == -1 && curCharacterIdx != -1 && !getCharacterExist(curCharacterIdx)) return; // 선택된게 없으면 캔슬.

        for (int i = 0; i < upperItemOutline.Length; i++)
        {
            upperItemOutline[i].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        itemManager.Instance.click_dice_changeNum(curCharacterIdx, idx, curItemIdx); // 아이템 사용.
        updateBigDiceItemCharacter(curCharacterIdx);
        if(curItemIdx != -1) clickItem(curItemIdx);
    }

    public void clickBigDiceItemCharacterEquip(int idx)
    {
        if (curItemIdx == -1 && curCharacterIdx != -1 && !getCharacterExist(curCharacterIdx)) return; // 선택된게 없으면 캔슬.

        for (int i = 0; i < upperItemOutline.Length; i++)
        {
            upperItemOutline[i].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        itemManager.Instance.click_equip_changeNum(curCharacterIdx, curItemIdx, idx);
        updateBigDiceItemCharacter(curCharacterIdx);
        if(curItemIdx != -1)clickItem(curItemIdx);
    }

    public void updateBigDiceItemCharacter(int characterIdx)
    {
        Debug.Log("cur Character : ");
        Debug.Log(characterIdx);
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
                    Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
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
                    Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            }
        }

    }
    public void bigDiceItemUpdateByDepth(int depth)
    {
        itemSelectDepth = depth;
        if (depth == 0)
        {
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

        deleteOtherLock(4);

        if (input != -1)
        {
            //backBlack.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 108f, 0f);
            BattleManager.Instance.updateMoveUI(4);
            BattleManager.Instance.witchPowerPhase();
            bigDicePowerEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 60f, 0f);
            bigDicePowerCancleObj.SetActive(true);
            lockState = 4; //클릭시 현재 스킬에 대한 설명으로 고정.
            updateBigDicePower();
        }
        else
        {
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
        if (curItemType == 3) //passive
        {
            bigDiceItemUpdateByDepth(0);
        }
        else if (itemSelectDepth == 0 || itemSelectDepth == 1)
        {
            bigDiceItemUpdateByDepth(itemSelectDepth);
        }
        else if(itemSelectDepth == 2){
            if (curItemType == 0) { //consume
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
                if (arrNum[i] <= 6 && arrNum[i] >= 1) underSkillDiceDescText[i].text = arrNum[i].ToString();
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
        for (int i=0;i<4;i++)
        {
            if(BattleManager.Instance.getDiceNum(i) >0 && BattleManager.Instance.getDiceNum(i)<=6) bigDiceSkillButton[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + BattleManager.Instance.getDiceNum(i));
            else bigDiceSkillButton[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + "none");


            if (BattleManager.Instance.getDiceTake(i) != -999)
            {
                bigDiceSkillButton[i].GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + BattleManager.Instance.getSkillName(BattleManager.Instance.getDiceTake(i)));

                string strTemp = "";
                if (BattleManager.Instance.getDiceTake(i) % 10 == 0) strTemp += "up_"; else strTemp += "down_";
                strTemp += (BattleManager.Instance.getDiceTake(i) / 10 + 1).ToString();
                
                if (i != 0 && BattleManager.Instance.getDiceTake(i) == BattleManager.Instance.getDiceTake(i - 1))
                {
                    bigDiceSkillChain[i - 1].GetComponent<Image>().sprite =
                   Resources.Load<Sprite>("sprite/TestSprite/diceImage/dice_skillChk_" + strTemp + "_chain");
                    strTemp += "_sub";
                }
                else if(i != 0 && BattleManager.Instance.getDiceTake(i) != BattleManager.Instance.getDiceTake(i - 1))
                {
                    bigDiceSkillChain[i - 1].GetComponent<Image>().sprite =
                   Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                }

                bigDiceSkillState[i].GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("sprite/TestSprite/diceImage/dice_skillChk_" + strTemp);
            }
            else {
                bigDiceSkillState[i].GetComponent<Image>().sprite =
                   Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");

                if (i != 0)
                {
                    bigDiceSkillChain[i - 1].GetComponent<Image>().sprite =
                   Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                }

            }

        }
    }

    public void updateBigDicePower()
    {
        for (int i = 0; i < 4; i++)
        {
            if (BattleManager.Instance.getDiceNum(i) > 0 && BattleManager.Instance.getDiceNum(i) <= 6) bigDicePowerButton[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + BattleManager.Instance.getDiceNum(i));
            else bigDicePowerButton[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + "none");


            if (BattleManager.Instance.getDiceTake(i) != -999)
            {

                string strTemp = "";
                if (BattleManager.Instance.getDiceTake(i) % 10 == 0) strTemp += "up_"; else strTemp += "down_";
                strTemp += (BattleManager.Instance.getDiceTake(i) / 10 + 1).ToString();

                if (i != 0 && BattleManager.Instance.getDiceTake(i) == BattleManager.Instance.getDiceTake(i - 1))
                {
                    bigDicePowerChain[i - 1].GetComponent<Image>().sprite =
                   Resources.Load<Sprite>("sprite/TestSprite/diceImage/dice_skillChk_" + strTemp + "_chain");
                    strTemp += "_sub";
                }
                else if (i != 0 && BattleManager.Instance.getDiceTake(i) != BattleManager.Instance.getDiceTake(i - 1))
                {
                    bigDicePowerChain[i - 1].GetComponent<Image>().sprite =
                   Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                }

                bigDicePowerState[i].GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("sprite/TestSprite/diceImage/dice_skillChk_" + strTemp);
            }
            else
            {
                bigDicePowerState[i].GetComponent<Image>().sprite =
                   Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");

                if (i != 0)
                {
                    bigDicePowerChain[i - 1].GetComponent<Image>().sprite =
                   Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                }

            }

        }

        for (int i = 4; i < 8; i++)
        {
            if (BattleManager.Instance.getDiceNum(i) > 0 && BattleManager.Instance.getDiceNum(i) <= 6) bigDicePowerButton[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + BattleManager.Instance.getDiceNum(i));
            else bigDicePowerButton[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + "none");


            if (BattleManager.Instance.getDiceTake(i) != -999)
            {
                string strTemp = "";
                if (BattleManager.Instance.getDiceTake(i) % 10 == 0) strTemp += "up_"; else strTemp += "down_";
                strTemp += (BattleManager.Instance.getDiceTake(i) / 10 + 1).ToString();

                if (i != 4 && BattleManager.Instance.getDiceTake(i) == BattleManager.Instance.getDiceTake(i - 1))
                {
                    bigDicePowerChain[i - 1].GetComponent<Image>().sprite =
                   Resources.Load<Sprite>("sprite/TestSprite/diceImage/dice_skillChk_" + strTemp + "_chain");
                    strTemp += "_sub";
                }
                else if (i != 4 && BattleManager.Instance.getDiceTake(i) != BattleManager.Instance.getDiceTake(i - 1))
                {
                    bigDicePowerChain[i - 1].GetComponent<Image>().sprite =
                   Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                }

                bigDicePowerState[i].GetComponent<Image>().sprite =
                    Resources.Load<Sprite>("sprite/TestSprite/diceImage/dice_skillChk_" + strTemp);
            }
            else
            {
                bigDicePowerState[i].GetComponent<Image>().sprite =
                   Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");

                if (i != 4)
                {
                    bigDicePowerChain[i - 1].GetComponent<Image>().sprite =
                   Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                }

            }

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
