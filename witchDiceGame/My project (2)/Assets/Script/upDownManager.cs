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
    public GameObject[] underSkillButton = new GameObject[8];
    public GameObject[] underSkillOutline = new GameObject[8];
    public GameObject[] underSkillDiceDescImage = new GameObject[4];
    public TextMeshProUGUI[] underSkillDiceDescText = new TextMeshProUGUI[4];

    [SerializeField]
    public GameObject[] upperItemButton = new GameObject[12];
    public GameObject[] upperItemOutline = new GameObject[12];

    [SerializeField]
    public GameObject[] upperItemTypeButton = new GameObject[4];
    public GameObject[] upperItemTypeOutline = new GameObject[4];

    [SerializeField]
    public GameObject backBlack;
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

    private int lockState = 0; //0 : free  1: underbar hover  2: upperbar hover 3: battleMode  4: witchPower 
    private int curSkill = -1;
    private int curItemIdx = -1;
    private int curItemType = 0;



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
        backBlack.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
        bigDiceSkillEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 327f, 0f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        moveBattleUI(moveArrY[0], underHoverBar[0]);
        moveBattleUI(moveArrY[1], upperHoverBar[0]);


    }

    //underBar, upperBar
    private float[] moveArrY = { -2f, 225f,};

    private void onOffUI(int idx, int opt) {
        moveArrY[idx] = moveConstY[opt, idx];
    }
    //off, on
    private float[,] moveConstY = {
        {-2f, 225f},
        { 55f, 168f}
    };

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
    public void updateUpperItem(int idx, int typeIdx, string name)
    {
        if (typeIdx == -1)
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
        if (idx == 11 || itemManager.Instance.getCurItem(idx) != null)
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
        if(curItemIdx != idx)
        {
            upperItemOutline[idx].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            if(curItemIdx != -1)itemManager.Instance.hoverInItem(curItemIdx);
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

    public void hoverInSkillTypeButton(int idx)
    {
        upperItemTypeOutline[idx].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
    }
    public void hoverOutSkillTypeButton(int idx)
    {
        if (curItemType != idx)
        {
            upperItemTypeOutline[idx].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
    }

    public void battleStart()
    {
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
            if(curSkill != -1)BattleManager.Instance.makeSkillCommand(curSkill / 2, curSkill % 2);
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
        for (int idx = 0;idx< 8;idx++) {
            underSkillOutline[idx].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        curSkill = (input /10)*2 + input%10;
        if (input != -1)
        {
            //deleteOtherLock(1);
            backBlack.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 108f, 0f);
            bigDiceSkillEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 120f, 0f);
            hoverInUnderBarSkill(curSkill);
            lockState = 1; //클릭시 현재 스킬에 대한 설명으로 고정.
            updateBigDiceSkill();
        }
        else {
            backBlack.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
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
            bigDicePowerCancleObj.SetActive(false);
            clickEnterPower(-1);
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
        curItemIdx = input;
        if (input != -1)
        {
            //deleteOtherLock(2);
           // curItemIdx = input;
            backBlack.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 108f, 0f);
            //bigDiceSkillEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 120f, 0f);
            hoverInUpperBar(input);
            lockState = 2; //클릭시 현재 스킬에 대한 설명으로 고정.
            //updatebigDiceSkill();
        }
        else {
            backBlack.GetComponent<RectTransform>().anchoredPosition = new Vector3(0f, 360f, 0f);
            lockState = 0;
            curItemIdx = input;
            hoverOutUpperBar(0);
            //itemManager.Instance.hoverOutItem(input);
            
        }
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
        hoverInSkillTypeButton(idx);
        curItemType = idx;
        deleteOtherLock(0);
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
