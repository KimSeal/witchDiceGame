using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AnimatedBattleText.Examples;
public class BattleManager : MonoBehaviour
{

    

    [SerializeField]
    public Sprite[] diceSprite = new Sprite[6];
    [SerializeField]
    public GameObject[] myChkDice = new GameObject[4];
    [SerializeField]
    public GameObject[] enemyChkDice = new GameObject[4];
    [SerializeField]
    public GameObject chooseDiceObj;
    [SerializeField]
    public GameObject damageTextObj;
    [SerializeField]
    public GameObject passiveEffObj;
    [SerializeField]
    public GameObject diceRollEff;
    [SerializeField]
    public GameObject hitEff;
    [SerializeField]
    public GameObject coinEff;
    [SerializeField]
    public GameObject bagShake;
    [SerializeField]
    public GameObject starEff;

    [SerializeField]
    public GameObject brokenEff;

    public int chooseDiceIdx;

    //선공 팀 구분
    public int firstAttackTeam = 1;


    //타겟팅을 위한 시스템
    public int clickState = 0;
    public int clickMonster = -1;
    public int clickSelf = -1;

    [SerializeField]
    private GameObject fireObject;
    [SerializeField]
    private GameObject[] battleFireObject = new GameObject[16];
    //phase 흐름을 위한 시스템
    //private int battlePhaseState = 0;


    // Start is called before the first frame update
    private static BattleManager instance = null;

    //아군/적군 캐릭터의 상태를 담는 배열
    private Character[] myCharacter = new Character[4];
    private Character[] enemyCharacter = new Character[4];

    //수정할 주사위를 담는 곳
    private Dice chooseDice = new Dice();

    //아군/적군 주사위의 상태를 담는 배열
    private Dice[] myDice = new Dice[4];
    private Dice[] enemyDice = new Dice[4];
    private int[] myDiceNum = new int[4] { -999, -999, -999, -999 };
    private int[] enemyDiceNum = new int[4] { -999, -999, -999, -999 };
    //private int[] DiceSel = new int[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; 안쓰는거일걸..?

    //아군/적군 스킬이 할당된 주사위들의 상태를 담는 배열(누구의 어떤 스킬이 담겨있는지 할당된다)
    private int[] myDiceTake = new int[4] { -999, -999, -999, -999 };
    private int[] enemyDiceTake = new int[4] { -999, -999, -999, -999 };
    //이번 턴에 사용된 스킬의 경우 넘어가야 하기에 사용여부를 담는다.
    //(보스에게 2개 이상의 스킬을 담을 경우 추가 배열 만들 필요가 있다! - 아직 미개발)
    private bool[,] mySkillUsed = new bool[4, 2];
    private bool[,] enemySkillUsed = new bool[4, 2];

    //UI 테스트
    public GameObject[] myDiceUI = new GameObject[4];
    public GameObject[] enemyDiceUI = new GameObject[4];
    [SerializeField] private GameObject[] diceUIChk = new GameObject[8]; // obj_skill_Chk_(number)
    [SerializeField] private GameObject[] diceUIChain = new GameObject[6]; // obj_ my/enemyChain_ number
    [SerializeField] public GameObject[] enemyChainAnim =new GameObject[4];

    [SerializeField] private GameObject[] myDiceStateUI = new GameObject[4];
    [SerializeField] private GameObject[] enemyDiceStateUI = new GameObject[4];
    private Animator[] myDiceStateAnim = new Animator[4];
    private Animator[] enemyDiceStateAnim = new Animator[4];

    [SerializeField] private GameObject characterUI; // characterUI
    public GameObject diceFullUI;
    public GameObject witchHatButton;
    public GameObject[] witchPowerSelectButton = new GameObject[7];

    /*
    [SerializeField]
    public GameObject[] diceArrowSet = new GameObject[8]; //arrowSet_(number)
    */
    [SerializeField]
    public GameObject[] myCharacterObjUI = new GameObject[4];
    public Animator[] myCharacterObjUIAnim = new Animator[4];

    [SerializeField]
    private GameObject[] myCharacterShadowObjUI = new GameObject[4];
    [SerializeField]
    private GameObject[] myCharacterObjEntityUI = new GameObject[4];
    private ParticleSystem[,] myFireObj = new ParticleSystem[4, 2];

    [SerializeField] private GameObject[] enemyCharacterObjUI = new GameObject[4];
    private Animator[] enemyCharacterObjUIAnim = new Animator[4];
    [SerializeField] private GameObject[] enemyCharacterShadowObjUI = new GameObject[4];
    [SerializeField] private GameObject[] enemyCharacterObjEntityUI = new GameObject[4];
    private ParticleSystem[,] enemyFireObj = new ParticleSystem[4, 2];

    private int[] myCharacterAtkReady = { 0, 0, 0, 0 };
    private int[] enemyCharacterAtkReady = { 0, 0, 0, 0 };

    [SerializeField]
    public GameObject specialTextManager;

    // 타겟팅시 일시정지를 위한 코루틴 저장함수.
    //private IEnumerator battleTimer = null;

    [SerializeField] private GameObject resultObj_all; //bosang_ui
    [SerializeField] public GameObject[] resultEff = new GameObject[3];
    [SerializeField] public GameObject[] resultItemTypeObj = new GameObject[3];
    [SerializeField] private GameObject[] resultObjInit = new GameObject[12]; //obj_resultUI _board, _itemLogo, _itemName, _itemDesc + (number)
    private GameObject[,] resultObj = new GameObject[3, 4];
    private Item[] resultItem = new Item[3];

    //phase버튼 누를수 있는지
    //private bool clickAble = true;
    public int curPhase = -1;
    // -1 : 아직 시작안함
    // 0 : Stage-Start 페이즈
    // 1 : Dice-Throw Phase
    // 2 : Dice-Fix Phase
    // 3 : Skill-Select 페이즈
    // 4 : Battle 페이즈 - 주사위 세팅
    // 5 : Battle 페이즈 - 스킬에서 주사위 사용
    // 6 : End-Phase
    //-999 : 연타 방지(즉, 버그 차단을 위한 경우의 수)

    private int currentMoveUI = 0;
    private int currentLightUI = 0;

    [SerializeField] private GameObject[] witchPowerObj = new GameObject[3]; //obj_backGround_field witchPower_button_left witchPower_button_right
    [SerializeField] public GameObject witchPowerSelectObjEntity;
    [SerializeField] public GameObject[] witchPowerSelectObj = new GameObject[7];
    [SerializeField] public GameObject[] witchPowerSelectObjOutline = new GameObject[7];
    [SerializeField] private GameObject[] backGroundObj = new GameObject[5]; //obj_backGround_ field, witch_witchPowerSelect, backGround, witch_skillSelect_body, witch_skillSelect_face
    
    //[SerializeField]
    //public GameObject[] diceArrow = new GameObject[8]; //obj_battleDice_arrow_(number)

    private int[] clickedDice = new int[2];

    // 적군이 주사위를 보고 스킬을 배치하게 하기 위한 변수들//
    Skill[] enemySkill = new Skill[8];
    int[] enemySkillDiceNum = new int[8];
    int[,] enemySkillDiceVal = new int[8, 4];


    //주사위 밑에 HP UI
    [SerializeField] private GameObject[] myHpUI = new GameObject[4];
    [SerializeField] private GameObject[] enemyHpUI = new GameObject[4];

    [SerializeField] private GameObject[] myHpUIBack = new GameObject[4];
    [SerializeField] private GameObject[] enemyHpUIBack = new GameObject[4];

    //전투 위에 뜨는 밸류(공격전에 몇 들어가는 지 보여주는 거)
    [SerializeField] private GameObject battleTextObj;

    [SerializeField] private GameObject battleDescBox; //board_descBoard
    [SerializeField] private GameObject battleDescBoxName; // board_battle_Info_name
    [SerializeField] private GameObject battleDescBoxInfo; //board_battle_info_value
    [SerializeField] private GameObject battleDescBoxCharacter;//board_battle_Info_character
    [SerializeField] private GameObject[] faceDescInit = new GameObject[8];
    GameObject[,] faceDesc = new GameObject[2, 4];

    [SerializeField] private GameObject diceDescBox; //ui_battle_board_dice
    [SerializeField] private GameObject[] diceDesc = new GameObject[6];

    private int[] myDiceState = { 0, 0, 0, 0 };
    private int[] enemyDiceState = { 0, 0, 0, 0 };

    //스킬 설명을 위해 준비된 칸
    [SerializeField] private GameObject skillDescBox; //ui_battle_board_skill
    [SerializeField] private GameObject[] skillDescBox_title = new GameObject[2]; //board_skillDesc_skillTitle_(number)
    [SerializeField] private GameObject[] skillDescBox_info = new GameObject[2]; //board_skillDesc_skillInfo_(number)
    [SerializeField] private GameObject[] skillDescBox_image = new GameObject[2]; //board_skillDesc_skillImage_(number)
    [SerializeField] public GameObject[] skillDescBox_dice_0 = new GameObject[4]; //board_skill

    [SerializeField]
    public GameObject[] skillDescBox_dice_1 = new GameObject[4];

    [SerializeField] private GameObject equipDescBox; ////ui_battle_board_equip
    [SerializeField] private GameObject[] equipDescBox_title = new GameObject[2]; //board_equipDesc_equipTitle_(number)
    [SerializeField] private GameObject[] equipDescBox_info = new GameObject[2]; //board_equipDesc_equipInfo_(number)
    [SerializeField] private GameObject[] equipDescBox_image = new GameObject[2]; //board_equipDesc_equipImage_(number)


    [SerializeField]
    public GameObject giveUpBtn;
    [SerializeField]
    public GameObject battleBagBtn;

    private bool giveUpChk = false;

    [SerializeField]
    public GameObject underUI;
    [SerializeField]
    public GameObject upperUI;


    public void hoverInCharacter(int idx)
    {
        if (idx < 4 && myCharacter[idx] != null && myCharacter[idx].getCurState() == 0)
        {
            ToolBarManager.Instance.setToolBar(myCharacter[idx]);
            myCharacterObjUI[idx].GetComponent<SpriteRenderer>().material.SetInt("_Radius", (int)(myCharacterObjUI[idx].GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) * (int)(myCharacterObjUI[idx].GetComponent<SpriteRenderer>().sprite.pixelsPerUnit));
        }
        else if (idx >= 4 && idx<8 && enemyCharacter[idx - 4] != null && enemyCharacter[idx-4].getCurState() == 0)
        {
            
            ToolBarManager.Instance.setToolBar(enemyCharacter[idx-4]);
            enemyCharacterObjUI[idx - 4].GetComponent<SpriteRenderer>().material.SetInt("_Radius", (int)(enemyCharacterObjUI[idx - 4].GetComponent<SpriteRenderer>().sprite.pixelsPerUnit) * (int)(enemyCharacterObjUI[idx - 4].GetComponent<SpriteRenderer>().sprite.pixelsPerUnit));
        }
    }

    public void hoverOutCharacter(int idx)
    {
        for (int i = 0; i < 4; i++)
        {
            enemyCharacterObjUI[i].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
            myCharacterObjUI[i].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
        }
        ToolBarManager.Instance.toolBarOnOff(0);
    }

    public void useGiveUpBtn()
    {
        if (giveUpChk)
        {
            giveUpChk = false;
            giveUpBtn.GetComponent<Animator>().Play("unactive");
        }
        else if (!giveUpChk)
        {
            if (AdventureManager.Instance.getTutorial() != 0)
            {
                giveUpChk = false;
                fullUI.showFull(14);
                giveUpBtn.GetComponent<Animator>().Play("unactive");
            }
            else
            {
                giveUpChk = true;
                fullUI.showFull(15);
                giveUpBtn.GetComponent<Animator>().Play("active");
            }
        }
    }

    public void hoverRotateAble(GameObject gameObjectTemp, int eventType, bool onOff) {
        if (eventType == 0) gameObjectTemp.GetComponent<hoverRotate>().shakeAble(onOff);
        else if (eventType == 1) gameObjectTemp.GetComponent<hoverRotate>().expandAble(onOff);
    }

    public void shakeObject(GameObject gameObjectTemp) {
        gameObjectTemp.GetComponent<hoverRotate>().shakeStart();
    }
    public void shakeBag()
    {
        shakeObject(bagShake);
    }
    public Character getCharacter(int a) {
        if (a < 4) return myCharacter[a];
        a -= 4;
        return enemyCharacter[a];
    }
    public int getDiceNum(int a)
    {
        if (a < 4) return myDiceNum[a];
        a -= 4;
        return enemyDiceNum[a];
    }
    public int getDiceTake(int a)
    {
        if (a < 4) return myDiceTake[a];
        a -= 4;
        return enemyDiceTake[a];
    }
    public string getSkillName(int skillIdx)
    {
        return myCharacter[skillIdx / 10].skillUse(skillIdx % 10).getSkillName();
    }

    int curSelectInfo = 0;
    int hoverCharacterIdx = -1;

    int bossPhase = 0;
    public void changeBossPhase(int a)
    {

        if (a == 100) bossPhase = a; // 부엉이 보스인 경우.(2페이즈)
        else if (a == 101) bossPhase = a; // 부엉이 보스인 경우.(2페이즈)
        else bossPhase = 0;
    }
    private void myDiceChange(int idx, int characterIdx, int skillIdx)
    {
        shakeObject(myDiceUI[idx]);
        if (skillIdx == -999)
        {
            if (myDiceTake[idx] != -999)
            { //제거인 경우, 해당 주사위를 사용하던 스킬 불 꺼트리기
                myFireObj[myDiceTake[idx] / 10, myDiceTake[idx] % 10].Stop(true);
            }
            myDiceTake[idx] = -999;
            return;
        }
        else
        {
            myDiceTake[idx] = characterIdx * 10 + skillIdx; //캐릭터하고 사용하는 스킬에 대해 값 생성
            
        }
    }
    private void enemyDiceChange(int idx, int skillIdx) {

        if (skillIdx == -999)
        {
            if (enemyDiceTake[idx] != -999) { //제거인 경우, 해당 주사위를 사용하던 스킬 불 꺼트리기
                enemyFireObj[enemyDiceTake[idx] / 10, enemyDiceTake[idx] % 10].Stop(true);
            }
            enemyDiceTake[idx] = -999;
            return;
        }
        else
        {
            enemyDiceTake[idx] = (skillIdx % 4) * 10 + skillIdx / 4; //캐릭터하고 사용하는 스킬에 대해 값 생성
        }
    }
    private void drawSkill(Character character)
    {
        for (int skillIdx = 0; skillIdx < 2; skillIdx++)
        {
            Skill thisSkill = character.skillUse(skillIdx);
            if (character.getDestiny().DestinyIdx < 10001 || jsonDataManager.Instance.getMonsterSkill(character.getDestiny().DestinyIdx, skillIdx)) // 만난적있는 지 확인
            {

                if (Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + thisSkill.getSkillName()) != null)
                {
                    skillDescBox_image[skillIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + thisSkill.getSkillName());
                }
                else
                {
                    skillDescBox_image[skillIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_noImage");
                }
                skillDescBox_info[skillIdx].GetComponent<TextMeshPro>().text = thisSkill.getCommand();
                skillDescBox_title[skillIdx].GetComponent<TextMeshPro>().text = thisSkill.getSkillName();
                for (int diceIdx = 0; diceIdx < 4; diceIdx++)
                {
                    if (skillIdx == 0) skillDescBox_dice_0[diceIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_" + thisSkill.getNeedDice(diceIdx).ToString());
                    if (skillIdx == 1) skillDescBox_dice_1[diceIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_" + thisSkill.getNeedDice(diceIdx).ToString());

                }
            }
            else {
                skillDescBox_image[skillIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_noImage");
                skillDescBox_info[skillIdx].GetComponent<TextMeshPro>().text = TalkManager.Instance.getDesc(17);
                skillDescBox_title[skillIdx].GetComponent<TextMeshPro>().text = "Not Found";
                for (int diceIdx = 0; diceIdx < 4; diceIdx++)
                {
                    //만난적 없더라도 스킬 대처는 할 수 있도록
                    if (skillIdx == 0) skillDescBox_dice_0[diceIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_" + thisSkill.getNeedDice(diceIdx).ToString());
                    if (skillIdx == 1) skillDescBox_dice_1[diceIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_" + thisSkill.getNeedDice(diceIdx).ToString());

                }
            }
        }
    }
    private void drawDice(Character character)
    {
        for (int diceIdx = 0; diceIdx < 6; diceIdx++)
        {
            diceDesc[diceIdx].GetComponent<SpriteRenderer>().sprite = diceSprite[character.getDice(diceIdx) - 1];
        }
    }

    private void drawEquip(Character character)
    {
        for (int Idx = 0; Idx < 2; Idx++)
        {
            Item thisItem = character.getItem(Idx);
            if (thisItem == null)
            {
                equipDescBox_image[Idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                equipDescBox_title[Idx].GetComponent<TextMeshPro>().text = "";
                equipDescBox_info[Idx].GetComponent<TextMeshPro>().text = "";
            }
            else
            {
                equipDescBox_image[Idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/equipItemSprite/spr_item_equip_" + thisItem.getItemName());
                equipDescBox_title[Idx].GetComponent<TextMeshPro>().text = thisItem.getItemName();
                equipDescBox_info[Idx].GetComponent<TextMeshPro>().text = thisItem.getContent();
            }
        }
    }
    private int getTotalHp(Character character)
    {
        return character.getHp() + character.getCharacter_battle().getArmor();
    }
    private int getTotalAtk(Character character)
    {
        return character.getPhyAtk() + character.getCharacter_battle().getAtk();
    }

    //전투 ui 에 대한 함수들 모음 updateBattleUI()로 지속적으로 업데이트 해줄것 
    #region
    private void writeBattleInfo(Character character)
    {
        battleDescBoxCharacter.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + character.getName() + "/animator_" + character.getName());
        battleDescBoxName.GetComponent<TextMeshPro>().text = character.getName();
        battleDescBoxInfo.GetComponent<TextMeshPro>().text =
            character.getHp() + "/" + character.getMaxHp() + "( +" + character.getCharacter_battle().getArmor() + " )\n\n"
            //(getTotalHp(character)).ToString() + "(" + character.getHp().ToString() + "+" + character.getCharacter_battle().getArmor().ToString() +  ")" + "\n" 
            + (getTotalAtk(character)).ToString() + "(" + character.getPhyAtk().ToString() + "/" + character.getCharacter_battle().getAtk().ToString() + ")" + "\n";
    }

    private bool characterInfoOpen = false;

    private void makeEmptyBattleInfoBox() //전투 정보 ui 초기화
    {
        curSelectInfo = -1;
        hoverCharacterIdx = -1;
        diceDescBox.SetActive(false);
        skillDescBox.SetActive(false);
        equipDescBox.SetActive(false);
        darkFaceImage(0, 0, true);
        battleDescBoxCharacter.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/animator_noneCharacter");

        battleDescBoxName.GetComponent<TextMeshPro>().text = "";
        battleDescBoxInfo.GetComponent<TextMeshPro>().text = "";
    }
    public void updateBattleUI() //현재 정보를 바탕으로 battle ui 업데이트
    {
        if (hoverCharacterIdx >= 0 && hoverCharacterIdx < 4) {
            if (!(myCharacter[hoverCharacterIdx] == null && myCharacter[hoverCharacterIdx].getCurState() != 0))
            {
                hoverCharacterIdx = -1;
            }
        }
        if (hoverCharacterIdx >= 4 && hoverCharacterIdx < 8)
        {
            if (!(enemyCharacter[hoverCharacterIdx - 4] == null && enemyCharacter[hoverCharacterIdx - 4].getCurState() != 0))
            {
                hoverCharacterIdx = -1;
            }
        }
        if (hoverCharacterIdx == -1) //선택된 캐릭터가 없을 경우 비게 만들기
        {
            makeEmptyBattleInfoBox();
        }
        else if (curSelectInfo == -1) //선택된 캐릭터는 있으나 선택된 ui가 없는 경우
        {
            diceDescBox.SetActive(false);
            skillDescBox.SetActive(false);
            equipDescBox.SetActive(false);
            if (hoverCharacterIdx < 4 && hoverCharacterIdx >= 0) writeBattleInfo(myCharacter[hoverCharacterIdx]);
            if (hoverCharacterIdx < 8 && hoverCharacterIdx >= 4) writeBattleInfo(enemyCharacter[hoverCharacterIdx - 4]);
        }
        else clickBattleUIInfo(curSelectInfo); //선택된 캐릭터도, 선택된 ui도 있는 경우
    }
    private bool characterInfoOpenAble = true; //다른 목적으로 쓰는 중(최종 보상 획득 씬에서 아이템 선택창 나왔는지 여부)
    private bool getLoseChk = false; // 패배씬 나왔을때 true바꿈. 패배씬 나오는 동안은 캐릭터랑 아이템 사용 안되도록
    public void clickCharacterInfoBox()
    {
        if (!getLoseChk)
        {
            if (characterInfoOpenAble || (!characterInfoOpenAble && !itemManager.Instance.getItemBoxOpen() && !itemManager.Instance.getItemBoxMove()))
            {
                makeEmptyBattleInfoBox();
                if (!characterInfoOpen)
                {

                    characterInfoOpen = true;
                    battleDescBox.transform.position = new Vector3(0, 0, battleDescBox.transform.position.z);
                }
                else
                {
                    characterInfoOpen = false;
                    battleDescBox.transform.position = new Vector3(0, 500, battleDescBox.transform.position.z);
                }
            }
            else if (!itemManager.Instance.getItemBoxMove()) { flipBag_battle(); }
        }
    }
    public void clickBattleUIInfo(int idx) //전투 ui에서 뭐볼지 선택하는 경우
    {
        if (hoverCharacterIdx != -1)
        {
            curSelectInfo = idx;
            if (curSelectInfo == 0)
            {
                diceDescBox.SetActive(true);
                skillDescBox.SetActive(false);
                equipDescBox.SetActive(false);
            }
            if (curSelectInfo == 1)
            {
                diceDescBox.SetActive(false);
                skillDescBox.SetActive(true);
                equipDescBox.SetActive(false);
            }
            if (curSelectInfo == 2)
            {
                diceDescBox.SetActive(false);
                skillDescBox.SetActive(false);
                equipDescBox.SetActive(true);
            }
            clickSkillDesc(hoverCharacterIdx);
        }

    }
    private void darkFaceImage(int enemy, int idx, bool all) //전투 ui의 캐릭터 얼굴을 그리기 위한 함수
    {
        for (int i = 0; i < 4; i++)
        {
            Material material = faceDesc[0, i].GetComponent<SpriteRenderer>().material;
            material.SetFloat("_Transparency", 0.7f);
            Material material2 = faceDesc[1, i].GetComponent<SpriteRenderer>().material;
            material2.SetFloat("_Transparency", 0.7f);
        }
        if (!all)
        {
            Material material3 = faceDesc[enemy, idx].GetComponent<SpriteRenderer>().material;
            material3.SetFloat("_Transparency", 0.0f);
        }
    }

    public void hoverInSkillDesc(int i)
    {
        if (hoverCharacterIdx != i) {
            Material material3 = faceDesc[i / 4, i % 4].GetComponent<SpriteRenderer>().material;
            material3.SetFloat("_Transparency", 0.0f);
        }

    }
    public void hoverOutSkillDesc(int i) {
        if (hoverCharacterIdx != i)
        {
            Material material3 = faceDesc[i / 4, i % 4].GetComponent<SpriteRenderer>().material;
            material3.SetFloat("_Transparency", 0.7f);
        }
    }
    public void clickSkillDesc(int i) //전투 ui에서 캐릭터 얼굴에 가져다댄 후 정보 출력
    {
        if (i >= 0 && i < 4)
        {
            if (myCharacter[i] != null && myCharacter[i].getCurState() == 0)
            {
                SoundManager_Sfx.Instance.playSound(4);
                if (curSelectInfo == 0)
                {
                    drawDice(myCharacter[i]);

                }
                else if (curSelectInfo == 1)
                {
                    drawSkill(myCharacter[i]);
                }
                else if (curSelectInfo == 2)
                {
                    drawEquip(myCharacter[i]);
                }
                hoverCharacterIdx = i;
                darkFaceImage(0, i, false); //해당 캐릭터 얼굴 어둡게 바꾸고
                writeBattleInfo(myCharacter[i]); //정보 출력
            }
            else {
                SoundManager_Sfx.Instance.playSound(7);
                makeEmptyBattleInfoBox();
            }
        }
        else if (i >= 4 && i < 8)
        {
            i -= 4;
            if (enemyCharacter[i] != null && enemyCharacter[i].getCurState() == 0)
            {
                SoundManager_Sfx.Instance.playSound(4);
                if (curSelectInfo == 0)
                {
                    drawDice(enemyCharacter[i]);
                }
                else if (curSelectInfo == 1)
                {
                    drawSkill(enemyCharacter[i]);
                }
                else if (curSelectInfo == 2)
                {
                    drawEquip(enemyCharacter[i]);
                }
                darkFaceImage(1, i, false);
                writeBattleInfo(enemyCharacter[i]);
                hoverCharacterIdx = i + 4;
            }
            else
            {
                SoundManager_Sfx.Instance.playSound(7);
                makeEmptyBattleInfoBox();
            }
        }
        else
        {
            makeEmptyBattleInfoBox();
        }
    }

    #endregion  

    public void updateHp()
    {
        for (int i = 0; i < 4; i++)
        {


            if (myCharacter[i] != null && myCharacter[i].getCurState() == 0)
            {
                myHpUI[i].GetComponent<TextMeshPro>().text = myCharacter[i].getHp().ToString();
                if (myCharacter[i].getHp() <= 0)
                {
                    myHpUIBack[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/hpUI/spr_hp_0");
                }
                else
                {
                    float hpTemp = (float)myCharacter[i].getMaxHp() * 0.2f;
                    int hpTemp2 = (int)((float)myCharacter[i].getHp() / hpTemp);
                    if (hpTemp2 == 5) hpTemp2 = 4; 
                    myHpUIBack[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/hpUI/spr_hp_" + (hpTemp2 +1).ToString());
                }
            }
            else
            {
                myHpUI[i].GetComponent<TextMeshPro>().text = "";
                myHpUIBack[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/hpUI/spr_hp_0");
            }

            if (enemyCharacter[i] != null && enemyCharacter[i].getCurState() == 0)
            {
                enemyHpUI[i].GetComponent<TextMeshPro>().text = enemyCharacter[i].getHp().ToString();
                if (enemyCharacter[i].getHp() <= 0)
                {
                    enemyHpUIBack[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/hpUI/spr_hp_0");
                }
                else
                {
                    float hpTemp = (float)enemyCharacter[i].getMaxHp() * 0.2f;
                    int hpTemp2 = (int)((float)enemyCharacter[i].getHp() / hpTemp);
                    if (hpTemp2 == 5) hpTemp2 = 4;
                    enemyHpUIBack[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/hpUI/spr_hp_" + (hpTemp2 + 1).ToString());
                }
            }
            else
            {
                enemyHpUI[i].GetComponent<TextMeshPro>().text = "";
                enemyHpUIBack[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/hpUI/spr_hp_0");
            }
        }
        updateInfoUIFaceUpdate();
    }
    private void InitSetOfEnemySkill() //추후 적군 스킬 자동 발사를 위해 스킬을 미리 받아둔다.
    {

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 2; j++)
            {

                if (enemyCharacter[i] != null && enemyCharacter[i].getCurState() == 0)
                {
                    enemySkill[i + j * 4] = enemyCharacter[i].skillUse(j);
                    enemySkillDiceNum[i + j * 4] = enemySkill[i + j * 4].getNeedDiceNum();
                    for (int k = 0; k < 4; k++)
                    {
                        enemySkillDiceVal[i + j * 4, k] = enemySkill[i + j * 4].getNeedDice(k);
                    }
                }
                else // 스킬을 받아올 수 없는 경우 -999(의미없는 값 넣기)  스킬에 대해서 접근 안하게 조심할것(추후에 빈 스킬 추가하는 방법도?)
                {
                    enemySkillDiceNum[i + j * 4] = -999;
                    for (int k = 0; k < 4; k++)
                    {
                        enemySkillDiceVal[i + j * 4, k] = -999;
                    }
                }
            }
        }
    }

    //현재 주사위 값들을 기반으로 스킬을 할당한다.
    private void MakeEnemyAttackSet() //주사위 건너뛰고 발동하는 에러가 있다. 수정 필요
    {
        List<int> liveCharacterList = new List<int>();
        List<int> liveSkillList = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            enemyFireObj[i, 0].Stop();
            enemyFireObj[i, 1].Stop();
            enemyDiceChange(i, -999);
        }
        for (int i = 0; i < 4; i++)
        {
            if (enemyCharacter[i] != null && enemyCharacter[i].getCurState() == 0)
            {
                liveCharacterList.Add(i);
            }
        }
        for (int i = 0; i < 8; i++)
        {
            if (enemySkillDiceNum[i] != -999) { liveSkillList.Add(i); }
        }

        for (int skillIdx0 = liveSkillList.Count - 1; skillIdx0 >= 0; skillIdx0--)
        {
            //특수 변수 확인
            int characterIdxTemp = liveSkillList[skillIdx0] % 4;
            int skillIdxTemp = liveSkillList[skillIdx0] / 4;
            //만약 special한 공격이고(스택사용)
            if (enemyCharacter[characterIdxTemp].getCharacter_battle().getSpecialVal() != enemyCharacter[characterIdxTemp].skillUse(skillIdxTemp).getSpecialVal()) //만약 조건하고 다른경우 건너뛴다.
            {
                continue;
            }

            int skillIdx = liveSkillList[skillIdx0];
            for (int diceIdx = 0; diceIdx <= liveCharacterList.Count - enemySkillDiceNum[skillIdx]; diceIdx++)
            {

                //필요 주사위가 1칸인 경우
                if (enemySkillDiceNum[skillIdx] == 1)
                {
                    if (enemyDiceTake[liveCharacterList[diceIdx]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 0], enemyDiceNum[liveCharacterList[diceIdx]])) { // 첫번쨰주사위가 겹치는 경우
                        enemyDiceChange(liveCharacterList[diceIdx], skillIdx);
                        liveCharacterList.RemoveAt(diceIdx);
                        break;
                    }

                }
                //필요 주사위가 2칸인 경우
                else if (enemySkillDiceNum[skillIdx] == 2)
                {
                    if (enemyDiceTake[liveCharacterList[diceIdx]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 0], enemyDiceNum[liveCharacterList[diceIdx]]) &&
                        enemyDiceTake[liveCharacterList[diceIdx + 1]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 1], enemyDiceNum[liveCharacterList[diceIdx + 1]]))
                    { // 첫번쨰주사위가 겹치는 경우
                        enemyDiceChange(liveCharacterList[diceIdx], skillIdx);
                        enemyDiceChange(liveCharacterList[diceIdx + 1], skillIdx);
                        //liveCharacterList.RemoveAt(diceIdx);
                        //liveCharacterList.RemoveAt(diceIdx);
                        break;
                    }

                }
                else if (enemySkillDiceNum[skillIdx] == 3) //필요 주사위가 3칸인 경우
                {
                    if (enemyDiceTake[liveCharacterList[diceIdx]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 0], enemyDiceNum[liveCharacterList[diceIdx]]) &&
                        enemyDiceTake[liveCharacterList[diceIdx + 1]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 1], enemyDiceNum[liveCharacterList[diceIdx + 1]]) &&
                        enemyDiceTake[liveCharacterList[diceIdx + 2]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 2], enemyDiceNum[liveCharacterList[diceIdx + 2]]))
                    {
                        enemyDiceChange(liveCharacterList[diceIdx], skillIdx);
                        enemyDiceChange(liveCharacterList[diceIdx + 1], skillIdx);
                        enemyDiceChange(liveCharacterList[diceIdx + 2], skillIdx);
                        /*liveCharacterList.RemoveAt(diceIdx);
                        liveCharacterList.RemoveAt(diceIdx);
                        liveCharacterList.RemoveAt(diceIdx);*/
                        break;

                    }

                }
                else if (enemySkillDiceNum[skillIdx] == 4) //필요 주사위가 4칸인 경우
                {
                    if (enemyDiceTake[liveCharacterList[diceIdx]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 0], enemyDiceNum[liveCharacterList[diceIdx]]) &&
                        enemyDiceTake[liveCharacterList[diceIdx + 1]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 1], enemyDiceNum[liveCharacterList[diceIdx + 1]]) &&
                        enemyDiceTake[liveCharacterList[diceIdx + 2]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 2], enemyDiceNum[liveCharacterList[diceIdx + 2]]) &&
                        enemyDiceTake[liveCharacterList[diceIdx + 3]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 3], enemyDiceNum[liveCharacterList[diceIdx + 3]]))
                    {
                        enemyDiceChange(liveCharacterList[diceIdx], skillIdx);
                        enemyDiceChange(liveCharacterList[diceIdx + 1], skillIdx);
                        enemyDiceChange(liveCharacterList[diceIdx + 2], skillIdx);
                        enemyDiceChange(liveCharacterList[diceIdx + 3], skillIdx);
                        /*liveCharacterList.RemoveAt(diceIdx);
                        liveCharacterList.RemoveAt(diceIdx);
                        liveCharacterList.RemoveAt(diceIdx);
                        liveCharacterList.RemoveAt(diceIdx);*/
                        break;
                    }

                }
            }
        }
    }
    private bool MakeMyAttackSet(bool onlyChk, int characterIdx, int skillSelIdx, int selDiceIdx)
    {
        if (myCharacter[characterIdx] == null || myDice[selDiceIdx] == null) return false;

        Skill skill = myCharacter[characterIdx].skillUse(skillSelIdx);
        List<int> liveCharacterList = new List<int>();
        for (int i = selDiceIdx; i < 4; i++)
        {
            if (myCharacter[i] != null && myCharacter[i].getCurState() == 0)
            {
                liveCharacterList.Add(i);
            }
        }
        if (skill.getNeedDiceNum() > liveCharacterList.Count)
        {
            return false;
        }

        //필요 주사위가 1칸인 경우
        if (skill.getNeedDiceNum() == 1)
        {
            if (myDiceTake[liveCharacterList[0]] == -999 && condition_diceSkillCheck(skill.getNeedDice(0), myDiceNum[liveCharacterList[0]]))
            { // 첫번쨰주사위가 겹치는 경우
                if (!onlyChk) myDiceChange(liveCharacterList[0], characterIdx, skillSelIdx);
                return true;
            }
        }
        //필요 주사위가 2칸인 경우
        else if (skill.getNeedDiceNum() == 2)
        {
            if (myDiceTake[liveCharacterList[0]] == -999 && condition_diceSkillCheck(skill.getNeedDice(0), myDiceNum[liveCharacterList[0]]) &&
                myDiceTake[liveCharacterList[1]] == -999 && condition_diceSkillCheck(skill.getNeedDice(1), myDiceNum[liveCharacterList[1]]))
            { // 첫번쨰주사위가 겹치는 경우
                if (!onlyChk)
                {
                    myDiceChange(liveCharacterList[0], characterIdx, skillSelIdx);
                    myDiceChange(liveCharacterList[1], characterIdx, skillSelIdx);
                }
                return true;
            }

        }
        else if (skill.getNeedDiceNum() == 3)
        {
            if (myDiceTake[liveCharacterList[0]] == -999 && condition_diceSkillCheck(skill.getNeedDice(0), myDiceNum[liveCharacterList[0]]) &&
                myDiceTake[liveCharacterList[1]] == -999 && condition_diceSkillCheck(skill.getNeedDice(1), myDiceNum[liveCharacterList[1]]) &&
                myDiceTake[liveCharacterList[2]] == -999 && condition_diceSkillCheck(skill.getNeedDice(2), myDiceNum[liveCharacterList[2]]))
            { // 첫번쨰주사위가 겹치는 경우
                if (!onlyChk)
                {
                    myDiceChange(liveCharacterList[0], characterIdx, skillSelIdx);
                    myDiceChange(liveCharacterList[1], characterIdx, skillSelIdx);
                    myDiceChange(liveCharacterList[2], characterIdx, skillSelIdx);
                }
                return true;
            }
        }
        else if (skill.getNeedDiceNum() == 4)
        {
            if (myDiceTake[liveCharacterList[0]] == -999 && condition_diceSkillCheck(skill.getNeedDice(0), myDiceNum[liveCharacterList[0]]) &&
                myDiceTake[liveCharacterList[1]] == -999 && condition_diceSkillCheck(skill.getNeedDice(1), myDiceNum[liveCharacterList[1]]) &&
                myDiceTake[liveCharacterList[2]] == -999 && condition_diceSkillCheck(skill.getNeedDice(2), myDiceNum[liveCharacterList[2]]) &&
                myDiceTake[liveCharacterList[3]] == -999 && condition_diceSkillCheck(skill.getNeedDice(3), myDiceNum[liveCharacterList[3]]))
            { // 첫번쨰주사위가 겹치는 경우
                if (!onlyChk)
                {
                    myDiceChange(liveCharacterList[0], characterIdx, skillSelIdx);
                    myDiceChange(liveCharacterList[1], characterIdx, skillSelIdx);
                    myDiceChange(liveCharacterList[2], characterIdx, skillSelIdx);
                    myDiceChange(liveCharacterList[3], characterIdx, skillSelIdx);
                }
                return true;
            }
        }

        return false;
    }

    private bool readyBattleChk = false;
    public bool skillEmptyChk()
    {
        return readyBattleChk && 
            myDiceTake[0] == -999 &&
            myDiceTake[1] == -999 &&
            myDiceTake[2] == -999 &&
            myDiceTake[3] == -999 ;
    }
    public void skillEmptyChkEnd()
    {
        readyBattleChk = false;
    }
    public void click_dice(int diceIdx)
    {
        

        if (currentLightUI == 0 && currentMoveUI == 0)
        {
            if (curPhase == 3)
            {
                SoundManager_Sfx.Instance.playSound(4);
                if (diceIdx >= 0 && diceIdx < 4) click_characterSkill_Dice(diceIdx); //아군 스킬 배정용
                else if (diceIdx >= 4 && diceIdx < 8) click_enemySkill_Dice(diceIdx);
            }
            else if (curPhase == 5)
            {
                SoundManager_Sfx.Instance.playSound(3);
                if (diceIdx >= 0 && diceIdx < 4) click_BattleSkill_dice(diceIdx); //아군 스킬 사용
            }
        }
    }

    //주사위 UI를 해당 높이로 변경한다
    private IEnumerator MoveUI(GameObject gameObjTemp, float inputY)
    {
        currentMoveUI++;
        float termY = 0.2f;
        Vector3 destination = new Vector3(gameObjTemp.transform.position.x, inputY, 0);
        if (gameObjTemp.transform.position.y < inputY)
        {
            termY *= -1;

            while (gameObjTemp.transform.position.y < inputY + termY)
            {
                gameObjTemp.transform.position = Vector3.Lerp(gameObjTemp.transform.position, destination, 0.05f);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            while (gameObjTemp.transform.position.y > inputY + termY)
            {
                gameObjTemp.transform.position = Vector3.Lerp(gameObjTemp.transform.position, destination, 0.05f);
                yield return new WaitForSeconds(0.01f);
            }
        }
        gameObjTemp.transform.position = destination;


        currentMoveUI--;
    }
    private IEnumerator MoveUI(GameObject gameObjTemp, float inputY, float delayTemp)
    {
        currentMoveUI++;
        yield return new WaitForSeconds(delayTemp);
        float termY = 0.2f;
        Vector3 destination = new Vector3(gameObjTemp.transform.position.x, inputY, 0);
        if (gameObjTemp.transform.position.y < inputY)
        {
            termY *= -1;

            while (gameObjTemp.transform.position.y < inputY + termY)
            {
                gameObjTemp.transform.position = Vector3.Lerp(gameObjTemp.transform.position, destination, 0.05f);
                yield return new WaitForSeconds(0.01f);
            }
        }
        else
        {
            while (gameObjTemp.transform.position.y > inputY + termY)
            {
                gameObjTemp.transform.position = Vector3.Lerp(gameObjTemp.transform.position, destination, 0.05f);
                yield return new WaitForSeconds(0.01f);
            }
        }
        gameObjTemp.transform.position = destination;
        
        currentMoveUI--;
    }

    //스킬 선택 시 주사위UI를 업데이트 하는 함수
    void updateMyDiceUI()
    {
        int curSkillVal = -999;
        int startIdx = -999;
        int endIdx = -999;

        //초기화
        for (int i = 0; i < 4; i++)
        {
            if (i < 3) diceUIChain[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            diceUIChk[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
        for (int i = 0; i < 4; i++)
        {
            if (myCharacter[i] == null || myCharacter[i].getCurState() != 0) continue;
            if (curSkillVal != -999)// 이미 시작점이 있는 경우
            {
                if (myDiceTake[i] != -999) // 해당 주사위가 빈칸이 아니면
                {
                    if (myDiceTake[i] == curSkillVal)  //같은 값을 받았을때
                    {
                        updateDiceUI_draw(curSkillVal / 10, curSkillVal % 10, i, false); //서브그리기
                    }
                    else // 다른 값을 받았을때
                    {
                        updateDiceUI_draw_chain(curSkillVal / 10, curSkillVal % 10, startIdx, endIdx); //이전 기반으로 chain 걸기
                        curSkillVal = myDiceTake[i];
                        updateDiceUI_draw(curSkillVal / 10, curSkillVal % 10, i, true); //스타트 그리기
                        startIdx = i;
                    }
                    endIdx = i;   //end 업데이트
                }

            }
            else // 시작점을 찾고 있는 경우
            {
                if (myDiceTake[i] != -999) // 해당 주사위가 빈칸이 아니면
                {
                    curSkillVal = myDiceTake[i];
                    updateDiceUI_draw(curSkillVal / 10, curSkillVal % 10, i, true); //스타트 그리기
                    startIdx = i; endIdx = i;
                }
            }

        }
        if (curSkillVal != -999) //끝에 도달했지만 chain이 필요한 경우
        {
            updateDiceUI_draw_chain(curSkillVal / 10, curSkillVal % 10, startIdx, endIdx); //이전 기반으로 chain 걸기
        }
    }

    //적팀 조합에 대한 주사위 UI를 업데이트 하는 함수 : 아직 테스트 못해봄
    void updateEnemyDiceUI()
    {
        int curSkillVal = -999;
        int startIdx = -999;
        int endIdx = -999;
        for (int i = 4; i < 8; i++)
        {
            if (i < 7) diceUIChain[i - 1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            diceUIChk[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
        for (int i = 4; i < 8; i++)
        {
            if (enemyCharacter[i - 4] == null || enemyCharacter[i - 4].getCurState() != 0) continue;

            if (curSkillVal != -999)// 이미 시작점이 있는 경우
            {

                if (enemyDiceTake[i - 4] != -999) // 해당 주사위가 빈칸이 아니면
                {
                    if (enemyDiceTake[i - 4] == curSkillVal)  //같은 값을 받았을때
                    {
                        updateDiceUI_draw(curSkillVal / 10, curSkillVal % 10, i, false); //서브그리기
                    }
                    else // 다른 값을 받았을때
                    {
                        updateDiceUI_draw_chain(curSkillVal / 10, curSkillVal % 10, startIdx - 1, endIdx - 1); //이전 기반으로 chain 걸기 (chain은 6개자리 사이즈를 사용하므로 1씩 빼주었다)
                        curSkillVal = enemyDiceTake[i - 4];
                        updateDiceUI_draw(curSkillVal / 10, curSkillVal % 10, i, true); //스타트 그리기
                        startIdx = i;
                    }
                    endIdx = i;   //end 업데이트
                }

            }
            else // 시작점을 찾고 있는 경우
            {
                if (enemyDiceTake[i - 4] != -999) // 해당 주사위가 빈칸이 아니면
                {
                    curSkillVal = enemyDiceTake[i - 4];
                    updateDiceUI_draw(curSkillVal / 10, curSkillVal % 10, i, true); //스타트 그리기
                    startIdx = i; endIdx = i;
                }
            }

        }
        if (curSkillVal != -999) //끝에 도달했지만 chain이 필요한 경우
        {
            updateDiceUI_draw_chain(curSkillVal / 10, curSkillVal % 10, startIdx - 1, endIdx - 1); //이전 기반으로 chain 걸기 chain은 6개자리 사이즈를 사용하므로 1씩 빼주었다)
        }
    }

    //주사위를 그리기 위한 함수
    void updateDiceUI_draw(int characterIdx, int skillIdx, int diceIdx, bool startPoint)
    {
        string strTemp = "dice_skillChk_";
        if (skillIdx == 0) strTemp += "up_"; else strTemp += "down_";
        strTemp += (characterIdx + 1).ToString();


        if (startPoint) diceUIChk[diceIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + strTemp);
        else
        {
            diceUIChk[diceIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + strTemp + "_sub");
        }
        /*
        if (characterIdx < 4)
        {
            mySkillUsed[characterIdx, skillIdx] = true;
        }
        else //적군이 사용할 경우 적군 주사위에 적용
        {
            enemySkillUsed[characterIdx, skillIdx] = true;
        }*/
    }
    //주사위간 chain을 그리기 위한 함수
    void updateDiceUI_draw_chain(int characterIdx, int skillIdx, int diceStartIdx, int diceEndIdx)
    {
        string strTemp = "dice_skillChk_";
        if (skillIdx == 0) strTemp += "up_"; else strTemp += "down_";
        strTemp += (characterIdx + 1).ToString();
        for (int i = diceStartIdx; i < diceEndIdx; i++)
        {
            diceUIChain[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + strTemp + "_chain");
        }
    }

    public void updateBattleBackground(string backgroundStr)
    {
        backGroundObj[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/battleUI/spr_field_" + backgroundStr);
    }

    //phase관리를 위한 코루틴
    //(코루틴이 너무 중첩해서 생기는 거 방지를 위해 만들어둠)
    bool phaseMoveChk(int phaseIdx)
    {
        return (curPhase == phaseIdx && currentLightUI == 0 && currentMoveUI == 0);
    }



    private IEnumerator startPhaseManage()
    {
        witchHatButton.SetActive(false);
        upDownManager.Instance.setItemTypeButtonLock(false);
        startBattlePhase();
        itemManager.Instance.enterBattlePhase();
        do {
            
            infoBtn.GetComponent<BoxCollider2D>().enabled = true;
            yield return new WaitUntil(() => phaseMoveChk(1));
            StartCoroutine(diceThrowPhase());
            yield return new WaitUntil(() => phaseMoveChk(2));
            curPhase = 3;
            //StartCoroutine(witchPowerPhase());
            yield return new WaitUntil(() => phaseMoveChk(3));
            StartCoroutine(skillSelectPhase());
            yield return new WaitUntil(() => phaseMoveChk(4));
            StartCoroutine(readyBattlePhase());
            yield return new WaitUntil(() => phaseMoveChk(5));
            StartCoroutine(battlePhase());
            yield return new WaitUntil(() => phaseMoveChk(6));
            upDownManager.Instance.setItemTypeButtonLock(false);
            StartCoroutine(endPhase());

            yield return new WaitUntil(() => curPhase != 6 && currentLightUI == 0 && currentMoveUI == 0);
            //페이즈가 1로 돌아가지 않았다면(승패 결정) 전투 종료로 반복문 탈출.
        } while (curPhase == 1);

        upDownManager.Instance.setItemTypeButtonLock(false);
        setCurClickSkill(-1);
        deleteSkillCommand();

        //정면 보는 마녀
        updateMoveUI(0);
        upDownManager.Instance.resetUI();

    }

    //DiceThrow Phase  Start (phase 1- dice throw start)//
    private IEnumerator diceThrowPhase()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                myFireObj[i, j] = battleFireObject[i * 2 + j].GetComponent<ParticleSystem>(); myFireObj[i, j].Stop();
                enemyFireObj[i, j] = battleFireObject[i * 2 + j + 8].GetComponent<ParticleSystem>(); enemyFireObj[i, j].Stop();
            }
        }
        witchPowerObj[0].GetComponent<hoverRotate>().expandEnd();
        witchPowerObj[1].GetComponent<hoverRotate>().expandEnd();
        witchPowerObj[2].GetComponent<hoverRotate>().expandEnd();

        for (int i = 0; i < 4; i++)
        { //처음에는 건들여도 별 변화 없도록
            hoverRotateAble(myDiceUI[i], 0, false);
            hoverRotateAble(enemyDiceUI[i], 0, false);
            hoverRotateAble(myDiceUI[i], 1, false);
            hoverRotateAble(enemyDiceUI[i], 1, false);
        }
        for (int i = 0; i < 4; i++)
        {
            myDiceUI[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
            enemyDiceUI[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
        }

        //주사위 굴리기 UI(ui 초기화)

        /*
        for (int i = 0; i < 4; i++)
        {
            StartCoroutine(makeDark(myCharacterObjUI[i], 0.7f));
            StartCoroutine(makeDark(enemyCharacterObjUI[i], 0.7f));
        }
        */
        //updateMoveUI(4);
        /*
        StartCoroutine(MoveUI(characterUI, -75.0f));
        StartCoroutine(MoveUI(diceFullUI, -58.0f));
        StartCoroutine(MoveUI(backGroundObj[0], -78.0f)); // 78f : skillSelect  62f: battle

        StartCoroutine(MoveUI(backGroundObj[3], -250.0f));
        */
        updateBlackAlpha(0);

        diceThrowChk = false;
        StartCoroutine(Dice_Throw_Phase());
        yield return new WaitUntil(() => diceThrowChk);

        
        //yield return new WaitForSeconds(1f);
        curPhase = 2;
    }
    private bool diceThrowChk = false;

    public GameObject[] diceCoverAnimation = new GameObject[8];

    public IEnumerator Dice_Throw_Phase()
    {
        if (curPhase == 1)
        {
            if (AdventureManager.Instance.getTutorial() == 7) {//만약 튜토리얼 중인경우 5번 대화(당황하는 남주인공)
                TalkManager.Instance.startTalk(5);
                yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
            }
            else if (AdventureManager.Instance.getTutorial() == 11) //만약 튜토리얼 중인경우 7번 대화(마녀의 운명 마법 사용)
            {
                TalkManager.Instance.startTalk(7);
                yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
            }
            //아군 모든 주사위 던지기
            for (int i = 0; i < 4; i++)
            {
                
                if (myCharacter[i] != null && myCharacter[i].getCurState() == 0)
                {
                    myDice[i] = myCharacter[i].dice;
                    diceCoverAnimation[i].GetComponent<Animator>().Play("diceRoll");
                    diceCoverAnimation[i].transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 4) * -90);
                    //Instantiate(diceRollEff, myDiceUI[i].transform.position, Quaternion.Euler(0, 0, Random.Range(0, 4) * -90)); //사용된 아이템에 대해 effect
                    
                    SoundManager_Sfx.Instance.playSound(2);
                    yield return new WaitForSeconds(0.25f);
                    
                    myDice[i].throwDice();
                    myDiceNum[i] = myDice[i].getNum();
                    //임시 주사위 UI 변경
                    myDiceUI[i].transform.rotation = Quaternion.Euler(0, 0, myDice[i].getDir() * -90);
                    myDiceUI[i].GetComponent<SpriteRenderer>().sprite = diceSprite[myDice[i].getNum() - 1];
                }
                else
                {
                    myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                }
            }

            //적군 모든 주사위 던지기
            for (int i = 0; i < 4; i++)
            {
                if (enemyCharacter[i] != null && enemyCharacter[i].getCurState() == 0)
                {
                    enemyDice[i] = enemyCharacter[i].dice;
                    diceCoverAnimation[i+4].GetComponent<Animator>().Play("diceRoll");
                    diceCoverAnimation[i+4].transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 4) * -90);
                    //Instantiate(diceRollEff, enemyDiceUI[i].transform.position, Quaternion.Euler(0, 0, Random.Range(0, 4) * -90)); //사용된 아이템에 대해 effect
                    SoundManager_Sfx.Instance.playSound(2);
                    yield return new WaitForSeconds(0.25f);
                    enemyDice[i].throwDice();
                    enemyDiceNum[i] = enemyDice[i].getNum();

                    enemyDiceUI[i].transform.rotation = Quaternion.Euler(0, 0, enemyDice[i].getDir() * -90);
                    enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = diceSprite[enemyDice[i].getNum() - 1];
                }
                else
                {
                    enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                }
            }
            if (myDiceState[0] != 0 || myDiceState[1] != 0 || myDiceState[2] != 0 || myDiceState[3] != 0 ||
                enemyDiceState[0] != 0 || enemyDiceState[1] != 0 || enemyDiceState[2] != 0 || enemyDiceState[3] != 0) {
                yield return new WaitForSeconds(0.2f);

                //주사위 상태 배치
                for (int i = 0; i < 4; i++)
                {
                    if (myDiceState[i] != 0) {
                        
                        SoundManager_Sfx.Instance.playSound(73);
                        myDiceNum[i] = myDiceState[i];
                        //임시 주사위 UI 변경
                        myDiceUI[i].transform.rotation = Quaternion.Euler(0, 0, myDice[i].getDir() * -90);
                        myDiceUI[i].GetComponent<SpriteRenderer>().sprite = diceSprite[myDiceNum[i] - 1];
                        changeDiceState(i, -999);
                        yield return new WaitForSeconds(0.1f);
                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (enemyDiceState[i] != 0)
                    {
                        SoundManager_Sfx.Instance.playSound(73);
                        enemyDiceNum[i] = enemyDiceState[i];
                        //임시 주사위 UI 변경
                        enemyDiceUI[i].transform.rotation = Quaternion.Euler(0, 0, enemyDice[i].getDir() * -90);
                        enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = diceSprite[enemyDiceNum[i] - 1];
                        changeDiceState(i + 4, -999);
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }


            if (AdventureManager.Instance.getTutorial() != 0 && AdventureManager.Instance.getTutorial() < 11) { witchHatButton.SetActive(false); }
            else witchHatButton.SetActive(true);
            //임시로 넣어둠. 이곳에 적군 스킬 자동배치 함수가 들어가야 한다!
            MakeEnemyAttackSet();
            updateEnemyDiceUI();

           

            diceThrowChk = true;
        }
    }

    //DiceThrow Phase End (phase 1- dice throw finish)//

    /// Witch Power Start (Phase 2- witch Power Select)///

    private int witchPowerState = 0;       //현재 보고 있는 마녀 능력의 index를 담는 변수

    private int witchPowerClickState = -1; //현재 마녀 능력 사용에 필요한 dice 수를 담는다 

    public bool witchPowerAble(int powerIdx)
    {
        if (powerIdx == 1) return true;
        if ((powerIdx == 2 || powerIdx == 3) && jsonDataManager.Instance.getChapterRead(0, 2) ==2 ) return true;
        return false;
    }
    //witch Power 선택 시작!
    public void witchPowerEffDestroy()
    {
        GameObject witchPowerDestroyObj = Instantiate(hitEff, witchPowerObj[0].transform.position, Quaternion.Euler(0, 0, 0));
        witchPowerDestroyObj.GetComponent<Animator>().Play("witchPowerDestroy");
    }
    public void witchPowerPhase()
    {
        /*
        //첫 튜토리얼에서는 마녀의 능력을 사용하지 않는다. X
        
        if (AdventureManager.Instance.getTutorial() == 1 || AdventureManager.Instance.getTutorial() == 2) { curPhase = 3; }
        else
        {
        */

        for (int i=0;i<7;i++)
        {
            if (witchPowerAble(i)) //나중에 챕터별 unlock 추가 필요
            {
                witchPowerSelectButton[i].GetComponent<SpriteRenderer>().sprite =
                    Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPowerNum/spr_witchPowerSelectNum_" + i.ToString());
            }
            else
            {
                witchPowerSelectButton[i].GetComponent<SpriteRenderer>().sprite =
                    Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPowerNum/spr_witchPowerSelectNum_lock");
            }
        }
        
        backGroundObj[3].GetComponent<Animator>().Play("ChangeBattleToPower");
        backGroundObj[4].GetComponent<Animator>().Play("ChangeBattleToPower");

        for (int i=0;i<4;i++)
        {
            myDiceUI[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.7f);
            enemyDiceUI[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.7f);
        }
        

            witchPowerObj[0].SetActive(true);
            witchPowerObj[1].SetActive(true);
            witchPowerObj[2].SetActive(true);
            witchPowerSelectObjEntity.SetActive(true);
            hoverInWitchPowerNum(1);
           witchPowerObj[0].GetComponent<Animator>().Play("Create");
            //witchPowerObj[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPower_noUse");
            for (int i = 0; i < 3; i++) witchPowerObj[i].transform.position = new Vector3(witchPowerObj[i].transform.position.x, 30, witchPowerObj[i].transform.position.z);

            /*
            if (AdventureManager.Instance.getTutorial() == 3) //만약 튜토리얼 중인경우 7번 대화(마녀의 운명 마법 사용)
            {
                TalkManager.Instance.startTalk(8);
                yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
            }
            */
            witchPowerState = 1;
            witchPowerClickState = -1;

            witchPowerState_Change(0);
        //}
    }
    public int getCurWitchPower()
    {
        return witchPowerState;
    }
    public void hoverInWitchPowerNum(int idx)
    {

        if (!witchPowerAble(idx)) //챕터상 아직 쓸 수 없는 경우 통과 
        {
            return;
        }

        for (int i=0;i<7;i++)
        {
            witchPowerSelectObjOutline[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        witchPowerSelectObjOutline[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPowerNum/spr_witchPowerSelectNum_outline");

        witchPowerClickState = 6;
        int witchPowerTemp = idx;
        int diceNum = 6;
        if (witchPowerTemp >= 0 && witchPowerTemp <= 3) diceNum = 1; // 능력이 reroll0, reroll, add, sub, 이어서 주사위 하나만 쓰는 경우
        if (witchPowerTemp == 4 || witchPowerTemp == 5) diceNum = 2; // 능력이 swap, copy 여서 주사위 2개를 쓰는 경우
        if (witchPowerTemp == 6) diceNum = 1; //능력이 모두 바꾸기라서 주사위 하나만 쓰는 경우.

        clickedDice[0] = -1;
        clickedDice[1] = -1;

        witchPowerClickState = diceNum;
        witchPowerState = idx;
        witchPowerObj[0].GetComponent<Animator>().Play(witchPowerTemp.ToString());

    }
    public void hoverOutWitchPowerNum()
    {
        
    }

    public void witchPowerLookUpdate(int idx)
    {
        if (idx < 0) backGroundObj[4].GetComponent<Animator>().Play("eyeMove 1");
        else backGroundObj[4].GetComponent<Animator>().Play("look" + idx.ToString());
    }

    //마녀 파워 선택 (좌우)
    public void hoverInWitchPowerNextButton(int idx)
    {
        witchPowerObj[idx+1].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
    }
    public void hoverOutWitchPowerNextButton() {
        witchPowerObj[1].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
        witchPowerObj[2].GetComponent<SpriteRenderer>().material.SetInt("_Radius",0);
    }
    public void witchPowerState_Change(int dir)
    {
        SoundManager_Sfx.Instance.playSound(0);


        //);
        witchPowerClickState = 6;
        int preState = witchPowerState;
        if (dir == 1)
        {
            do
            {
                witchPowerState++;
                if (witchPowerState > 6) witchPowerState = 0;
                hoverInWitchPowerNum(witchPowerState);
            } while (!witchPowerAble(witchPowerState));

        }
        else if(dir == -1)
        {
            do
            {
                witchPowerState--;
                if (witchPowerState < 0) witchPowerState = 6;
                hoverInWitchPowerNum(witchPowerState);
            } while (!witchPowerAble(witchPowerState));
        }
        if (preState != witchPowerState) {
            shakeObject(witchPowerObj[0]);
            if (dir == -1) shakeObject(witchPowerObj[1]);
            else if (dir == 1) shakeObject(witchPowerObj[2]);
        }
    }

    public int[] needJewel = {0, 1, 3, 3, 2, 3, 1};
    //주사위 고르기. 마녀 스킬 추가되면 여기서 작업할것

    public Sprite getDiceSprite(int opt, int idx)
    {
        if (opt == 0) {
            if (idx < 4) {
                return myDiceUI[idx].GetComponent<SpriteRenderer>().sprite; 
            }
            else
            {
                return enemyDiceUI[idx-4].GetComponent<SpriteRenderer>().sprite;
            }
        }
        if (opt == 1) {
            return diceUIChk[idx].GetComponent<SpriteRenderer>().sprite;
         }
        if (opt == 2) {
             return diceUIChain[idx].GetComponent<SpriteRenderer>().sprite;
        }

        return null;
    }
    public void select_witchPower_Dice(int idx) {

        if (getCharacter(idx) == null || getCharacter(idx).getCurState() != 0) {
            return;
        }

        if (witchPowerClickState == 2) { //다중 선택일 경우, 다음거 선택할수 있도록 설정.
            witchPowerClickState = 1;
            clickedDice[witchPowerClickState] = idx;
            return;
        }

        if (witchPowerClickState == 1 && clickedDice[1] != -1 && clickedDice[0] == clickedDice[1]) {//같은 거 선택 금지
            return;
        }


        if (AdventureManager.Instance.getTutorial() == 15) AdventureManager.Instance.setTutorial(16);
        clickedDice[0] = idx;
        int witchPowerTemp = witchPowerState;

        if (needJewel[witchPowerTemp] > AdventureManager.Instance.getAdventureJewel()) {
            //마녀 능력 값이 부족한 경우
            fullUI.showFull(70);
            return;
        }

        AdventureManager.Instance.addMoney(1, needJewel[witchPowerTemp] * -1);
        //backGroundObj[4].GetComponent<Animator>().Play("DiceUse");
        if (witchPowerTemp == 0) //reroll 스킬 사용시 
        {
            rerollDice(clickedDice[0]);
            SoundManager_Sfx.Instance.playSound(2);
            upDownManager.Instance.activeWitchPowerDice(0,clickedDice[0]);
        }
        if (witchPowerTemp == 1) //reroll 스킬 사용시 
        {
            rerollDice(clickedDice[0]);
            SoundManager_Sfx.Instance.playSound(2);
            upDownManager.Instance.activeWitchPowerDice(1, clickedDice[0]);
        }
        if (witchPowerTemp == 2) //add 스킬 사용시 
        {
            addDice(clickedDice[0], true);
            SoundManager_Sfx.Instance.playSound(20);
            upDownManager.Instance.activeWitchPowerDice(2, clickedDice[0]);
        }
        if (witchPowerTemp == 3) //sub 스킬 사용시 
        {
            addDice(clickedDice[0], false);
            SoundManager_Sfx.Instance.playSound(16);
            upDownManager.Instance.activeWitchPowerDice(3, clickedDice[0]);
        }
        
        /*
        //스킬 사용되었으니 변경 값에 대하여 이펙트 생성
        for (int i = 0; i < 2; i++)
        {
            if (clickedDice[i] >= 0 && clickedDice[i] <= 3)
            {
                Instantiate(diceRollEff, myDiceUI[clickedDice[i]].transform.position, Quaternion.Euler(0, 0, Random.Range(0, 4) * -90)); //사용된 아이템에 대해 effect
                SoundManager_Sfx.Instance.playSound(2);
            }
            if (clickedDice[i] >= 4 && clickedDice[i] <= 7)
            {
                Instantiate(diceRollEff, enemyDiceUI[clickedDice[i] - 4].transform.position, Quaternion.Euler(0, 0, Random.Range(0, 4) * -90)); //사용된 아이템에 대해 effect
                SoundManager_Sfx.Instance.playSound(2);
            }
        }
        */
        //yield return new WaitForSeconds(1f);

        GameObject witchPowerDestroyObj = Instantiate(hitEff, witchPowerObj[0].transform.position, Quaternion.Euler(0, 0, 0));
        witchPowerDestroyObj.GetComponent<Animator>().Play("witchPowerDestroy");

        //바뀐 아군 주사위 스킬 배치 변경
        for (int clickIdx = 0; clickIdx < 2; clickIdx++)
        {
            if (clickedDice[clickIdx] != -1 && clickedDice[clickIdx] >= 0 && clickedDice[clickIdx] < 4)
            {
                int skillTemp = myDiceTake[clickedDice[clickIdx]];
                for (int i = 0; i < 4; i++) if (myDiceTake[i] == skillTemp) myDiceTake[i] = -999;
            }
        }

        MakeEnemyAttackSet();
       
        updateMyDiceUI();
        updateEnemyDiceUI();
        
        //setCurClickSkill(-1);

        clickedDice[0] = -1;
        clickedDice[1] = -1;
        upDownManager.Instance.updateBigDicePower();
        //주사위 선택 종료시 버튼 이동
        //직관성을 위해 나눔

        //다음 페이즈로 넘어가는 부분


    }
    public void setCurClickSkill(int input) {
        curClickSkill = input;
        upDownManager.Instance.clickSkill(curClickSkill);
    }
    private void addDice(int idx, bool add)
    {
        int num;
        if (idx < 4)
        {
            if (myDice[idx] != null)
            {
                num = myDiceNum[idx];
                if (add) num++; else num--;
                if (num == 0) num = 6;
                if (num == 7) num = 1;
                myDiceNum[idx] = num;
                myDiceUI[idx].transform.rotation = Quaternion.Euler(0, 0, 0);
                myDiceUI[idx].GetComponent<SpriteRenderer>().sprite = diceSprite[num - 1];
            }
        }
        else
        {
            idx -= 4;
            if (enemyDice[idx] != null)
            {
                num = enemyDiceNum[idx];
                if (add) num++; else num--;
                if (num == 0) num = 6;
                if (num == 7) num = 1;
                enemyDiceNum[idx] = num;
                enemyDiceUI[idx].transform.rotation = Quaternion.Euler(0, 0, 0);
                enemyDiceUI[idx].GetComponent<SpriteRenderer>().sprite = diceSprite[num - 1];
            }
        }
        witchPowerClickState = -1;
    }
    private void rerollDice(int idx)
    {
        if (idx < 4)
        {
            if (myCharacter[idx] != null && myCharacter[idx].getCurState() == 0)
            {
                myDice[idx] = myCharacter[idx].dice;
                myDiceNum[idx] = myCharacter[idx].dice.throwDiceExcept();
                
                myDiceUI[idx].transform.rotation = Quaternion.Euler(0, 0, myDice[idx].dir * -90);
                myDiceUI[idx].GetComponent<SpriteRenderer>().sprite = diceSprite[myDice[idx].getNum() - 1];
            }
        }
        else
        {
            idx -= 4;
            if (enemyCharacter[idx] != null && enemyCharacter[idx].getCurState() == 0)
            {
                enemyDice[idx] = enemyCharacter[idx].dice;
                enemyDiceNum[idx] = enemyCharacter[idx].dice.throwDiceExcept();
                enemyDiceUI[idx].transform.rotation = Quaternion.Euler(0, 0, enemyDice[idx].dir * -90);
                enemyDiceUI[idx].GetComponent<SpriteRenderer>().sprite = diceSprite[enemyDice[idx].getNum() - 1];
            }
        }
        witchPowerClickState = -1;
    }
    //turn Dice는 직관성 문제로 인해 제거됨.
    public void turnDice(int input)
    {
        int idx = input / 10;
        int dir = input % 10;
        if (idx < 4)
        {
            if (myDice[idx] != null)
            {
                myDice[idx].turnDice(dir);
                myDiceNum[idx] = myDice[idx].getNum();
                myDiceUI[idx].transform.rotation = Quaternion.Euler(0, 0, myDice[idx].dir * -90);
                myDiceUI[idx].GetComponent<SpriteRenderer>().sprite = diceSprite[myDice[idx].getNum() - 1];
                SoundManager_Sfx.Instance.playSound(2);
            }
        }
        else
        {
            idx -= 4;
            if (enemyDice[idx] != null)
            {
                enemyDice[idx].turnDice(dir);
                enemyDiceNum[idx] = enemyDice[idx].getNum();
                enemyDiceUI[idx].transform.rotation = Quaternion.Euler(0, 0, enemyDice[idx].dir * -90);
                enemyDiceUI[idx].GetComponent<SpriteRenderer>().sprite = diceSprite[enemyDice[idx].getNum() - 1];
                SoundManager_Sfx.Instance.playSound(2);
            }
        }
        witchPowerClickState = -1;
    }

    //마녀 좌우 선택 UI 천천히 제거
    public void deleteWitchPowerUI()
    {
        for (int i = 0; i < 4; i++)
        {
            myDiceUI[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
            enemyDiceUI[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
        }
        backGroundObj[3].GetComponent<Animator>().Play("ChangePowerToBattle");
        backGroundObj[4].GetComponent<Animator>().Play("ChangePowerToBattle");
        for (int i = 1; i < 3; i++) witchPowerObj[i].transform.position = new Vector3(witchPowerObj[i].transform.position.x, 300, witchPowerObj[i].transform.position.z);

            witchPowerObj[0].SetActive(false);
            witchPowerObj[1].SetActive(false);
            witchPowerObj[2].SetActive(false);
        witchPowerSelectObjEntity.SetActive(false);
    }
    //마녀 좌우 선택 UI 천천히 생성

    /// Witch Power End (Phase 2- witch Power Select)///



    // Character Skill Select Start (Phase 3 - Character Skill Select)///

    private int curClickSkill = -1; //마지막으로 클릭한 스킬 정보를 저장한다. 저장형식은 characterIdx * 10 + skillIdx의 형태를 띈다. 선택된게 없으면 -1을 갖는다.

    private IEnumerator skillSelectPhase()
    {
        readyBattleChk = true;
        setCurClickSkill(-1);
        deleteSkillCommand();

        //정면 보는 마녀


        updateMoveUI(0);
        /*
        StartCoroutine(MoveUI(diceFullUI, 60.0f));
        StartCoroutine(MoveUI(backGroundObj[0], 0.0f)); // 78f : skillSelect  62f: battle
        StartCoroutine(MoveUI(backGroundObj[1], -300f));
        StartCoroutine(MoveUI(backGroundObj[3], 140f, 0.5f)); //59f

        StartCoroutine(MoveUI(characterUI, 0.0f)); //
        StartCoroutine(MoveUI(skillSelectUI[8], -50.0f)); //
        */

        for (int i = 0; i < 4; i++)
        {
            hoverRotateAble(myDiceUI[i], 0, false);
            hoverRotateAble(myDiceUI[i], 1, false);

            if (enemyDiceTake[i] != -999) hoverRotateAble(enemyDiceUI[i], 1, true);
            else hoverRotateAble(enemyDiceUI[i], 1, false);

        }

        if (AdventureManager.Instance.getTutorial() == 7)
        {//만약 튜토리얼 중인경우 5번 대화(당황하는 남주인공)
            TalkManager.Instance.startTalk(39);
            yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());

            yield return new WaitUntil(() => AdventureManager.Instance.getTutorial() == 8);

            TalkManager.Instance.startTalk(40);
            yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());

            yield return new WaitUntil(() => AdventureManager.Instance.getTutorial() == 9);
            TalkManager.Instance.startTalk(41);
            yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
        }
        if (AdventureManager.Instance.getTutorial() == 11)
        {//주사위 변경, 운명 마법 사용 관련
            TalkManager.Instance.setDescClickLock(true);
            TalkManager.Instance.setDescIdx(59);
            itemManager.Instance.getItemResult(1, 9);
            TalkManager.Instance.startTalk(43);
            yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());


            yield return new WaitUntil(() => AdventureManager.Instance.getTutorial() == 12);
            TalkManager.Instance.setDescClickLock(true);
            TalkManager.Instance.setDescIdx(60);
            TalkManager.Instance.startTalk(44);
            yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());

            yield return new WaitUntil(() => AdventureManager.Instance.getTutorial() == 13);
            TalkManager.Instance.setDescClickLock(true);
            TalkManager.Instance.setDescIdx(61);

            yield return new WaitUntil(() => AdventureManager.Instance.getTutorial() == 14);
            TalkManager.Instance.setDescClickLock(true);
            TalkManager.Instance.setDescIdx(62);
            TalkManager.Instance.startTalk(45);
            yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());

            yield return new WaitUntil(() => AdventureManager.Instance.getTutorial() == 15);
            TalkManager.Instance.setDescClickLock(true);
            TalkManager.Instance.setDescIdx(63);
            TalkManager.Instance.startTalk(46);
            yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());

            yield return new WaitUntil(() => AdventureManager.Instance.getTutorial() == 16);
            TalkManager.Instance.setDescClickLock(true);
            TalkManager.Instance.setDescIdx(64);

            yield return new WaitUntil(() => AdventureManager.Instance.getTutorial() == 17);
            TalkManager.Instance.setDescClickLock(false);
            TalkManager.Instance.setDescIdx(-1);
        }
        /*
        if (AdventureManager.Instance.getTutorial() == 1)
        {
            TalkManager.Instance.startTalk(6);
            yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());

        } //튜토리얼에서 주사위 굴리기를 알려주기 위한 대화
        */
        yield return new WaitUntil(() => currentMoveUI == 0 && currentLightUI == 0); //
    }



    //스킬 선택 중 버튼 클릭에 대한 코드
    public void click_characterSkill_Button(int input)
    {
        if (curPhase == 3 && currentLightUI == 0 && currentMoveUI == 0)
        {
            int characterIdx = input / 10;
            int skillIdx = input % 10;
            for (int i = 0; i < 4; i++) {
                hoverRotateAble(myDiceUI[i], 1, false);
            }
            if (myCharacter[characterIdx] != null && myCharacter[characterIdx].getCurState() == 0)
            {
                if (AdventureManager.Instance.getTutorial() == 7) {
                    AdventureManager.Instance.setTutorial(8);
                }
                for (int diceIdx = 0; diceIdx < 4; diceIdx++)
                {
                    if (MakeMyAttackSet(true, input / 10, input % 10, diceIdx))
                    {
                        //스킬 배치 가능한 곳이 흔들림.
                        hoverRotateAble(myDiceUI[diceIdx], 1, true);
                        shakeObject(myDiceUI[diceIdx]);
                    }
                }
                setCurClickSkill(input);
                makeSkillCommand(characterIdx, skillIdx);
                SoundManager_Sfx.Instance.playSound(0);
            }

            for (int i = 0; i < 4; i++) //이미 스킬이 있는 경우는 해제할 수 있어야 하므로
            {
                if (myDiceTake[i] != -999)
                {
                    hoverRotateAble(myDiceUI[i], 1, true);
                }
            }
        }
    }

    //조건과 주사위 일치 여부 확인
    private bool condition_diceSkillCheck(int condition, int diceVal) {
        if (condition == 0) return false; //사용하지 않는 값이 들어온 오류
        if (condition >= 1 && condition <= 6) return diceVal == condition;
        if (condition == 7) return diceVal % 2 == 1;
        if (condition == 8) return diceVal % 2 == 0;
        if (condition == 9) return true;

        if (condition > 10 && condition <= 16) {
            return (diceVal <= condition % 10);
        }

        if (condition > 20 && condition <= 26)
        {
            return (diceVal >= condition % 10);
        }
        return false;
    }

    //스킬 선택 중 주사위 클릭에 대한 코드
    private void click_characterSkill_Dice(int diceIdx)
    {
        if (curPhase == 3 && myCharacter[diceIdx] != null && myCharacter[diceIdx].getCurState() == 0 && currentLightUI == 0 && currentMoveUI == 0)
        {

            if (myDiceTake[diceIdx] != -999)
            {
                int deleteSkill = myDiceTake[diceIdx];
                for (int i = 0; i < 4; i++)
                {   //해당 스킬에 대한 모든 주사위 해제
                    if (myDiceTake[i] == deleteSkill)
                    {
                        myDiceChange(i, 0, -999);
                        //현재 클릭한 스킬이 있고 ,만약 삭제했음에도 현재 스킬을 받아드릴수 있는 경우. 얘도 마우스 겹쳤을때 확대 가능하도록
                        if (curClickSkill != -1 && MakeMyAttackSet(true, curClickSkill / 10, curClickSkill % 10, i))
                        {
                            hoverRotateAble(myDiceUI[diceIdx], 1, true);
                        }
                    }
                }

                updateMyDiceUI();
                //해당 스킬에 대한 버튼 해제
                mySkillUsed[(deleteSkill / 10), (deleteSkill % 10)] = false;

                //종료 후에 스킬을 가지고 있는 곳들은 댓을때 확대가 가능하도록.
                for (int i = 0; i < 4; i++)
                {
                    if (myDiceTake[i] != -999) hoverRotateAble(myDiceUI[i], 1, true);
                }

            }
            else if (curClickSkill != -1) //스킬 선택을 했으며 해당 주사위가 비어있는 경우.
            {
                int characterIdx = curClickSkill / 10;
                int skillIdx = curClickSkill % 10;

                Skill useSkill = myCharacter[characterIdx].skillUse(skillIdx);
                int needDiceNum = useSkill.getNeedDiceNum();
                //가능한지 확인
                if (MakeMyAttackSet(true, characterIdx, skillIdx, diceIdx))
                {   //가능한 경우 중복 스킬 제거, 배치, 주사위의 ui 업데이트

                    for (int i = 0; i < 4; i++)
                    {
                        if (myDiceTake[i] == curClickSkill)
                        {
                            shakeObject(myDiceUI[i]);
                            diceUIChk[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                            if (i < 3) diceUIChain[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");//연결 체인도 제거
                                                                                                                                                               //myDiceTake[i] = -999;
                            myDiceChange(i, 0, -999);
                        }
                    }
                    MakeMyAttackSet(false, characterIdx, skillIdx, diceIdx);
                    updateMyDiceUI();
                    mySkillUsed[characterIdx, skillIdx] = true;
                }
                else //불가능한 경우
                {
                    makeSkillCommand(characterIdx, skillIdx);
                    Debug.Log("It can't! - wrong Dice Problem");
                }
                //setCurClickSkill(-1);
                for (int i = 0; i < 4; i++)
                {
                    if (myDiceTake[i] != -999) hoverRotateAble(myDiceUI[i], 1, true);
                    else hoverRotateAble(myDiceUI[i], 1, false);
                }
            }
            //주사위에 할당된 스킬도 클릭된 스킬도 없다면 아무것도 하지 않는다.

            upDownManager.Instance.updateBigDiceSkill();
        }
    }

    private void click_enemySkill_Dice(int diceIdx)
    {
        diceIdx -= 4;
        if (curPhase == 3 && enemyCharacter[diceIdx] != null && enemyCharacter[diceIdx].getCurState() == 0 && currentLightUI == 0 && currentMoveUI == 0)
        {
            if (enemyDiceTake[diceIdx] != -999) //해당 주사위가 비어있지 않은 경우
            {
                if (curClickSkill != -1) { //현재 선택한 스킬이 있는 경우
                    int characterIdx = curClickSkill / 10;
                    int skillIdx = curClickSkill % 10;
                    makeSkillCommand(characterIdx, skillIdx);
                    setCurClickSkill(-1);
                    for (int i = 0; i < 4; i++) //이미 스킬이 있는 경우는 해제할 수 있어야 하므로
                    {
                        if (myDiceTake[i] != -999)
                        {
                            hoverRotateAble(myDiceUI[i], 1, true);
                        }
                        else hoverRotateAble(myDiceUI[i], 1, false);
                    }

                }

                shakeObject(enemyDiceUI[diceIdx]);
                int enemyCharacterIdx = enemyDiceTake[diceIdx] / 10;
                int enemySkillIdx = enemyDiceTake[diceIdx] % 10;
                makeSkillCommand(enemyCharacterIdx + 4, enemySkillIdx);
                //Skill useSkill = enemyCharacter[enemyCharacterIdx].skillUse(enemySkillIdx);
                //int needDiceNum = useSkill.getNeedDiceNum();
            }
            //주사위에 할당된 스킬도 클릭된 스킬도 없다면 아무것도 하지 않는다.
        }
    }
    //스킬이 눌려서 해당 skill에 대한 내용을 출력해야하는 경우
    public Skill getSkillTake(int idx) {
        
        Skill thisSkill = null;
        int characterIdx = 0;
        int skillIdx = 0;
        if (idx < 4) {
            characterIdx = myDiceTake[idx] / 10;
            skillIdx = myDiceTake[idx] % 10;
            thisSkill = myCharacter[characterIdx].skillUse(skillIdx);
        
        }
        else if (idx < 8)
        {
            characterIdx = enemyDiceTake[idx-4] / 10;
            skillIdx = enemyDiceTake[idx-4] % 10;
            thisSkill = enemyCharacter[characterIdx].skillUse(skillIdx);
        }
        return thisSkill;
    }
    public void makeSkillCommand(int characterIdx, int skillIdx)
    {
        Skill thisSkill = null;
        if (characterIdx >= 0 && characterIdx < 4) { thisSkill = myCharacter[characterIdx].skillUse(skillIdx); }
        else if (characterIdx >= 4 && characterIdx < 8) { thisSkill = enemyCharacter[characterIdx - 4].skillUse(skillIdx); }

        //적군 스킬이면서 본적 없는 스킬인 경우
        if (characterIdx >= 4 && characterIdx < 8 && !jsonDataManager.Instance.getMonsterSkill(enemyCharacter[characterIdx - 4].getDestiny().DestinyIdx, skillIdx))
        {
            upDownManager.Instance.skillDescUpdate("noImage", thisSkill.getNeedDice(0), thisSkill.getNeedDice(1),
                thisSkill.getNeedDice(2), thisSkill.getNeedDice(3), "Not Found", TalkManager.Instance.getDesc(17));
        }
        else
        {
            if (Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + thisSkill.getSkillName()) != null)
            {
                upDownManager.Instance.skillDescUpdate(thisSkill.getSkillName(), thisSkill.getNeedDice(0), thisSkill.getNeedDice(1),
                thisSkill.getNeedDice(2), thisSkill.getNeedDice(3), thisSkill.getSkillName(), thisSkill.getCommand());
            }
            else
            {
                upDownManager.Instance.skillDescUpdate("noImage", thisSkill.getNeedDice(0), thisSkill.getNeedDice(1),
                thisSkill.getNeedDice(2), thisSkill.getNeedDice(3), thisSkill.getSkillName(), thisSkill.getCommand());
            }
        }

    }
    void deleteSkillCommand()
    {
        upDownManager.Instance.skillDescUpdate("none", 0, 0, 0, 0, "", "");
    }

    public void flipBag_battle()    // 가방 키고 끄는 함수. 4페이즈, 5페이즈 일땐 끌수 없게 한다.
    {
        if (!getLoseChk && curPhase != 4 && curPhase != 5)
        {
            //아이템 킬꺼면 캐릭터창 종료
            if (characterInfoOpen) clickCharacterInfoBox();
            SoundManager_Sfx.Instance.playSound(0);
            itemManager.Instance.flipItemBox_BattleUI();
        }
    }


    //phase넘어가기
    public void moveToBattlePhase()
    {
        if (AdventureManager.Instance.getTutorial() == 7 ||
            AdventureManager.Instance.getTutorial() == 8) {
            fullUI.showFull(58);
            return;
        }
        if (curPhase == 3 && currentLightUI == 0 && currentMoveUI == 0)
        {
            if (!itemManager.Instance.getItemBoxOpen())
            {
                readyBattleChk = false;
                curPhase = 4;
                itemManager.Instance.flipItemBox_BattleUI(); //넘어갈때 passive칸을 켜준다.
            }
            else
            {
                itemManager.Instance.flipItemBox_BattleUI();// 열려있다면 끄고 넘어간다.
            }
        }
    }

    private IEnumerator readyBattlePhase()
    {
        if (curPhase == 4 && currentLightUI == 0 && currentMoveUI == 0)
        {
            upDownManager.Instance.clickItemTypeButton(3);
            upDownManager.Instance.setItemTypeButtonLock(true);
            witchHatButton.SetActive(false);
            battleBagBtn.GetComponent<CircleCollider2D>().enabled = false;
            for (int i = 0; i < 4; i++)
            {
                //이전에 달려있던 돌아가는 거 다 막아버리기
                hoverRotateAble(myDiceUI[i], 0, false);
                hoverRotateAble(myDiceUI[i], 1, false);

                hoverRotateAble(enemyDiceUI[i], 0, false);
                hoverRotateAble(enemyDiceUI[i], 1, false);
            }


            curPhase = -999;

            //updateMoveUI(3);



            //다음 페이즈로 넘어가는 부분
            yield return new WaitUntil(() => currentMoveUI == 0 && currentLightUI == 0); //

            //스킬 이미지를 각 주사위에 배치
            int curDiceNum = 0;
            string skillNameTake = "";
            for (int i = 0; i < 4; i++)  //아군 주사위 배치
            {
                if (myDice[i] == null || getCharacter(i) == null || getCharacter(i).getCurState() != 0) continue;
                myDiceUI[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                curDiceNum = myDiceTake[i];
                if (curDiceNum == -999)
                {
                    myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                }
                else
                {
                    myFireObj[curDiceNum / 10, curDiceNum % 10].Play(true);
                    skillNameTake = myCharacter[curDiceNum / 10].skillUse(curDiceNum % 10).getSkillName();
                    if (Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + skillNameTake) == null)
                    {
                        skillNameTake = "noImage";
                    }
                    shakeObject(myDiceUI[i]);
                    myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + skillNameTake);
                    yield return new WaitForSeconds(0.2f);
                }

            }
            for (int i = 0; i < 4; i++) //적군 주사위 배치
            {
                if (enemyDice[i] == null || getCharacter(i+4) == null || getCharacter(i+4).getCurState() != 0) continue;
                enemyDiceUI[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                curDiceNum = enemyDiceTake[i];
                if (curDiceNum == -999)
                {
                    enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                }
                else
                {
                    enemyFireObj[curDiceNum / 10, curDiceNum % 10].Play(true);
                    skillNameTake = enemyCharacter[curDiceNum / 10].skillUse(curDiceNum % 10).getSkillName();
                    if (Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + skillNameTake) == null)
                    {
                        skillNameTake = "noImage";
                    }
                    shakeObject(enemyDiceUI[i]);
                    enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + skillNameTake);
                    yield return new WaitForSeconds(0.2f);
                }
            }
            yield return new WaitForSeconds(0.3f);
           

             curPhase = 5;
        }
    }



    // Character Skill Select End (Phase 3 - Character Skill Select)//////


    // Character Battle Start (Phase 5 - true battle phase)//////
    private int clickDice_battlePhase = -999;

    private int[] clickCharacter = new int[8];         //클릭된 캐릭터 종류
    //private bool endClickEnemy;
    private bool[] characterClickAble = new bool[8]; //스킬 타겟 설정시 클릭이 가능한지
    private int characterTargetIdx = -999;                           //지금까지 스킬 타겟팅을 위해 클릭한 character의 수

    public void hoverInTarget(int idx)
    {
        battleTargetUI[idx].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.7f);
    }
    public void hoverOutTarget(int idx)
    {
        battleTargetUI[idx].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
    }
    private IEnumerator clickEnemy_Coroutine(int clickEnemyNum, int clickAbleTeam) //clickAbleTeam은 0 : 아군 대상 / 1: 적군대상 / 2 : 전체 대상을 의미한다.
    { //캐릭터 클릭을 위한 코루틴(입력된 갯수만큼 반복될 예정)
        characterTargetIdx = 0;   //character인덱스 초기화

        for (int i = 0; i < clickCharacter.Length; i++) { //모든 클릭된 캐릭터 초기화
            clickCharacter[i] = -999;
        }
        if (clickEnemyNum > 0) // chihuahua test
        {
            //지금 고민중인거는 죽은 캐릭터 위치 클릭가능하게 하나? -> 일단 null일때 조건문 없애놈 -> 근데 아닌거 같아서 걍 null일때 안생기게 해둠
            if (clickAbleTeam != 2)
            {//아군 선택만 가능한 경우
                for (int i = 0; i < 4; i++)
                {
                    if (myCharacter[i] != null && myCharacter[i].getCurState() != 2)
                    {
                        battleTargetUI[i].SetActive(true);
                        shakeObject(battleTargetUI[i]);
                        battleTargetUI[i].GetComponent<Animator>().Play("Create");
                        battleTargetUI[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
                        characterClickAble[i] = true;
                    }
                }
            }
            else if (clickAbleTeam != 1)
            {//적군 선택만 가능한 경우
                for (int i = 4; i < 8; i++)
                {
                    if (enemyCharacter[i-4] != null && enemyCharacter[i-4].getCurState() != 2)
                    {
                        //if (enemyCharacter[i-4] != null && enemyCharacter[i-4].getCurState() != 2) {
                        battleTargetUI[i].SetActive(true);
                        //shakeObject(battleTargetUI[i]);
                        battleTargetUI[i].GetComponent<Animator>().Play("Create");
                        battleTargetUI[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
                        characterClickAble[i] = true;
                    }
                        //}  
                }
            }

            while (characterTargetIdx < clickEnemyNum) //클릭된 캐릭터 값을 선택수만큼 배열에 저장
            {
                yield return new WaitUntil(() => clickCharacter[characterTargetIdx] != -999);
                characterTargetIdx++;
            }

            //클릭하지 못하게 바꾸기
            for (int i = 0; i < 8; i++)
            {
                battleTargetUI[i].SetActive(false);
                characterClickAble[i] = false;
            }
        }

        //해제해버리면 밖에서 못쓰니 밖에서 해제해줘야합니다!
    }



    public void click_battle_character(int characterIdxInput)
    {   //캐릭터를 누르면 해당 캐릭터 클릭이 비활성화되고
        if (curPhase == 5 && characterTargetIdx != -999 && characterClickAble[characterIdxInput] && !KillAnimationManager.Instance.getKillAnimationPlay())
        {
            clickCharacter[this.characterTargetIdx] = characterIdxInput; //누른 캐릭터 저장
            battleTargetUI[characterIdxInput].SetActive(false); //해당 target ui 배활성화
            characterClickAble[characterIdxInput] = false;  //누를수 없게 변경
        }
    }
    //적군의 clickArray를 자동으로 만들어준다. (아직 테스트 안해봄)
    private void makeEnemyClick(int clickEnemyNum, int clickAbleTeam)
    {
        for (int i = 0; i < clickCharacter.Length; i++)
        { //모든 클릭된 캐릭터 초기화
            clickCharacter[i] = -999;
        }
        //지금 고민중인거는 죽은 캐릭터 위치 클릭가능하게 하나? -> 일단 null일때 조건문 없애놈


        if (clickAbleTeam == 1)
        {//아군에 대한 스킬인 경우
            for (int i = 4; i < 8; i++)
            {
                if (enemyCharacter[i - 4] != null && enemyCharacter[i - 4].getCurState() == 0) {
                    characterClickAble[i] = true;
                }
            }
            for (int i = 0; i < clickEnemyNum; i++)
            {
                int temp = enemy_target_auto_random(1);
                clickCharacter[i] = temp;
                characterClickAble[temp] = false;
            }
        }
        else if (clickAbleTeam == 2)
        {//적군 선택만 가능한 경우
            for (int i = 0; i < 4; i++)
            {
                if (myCharacter[i] != null && myCharacter[i].getCurState() == 0) {
                    characterClickAble[i] = true;
                }
            }
            for (int i = 0; i < clickEnemyNum; i++) {
                int temp = enemy_target_auto(1);
                clickCharacter[i] = temp;
                characterClickAble[temp] = false;
            }

        }
        else if (clickAbleTeam == 0)
        {//전체 대상인 경우
            for (int i = 0; i < 4; i++)
            {
                if (myCharacter[i] != null && myCharacter[i].getCurState() == 0) {
                    characterClickAble[i] = true;
                }
            }
            for (int i = 4; i < 8; i++)
            {
                if (enemyCharacter[i - 4] != null && enemyCharacter[i - 4].getCurState() == 0)
                {
                    characterClickAble[i] = true;
                }
            }
            for (int i = 0; i < clickEnemyNum; i++)
            {
                int temp = enemy_target_auto_random(0);
                clickCharacter[i] = temp;
                characterClickAble[temp] = false;
            }
        }


        //클릭하지 못하게 바꾸기
        for (int i = 0; i < 8; i++)
        {
            characterClickAble[i] = false;
        }

    }
    public int enemy_target_auto_random(int targetTeam)
    {
        //전체에 대해 가해지는 스킬인 경우
        if (targetTeam == 0)
        {
            int characterNum = 0;
            int targetNum = 0;
            for (int i = 0; i < 8; i++)
            {
                if (characterClickAble[i]) characterNum++;
            }
            targetNum = Random.Range(0, characterNum);

            for (int i = 0; i < 8; i++)
            {
                if (characterClickAble[i])
                {
                    targetNum--;
                    if (targetNum < 0) return i;
                }
            }
            return 0;
        }
        if (targetTeam == 1) // 아군에 대해 가해지는 스킬인 경우
        {
            int characterNum = 0;
            int targetNum = 0;
            for (int i = 4; i < 8; i++)
            {
                if (characterClickAble[i]) characterNum++;
            }
            targetNum = Random.Range(0, characterNum);

            for (int i = 4; i < 8; i++)
            {
                if (characterClickAble[i])
                {
                    targetNum--;
                    if (targetNum < 0) return i;
                }
            }
            return 0;
        }
        return 0;
    }
    public int enemy_target_auto(int inputTargetNum)
    {
        //대상이 한명일 경우
        if (inputTargetNum == 1)
        {
            //대상이 적군일 경우 거리 기반으로 가장 앞에 있는 적이 잘 맞도록 만들어 낸다.
            int characterNum = 0;
            int targetNum = 0;
            for (int i = 0; i < 4; i++)
            {
                if (characterClickAble[i]) characterNum++;
            }
            if (characterNum == 1) targetNum = 0;
            if (characterNum == 2) targetNum = Random.Range(0, 3);
            if (characterNum == 3) targetNum = Random.Range(0, 6);
            if (characterNum == 4) targetNum = Random.Range(0, 10);

            if (targetNum > 5) targetNum = 3;
            else if (targetNum > 2) targetNum = 2;
            else if (targetNum > 0) targetNum = 1;

            for (int i = 0; i < 4; i++)
            {
                if (characterClickAble[i])
                {
                    targetNum--;
                    if (targetNum < 0) return i;
                }
            }
            return 0;
        }
        return 0;
    }

    //미완 : 공격 연동 & 스킬 데미지 & 적군 공격 등의 연동이 되어있지 않다. 
    //아군&적군은 죽으면 운명 끊기는 거 꼭 확인할것!

    //공격packet 생성 함수 호출 시 만드는 사용 주사위 배열
    int[] makeDiceArrToMakePacket = new int[4];
    //공격 packet 생성 시 받아오는 변수
    List<TakeSkillPacket> takeSkillPacketArr = new List<TakeSkillPacket>();

    //공격 packet 생성 함수 호출시 사용되는 주사위 배열을 만들어내는 함수
    private void makeMyDice_BattlePhase(int startIdx, int needDiceNum)
    {
        for (int i = 0; i < 4; i++) { //초기화
            makeDiceArrToMakePacket[i] = -999;
        }
        int curIdx = 0;
        for (int i = 0; i < 4; i++) { //유의미한 길이만큼 길이 생성
            if (startIdx + i < 4 && myDiceNum[startIdx + i] != -999) //유의미한 주사위 값일 경우만
            {
                makeDiceArrToMakePacket[curIdx] = myDiceNum[startIdx + i]; //해당 주사위값 넣기
                curIdx++;
                if (curIdx == needDiceNum) i = 4;// 주사위 수 다채웠으면 종료
            }

        }

    }
    private void makeEnemyDice_BattlePhase(int startIdx, int needDiceNum)
    {
        for (int i = 0; i < 4; i++)
        { //초기화
            makeDiceArrToMakePacket[i] = -999;
        }
        int curIdx = 0;

        for (int i = 0; i < 4; i++)
        { //유의미한 길이만큼 길이 생성
            if (startIdx + i < 4 && enemyDiceNum[startIdx + i] != -999) //유의미한 주사위 값일 경우만
            {
                makeDiceArrToMakePacket[curIdx] = enemyDiceNum[startIdx + i]; //해당 주사위값 넣기
                curIdx++;
                if (curIdx == needDiceNum) i = 4;// 주사위 수 다채웠으면 종료
            }

        }

    }

    private void battleAnimationControl(int characterIdx, int option)
    {
        //option은 변화시킬 대상에 대한 정보
        //0 : empty(아마 원복에 쓸듯해서 방치)
        //1 : hit damage()
        //2 : dead (아직 미사용)
        if (characterIdx < 4)
        {
            if (option == 1)
            {
                myCharacterObjUIAnim[characterIdx].Play("Hit");
            }
            else if (option == 2) myCharacterObjUIAnim[characterIdx].Play("Dead");
        }
        else
        {
            characterIdx -= 4;
            if (option == 1) enemyCharacterObjUIAnim[characterIdx].Play("Hit");
            else if (option == 2) enemyCharacterObjUIAnim[characterIdx].Play("Dead");
        }
    }

    //공격시 애니메이션은 ready_start(공격전 - 수치 검색전)-ready_end(수치 계산 && 타겟팅 대기)
    //                    -atk_ start(타겟후 공격 애니메이션)- -atk_end(다음 공격 대기)
    //                    -return_start(귀환 시작 애니메이션) - return_end(귀환 끝) ->끝나고 idle로 넘어간다.
    //각각은 1부터 6로 지정.
    private Vector3[] myCharacterInitPosition = new Vector3[4];
    private Vector3[] enemyCharacterInitPosition = new Vector3[4];


    private void skillAnimationControl(bool team, int timing, int curIdx, Skill skill, int characterIdx, int enemyIdx, int skillIdx)
    //아군인지 아닌지, 현재 누른 수, 스킬, 사용 캐릭터 idx값, 피격 캐릭터 idx값, 스킬 사용번째(1번 공격인지 2번 공격인지)  
    {
        /*
        if (curIdx < skill.Anim)//애니메이션 성립 조건(스킬 내 타격 횟수보다 애니메이션이 아직 많이 남은 경우)
        {
            if (team)
            { //아군이 스킬을 사용한 경우.
              //회전 멈추기
                myCharacterPunch[characterIdx] = 0f;
                myCharacterSwing[characterIdx] = 0f;
                if (skill.AnimMove == 1) { // 근딜 등의 이유로 움직여야 하면 움직인다.
                    //각각 공격 시, 귀환시 타이밍
                    if(timing == 2) myCharacterObjEntityUI[characterIdx].transform.position = enemyCharacterInitPosition[enemyIdx];
                    if (timing == 5) myCharacterObjEntityUI[characterIdx].transform.position = myCharacterInitPosition[characterIdx];
                }

                int skillIdxTemp = myCharacter[characterIdx].getSkillIdx(skillIdx); //몇번째 스킬인지 확인(skill 자체의 idx가 아니라, 해당 캐릭터의 몇번째 스킬인지 ex - 용사의 2번째 스킬은 idx가 6이지만 1번째 스킬이라 1로 출력된다.)
                myCharacterObjUIAnim[characterIdx].Play("Skill_" + skillIdxTemp.ToString() + "_" + timing.ToString()); //Skill_(사용하는 스킬의 idx)_(몇번째 애니메이션);
            }
            else
            { //적군이 스킬을 사용한 경우

            }
        }
        else //스킬 애니메이션이 필요 없는 경우
        {*/
        if ((team && myCharacterAtkReady[characterIdx] == timing - 1) || (!team && enemyCharacterAtkReady[characterIdx] == timing - 1))//실행하려는 애니메이션이 현재 필요한 타이밍이 맞는지 확인
        {
            if (timing == 1) //땡기기
            {
                setCharacterAtkReady(team, characterIdx, 1);
            }
            if (timing == 3) //놓기
            {
                setCharacterAtkReady(team, characterIdx, 3);
            }
        }
        //}
    }
    private bool chainChk = false;
    private void DeadCharacterUpdate(int idx) //캐릭터가 죽을 경우(getcurstate가 2를 반환시) 작동한다. 
                                              //플레이어 죽음으로 맛있는데! 가 아니라 플레이어 받게 되면 애니메이션은 밖에서 해줌.
    {
        changeDiceState(idx, -999);
        
        diceCoverAnimation[idx].GetComponent<Animator>().Play("diceBoom");
        diceCoverAnimation[idx].transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 4) * -90);

        for (int i=0;i<4;i++) {
            if (idx < 4)
            {
                GameObject temp = Instantiate(brokenEff, myDiceUI[idx].transform.position, Quaternion.Euler(0, 0, 0));
                temp.GetComponent<brokenObj>().setBrokenDice();
            }
            else { 
                GameObject temp = Instantiate(brokenEff, enemyDiceUI[idx - 4].transform.position, Quaternion.Euler(0, 0, 0));
                temp.GetComponent<brokenObj>().setBrokenDice();
            }
            
        }

        if (idx < 4)
        {

            int diceNumTemp = myDiceTake[idx]; //죽은 캐릭터가 지니고 있는 주사위를 사용한 스킬 들 해제


            upDownManager.Instance.skillIconUpdate(idx * 2, "none2");
            upDownManager.Instance.skillIconUpdate(idx * 2 + 1, "none2");

            for (int i = 0; i < 4; i++)   // 죽은 캐릭터가 가지고 있는 스킬 모두 해제.
            {
                if (myDiceTake[i] / 10 == idx)
                {
                    mySkillUsed[myDiceTake[i] / 10, myDiceTake[i] % 10] = false;
                    myDiceChange(i, 0, -999);
                    myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (myDiceTake[i] == diceNumTemp) {
                    myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                    myDiceChange(i, 0, -999);
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (getCharacter(i) == null || getCharacter(i).getCurState() != 0)
                {
                    myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                }
            }

            myDiceNum[idx] = -999;
            myDice[idx] = null;

            updateMyDiceUI();
            myDiceUI[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
        else
        {
            idx -= 4;
            int[] beforeDiceTake = { 0, 0, 0, 0 };
            for (int i = 0; i < 4; i++) beforeDiceTake[i] = enemyDiceTake[i]; // chain 애니메이션 발동을 위해 이전 상태 저장.

            int diceNumTemp = enemyDiceTake[idx]; //죽은 캐릭터가 지니고 있는 주사위를 사용한 스킬 들 해제

            for (int i = 0; i < 4; i++)   // 죽은 캐릭터가 가지고 있는 스킬 모두 해제.
            {
                if (enemyDiceTake[i] / 10 == idx)
                {
                    enemySkillUsed[enemyDiceTake[i] / 10, enemyDiceTake[i] % 10] = false;
                    enemyDiceChange(i, -999);
                    enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                }
            }


            for (int i = 0; i < 4; i++)
            {
                if (enemyDiceTake[i] == diceNumTemp)
                {
                    enemyDiceChange(i, -999);
                    enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                }
            }
            for (int i = 4; i < 8; i++)
            {
                if (getCharacter(i) == null || getCharacter(i).getCurState() != 0)
                {
                    enemyDiceUI[i-4].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                }
            }

            enemyDiceNum[idx] = -999;
            enemyDice[idx] = null;

            enemySkill[idx] = null;     //적군 스킬 attackset시 포함 안되도록 설정
            enemySkill[idx + 4] = null;
            enemySkillDiceNum[idx] = -999;
            enemySkillDiceNum[idx + 4] = -999;

            int moneyTemp = enemyCharacter[idx].getMoney();
            moneyTemp = Random.Range(moneyTemp - (moneyTemp * 2 / 5), moneyTemp + (moneyTemp * 2 / 5));
            if (moneyTemp <= 0) moneyTemp = 1;

            for (int i = 0; i < moneyTemp; i++)
            {
                GameObject temp = Instantiate(coinEff, enemyCharacterObjUI[idx].transform.position, Quaternion.Euler(0, 0, 0)); //사용된 아이템에 대해 effect
                temp.GetComponent<coinMove>().changeDest(2);
            }
            /*
            
            int jewelTemp = 1;
            for (int i = 0; i < jewelTemp; i++)
            {
                GameObject temp = Instantiate(coinEff, enemyCharacterObjUI[idx].transform.position, Quaternion.Euler(0, 0, 0)); //사용된 아이템에 대해 effect
                temp.GetComponent<coinMove>().changeDest(3);
            }
            */
            updateEnemyDiceUI();
            enemyDiceUI[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            chainChk = false;
            for (int i=0;i<4;i++)
            {
                if (beforeDiceTake[i] != enemyDiceTake[i] && i != idx) {
                    chainChk = true;
                    enemyChainAnim[i].GetComponent<Animator>().Play("breakChain_" + Random.Range(0, 6).ToString());
                }
            }
            
        }
    }

    public void makeCoin(int dest, Vector3 point)
    {
        GameObject temp = Instantiate(coinEff, point, Quaternion.Euler(0, 0, 0)); //사용된 아이템에 대해 effect
        temp.GetComponent<coinMove>().changeDest(dest);
    }


    private string makeBattleFontSize(int input) //상단 텍스트 폰트 사이즈 생성을 담당. 1050뎀 이상일때 520을 최대로 둔다.
    {
        int result;
        if (input < 50) result = 120;
        else result = 120 + (input - 50) * 2 / 5;
        if (result > 500) result = 500;
        return result.ToString();
    }

    private void changeDiceState(int characterIdx, int stateChange)
    {

        if (stateChange == 0) return;
        if (stateChange == -999)
        {
            stateChange = 0;
        }
        if (characterIdx < 4)
        {
            myDiceState[characterIdx] = stateChange;
            myDiceStateAnim[characterIdx].Play(stateChange.ToString());
        }
        else
        {
            enemyDiceState[characterIdx - 4] = stateChange;
            enemyDiceStateAnim[characterIdx - 4].Play(stateChange.ToString());
        }
    }

    bool passiveItemChk = false;
    private IEnumerator passiveUpdateBeforClick(List<TakeSkillPacket> takeSkillPacketArr, int[] usedDiceArr, bool updateLook)
    {
        float activeTime = 0.1f;
        passiveItemChk = true;
        bool[] effectChk = { false, false, false, false, false, false, false, false, false, false, false, false };
        int conditionNum = 0;
        if (updateLook)
        {
            for (int takeSkillArrIdx = 0; takeSkillArrIdx < takeSkillPacketArr.Count; takeSkillArrIdx++)
            {
                if (takeSkillPacketArr[takeSkillArrIdx].getSkillType() == 0 && takeSkillPacketArr[takeSkillArrIdx].getVal() > 0)
                {
                    battleTextObj.GetComponent<TextMeshPro>().text = "<size=" + makeBattleFontSize(takeSkillPacketArr[takeSkillArrIdx].getVal()) + ">" +
                                                takeSkillPacketArr[takeSkillArrIdx].getVal().ToString() //상단부에 적용될 text값 적기
                                            + "</size>";
                    break;
                }
            }
        }
        for (int takeSkillArrIdx = 0; takeSkillArrIdx < takeSkillPacketArr.Count; takeSkillArrIdx++)
        {
            for (int passiveItemIdx = 0; passiveItemIdx < 11; passiveItemIdx++)
            { //모든 passive 아이템을 확인해서 takeSkillPacket 수정

                passiveReturn tempPassiveReturn = itemManager.Instance.usePassiveItem(takeSkillPacketArr, takeSkillPacketArr[takeSkillArrIdx], passiveItemIdx, usedDiceArr, 0);
                if (!effectChk[passiveItemIdx] && tempPassiveReturn.used && updateLook) //만약 적용이 되엇으며 그 결과를 보여줄 경우
                {
                    effectChk[passiveItemIdx] = true;
                    if (tempPassiveReturn.cal != "none") specialTextManager.GetComponent<ExampleTextManager>().ShowPassiveText(passiveItemIdx, tempPassiveReturn.cal + tempPassiveReturn.val.ToString());
                    //SoundManager_doremi.Instance.playDoremi(itemUseIdx++);
                    upDownManager.Instance.activePassiveItem(passiveItemIdx);
                    //GameObject temp = Instantiate(passiveEffObj, itemManager.Instance.getItemInventoryPosition(passiveItemIdx), new Quaternion(0, 0, 0, 0)); //사용된 아이템에 대해 effect
                    
                    SoundManager_Sfx.Instance.playSound(55 + conditionNum);
                    if(conditionNum < 8) conditionNum++;

                    if (tempPassiveReturn.cal != "none") {
                        for (int fontSizeIdx = 0; fontSizeIdx < 10; fontSizeIdx++)
                        {
                            if (takeSkillPacketArr[takeSkillArrIdx].getSkillType() % 1000 == 0)
                            {
                                battleTextObj.GetComponent<TextMeshPro>().text = "<size=" + makeBattleFontSize(takeSkillPacketArr[takeSkillArrIdx].getVal() + fontSizeIdx * fontSizeIdx * 2) + ">" +
                                    takeSkillPacketArr[takeSkillArrIdx].getVal().ToString() //상단부에 적용될 text값 적기
                                + "</size>";
                            }
                            yield return new WaitForSeconds(activeTime / 5.0f);
                        }
                        for (int fontSizeIdx = 10; fontSizeIdx > 0; fontSizeIdx--)
                        {
                            if (takeSkillPacketArr[takeSkillArrIdx].getSkillType() % 1000 == 0)
                            {
                                battleTextObj.GetComponent<TextMeshPro>().text = "<size=" + makeBattleFontSize(takeSkillPacketArr[takeSkillArrIdx].getVal() + fontSizeIdx * fontSizeIdx * 2) + ">" +
                                 takeSkillPacketArr[takeSkillArrIdx].getVal().ToString() //상단부에 적용될 text값 적기
                             + "</size>";
                                yield return new WaitForSeconds(activeTime / 5.0f);
                            }
                        }
                        yield return new WaitForSeconds(activeTime);

                        activeTime /= 1.5f;
                    }
                    else
                    {
                        yield return new WaitForSeconds(activeTime * 2f);
                        activeTime /= 1.5f;
                    }
                }
            }
        }
        passiveItemChk = false;
    }

    private void passiveUpdateAfterClick(List<TakeSkillPacket> takeSkillPacketArr, int[] usedDiceArr, bool updateLook)
    {
        bool[] effectChk = { false, false, false, false, false, false, false, false, false, false, false, false };
        bool itemActive = false;
        for (int takeSkillArrIdx = 0; takeSkillArrIdx < takeSkillPacketArr.Count; takeSkillArrIdx++)
        {
            for (int passiveItemIdx = 0; passiveItemIdx < 11; passiveItemIdx++)
            { //모든 passive 아이템을 확인해서 takeSkillPacket 수정
                passiveReturn tempPassiveReturn = itemManager.Instance.usePassiveItem(takeSkillPacketArr, takeSkillPacketArr[takeSkillArrIdx], passiveItemIdx, usedDiceArr, 1);

                if (!effectChk[passiveItemIdx])
                {
                    if (tempPassiveReturn.used && updateLook) //만약 적용이 되엇으며 그 결과를 보여줄 경우
                    {
                        itemActive = true;
                        if (tempPassiveReturn.cal != "none")
                        {
                            specialTextManager.GetComponent<ExampleTextManager>().ShowPassiveText(passiveItemIdx, tempPassiveReturn.cal + tempPassiveReturn.val.ToString());
                        }
                        //SoundManager_doremi.Instance.playDoremi(itemUseIdx++);
                        upDownManager.Instance.activePassiveItem(passiveItemIdx);
                        //GameObject temp = Instantiate(passiveEffObj, itemManager.Instance.getItemInventoryPosition(passiveItemIdx), new Quaternion(0, 0, 0, 0)); //사용된 아이템에 대해 effect
                        effectChk[passiveItemIdx] = true;
                    }
                }
            }
        }
        //if (itemActive) SoundManager_Sfx.Instance.playSound(63);
    }

    private void makeHitEffect(int tempTargetIdx)
    {
        //별이랑 원형 이펙트
        for (int i = 0; i < 3; i++)
        {
            GameObject startemp = Instantiate(starEff, battleTargetUI[tempTargetIdx].transform.position, Quaternion.Euler(0, 0, 0)); //사용된 아이템에 대해 effect
            startemp.GetComponent<effMove>().changeSprite();
        }

        for (int i = 0; i < 5; i++) {
            GameObject startemp2 = Instantiate(starEff, battleTargetUI[tempTargetIdx].transform.position, Quaternion.Euler(0, 0, 0)); //사용된 아이템에 대해 effect
            startemp2.GetComponent<effMove>().changeSprite(-999);
        }


        GameObject temp = Instantiate(hitEff, battleTargetUI[tempTargetIdx].transform.position, Quaternion.Euler(0, 0, 0)); //사용된 아이템에 대해 effect
        temp.GetComponent<Animator>().Play("TargetDestroy");
        Instantiate(hitEff, battleTargetUI[tempTargetIdx].transform.position + new Vector3(Random.Range(-15, 15), Random.Range(-15, 15), 0), Quaternion.Euler(0, 0, Random.Range(0, 4) * -90)); //사용된 아이템에 대해 effect
        SoundManager_Sfx.Instance.playSound(Random.Range(8, 11));
    }

    private bool makeCalculateText(bool myTeam, int skillType, int idx, int val, int height)
    {
        if (val == 0) return false;
        else
        {
            if (skillType == 0) { specialTextManager.GetComponent<ExampleTextManager>().printBattleUpgrade(0, myTeam, idx, -1 * val, height); return true; }
            if (skillType == 1) { specialTextManager.GetComponent<ExampleTextManager>().printBattleUpgrade(0, myTeam, idx, val, height); return true; }
            if (skillType == 2) { specialTextManager.GetComponent<ExampleTextManager>().printBattleUpgrade(5, myTeam, idx, -1 * val, height); return true; }
            /*
            if (myTeam) //아군 대상일 경우
            {
                
            }
            else // 적군 대상일 경우
            {
                if (skillType == 0) { specialTextManager.GetComponent<ExampleTextManager>().ShowEnemyTeamDamage(idx, val, height); return true; }
                if (skillType == 1) { specialTextManager.GetComponent<ExampleTextManager>().ShowEnemyTeamHeal(idx, val, height); return true; }
                if (skillType == 2) { specialTextManager.GetComponent<ExampleTextManager>().ShowEnemyTeamAtkUp(idx, val, height); return true; }
            }
            */
        }

        return false;
        //if (takeSkillPacketArr[takeSkillArrIdx].getSkillType() == 0) specialTextManager.GetComponent<ExampleTextManager>().ShowMyTeamDamage(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getVal());
    }


    private void fireMove(int idx) {
        /*
        for (int i=0;i<4;i++)
        {
            if(i != idx) myDiceUI[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.7f);
            else myDiceUI[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
            if (i != i + 4) enemyDiceUI[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.7f);
            else enemyDiceUI[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);
        }
        */
        if (idx == -999) fireObject.transform.position = new Vector3(-234f, 22f, 0f);
        else if (idx < 4) fireObject.transform.position = myDiceUI[idx].transform.position + new Vector3(0, -4, 0);
        else if (idx < 8) fireObject.transform.position = enemyDiceUI[idx - 4].transform.position + new Vector3(0, -4, 0);
    }

    private void battleAlphaControl_My(int skill)
    {
        for (int i = 0; i < 4; i++)
        {
            if (myDiceTake[i] == skill)
            {
                shakeObject(myDiceUI[i]);
                hoverRotateAble(myDiceUI[i], 1, true);
                Material material = myDiceUI[i].GetComponent<SpriteRenderer>().material;
                float curAlpha = material.GetFloat("_Transparency");
                material.SetFloat("_Transparency", 0.0f);
            }
            else {
                hoverRotateAble(myDiceUI[i], 1, false);
                Material material = myDiceUI[i].GetComponent<SpriteRenderer>().material;
                float curAlpha = material.GetFloat("_Transparency");
                material.SetFloat("_Transparency", 0.7f);
            }
        }
    }
    private void battleAlphaControl_Enemy(int skill)
    {
        for (int i = 0; i < 4; i++)
        {
            if (enemyDiceTake[i] == skill)
            {
                shakeObject(enemyDiceUI[i]);
                Material material = enemyDiceUI[i].GetComponent<SpriteRenderer>().material;
                float curAlpha = material.GetFloat("_Transparency");
                material.SetFloat("_Transparency", 0.0f);
            }
            else
            {
                Material material = enemyDiceUI[i].GetComponent<SpriteRenderer>().material;
                float curAlpha = material.GetFloat("_Transparency");
                material.SetFloat("_Transparency", 0.7f);
            }
        }
    }

    //현재 사용하고 있는 아군의 스킬을 정보를 받아온다.
    private SendSkillPacket sendSkillPacketTemp;
    public SendSkillPacket getCurSkillInfo()
    {
        return sendSkillPacketTemp;
    }
    private void takeSkillPacketLastFix(List<TakeSkillPacket> takeSkillPackets) //패시브 등 조건에 따른 추가 효과 세팅
    {
        bool[] check = { false, false, false, false, false };
        for (int i = 0; i < takeSkillPackets.Count; i++)
        {
            if (takeSkillPackets[i].getSkillType() >= 0 && takeSkillPackets[i].getSkillType() <= 4) check[takeSkillPackets[i].getSkillType()] = true;
        }
        for (int i = 0; i < check.Length; i++)
        {
            if (check[i])
            {
                takeSkillPackets.Insert(0, new TakeSkillPacket(0, 0, 0, 1000 + i));
            }
        }
    }

    private int[] textHeight = { 0, 0, 0, 0, 0, 0, 0, 0 };
    private Queue<int> skillResultQueueForAnim = new Queue<int>();  //return -1 : 처리하지 않음. 0 : 타격성공+생존 1 : 사망 2: 회피 3:버프
    private bool battleHitAnimEndChk = false;

    private IEnumerator battleHitAnim(int lastAttack, int lastDead)
    {
        int skillResult = 0;
        int tempTargetIdx = 0;
        bool boomChk = false;

        if (lastAttack >= 0 && lastDead >= 0) { //kill Animation
            KillAnimationManager.Instance.startAnimation(0, getCharacter(lastAttack), getCharacter(lastDead));
            yield return new WaitUntil(() => !KillAnimationManager.Instance.getKillAnimationPlay());
        }

        for (int takeSkillArrIdx = 0; takeSkillArrIdx < takeSkillPacketArr.Count; takeSkillArrIdx++) {
            skillResult = skillResultQueueForAnim.Dequeue();
            if (skillResult < 0) {//처리하지 않는 Animation인 경우 넘어간다.
                continue;
            }

            tempTargetIdx = takeSkillPacketArr[takeSkillArrIdx].getTargetIdx();
            Debug.Log(tempTargetIdx);
            if (tempTargetIdx < 4) //아군이 타겟일 경우
            {
                if (skillResult != 2) //회피가 아닌 경우
                {
                    if (makeCalculateText(true, takeSkillPacketArr[takeSkillArrIdx].getSkillType(), tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getVal(), textHeight[tempTargetIdx])){
                        textHeight[tempTargetIdx]++;
                    }
                }
                else{  //회피
                    specialTextManager.GetComponent<ExampleTextManager>().ShowMyTeamMiss(tempTargetIdx, textHeight[tempTargetIdx]++);
                }

                if (skillResult == 1) {
                    characterDamageMove(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getVal());
                    backGroundObj[4].GetComponent<Animator>().Play("BattleFaint");
                    battleAnimationControl(tempTargetIdx, 2);
                    DeadCharacterUpdate(tempTargetIdx);
                    boomChk = true;
                }
                else
                {
                    //주사위 상태 변화 실행
                    changeDiceState(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getStateChange());

                    if (skillResult == 0)
                    {  //대미지는 주었지만한 경우(현재 버프에 대한 구분이 없어서 추후 수정필요)
                        characterDamageMove(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getVal());
                        backGroundObj[4].GetComponent<Animator>().Play("BattleHit");
                        battleAnimationControl(tempTargetIdx, 1);
                    }
                }
            }
            else
            {
                if (skillResult != 2) {
                    if (makeCalculateText(false, takeSkillPacketArr[takeSkillArrIdx].getSkillType(), tempTargetIdx - 4, takeSkillPacketArr[takeSkillArrIdx].getVal(), textHeight[tempTargetIdx])) textHeight[tempTargetIdx]++;
                }
                else {
                    specialTextManager.GetComponent<ExampleTextManager>().ShowEnemyTeamMiss(tempTargetIdx - 4, textHeight[tempTargetIdx]++);
                }
                if (skillResult == 1) //사망한 경우
                {
                    characterDamageMove(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getVal());
                    backGroundObj[4].GetComponent<Animator>().Play("BattleKill");
                    battleAnimationControl(tempTargetIdx, 2);
                    DeadCharacterUpdate(tempTargetIdx);
                    boomChk = true;
                }
                else
                {
                    changeDiceState(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getStateChange());
                    if (skillResult == 0) { //대미지는 주었지만한 생존한 경우
                        characterDamageMove(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getVal());
                        backGroundObj[4].GetComponent<Animator>().Play("BattleShine");
                        battleAnimationControl(tempTargetIdx, 1);
                    }
                }

            }
            
            
        }


        if (boomChk) SoundManager_Sfx.Instance.playSound(75);

        battleHitAnimEndChk = false;

        updateHp();
        updateMyDiceUI();
        updateEnemyDiceUI();
        updateBattleUI();
    }

    private IEnumerator battlePhase()
    {
        
        clickDice_battlePhase = -999;
        //아직 스킬 애니메이션과의 연동 & 스킬 데미지 연동이 안되어있음.
        if (curPhase == 5)
        {
            int nextDice = 0;
            int nextSkill = -999;
            //아군 스킬 클릭 
            while (nextDice < 4)
            {
                if (winningCheck() != 0) break;
                if (myDiceTake[nextDice] != -999)
                {   //주사위 가장 앞에 있는 주사위 클릭을 위해 받아오고 click 기다리기

                    nextSkill = myDiceTake[nextDice];
                    battleAlphaControl_My(nextSkill);
                    //clickDice_battlePhase = -998;


                    fireMove(nextDice);

                    //yield return new WaitUntil(() => clickDice_battlePhase == nextSkill);
                    battleAlphaControl_My(nextSkill);
                    battleAlphaControl_Enemy(-1);
                    //diceArrowAnimationControl(nextDice, false);
                    //diceArrow[nextDice].GetComponent<Animator>().Play("noArrow");
                    /*
                    이후 passive 아이템 사용을 위해 사용된 주사위를 받아두기 위함.
                    */
                    int usedDiceIdx = 0;
                    int[] usedDiceArr = { -99999, -99999, -99999, -99999 };
                    for (int i = 0; i < 4; i++)
                    {
                        if (myDiceTake[i] == nextSkill)
                        {
                            usedDiceArr[usedDiceIdx] = myDiceNum[i];
                            usedDiceIdx++;
                        }
                    }
                    //스킬이 사용 코드 적히는 부분
                    int skillUseCharacter = nextSkill / 10;
                    int skillUseIdx = nextSkill % 10;
                    Skill curSkill = myCharacter[skillUseCharacter].skillUse(skillUseIdx); //사용하는 스킬에 대한 정보를 받아온다.

                    characterTargetIdx = 0;

                    skillAnimationControl(true, 1, 0, curSkill, skillUseCharacter, -999, skillUseIdx);//타겟팅 전 애니메이션 실행
                    yield return new WaitUntil(() => myCharacterAtkReady[skillUseCharacter] == 2);



                    //타겟이 정해지지 않은 takeSkillPacket 생성.
                    makeMyDice_BattlePhase(nextDice, curSkill.getNeedDiceNum());
                    for (int i = 0; i < clickCharacter.Length; i++) clickCharacter[i] = -999;
                    sendSkillPacketTemp = new SendSkillPacket(skillUseCharacter, myCharacter[skillUseCharacter].getSkillIdx(skillUseIdx), clickCharacter, makeDiceArrToMakePacket);
                    takeSkillPacketArr.Clear();
                    takeSkillPacketArr = myCharacter[skillUseCharacter].doSkill(sendSkillPacketTemp);
                    takeSkillPacketLastFix(takeSkillPacketArr);
                    //skill기반의 takeSkillPacket의 값 얻고 이벤트 보여주기
                    // 활성화 보여주고, 클릭 전 패시브 대상으로 하며

                    StartCoroutine(passiveUpdateBeforClick(takeSkillPacketArr, usedDiceArr, true));
                    yield return new WaitUntil(() => !passiveItemChk);


                    if (AdventureManager.Instance.getTutorial() == 9)
                    {
                        TalkManager.Instance.startTalk(42);
                        yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                        AdventureManager.Instance.setTutorial(10);
                    }


                    bool boomChk = false;
                    chainChk = false;
                    for (int i = 0; i < curSkill.getTargetChance(); i++) { // 해당 스킬이 공격하는 숫자
                        boomChk = false;
                        characterTargetIdx = 0;
                        for (int heightIdx = 0; heightIdx < textHeight.Length; heightIdx++) textHeight[heightIdx] = 0;
                        //SendSkillPacket sendSkillPacketTemp = new SendSkillPacket(skillUseCharacter, myCharacter[skillUseCharacter].getSkillIdx(skillUseIdx), clickCharacter, makeDiceArrToMakePacket);
                        if (curSkill.TargetAuto == 0)
                        {
                            yield return new WaitForSeconds(0.5f);
                        }
                        else
                        {
                            StartCoroutine(clickEnemy_Coroutine(curSkill.getTargetNum(), curSkill.getTargetTeam())); // 클릭 이벤트 시작
                            yield return new WaitUntil(() => (characterTargetIdx == curSkill.getTargetNum())); //필요한 캐릭터만큼 클릭된 경우 click 이벤트 종료!
                        }
                        characterTargetIdx = -999;

                        //스킬에 대한 공격용 Packet 생성
                        sendSkillPacketTemp.addClickCharacter(clickCharacter);
                        takeSkillPacketArr.Clear();
                        takeSkillPacketArr = myCharacter[skillUseCharacter].doSkill(sendSkillPacketTemp);
                        takeSkillPacketLastFix(takeSkillPacketArr);
                        //패시브 아이템들이 적용되는 연출 출력
                        StartCoroutine(passiveUpdateBeforClick(takeSkillPacketArr, usedDiceArr, false));
                        yield return new WaitUntil(() => !passiveItemChk);
                        passiveUpdateAfterClick(takeSkillPacketArr, usedDiceArr, true);

                        int tempTargetIdx;
                        int lastAtk = -1;
                        int lastDead = -1;
                        //만들어진 상호작용 Queue를 기반으로 전투 진행
                        for (int takeSkillArrIdx = 0; takeSkillArrIdx < takeSkillPacketArr.Count; takeSkillArrIdx++)
                        {
                            tempTargetIdx = takeSkillPacketArr[takeSkillArrIdx].getTargetIdx();
                            Debug.Log("target Idx : " + tempTargetIdx.ToString());
                            if (tempTargetIdx < 4) //아군 대상으로 스킬이 들어온 경우
                            {
                                if (myCharacter[tempTargetIdx] != null && myCharacter[tempTargetIdx].getCurState() == 0) //대상 존재시 damage text 출력
                                {
                                    int skillResult = myCharacter[tempTargetIdx].TakeSkillPacket(takeSkillPacketArr[takeSkillArrIdx]);
                                    skillResultQueueForAnim.Enqueue(skillResult);
                                    if (skillResult == 1 && winningCheck() > 0) { lastAtk = skillUseCharacter; lastDead = tempTargetIdx; }
                                }
                                else
                                {
                                    skillResultQueueForAnim.Enqueue(-1); //해당 스킬 결과에 대해서는 처리 하지 않음.
                                }
                            }
                            else // 적군 대상으로 스킬이 들어온 경우
                            {
                                if (enemyCharacter[tempTargetIdx - 4] != null && enemyCharacter[tempTargetIdx - 4].getCurState() == 0) //대상 존재시 damage text 출력
                                {
                                    int skillResult = enemyCharacter[tempTargetIdx - 4].TakeSkillPacket(takeSkillPacketArr[takeSkillArrIdx]);
                                    skillResultQueueForAnim.Enqueue(skillResult);
                                    if (skillResult == 1 && winningCheck() > 0) { lastAtk = skillUseCharacter; lastDead = tempTargetIdx; }
                                }
                                else
                                {
                                    skillResultQueueForAnim.Enqueue(-1); //해당 스킬 결과에 대해서는 처리 하지 않음.
                                }
                            }
                        }
                        
                        battleHitAnimEndChk = true;
                        StartCoroutine(battleHitAnim(lastAtk, lastDead));
                        yield return new WaitUntil(() => !battleHitAnimEndChk);

                        if (chainChk)
                        {
                            chainChk = false;
                            SoundManager_Sfx.Instance.playSound(74);
                        }
                        skillAnimationControl(true, 3, i, curSkill, skillUseCharacter, -999, skillUseIdx);//타겟팅 전 애니메이션 실행
                        yield return new WaitUntil(() => myCharacterAtkReady[skillUseCharacter] == 0);

                        if (winningCheck() != 0) //게임이 승리하여 공격할 적이 더이상 없는 경우
                        {
                            battleTextObj.GetComponent<TextMeshPro>().text = "";
                            break;
                        }

                        if (i + 1 < curSkill.getTargetChance()) { //공격 후에 공격기회가 더 남았으면 다시 뒤로 땡기기
                            skillAnimationControl(true, 1, 0, curSkill, skillUseCharacter, -999, skillUseIdx);//타겟팅 전 애니메이션 실행
                            yield return new WaitUntil(() => myCharacterAtkReady[skillUseCharacter] == 2);
                        }

                        battleTextObj.GetComponent<TextMeshPro>().text = "";
                        if (curSkill.TargetAuto == 0)
                        {
                            yield return new WaitForSeconds(0.5f);
                        }

                    }

                    for (int i = 0; i < 4; i++)
                    {
                        if (myDiceTake[i] == nextSkill)
                        {
                            myDiceChange(i, 0, -999);
                            myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                            diceUIChk[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                            //if (i != 3) diceUIChain[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                        }
                    }
                    updateMyDiceUI();
                    //
                    nextSkill = 0;

                }
                nextDice++;
            }
            yield return new WaitForSeconds(1.0f);
            nextDice = 0;

            //적군 스킬 자동 사용
            while (nextDice < 4)
            {
                if (winningCheck() != 0) break;
                if (enemyDiceTake[nextDice] != -999)
                {   //주사위 가장 앞에 있는 주사위 클릭을 위해 받아오고 click 기다리기
                    //diceArrow[nextDice + 4].GetComponent<Animator>().Play("yesArrow");
                    //diceArrowAnimationControl(nextDice + 4, true);
                    fireMove(nextDice + 4);

                    nextSkill = enemyDiceTake[nextDice];
                    battleAlphaControl_My(-1);
                    battleAlphaControl_Enemy(nextSkill);
                    
                    //스킬이 사용 코드 적히는 부분
                    int skillUseCharacter = nextSkill / 10;
                    int skillUseIdx = nextSkill % 10;
                    Skill curSkill = enemyCharacter[skillUseCharacter].skillUse(skillUseIdx); //사용하는 스킬에 대한 정보를 받아온다.

                    skillAnimationControl(false, 1, 0, curSkill, skillUseCharacter, -999, skillUseIdx);//타겟팅 전 애니메이션 실행
                    yield return new WaitUntil(() => enemyCharacterAtkReady[skillUseCharacter] == 2);
                    yield return new WaitForSeconds(0.2f);

                    jsonDataManager.Instance.meetMonsterSkill(enemyCharacter[skillUseCharacter].getDestiny().DestinyIdx, skillUseIdx);
                    bool boomChk = false;
                    chainChk = false;
                    for (int i = 0; i < curSkill.getTargetChance(); i++)
                    { // 해당 스킬이 공격하는 숫자
                        int[] textHeight = { 0, 0, 0, 0, 0, 0, 0, 0 };
                        makeEnemyClick(curSkill.getTargetNum(), curSkill.getTargetTeam()); // 적군의 공격 대상 만들기

                        //스킬에 대한 공격용 Packet 생성
                        makeEnemyDice_BattlePhase(nextDice, nextDice + curSkill.getNeedDiceNum());
                        sendSkillPacketTemp = new SendSkillPacket(skillUseCharacter, enemyCharacter[skillUseCharacter].getSkillIdx(skillUseIdx), clickCharacter, makeDiceArrToMakePacket);

                        takeSkillPacketArr.Clear();
                        takeSkillPacketArr = enemyCharacter[skillUseCharacter].doSkill(sendSkillPacketTemp);

                        int tempTargetIdx;
                        for (int takeSkillArrIdx = 0; takeSkillArrIdx < takeSkillPacketArr.Count; takeSkillArrIdx++)
                        {

                            tempTargetIdx = takeSkillPacketArr[takeSkillArrIdx].getTargetIdx();

                            if (tempTargetIdx < 4) //아군 대상으로 스킬이 들어온 경우
                            {
                                if (myCharacter[tempTargetIdx] != null && myCharacter[tempTargetIdx].getCurState() == 0) //대상 존재시 damage text 출력
                                {
                                    int skillResult = myCharacter[tempTargetIdx].TakeSkillPacket(takeSkillPacketArr[takeSkillArrIdx]);
                                    skillResultQueueForAnim.Enqueue(skillResult);
                                }
                                else
                                {
                                    skillResultQueueForAnim.Enqueue(-1);
                                }
                            }
                            else // 적군 대상으로 스킬이 들어온 경우
                            {
                                if (enemyCharacter[tempTargetIdx - 4] != null && enemyCharacter[tempTargetIdx - 4].getCurState() == 0) //대상 존재시 damage text 출력
                                {
                                    int skillResult = enemyCharacter[tempTargetIdx - 4].TakeSkillPacket(takeSkillPacketArr[takeSkillArrIdx]);
                                    skillResultQueueForAnim.Enqueue(skillResult);
                                }
                                else
                                {
                                    skillResultQueueForAnim.Enqueue(-1); //해당 스킬 결과에 대해서는 처리 하지 않음.
                                }

                            }

                        }

                        battleHitAnimEndChk = true;
                        StartCoroutine(battleHitAnim(-1, -1));
                        yield return new WaitUntil(() => !battleHitAnimEndChk);


                        if (chainChk)
                        {
                            chainChk = false;
                            SoundManager_Sfx.Instance.playSound(74);
                        }

                        skillAnimationControl(false, 3, i, curSkill, skillUseCharacter, -999, skillUseIdx);//타겟팅 전 애니메이션 실행
                        yield return new WaitUntil(() => enemyCharacterAtkReady[skillUseCharacter] == 0);

                        //공격할 아군이 더 남아있지 않으면 종료
                        if (winningCheck() != 0) break;

                        if (i + 1 < curSkill.getTargetChance())
                        { //공격 후에 공격기회가 더 남았으면 다시 뒤로 땡기기
                            yield return new WaitForSeconds(0.2f);
                            skillAnimationControl(false, 1, 0, curSkill, skillUseCharacter, -999, skillUseIdx);//타겟팅 전 애니메이션 실행
                            yield return new WaitUntil(() => enemyCharacterAtkReady[skillUseCharacter] == 2);
                        }

                    }
                    //diceArrowAnimationControl(nextDice + 4, false);
                    for (int i = 0; i < 4; i++)
                    {
                        if (enemyDiceTake[i] == nextSkill)
                        {
                            enemyDiceChange(i, -999);
                            enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                            /*if (i != 3)
                            {
                                Debug.Log("chain delete " + i.ToString());
                                diceUIChain[i + 3].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                            }*/

                        }
                    }
                    updateEnemyDiceUI();
                    battleAlphaControl_Enemy(-1);
                    //
                    nextSkill = 0;
                    yield return new WaitForSeconds(1.0f);
                }
                nextDice++;
            }
            fireMove(-999);


            for (int i = 0; i < 4; i++)
            {
                myDiceChange(i, 0, -999);
                myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                diceUIChk[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                if (i != 3) diceUIChain[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");

                enemyDiceChange(i, -999);
                enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                diceUIChk[i + 4].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                if (i + 3 != 6) diceUIChain[i + 3].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            }
            nextDice = 0;
            //배틀 끝나서 모두 사용됨.
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    mySkillUsed[i, j] = false;
                    enemySkillUsed[i, j] = false;
                }
            }
            curPhase = 6;
        }

    }

    public void click_BattleSkill_dice(int input)
    {
        if (input < 4)
        {
            clickDice_battlePhase = myDiceTake[input];
        }
        else
        {
            clickDice_battlePhase = enemyDiceTake[input - 4];
        }

    }

    // Character Battle End (Phase 5 - true battle phase)//////

    // End Phase Start(phase 6 - check game finish)//

    private int winningCheck()
    {

        if ((myCharacter[0] == null || myCharacter[0].getCurState() == 2) &&
            (myCharacter[1] == null || myCharacter[1].getCurState() == 2) &&
            (myCharacter[2] == null || myCharacter[2].getCurState() == 2) &&
            (myCharacter[3] == null || myCharacter[3].getCurState() == 2))
        {
            return 2;
        }
        //적군 전멸
        else if ((enemyCharacter[0] == null || enemyCharacter[0].getCurState() == 2) &&
            (enemyCharacter[1] == null || enemyCharacter[1].getCurState() == 2) &&
            (enemyCharacter[2] == null || enemyCharacter[2].getCurState() == 2) &&
            (enemyCharacter[3] == null || enemyCharacter[3].getCurState() == 2))
        {
            return 1;
        }
        return 0;
    }

    private void makeRandomResult()
    {
        int resultType0, resultType1, resultType2;
        int resultIdx0, resultIdx1, resultIdx2;

        resultType0 = Random.Range(0, 3);
        if (resultType0 == 2) resultType0++;
        resultIdx0 = Random.Range(1, itemManager.Instance.getItemListCount(resultType0));
        resultItem[0] = itemManager.Instance.getItem(resultType0, resultIdx0);

        resultType1 = Random.Range(0, 3);
        if (resultType1 == 2) resultType1++;
        resultIdx1 = Random.Range(1, itemManager.Instance.getItemListCount(resultType1));
        if (resultType0 == resultType1 && resultIdx0 == resultIdx1)
        {
            resultIdx1 += 1;
            if (itemManager.Instance.getItemListCount(resultType1) == resultIdx1) resultIdx1 = 1;
        }
        resultItem[1] = itemManager.Instance.getItem(resultType1, resultIdx1);

        resultType2 = Random.Range(0, 3);
        if (resultType2 == 2) resultType2++;
        resultIdx2 = Random.Range(1, itemManager.Instance.getItemListCount(resultType2));
        if ((resultType2 == resultType0 && resultIdx2 == resultIdx0) || (resultType2 == resultType1 && resultIdx2 == resultIdx1))
        {
            resultIdx2 += 1;
            if (itemManager.Instance.getItemListCount(resultType2) == resultIdx2) resultIdx2 = 1;
            if ((resultType2 == resultType0 && resultIdx2 == resultIdx0) || (resultType2 == resultType1 && resultIdx2 == resultIdx1))
            {
                resultIdx2 += 1;
                if (itemManager.Instance.getItemListCount(resultType2) == resultIdx2) resultIdx2 = 1;
            }
        }
        resultItem[2] = itemManager.Instance.getItem(resultType2, resultIdx2);

        resultItemTypeObj[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_itemType_" + typeArr[resultType0]);
        resultItemTypeObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_itemType_" + typeArr[resultType1]);
        resultItemTypeObj[2].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_itemType_" + typeArr[resultType2]);

    }
    string[] typeArr = { "consume", "dice", "equip", "passive", "destiny" };
    int[] typeArr2 = { 78, 79, 80, 81, 82 };
    private void printRandomResult(int i, bool pointOn)
    {
        resultEff[i].transform.position = new Vector3(100f * i - 100f, 0f, 0f);
        resultItemTypeObj[i].transform.position = new Vector3(100f * i - 100, 300f, 0f);
        resultEff[i].GetComponent<Animator>().Play("Eff");
        //if (pointOn) resultObj[i, 0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/battleResultUI/spr_selectUI_board");
        //else
        {
            resultObj[i, 0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/battleResultUI/140/spr_selectUI_board_" + resultItem[i].getRare() + "_140");
        }

        resultObj[i, 1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/" + typeArr[resultItem[i].getType()] + "ItemSprite/spr_item_" + typeArr[resultItem[i].getType()] + "_" + resultItem[i].getItemName());
        resultObj[i, 2].GetComponent<TextMeshPro>().text = resultItem[i].getItemName();
        resultObj[i, 3].GetComponent<TextMeshPro>().text = "<size=80>- "+ TalkManager.Instance.getDesc(typeArr2[resultItem[i].getType()]) + " -</size>" + "\n\n" + resultItem[i].getContent();
    }
    public void pointEnterRandomResult(int i) { resultObj[i, 0].GetComponent<SpriteRenderer>().material.SetFloat("_Radius", 1f); }
    // printRandomResult(i, true);}
    public void pointExitRandomResult(int i) {
        resultObj[i, 0].GetComponent<SpriteRenderer>().material.SetFloat("_Radius", 0f);
    }
    //printRandomResult(i, false); }

    bool bosang_click = false;

    [SerializeField]
    public GameObject infoBtn;

    private IEnumerator endPhase()
    {
        battleBagBtn.GetComponent<CircleCollider2D>().enabled = true;
        itemManager.Instance.flipItemBox_BattleUI(); //켜진 item box 끄기
        yield return new WaitForSeconds(0.2f);
        int result = winningCheck();
        //tutorial용 전투 종료 확인

        //아군 전멸
        if (result == 2 || giveUpChk)
        {
            infoBtn.GetComponent<BoxCollider2D>().enabled = false;
            if (characterInfoOpen) {
                clickCharacterInfoBox();
            }

            itemManager.Instance.endOfBattlePhase();
            //AdventureManager.Instance.loseGame();
            CameraManager.Instance.resultScreenActive(0);
            getLoseChk = true;
            yield return new WaitUntil(() => !(CameraManager.Instance.getLoseScreenActive()));
            getLoseChk = false;

            if (characterInfoOpen) clickCharacterInfoBox();
            //CameraManager.Instance.loseScreenUnActive();
            AdventureManager.Instance.exitBattleCanvas(false); // 게임이 오버되었음을 전달

            for (int i = 0; i < 4; i++)
            {
                enemyDiceState[i] = 0;
                enemyDiceStateAnim[i].Play("0");
                myDiceState[i] = 0;
                myDiceStateAnim[i].Play("0");
            }


            adventureStartChk = false;
            curPhase = 0;

        }
        //적군 전멸
        else if (result == 1)
        {
            infoBtn.GetComponent<BoxCollider2D>().enabled = false;
            if (characterInfoOpen)
            {
                clickCharacterInfoBox();
            }

            if (AdventureManager.Instance.getTutorial() == 2) AdventureManager.Instance.setTutorial(3);
            if (AdventureManager.Instance.getTutorial() == 17) //만약 튜토리얼 중인경우 7번 대화(마녀의 운명 마법 사용)
            {
                TalkManager.Instance.startTalk(9);
                yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
            }
            if (bossPhase == 101 ) //만약 튜토리얼 중인경우 7번 대화(마녀의 운명 마법 사용)
            {
                TalkManager.Instance.startTalk(12);
                yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                bossPhase = 0;
            }

            if (bossPhase == 0)
            {
                itemManager.Instance.endOfBattlePhase();
                TalkManager.Instance.setDescClickLock(true);
                TalkManager.Instance.setDescIdx(92);
                
                
                yield return new WaitForSeconds(0.2f);
                for (int i = 0; i < 4; i++)
                {
                    myHpUI[i].GetComponent<TextMeshPro>().text = "";
                    enemyHpUI[i].GetComponent<TextMeshPro>().text = "";
                }
                    //보상 나와있으면 캐릭터 창 끌수 있도록
                    if (characterInfoOpen) {
                    clickCharacterInfoBox();
                }
                characterInfoOpenAble = false;
                bosang_click = true;
                
                if (AdventureManager.Instance.getTutorial() != 10)
                {
                    //랜덤 아이템 배정하고 출력
                    makeRandomResult();
                    resultItemTemp = 0.0f;
                    resultItemPopChk = true;
                    resultObj_all.transform.position = new Vector3(0f, 0f, resultObj_all.transform.position.z);
                    resultObj_all.GetComponent<Animator>().Play("Change");
                    yield return new WaitUntil(() => !resultItemPopChk); // 튀어오른 아이템이 0에 도달시
                    for (int i = 0; i < 3; i++) printRandomResult(i, false); //eff 시작

                    if (AdventureManager.Instance.getTutorial() == 17)
                    {
                        TalkManager.Instance.startTalk(48);
                        yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                        AdventureManager.Instance.setTutorial(18);
                    }
                    

                    yield return new WaitForSeconds(0.25f); //effect 끝날때까지 대기

                    for(int i=0;i<3;i++) resultObj[i, 0].transform.position = new Vector3(-100 + 100f * i, 0f, resultObj_all.transform.position.z);

                    
                }
                else if (AdventureManager.Instance.getTutorial() == 10) //만약 튜토리얼 중인경우 7번 대화(마녀의 운명 마법 사용)
                {
                    AdventureManager.Instance.setTutorial(11);
                    bosang_click = false;
                }
                
  

                setCurClickSkill(-1);
                deleteSkillCommand();

                //정면 보는 마녀


                updateMoveUI(0);


                yield return new WaitUntil(() => !bosang_click);
                TalkManager.Instance.setDescIdx(0);
                TalkManager.Instance.setDescClickLock(false);
                for (int i = 0; i < 3; i++)
                {
                    resultItemTypeObj[i].transform.position = new Vector3(100f * i - 100f, 300f, 0f);//eff 삭제
                    resultObj[i, 0].transform.position = new Vector3(-100 + 100f * i, 300f,0f);
                    resultEff[i].transform.position = new Vector3(100f * i - 100f, 300f, 0f);//eff 삭제
                }
                    CharacterManager.Instance.character_reset();
                for (int i = 0; i < 4; i++) //캐릭터 원래 위치에 character 넣기
                {
                    if (myCharacter[i] != null && myCharacter[i].getReviveUnit() && myCharacter[i].getCurState() != 0)
                    { //만약 부활캐릭터이면서 해당 캐릭터가 죽은 경우
                        myCharacter[i].setHp(1);
                    }

                    if (myCharacter[i] == null || myCharacter[i].getCurState() != 0) continue;
                    if (myCharacter[i].getCharacter_battle().getOriginIdx() >= 0 && myCharacter[i].getCharacter_battle().getOriginIdx() <= 3)
                    {
                        CharacterManager.Instance.character_battleEnd_deepCopy(myCharacter[i].getCharacter_battle().getOriginIdx(), myCharacter[i]);
                    }
                }

                while (!AdventureManager.Instance.exitBattleCanvas(true))
                {
                    yield return new WaitForSeconds(0.5f);
                }
                characterInfoOpenAble = true;
                for (int i = 0; i < 4; i++)
                {
                    enemyDiceState[i] = 0;
                    enemyDiceStateAnim[i].Play("0");
                    myDiceState[i] = 0;
                    myDiceStateAnim[i].Play("0");
                }
                curPhase = 0;
            }
            else if (bossPhase == 100) //안경 선배가 보스고 1페이즈 인경우
            {
                if (jsonDataManager.Instance.getChapterRead(0, 2) == 0)
                {
                    TalkManager.Instance.startTalk(22);
                    yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
                }
                bossPhase = 0; //보스 페이즈를 0으로 변경
                curPhase = 1;
                setEnemyCharacter(0, 10013);
                hoverOutCharacter(4);
            }


        }
        //전투 지속 필요
        else
        {
            curPhase = 1;
        }
        yield return new WaitForSeconds(0.2f);
    }

    public void click_bosang(int i) //보상 획득
    {
        if (i == -1)
        {
            if (!resultItemPopChk)
            {
                SoundManager_Sfx.Instance.playSound(7);
                resultObj_all.transform.position = new Vector3(0f, 300f, resultObj_all.transform.position.z);
                bosang_click = false;
                
            }
            return;
        }
        int result = itemManager.Instance.getItemResult(resultItem[i].getType(), resultItem[i].getIdx());
        if (result == 0)
        {
            SoundManager_Sfx.Instance.playSound(4);
            resultObj_all.transform.position = new Vector3(0f, 300f, resultObj_all.transform.position.z);
            bosang_click = false;
        }
        else
        {
            fullUI.showFull(0);
            SoundManager_Sfx.Instance.playSound(7);
        }
    }

    
    // End Phase End (phase 6 - check game finish)//




    private bool[] witchSkillUsed = new bool[2];
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

    public static BattleManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }

    void Start()
    {
        resultItemPopChk = false;
        resultItemTemp = 0;
    //캐릭터 정보하고 아이템 창 control을 위한 변수
        characterInfoOpenAble = true;
        getLoseChk = false;
        needJewel[0] = 0;needJewel[1] = 1;needJewel[2] = 3;needJewel[3] = 3;needJewel[4] = 2;needJewel[5] = 3; needJewel[6] = 1;

        for (int i = 0; i < 4; i++)
        {
            myCharacterSwing[i] = 0f;
            enemyCharacterSwing[i] = 0f;
            myCharacterPunch[i] = 0;
            enemyCharacterPunch[i] = 0;
            for (int j = 0; j < 2; j++) {
                myFireObj[i, j] = battleFireObject[i * 2 + j].GetComponent<ParticleSystem>(); myFireObj[i, j].Stop();
                enemyFireObj[i, j] = battleFireObject[i * 2 + j + 8].GetComponent<ParticleSystem>(); enemyFireObj[i, j].Stop();
            }
            myDiceStateAnim[i] = myDiceStateUI[i].GetComponent<Animator>();
            enemyDiceStateAnim[i] = enemyDiceStateUI[i].GetComponent<Animator>();

            myCharacterPosition[i] = myCharacterObjEntityUI[i].transform.position;
            enemyCharacterPosition[i] = enemyCharacterObjEntityUI[i].transform.position;

            myCharacterObjUIAnim[i] = myCharacterObjUI[i].GetComponent<Animator>();
            enemyCharacterObjUIAnim[i] = enemyCharacterObjUI[i].GetComponent<Animator>();

            myCharacterInitPosition[i] = myCharacterObjEntityUI[i].transform.position;
            enemyCharacterInitPosition[i] = enemyCharacterObjEntityUI[i].transform.position;
        }
        battleTextObj.GetComponent<TextMeshPro>().text = "";

        for (int i = 0; i < 3; i++) for (int j = 0; j < 4; j++) resultObj[i, j] = resultObjInit[i * 4 + j];
        // Hp 관련 UI, targeting을 위한 object find 
        for (int i = 0; i < 2; i++) for (int j = 0; j < 4; j++) faceDesc[i, j] = faceDescInit[i * 4 + j];

        //주사위 정보로 먼저 ui 출력
        curSelectInfo = 0;
        diceDescBox.SetActive(true);
        skillDescBox.SetActive(false);
        equipDescBox.SetActive(false);

        curPhase = 0;
        //마녀 능력 임시 배치
        witchPowerState = 1;
    }

    private float[] myCharacterPunch = { 0, 0, 0, 0 };
    private float[] myCharacterSwing = { 0, 0, 0, 0 };
    private float[] enemyCharacterPunch = { 0, 0, 0, 0 };
    private float[] enemyCharacterSwing = { 0, 0, 0, 0 };
    private Vector3[] myCharacterPosition = new Vector3[4];
    private Vector3[] enemyCharacterPosition = new Vector3[4];

    private bool adventureStartChk = false;

    // Update is called once per frame
    private float swingDescVal(float a)
    {
        if (a / 200.0f > 0.0005f)
        {
            return a / 200.0f;
        }
        return 0.0005f;
    }

    private void setCharacterAtkReady(bool team, int idx, int changeVal)
    {
        if (team) { //아군
            myCharacterAtkReady[idx] = changeVal;
        }
        else {
            enemyCharacterAtkReady[idx] = changeVal;
        }
    }

    //diceFullUI, ChracterUI, SkillSelectUI[8], field, witchbody(powerSelect), background, witchbody(default) 
    private float[] moveArrY = { 32f, -23f, 0f, -13f, -475f, 0f, 95f };

    //normal / upper/ under / battle / dicePower
    private float[,] moveConstY = {
        { 45f, -23f, 0f, -13f, -475f, 0f, 132f},
        {6f, -30f, 0f, -18f, -475f, 0f, 115f},
        { 60f, 15f, 0f, 20f, -475f, 0f, 100f},
        { 42f, -26f, 0f,-22f, -475f, 0f, 50f},
        { -130f, -150f, 0f, -90f, -475f, 0f, 10f}
    };
    /*
     * {
        { 45f, -23f, 0f, -13f, -475f, 0f, 132f},
        {6f, -30f, 0f, -18f, -475f, 0f, 115f},
        { 60f, 15f, 0f, 20f, -475f, 0f, 100f},
        { 42f, -26f, 0f,-22f, -475f, 0f, 50f},
        { -130f, -150f, 0f, -90f, -125f, 0f, -250f}
    };
     */

    //field, witchbody(powerSelect), background, witchbody(default), witchface
    float[] backgroundBlackAlpha = { 0f, 0f, 0f, 0f, 0f };
    
    //normal / upper/ under / battle / dicePower
    private float[,] alphaConst = {
        { 0f, 0.7f, 0f, 0.7f, 0.7f},
        { 0f, 0.7f, 0f, 0.7f, 0.7f},
        { 0f, 0.7f, 0f, 0.7f, 0.7f},
        { 0f, 0.7f, 0f, 0.7f, 0.7f},
        { 0.7f, 0f, 0f, 0f, 0f}
    };

    public void updateMoveUI(int idx) {
        for (int i=0;i<moveArrY.Length;i++)
        {
            moveArrY[i] = moveConstY[idx,i];
        }

        updateBlackAlpha(idx);
    }

    public void updateBlackAlpha(int idx) {
        for (int i = 0; i < backgroundBlackAlpha.Length; i++)
        {
            backgroundBlackAlpha[i] = alphaConst[idx, i];
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            specialTextManager.GetComponent<ExampleTextManager>().printTest();
        }
        
    }
    private bool resultItemPopChk = false;
    private float resultItemTemp=0;
    void FixedUpdate()
    {
        
        if (resultItemPopChk) //item 튕기기 중인경우
        {
            for (int i = 0; i < 3; i++)
            {
                resultItemTypeObj[i].transform.rotation = Quaternion.Euler(0, 0, 360f * (resultItemTemp - 2.0f) * (resultItemTemp - 2.0f));
                resultItemTypeObj[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", resultItemTemp / 2.0f);
                if (resultItemTemp < 1f)
                {
                    resultItemTypeObj[i].transform.position = Vector3.Lerp(
                        new Vector3(-100f + 100f * i, 50f, 0f), new Vector3(-100f + 100f * i, -100f, 0f), (1 - resultItemTemp) * (1 - resultItemTemp));
                    resultItemTemp += 0.01f;

                }
                else if (resultItemTemp < 2.0f)
                {
                    resultItemTypeObj[i].transform.position = Vector3.Lerp(
                        new Vector3(-100f + 100f * i, 50f, 0f), new Vector3(-100f + 100f * i, 0f, 0f), (resultItemTemp - 1.0f) * (resultItemTemp - 1.0f));
                    resultItemTemp += 0.01f;
                }
                else {
                    for (int t = 0; t < 10; t++)
                    {
                        GameObject startemp2 = Instantiate(starEff, resultItemTypeObj[i].transform.position, Quaternion.Euler(0, 0, 0)); //사용된 아이템에 대해 effect
                        startemp2.GetComponent<effMove>().changeSprite(-999);
                    }
                    resultItemPopChk = false;
                }
            }
        }
        
        if (adventureStartChk)
        {
            for (int i = 0; i < 4; i++)
            {
                if (myCharacterSwing[i] < 0.0f) myCharacterSwing[i] = 0.0f;
                myCharacterObjEntityUI[i].transform.position = new Vector3(
                        myCharacterPosition[i].x - (10 * myCharacterSwing[i] * Mathf.Sin(Mathf.PI * myCharacterPunch[i])),
                        myCharacterObjEntityUI[i].transform.position.y, myCharacterObjEntityUI[i].transform.position.z);

                myCharacterObjUI[i].transform.rotation = Quaternion.Euler(0, 0, myCharacterSwing[i] * Mathf.Sin(Mathf.PI * myCharacterPunch[i]) * 90);

                if (myCharacterAtkReady[i] == 0) //공격 준비 상태가 아닌경우 
                {
                    myCharacterPunch[i] += 0.05f;
                    if (myCharacterPunch[i] >= 2) myCharacterPunch[i] -= 2.0f;
                    if (myCharacterSwing[i] > 0) myCharacterSwing[i] -= swingDescVal(myCharacterSwing[i]);//0.005f;
                }
                else if (myCharacterAtkReady[i] == 1 || myCharacterAtkReady[i] == 2){ //공격 준비 상태인경우
                    //각도따라 수구리 다르게
                    if (myCharacterPunch[i] <= 0.4f) myCharacterPunch[i] += 0.1f;
                    else if (myCharacterPunch[i] >= 1.5f) myCharacterPunch[i] += 0.1f;
                    else if (myCharacterPunch[i] >= 0.6f) myCharacterPunch[i] -= 0.1f;
                    if (myCharacterPunch[i] >= 2) myCharacterPunch[i] -= 2.0f;

                    if (myCharacterPunch[i] > 0.4f && myCharacterPunch[i] < 0.6f) myCharacterAtkReady[i] = 2; // 당기기 끝난다는 거 체크

                    if (myCharacterSwing[i] < 0.75f) myCharacterSwing[i] += 0.05f; //당기는 정도
                }
                else if (myCharacterAtkReady[i] == 3)
                { //공격 준비 상태인경우
                    //각도따라 수구리 다르게
                    if (myCharacterPunch[i] <= 1.2f) myCharacterPunch[i] += 0.3f;
                    else if (myCharacterPunch[i] >= 1.8f) myCharacterPunch[i] -= 0.3f;
                    else
                    {
                        myCharacterPunch[i] = 1.5f;
                        myCharacterAtkReady[i] = 0;
                    }
                }
            }
            
            for (int i = 0; i < 4; i++)
            {
                if (enemyCharacterSwing[i] < 0.0f) enemyCharacterSwing[i] = 0.0f;
                enemyCharacterObjEntityUI[i].transform.position = new Vector3(
                        enemyCharacterPosition[i].x + (10 * enemyCharacterSwing[i] * Mathf.Sin(Mathf.PI * enemyCharacterPunch[i])),
                        enemyCharacterObjEntityUI[i].transform.position.y, enemyCharacterObjEntityUI[i].transform.position.z);

                enemyCharacterObjUI[i].transform.rotation = Quaternion.Euler(0, 0, enemyCharacterSwing[i] * Mathf.Sin(Mathf.PI * enemyCharacterPunch[i]) * -90);

                if (enemyCharacterAtkReady[i] == 0) //공격 준비 상태가 아닌경우 
                {
                    enemyCharacterPunch[i] += 0.05f;
                    if (enemyCharacterPunch[i] >= 2f) enemyCharacterPunch[i] -= 2.0f;
                    if (enemyCharacterSwing[i] > 0f) enemyCharacterSwing[i] -= swingDescVal(enemyCharacterSwing[i]);//0.005f;
                }
                else if (enemyCharacterAtkReady[i] == 1 || enemyCharacterAtkReady[i] == 2)
                { //공격 준비 상태인경우
                    //각도따라 수구리 다르게
                    if (enemyCharacterPunch[i] <= 0.4f) enemyCharacterPunch[i] += 0.1f;
                    else if (enemyCharacterPunch[i] >= 1.5f) enemyCharacterPunch[i] += 0.1f;
                    else if (enemyCharacterPunch[i] >= 0.6f) enemyCharacterPunch[i] -= 0.1f;
                    if (enemyCharacterPunch[i] >= 2) enemyCharacterPunch[i] -= 2.0f;

                    if (enemyCharacterPunch[i] > 0.4f && enemyCharacterPunch[i] < 0.6f) enemyCharacterAtkReady[i] = 2; // 당기기 끝난다는 거 체크

                    if (enemyCharacterSwing[i] < 0.75f) enemyCharacterSwing[i] += 0.05f; //당기는 정도
                }
                else if (enemyCharacterAtkReady[i] == 3)
                { //공격 준비 상태인경우
                    //각도따라 수구리 다르게
                    if (enemyCharacterPunch[i] <= 1.2f) enemyCharacterPunch[i] += 0.3f;
                    else if (enemyCharacterPunch[i] >= 1.8f) enemyCharacterPunch[i] -= 0.3f;
                    else
                    {
                        enemyCharacterPunch[i] = 1.5f;
                        enemyCharacterAtkReady[i] = 0;
                    }
                }
            }
        }

        moveBattleUI(moveArrY[0], diceFullUI);
        moveBattleUI(moveArrY[1], characterUI);

        moveBattleUI(moveArrY[3], backGroundObj[0]);
        moveBattleUI(moveArrY[4], backGroundObj[1]);
       // moveBattleUI(6, backGroundObj[0]);
        moveBattleUI(moveArrY[6], backGroundObj[3]);

        changeBlackAlpha(backgroundBlackAlpha[0], backGroundObj[0]);
        changeBlackAlpha(backgroundBlackAlpha[1], backGroundObj[1]);
        changeBlackAlpha(backgroundBlackAlpha[3], backGroundObj[3]);
        changeBlackAlpha(backgroundBlackAlpha[4], backGroundObj[4]);

    }

    

    private void changeBlackAlpha(float inputAlpha, GameObject gameObjTemp)
    {
            
        Material material = gameObjTemp.GetComponent<SpriteRenderer>().material;
        float curAlpha = material.GetFloat("_Transparency");
        
        if (Mathf.Abs(curAlpha - inputAlpha) >= 0.05) {
            if (curAlpha < inputAlpha)
            {
                curAlpha += 0.05f;
                material.SetFloat("_Transparency", curAlpha);
            }
            else if (curAlpha > inputAlpha)
            {
                curAlpha -= 0.05f;
                material.SetFloat("_Transparency", curAlpha);
            } 
        }
        else if(curAlpha != inputAlpha)
        {
            material.SetFloat("_Transparency", inputAlpha);
        }
    }

    private void moveBattleUI(float inputY, GameObject gameObjTemp) {
        Vector3 destination = new Vector3(gameObjTemp.transform.position.x, inputY, 0);
        float termY = 0.2f;
        if (gameObjTemp.transform.position.y < inputY)
        {
            termY *= -1;

            if (gameObjTemp.transform.position.y < inputY + termY)
            {
                gameObjTemp.transform.position = Vector3.Lerp(gameObjTemp.transform.position, destination, 0.05f);
            }
            else {
                gameObjTemp.transform.position = destination;
            }
        }
        else
        {
            if (gameObjTemp.transform.position.y > inputY + termY)
            {
                gameObjTemp.transform.position = Vector3.Lerp(gameObjTemp.transform.position, destination, 0.05f);
            }
            else {
                gameObjTemp.transform.position = destination;
            }
        }
    }



    private void characterDamageMove(int idx, int damage)
    {
        Debug.Log(idx.ToString() + ":::" + damage.ToString());
        if (damage < 0)
        {
            Debug.Log("Error Damage : " + damage.ToString());
            damage = 0;
        }
        if (damage > 1000) damage = 1000;
        float temp = Mathf.Sqrt(damage / 1000.0f);
        if(damage == 0) temp = 0; 
        if (idx < 4)
        {
            myCharacterPunch[idx] = 0;
            myCharacterSwing[idx] = temp;
        }
        else {
            enemyCharacterPunch[idx-4] = 0;
            enemyCharacterSwing[idx - 4] = temp;
        }

        Debug.Log("shakeAmount");
        if(idx < 4) CameraManager.Instance.attackShakeStart(Mathf.Sqrt(damage));
        else CameraManager.Instance.attackShakeStart(Mathf.Sqrt(damage) * -1);


        makeHitEffect(idx);

        //CameraManager.Instance.VibrateForeTime(0.1f, temp * 5);//데미지만큼 더 흔들리게

    }

    public void startBattle_fromAdventure()
    {
        giveUpChk = true; //전투 시작시에는 항복 꺼두기
        useGiveUpBtn(); 
        
        for (int i = 0; i < 8; i++) {
           // diceArrowAnimationControl(i, false);
        }
        for (int i = 0; i < 4; i++)
        {
            myCharacterPunch[i] = 0f;
            myCharacterSwing[i] = 0f;
            enemyCharacterPunch[i] = 0f;
            enemyCharacterSwing[i] = 0f;

            changeDiceState(i, 0);
            changeDiceState(i+4, 0);
            myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        adventureStartChk = true;
        StartCoroutine(startPhaseManage());
    }

    private IEnumerator makeDark(GameObject gameobj, float alphaVal)
    {

        if (alphaVal == 0.7f)
        {
            currentLightUI++;
            Material material = gameobj.GetComponent<SpriteRenderer>().material;
            float curAlpha = material.GetFloat("_Transparency");

            while (curAlpha < alphaVal)
            {
                material.SetFloat("_Transparency", curAlpha);
                curAlpha += 0.05f;
                yield return new WaitForSeconds(0.01f);
            }
            currentLightUI--;
        }
    }


    private IEnumerator makeBright(GameObject gameobj, float alphaVal)
    {
        if (true)//alphaVal == 0.0f)
        {
            currentLightUI++;
            Material material = gameobj.GetComponent<SpriteRenderer>().material;
            float curAlpha = material.GetFloat("_Transparency");

            while (curAlpha > alphaVal)
            {
                material.SetFloat("_Transparency", curAlpha);
                curAlpha -= 0.05f;
                yield return new WaitForSeconds(0.01f);
            }
            currentLightUI--;
        }
    }
    /*
   

    private void clearBattle()
    {
        for (int i = 0; i < 4; i++)
        {
            myCharacter[i] = null;
            enemyCharacter[i] = null;
            myDice[i] = null;
            enemyDice[i] = null;
            myDiceNum[i] = -999; enemyDiceNum[i] = -999;
            myDiceTake[i] = -999; enemyDiceTake[i] = -999;
            mySkillUsed[i, 0] = false; enemySkillUsed[i, 1] = false;
        }
        curPhase = -1;
        witchSkillUsed[0] = false;
        witchSkillUsed[1] = false;
        chooseDice = null;

    }
    */
    [SerializeField]private GameObject[] battleTargetUI = new GameObject[8];

    private void updateInfoUIFaceUpdate() //battle ui에서 얼굴 업데이트
    {
        for (int i = 0; i < 4; i++)
        {
            if (myCharacter[i] != null && myCharacter[i].getCurState() == 0)
            {
                if (Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + myCharacter[i].getName() + "_face") != null)
                {
                    faceDesc[0, i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + myCharacter[i].getName() + "_face");
                }
                else { faceDesc[0, i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_noImage_face"); }
            }
            else {
                faceDesc[0, i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            }

            if (enemyCharacter[i] != null && enemyCharacter[i].getCurState() == 0)
            {
                faceDesc[1, i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_enemy_face");
            }
            else
            {
                faceDesc[1, i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            }
        }
    }
 
    public void startBattlePhase()
    {
        readyBattleChk = false;
        if (AdventureManager.Instance.getTutorial() == 1 || AdventureManager.Instance.getTutorial() == 2)
        {
            backGroundObj[3].GetComponent<Animator>().Play("Empty");
            backGroundObj[4].GetComponent<Animator>().Play("Empty");
        }
        else {
            backGroundObj[3].GetComponent<Animator>().Play("GrinIdle");
            backGroundObj[4].GetComponent<Animator>().Play("eyeIdle");
        }
        infoBtn.GetComponent<BoxCollider2D>().enabled = true;
        //선택된 주사위 이미지 초기화
        chooseDiceObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        
        //UI test
        for (int i = 0; i < 4; i++)
        {
            myDiceTake[i] = -999;
            enemyDiceTake[i] = -999;
            mySkillUsed[i, 0] = false;
            mySkillUsed[i, 1] = false;
            enemySkillUsed[i, 0] = false;
            enemySkillUsed[i, 1] = false;
            myDiceNum[i] = -999;
            enemyDiceNum[i] = -999;
            //적군
            //enemyCharacter[i] = CharacterManager.Instance.getCharacter(false, i); //현재는 null인경우로 체크하는데 나중에 null말고 빈값을 주어야함.
            if (!CharacterManager.Instance.character_deepCopy(ref enemyCharacter[i], CharacterManager.Instance.getCharacter(false, i)))
            {
                enemyCharacter[i] = null;
                enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            }

            if (enemyCharacter[i] != null && enemyCharacter[i].getCurState() == 0)
            {
                
                enemyDice[i] = new Dice();
                enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            }
            else
            {
                enemyDice[i] = null;
                enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            }
            //어군
            //myCharacter[i] = CharacterManager.Instance.getCharacter(true, i);
            if(!CharacterManager.Instance.character_deepCopy(ref myCharacter[i], CharacterManager.Instance.getCharacter(true, i))){
                myCharacter[i] = null;
                myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            }

            if (myCharacter[i] != null && myCharacter[i].getCurState() == 0)
            {
                myCharacter[i].getCharacter_battle().setOriginIdx(i); //돌아갈때 원래 위치 저장하기 위함
                myDice[i] = new Dice(myCharacter[i].getDiceObj());
                myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            }
            else
            {
                myDice[i] = null;
                myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            }

        }

        //아군이 선택할 스킬들에 대하여 이미지 업데이트
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                if (myCharacter[i] != null && myCharacter[i].getCurState() == 0)
                {
                    if (Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + myCharacter[i].getSkillName(j)) != null)
                    {
                        upDownManager.Instance.skillIconUpdate(i * 2 + j, myCharacter[i].getSkillName(j));
                    }
                    else
                    {
                        upDownManager.Instance.skillIconUpdate(i * 2 + j, "noImage");
                    }
                }
                else
                {
                    upDownManager.Instance.skillIconUpdate(i * 2 + j, "none2");
                }
            }
        }


        //배틀시 타겟에 대한 UI 비활성
        for (int i=0;i<8;i++)
        {
            Material material = battleTargetUI[i].GetComponent<SpriteRenderer>().material;
            material.SetFloat("_Transparency", 0.0f);
            battleTargetUI[i].SetActive(false);
        }

        //animator 적용
        for (int i=0;i<4;i++)
        {
            if (myCharacter[i] == null || myCharacter[i].getCurState() == 2)
            {
                //추후 null로 바꿀것
                myCharacterObjUIAnim[i].runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/animator_noneCharacter");
                myCharacterShadowObjUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");

            }
            else
            {
                string temp = myCharacter[i].getDestiny().getName();
                myCharacterObjUIAnim[i].runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + temp + "/animator_" + temp);
                myCharacterShadowObjUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/spr_character_shadow_" + myCharacter[i].getShadow().ToString());
            }
            
            if (enemyCharacter[i] == null || enemyCharacter[i].getCurState() == 2)
            {
                //추후 null로 바꿀것
                enemyCharacterObjUIAnim[i].runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/animator_noneCharacter");
                enemyCharacterShadowObjUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            }
            else
            {
                string temp = enemyCharacter[i].getDestiny().getName();
                enemyCharacterObjUIAnim[i].runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + temp + "/animator_" + temp);
                enemyCharacterShadowObjUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/spr_character_shadow_" + enemyCharacter[i].getShadow().ToString());
                if (
                    (jsonDataManager.Instance.getChapterRead(0, 2) == 0 && (enemyCharacter[i].getDestiny().getDestinyIdx() == 10012 || enemyCharacter[i].getDestiny().getDestinyIdx() == 10003 || enemyCharacter[i].getDestiny().getDestinyIdx() == 10004))
                ){
                    enemyCharacterObjUIAnim[i].runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + temp + "/animator_" + temp  +"_2");
                }
                
            }

        }
        for (int i = 0; i < 4; i++)
        {
            myCharacterObjUIAnim[i].Play("Idle");
            enemyCharacterObjUIAnim[i].Play("Idle");
        }

        witchPowerObj[0].SetActive(false);
        witchPowerObj[1].SetActive(false);
        witchPowerObj[2].SetActive(false);
        witchPowerSelectObjEntity.SetActive(false);
        updateHp();
        InitSetOfEnemySkill();
        updateInfoUIFaceUpdate();
        curPhase = 1;
        
    }
    public void setEnemyCharacter(int placeIdx, int characterIdx)
    {
        CharacterManager.Instance.emptyEnemyCharacter(placeIdx);

        CharacterManager.Instance.setCharacter(placeIdx, characterIdx); //캐릭터 세팅
        CharacterManager.Instance.character_deepCopy(ref enemyCharacter[placeIdx], CharacterManager.Instance.getCharacter(false, placeIdx));

        enemyDice[placeIdx] = new Dice();
        enemyDiceUI[placeIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");

        //캐릭터 세팅을 반영
        string temp = enemyCharacter[placeIdx].getDestiny().getName();
        enemyCharacterObjUIAnim[placeIdx].runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + temp + "/animator_" + temp);
        enemyCharacterShadowObjUI[placeIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/spr_character_shadow_" + enemyCharacter[placeIdx].getShadow().ToString());

        enemyCharacterObjUIAnim[placeIdx].Play("Create");

        updateHp();
        InitSetOfEnemySkill();
        updateInfoUIFaceUpdate();
    }
   
}
