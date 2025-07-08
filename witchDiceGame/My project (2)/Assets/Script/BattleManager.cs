using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public int chooseDiceIdx;

    //선공 팀 구분
    public int firstAttackTeam = 1;


    //타겟팅을 위한 시스템
    public int clickState = 0;
    public int clickMonster = -1;
    public int clickSelf = -1;

    //phase 흐름을 위한 시스템
    private int battlePhaseState = 0;


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
    public GameObject[] diceUIChk = new GameObject[8];
    private GameObject[] diceUIChain = new GameObject[6];

    private Animator[] myDiceStateAnim = new Animator[4];
    private Animator[] enemyDiceStateAnim = new Animator[4];

    private GameObject characterUI;
    public GameObject diceFullUI;
    private GameObject[] diceArrowSet = new GameObject[8];

    private GameObject[] myCharacterObjUI = new GameObject[4];
    private Animator[] myCharacterObjUIAnim = new Animator[4];
    private GameObject[] myCharacterShadowObjUI = new GameObject[4];
    private GameObject[] myCharacterObjEntityUI = new GameObject[4];
    private ParticleSystem[,] myFireObj = new ParticleSystem[4, 2];

    private GameObject[] enemyCharacterObjUI = new GameObject[4];
    private Animator[] enemyCharacterObjUIAnim = new Animator[4];
    private GameObject[] enemyCharacterShadowObjUI = new GameObject[4];
    private GameObject[] enemyCharacterObjEntityUI = new GameObject[4];
    private ParticleSystem[,] enemyFireObj = new ParticleSystem[4, 2];

    private int[] myCharacterAtkReady = { 0,0,0,0};
    private int[] enemyCharacterAtkReady = { 0, 0, 0, 0 };


    // 타겟팅시 일시정지를 위한 코루틴 저장함수.
    private IEnumerator battleTimer = null;

    private GameObject resultObj_all;
    private GameObject resultExitBtn;
    private GameObject[,] resultObj = new GameObject[3,4];
    private Item[] resultItem = new Item[3];

    //phase버튼 누를수 있는지
    private bool clickAble = true;
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

    private GameObject[] witchPowerObj = new GameObject[3];
    private GameObject[] backGroundObj = new GameObject[5];
    
    private int[] clickedDice = new int[2];
    
    // 적군이 주사위를 보고 스킬을 배치하게 하기 위한 변수들//
    Skill[] enemySkill = new Skill[8];
    int[] enemySkillDiceNum = new int[8];
    int[,] enemySkillDiceVal = new int[8, 4];


    //주사위 밑에 HP UI
    private GameObject[] myHpUI = new GameObject[4];
    private GameObject[] enemyHpUI = new GameObject[4];

    //전투 위에 뜨는 밸류(공격전에 몇 들어가는 지 보여주는 거)
    private GameObject battleTextObj;

    GameObject battleDescBox;
    GameObject battleDescBoxName;
    GameObject battleDescBoxInfo;
    GameObject battleDescBoxCharacter;
    GameObject[,] faceDesc = new GameObject[2, 4];

    GameObject diceDescBox;
    GameObject[] diceDesc = new GameObject[6];

    private int[] myDiceState = { 0, 0, 0, 0 };
    private int[] enemyDiceState = { 0, 0, 0, 0 };

    //스킬 설명을 위해 준비된 칸
    GameObject skillDescBox;
    GameObject[] skillDescBox_title = new GameObject[2];
    GameObject[] skillDescBox_info = new GameObject[2];
    GameObject[] skillDescBox_image = new GameObject[2];
    GameObject[,] skillDescBox_dice = new GameObject[2, 4];

    GameObject equipDescBox;
    GameObject[] equipDescBox_title = new GameObject[2];
    GameObject[] equipDescBox_info = new GameObject[2];
    GameObject[] equipDescBox_image = new GameObject[2];


    public Character getCharacter(int a) {
        if(a<4) return myCharacter[a];
        a -= 4;
        return enemyCharacter[a];
    }
    public int getDiceNum(int a)
    {
        if (a < 4) return myDiceNum[a];
        a -= 4;
        return enemyDiceNum[a];
    }

    int curSelectInfo = 0;
    int hoverCharacterIdx = -1;

    int bossPhase = 0;
    public void changeBossPhase(int a)
    {
        if(a == 100) bossPhase = a; // 부엉이 보스인 경우.(2페이즈)
    }
    private void myDiceChange(int idx, int characterIdx, int skillIdx)
    {
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
            myFireObj[characterIdx, skillIdx].Play(true);
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
            enemyFireObj[(skillIdx % 4), (skillIdx / 4)].Play(true);
        }
    }
    private void drawSkill(Character character)
    {
        for (int skillIdx = 0; skillIdx < 2; skillIdx++)
        {
            Skill thisSkill = character.skillUse(skillIdx);
            if (jsonDataManager.Instance.getMonsterSkill(character.getDestiny().DestinyIdx, skillIdx)) // 만난적있는 지 확인
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
                    skillDescBox_dice[skillIdx, diceIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_" + thisSkill.getNeedDice(diceIdx).ToString());
                }
            }
            else {
                skillDescBox_image[skillIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_noImage");
                skillDescBox_info[skillIdx].GetComponent<TextMeshPro>().text = "아직 몬스터의 스킬을 본적이 없습니다.";
                skillDescBox_title[skillIdx].GetComponent<TextMeshPro>().text = "Not Found";
                for (int diceIdx = 0; diceIdx < 4; diceIdx++)
                {
                    //만난적 없더라도 스킬 대처는 할 수 있도록
                    skillDescBox_dice[skillIdx, diceIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_" + thisSkill.getNeedDice(diceIdx).ToString());
                }
            }
        }
    }
    private void drawDice(Character character)
    {
        for (int diceIdx =0;diceIdx<6;diceIdx++)
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
        return  character.getPhyAtk() + character.getCharacter_battle().getAtk();
    }

    //전투 ui 에 대한 함수들 모음 updateBattleUI()로 지속적으로 업데이트 해줄것 
    #region
    private void writeBattleInfo(Character character)
    {
        battleDescBoxCharacter.GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + character.getName() + "/animator_" + character.getName());
        battleDescBoxName.GetComponent<TextMeshPro>().text = character.getName();
        battleDescBoxInfo.GetComponent<TextMeshPro>().text =
            character.getHp() + "/" + character.getMaxHp() + "( +" + character.getCharacter_battle().getArmor() + " )\n"
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
            if(!(myCharacter[hoverCharacterIdx] == null && myCharacter[hoverCharacterIdx].getCurState() != 0))
            {
                hoverCharacterIdx = -1;
            }
        }
        if (hoverCharacterIdx >= 4 && hoverCharacterIdx < 8)
        {
            if (!(enemyCharacter[hoverCharacterIdx-4] == null && enemyCharacter[hoverCharacterIdx-4].getCurState() != 0))
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
            if(hoverCharacterIdx < 4 && hoverCharacterIdx >=0 )writeBattleInfo(myCharacter[hoverCharacterIdx]);
            if (hoverCharacterIdx < 8 && hoverCharacterIdx >= 4) writeBattleInfo(enemyCharacter[hoverCharacterIdx-4]);
        }
        else clickBattleUIInfo(curSelectInfo); //선택된 캐릭터도, 선택된 ui도 있는 경우
    }
    public void clickCharacterInfoBox()
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
        for (int i=0;i<4;i++)
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

    public void clickSkillDesc(int i) //전투 ui에서 캐릭터 얼굴에 가져다댄 후 정보 출력
    {
        if (i >= 0 && i < 4)
        {
            if (myCharacter[i] != null && myCharacter[i].getCurState() == 0)
            {
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
                makeEmptyBattleInfoBox();
            }
        }
        else if (i >= 4 && i < 8)
        {
            i -= 4;
            if (enemyCharacter[i] != null && enemyCharacter[i].getCurState() == 0)
            {
                
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
                makeEmptyBattleInfoBox();
            }
        }
        else
        {
            makeEmptyBattleInfoBox();
        }
    }

    #endregion  

    private void updateHp()
    {
        for (int i=0;i<4;i++)
        {
            myHpUI[i].GetComponent<TextMeshPro>().text = "";
            
            
            if (myCharacter[i] != null) {
                myHpUI[i].GetComponent<TextMeshPro>().text = myCharacter[i].getHp().ToString();
            }

            if (enemyHpUI[i] != null)
            {
                enemyHpUI[i].GetComponent<TextMeshPro>().text = "";
                if (enemyCharacter[i] != null)
                {
                    enemyHpUI[i].GetComponent<TextMeshPro>().text = enemyCharacter[i].getHp().ToString();
                }
            }
            else
            {
                Debug.Log(i.ToString() + " / wtf where is it?!");
            }
        }
        updateInfoUIFaceUpdate();
    }
    private void InitSetOfEnemySkill() //추후 적군 스킬 자동 발사를 위해 스킬을 미리 받아둔다.
    {

        for (int i=0;i<4;i++)
        {
            for (int j=0;j<2;j++)
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
        for (int i=0;i<4;i++)
        {
            if(enemyCharacter[i] != null && enemyCharacter[i].getCurState() == 0)
            {
                liveCharacterList.Add(i);
            }
        }
        Debug.Log("add skill List");
        for (int i=0;i<8;i++)
        {
            if (enemySkillDiceNum[i] != -999)
            {
                
                Debug.Log(i);
                liveSkillList.Add(i); ;
            }
        }

        for (int skillIdx0=liveSkillList.Count-1; skillIdx0>=0;skillIdx0--)
        {
            //특수 변수 확인
            int characterIdxTemp = liveSkillList[skillIdx0] % 4;
            int skillIdxTemp = liveSkillList[skillIdx0] / 4;
            Debug.Log("skill Special Chk");
            Debug.Log(characterIdxTemp + " / " + skillIdxTemp);
            //만약 special한 공격이고(스택사용)
            if (enemyCharacter[characterIdxTemp].getCharacter_battle().getSpecialVal() != enemyCharacter[characterIdxTemp].skillUse(skillIdxTemp).getSpecialVal()) //만약 조건하고 다른경우 건너뛴다.
            {
                continue;
            }

            int skillIdx = liveSkillList[skillIdx0];
            for (int diceIdx=0; diceIdx <= liveCharacterList.Count - enemySkillDiceNum[skillIdx]; diceIdx++)
            {
                
                //필요 주사위가 1칸인 경우
                if (enemySkillDiceNum[skillIdx] == 1)
                {
                    if (enemyDiceTake[liveCharacterList[diceIdx]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 0], enemyDiceNum[liveCharacterList[diceIdx]])){ // 첫번쨰주사위가 겹치는 경우
                        enemyDiceChange(liveCharacterList[diceIdx], skillIdx);
                        liveCharacterList.RemoveAt(diceIdx);
                        break;
                    }

                }
                //필요 주사위가 2칸인 경우
                else if (enemySkillDiceNum[skillIdx] == 2)
                {
                    if (enemyDiceTake[liveCharacterList[diceIdx]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 0], enemyDiceNum[liveCharacterList[diceIdx]]) &&
                        enemyDiceTake[liveCharacterList[diceIdx+1]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 1], enemyDiceNum[liveCharacterList[diceIdx + 1]]))
                    { // 첫번쨰주사위가 겹치는 경우
                        enemyDiceChange(liveCharacterList[diceIdx], skillIdx);
                        enemyDiceChange(liveCharacterList[diceIdx+1], skillIdx);
                        //liveCharacterList.RemoveAt(diceIdx);
                        //liveCharacterList.RemoveAt(diceIdx);
                        break;
                    }

                }
                else if (enemySkillDiceNum[skillIdx] == 3) //필요 주사위가 3칸인 경우
                {
                    if (enemyDiceTake[liveCharacterList[diceIdx]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 0], enemyDiceNum[liveCharacterList[diceIdx]]) &&
                        enemyDiceTake[liveCharacterList[diceIdx+1]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 1], enemyDiceNum[liveCharacterList[diceIdx + 1]]) &&
                        enemyDiceTake[liveCharacterList[diceIdx+2]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 2], enemyDiceNum[liveCharacterList[diceIdx + 2]]))
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
                        enemyDiceTake[liveCharacterList[diceIdx+1]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 1], enemyDiceNum[liveCharacterList[diceIdx + 1]]) &&
                        enemyDiceTake[liveCharacterList[diceIdx+2]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 2], enemyDiceNum[liveCharacterList[diceIdx + 2]]) &&
                        enemyDiceTake[liveCharacterList[diceIdx+3]] == -999 && condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 3], enemyDiceNum[liveCharacterList[diceIdx + 3]]))
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
    private bool MakeMyAttackSet(int characterIdx, int skillSelIdx, int selDiceIdx)
    {
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
                myDiceChange(liveCharacterList[0], characterIdx ,skillSelIdx);
                return true;
            }
        }
                //필요 주사위가 2칸인 경우
        else if (skill.getNeedDiceNum() == 2)
        {
            if (myDiceTake[liveCharacterList[0]] == -999 && condition_diceSkillCheck(skill.getNeedDice(0), myDiceNum[liveCharacterList[0]]) &&
                myDiceTake[liveCharacterList[1]] == -999 && condition_diceSkillCheck(skill.getNeedDice(1), myDiceNum[liveCharacterList[1]]))
            { // 첫번쨰주사위가 겹치는 경우
                myDiceChange(liveCharacterList[0], characterIdx, skillSelIdx);
                myDiceChange(liveCharacterList[1], characterIdx, skillSelIdx);
                return true;
            }

        }
        else if (skill.getNeedDiceNum() == 3)
        {
            if (myDiceTake[liveCharacterList[0]] == -999 && condition_diceSkillCheck(skill.getNeedDice(0), myDiceNum[liveCharacterList[0]]) &&
                myDiceTake[liveCharacterList[1]] == -999 && condition_diceSkillCheck(skill.getNeedDice(1), myDiceNum[liveCharacterList[1]]) &&
                myDiceTake[liveCharacterList[2]] == -999 && condition_diceSkillCheck(skill.getNeedDice(2), myDiceNum[liveCharacterList[2]]))
            { // 첫번쨰주사위가 겹치는 경우
                myDiceChange(liveCharacterList[0], characterIdx, skillSelIdx);
                myDiceChange(liveCharacterList[1], characterIdx, skillSelIdx);
                myDiceChange(liveCharacterList[2], characterIdx, skillSelIdx);
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
                myDiceChange(liveCharacterList[0], characterIdx, skillSelIdx);
                myDiceChange(liveCharacterList[1], characterIdx, skillSelIdx);
                myDiceChange(liveCharacterList[2], characterIdx, skillSelIdx);
                myDiceChange(liveCharacterList[3], characterIdx, skillSelIdx);
                return true;
            }
        }

        return false;
    }


    public void click_dice(int diceIdx)
    {
        if (currentLightUI == 0 && currentMoveUI == 0)
        {
            if (curPhase == 2) click_witchPower_Dice(diceIdx);
            else if (curPhase == 3)
            {
                if (diceIdx >= 0 && diceIdx < 4) click_characterSkill_Dice(diceIdx); //아군 스킬 배정용
                else if (diceIdx >= 4 && diceIdx < 8) click_enemySkill_Dice(diceIdx);
            }
            else if (curPhase == 5) {
                if (diceIdx >= 0 && diceIdx < 4)  click_BattleSkill_dice(diceIdx); //아군 스킬 사용
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
        if(witchPowerMoveState != -1) witchPowerMoveState = -1; //마녀 스킬 선책이었을 경우.

        currentMoveUI--;
    }
    private IEnumerator MoveUI(GameObject gameObjTemp, float inputY, float delayTemp)
    {
        currentMoveUI++;
        yield return new WaitForSeconds(delayTemp);
        Debug.Log("MoveUI Start");
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
        if (witchPowerMoveState != -1) witchPowerMoveState = -1; //마녀 스킬 선책이었을 경우.
        currentMoveUI--;
    }

    //스킬 선택 시 주사위UI를 업데이트 하는 함수
    void updateMyDiceUI()
    {
        int curSkillVal = -999;
        int startIdx = -999;
        int endIdx = -999;

        //초기화
        for (int i=0;i<4;i++)
        {
            if(i<3)diceUIChain[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
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
            if (i < 7) diceUIChain[i-1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
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
                        updateDiceUI_draw_chain(curSkillVal / 10, curSkillVal % 10, startIdx-1, endIdx-1); //이전 기반으로 chain 걸기 (chain은 6개자리 사이즈를 사용하므로 1씩 빼주었다)
                        curSkillVal = enemyDiceTake[i - 4];
                        updateDiceUI_draw(curSkillVal / 10, curSkillVal % 10, i, true); //스타트 그리기
                        startIdx = i;
                    }
                    endIdx = i;   //end 업데이트
                }

            }
            else // 시작점을 찾고 있는 경우
            {
                if (enemyDiceTake[i-4] != -999) // 해당 주사위가 빈칸이 아니면
                {
                    curSkillVal = enemyDiceTake[i - 4];
                    updateDiceUI_draw(curSkillVal / 10, curSkillVal % 10, i, true); //스타트 그리기
                    startIdx = i; endIdx = i;
                }
            }

        }
        if (curSkillVal != -999) //끝에 도달했지만 chain이 필요한 경우
        {
            updateDiceUI_draw_chain(curSkillVal / 10, curSkillVal % 10, startIdx-1, endIdx-1); //이전 기반으로 chain 걸기 chain은 6개자리 사이즈를 사용하므로 1씩 빼주었다)
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
        Debug.Log(diceStartIdx.ToString() + " " + diceEndIdx.ToString());

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
    private IEnumerator phase_Manage_Coroutine()
    {
        Start_Battle_Phase();
        itemManager.Instance.enterBattlePhase();
        Debug.Log("Start Phase Test! curPhase is " + curPhase.ToString());
        //curPhase = 2;
        while (true)
        {
            yield return new WaitUntil(() => curPhase == 1 && currentLightUI == 0 && currentMoveUI == 0);
            Debug.Log("DiceThrow Test! curPhase is " + curPhase.ToString());
            StartCoroutine(DiceThrowPhase_Coroutine());
            yield return new WaitUntil(() => curPhase == 2 && currentLightUI == 0 && currentMoveUI == 0);
            Debug.Log("WitchPower Test! curPhase is " + curPhase.ToString());
            StartCoroutine(witchPowerPhase_Coroutine());
            yield return new WaitUntil(() => curPhase == 3 && currentLightUI == 0 && currentMoveUI == 0);
            
            StartCoroutine(skillSelectPhase_Coroutine());
            yield return new WaitUntil(() => curPhase == 4 && currentLightUI == 0 && currentMoveUI == 0);
            StartCoroutine(moveToBattlePhase_Coroutine());
            yield return new WaitUntil(() => curPhase == 5 && currentLightUI == 0 && currentMoveUI == 0);
            StartCoroutine(BattlePhase_Coroutine());
            yield return new WaitUntil(() => curPhase == 6 && currentLightUI == 0 && currentMoveUI == 0);
            StartCoroutine(EndPhase_Coroutine());
            yield return new WaitUntil(() => curPhase != 6 && currentLightUI == 0 && currentMoveUI == 0);
            if (curPhase != 1) break;
        }
    }



    //DiceThrow Phase  Start (phase 1- dice throw start)//
    private IEnumerator DiceThrowPhase_Coroutine()
    {
        //주사위 굴리기 UI(ui 초기화)
        StartCoroutine(MoveUI(characterUI, -75.0f));
        StartCoroutine(MoveUI(diceFullUI, -58.0f));

        for (int i=0;i<4;i++)
        {
            StartCoroutine(makeDark(myCharacterObjUI[i], 0.7f));
            StartCoroutine(makeDark(enemyCharacterObjUI[i], 0.7f));
        }

        StartCoroutine(MoveUI(backGroundObj[0], -78.0f)); // 78f : skillSelect  62f: battle
        
        StartCoroutine(MoveUI(backGroundObj[3], -250.0f));

        StartCoroutine(makeDark(backGroundObj[0], 0.7f));
        StartCoroutine(makeDark(backGroundObj[1], 0.7f));
        StartCoroutine(makeDark(backGroundObj[3], 0.7f));
        StartCoroutine(makeDark(backGroundObj[4], 0.7f));

        yield return new WaitUntil(() => currentLightUI == 0 && currentMoveUI == 0); //주사위 굴리는 애니메이션 추가 예정

        diceThrowChk = false;
        StartCoroutine(Dice_Throw_Phase());
        yield return new WaitUntil(() => diceThrowChk);


        yield return new WaitForSeconds(1f);
        Debug.Log("phase change to 2!");
        curPhase = 2;
    }
    private bool diceThrowChk = false;

   

    public IEnumerator Dice_Throw_Phase()
    {
        if (curPhase == 1)
        {
            //아군 모든 주사위 던지기
            for (int i = 0; i < 4; i++)
            {
                if (myDice[i] != null)
                {
                    Instantiate(diceRollEff, myDiceUI[i].transform.position, Quaternion.Euler(0, 0, Random.Range(0, 4) * -90)); //사용된 아이템에 대해 effect
                    yield return new WaitForSeconds(0.25f);
                    myDice[i].throwDice();
                    myDiceNum[i] = myDice[i].getNum();
                    //임시 주사위 UI 변경
                    myDiceUI[i].transform.rotation = Quaternion.Euler(0, 0, myDice[i].getDir() * -90);
                    myDiceUI[i].GetComponent<SpriteRenderer>().sprite = diceSprite[myDice[i].getNum() - 1];
                }
            }

            //적군 모든 주사위 던지기
            for (int i = 0; i < 4; i++)
            {
                if (enemyDice[i] != null)
                {
                    Instantiate(diceRollEff, enemyDiceUI[i].transform.position, Quaternion.Euler(0, 0, Random.Range(0, 4) * -90)); //사용된 아이템에 대해 effect
                    yield return new WaitForSeconds(0.25f);
                    enemyDice[i].throwDice();
                    enemyDiceNum[i] = enemyDice[i].getNum();

                    enemyDiceUI[i].transform.rotation = Quaternion.Euler(0, 0, enemyDice[i].getDir() * -90);
                    enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = diceSprite[enemyDice[i].getNum() - 1];
                }
            }
            if (myDiceState[0] != 0 || myDiceState[1] != 0 || myDiceState[2] != 0 || myDiceState[3] != 0 ||
                enemyDiceState[0] != 0 || enemyDiceState[1] != 0 || enemyDiceState[2] != 0 || enemyDiceState[3] != 0 ) {
                yield return new WaitForSeconds(1.0f);

                //주사위 상태 배치
                for (int i = 0; i < 4; i++)
                {
                    if (myDiceState[i] != 0) {
                        yield return new WaitForSeconds(0.25f);
                        myDiceNum[i] = myDiceState[i];
                        //임시 주사위 UI 변경
                        myDiceUI[i].transform.rotation = Quaternion.Euler(0, 0, myDice[i].getDir() * -90);
                        myDiceUI[i].GetComponent<SpriteRenderer>().sprite = diceSprite[myDiceNum[i] - 1];
                        changeDiceState(i, -999);

                    }
                }
                for (int i = 0; i < 4; i++)
                {
                    if (enemyDiceState[i] != 0)
                    {
                        enemyDiceNum[i] = enemyDiceState[i];
                        //임시 주사위 UI 변경
                        enemyDiceUI[i].transform.rotation = Quaternion.Euler(0, 0, enemyDice[i].getDir() * -90);
                        enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = diceSprite[enemyDiceNum[i] - 1];
                        changeDiceState(i + 4, -999);
                        yield return new WaitForSeconds(0.25f);
                    }
                }
            }

            //임시로 넣어둠. 이곳에 적군 스킬 자동배치 함수가 들어가야 한다!
            MakeEnemyAttackSet();
            Debug.Log("Dice Throw Make enemy Array : " + enemyDiceTake[0].ToString() + " " + enemyDiceTake[1].ToString() + " " + enemyDiceTake[2].ToString() + " " + enemyDiceTake[3].ToString() + " ");
            updateEnemyDiceUI();
            diceThrowChk = true;
        }
    }

    //DiceThrow Phase End (phase 1- dice throw finish)//

    /// Witch Power Start (Phase 2- witch Power Select)///

    private int witchPowerState = 0;       //현재 보고 있는 마녀 능력의 index를 담는 변수
    private int witchPowerMoveState = 0;   // 0 : 마녀 파워 선택
                                           // 1 : 마녀 주사위 선택
                                           // 2 : 결정 끝나고 넘어가는 중
                                           // -1 : 마녀 파워가 결정되는 상태가 아니다.
    private int witchPowerClickState = -1; //현재 마녀 능력 사용에 필요한 dice 수를 담는다 

    //witch Power 선택 시작!
    private IEnumerator witchPowerPhase_Coroutine()
    {
        witchPowerObj[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPower_noUse");
        StartCoroutine(makeBright(backGroundObj[1], 0.0f));
        StartCoroutine(createWitchPowerUI());
        StartCoroutine(MoveUI(backGroundObj[1], -108.0f));
        yield return new WaitUntil(() => currentLightUI == 0 && currentMoveUI == 0);
        witchPowerState = 0;
        witchPowerMoveState = 0;
        witchPowerClickState = -1;
        
    }

    public int getWitchPower(int idx)
    {
        return witchPowerIdx[idx];
    }
    public void setWitchPower(int idx, int witchPower)
    {
        witchPowerIdx[idx] = witchPower;
    }


    private int[] witchPowerIdx = new int[3]; //현재 선택된 마녀 파워 Idx
    //마녀 파워 선택 (좌우)
    public void witchPowerState_Change(int dir)
    {
        Debug.Log(witchPowerMoveState);
        if (curPhase == 2 && currentLightUI == 0 && currentMoveUI == 0)
        {
            
            if (witchPowerMoveState == 0) //마녀 파워 선택을 하는 경우.
            {
                
                if (dir == 1)
                {
                    witchPowerState++;
                    if (witchPowerState > 2) witchPowerState = 0;
                }
                else
                {
                    witchPowerState--;
                    if (witchPowerState < 0) witchPowerState = 2;
                }
                //테스트를 위한 turn 능력이미지
                if (witchPowerState == 0) {
                    witchPowerObj[0].GetComponent<Animator>().Play("0");
                    //witchPowerUI.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPower_noUse"); 
                }
                else if (witchPowerState == 1) {
                    witchPowerObj[0].GetComponent<Animator>().Play(witchPowerIdx[1].ToString());
                    //witchPowerUI.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPower_turn"); 
                }
                else if (witchPowerState == 2) {
                    witchPowerObj[0].GetComponent<Animator>().Play(witchPowerIdx[2].ToString());
                    //witchPowerUI.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPower_turn_blue"); 
                }
            }
        }
    }

    //witch Power 선택한 경우의 함수
    public void witchPowerClick()
    {
        if (curPhase == 2 && currentLightUI == 0 && currentMoveUI == 0)
        {
            if (witchPowerMoveState == 0)
            {
                witchPowerMoveState = 1; //마녀 파워 선택 종료
                StartCoroutine(deleteWitchPowerUI());
                StartCoroutine(select_witchPower_Dice());
            }
        }
    }

    //주사위 고르기. 마녀 스킬 추가되면 여기서 작업할것
    private IEnumerator select_witchPower_Dice() {
        if (curPhase == 2)
        {
            //주사위 선택
            if (witchPowerMoveState == 1)
            {
                witchPowerClickState = 6;
                int witchPowerTemp = witchPowerIdx[witchPowerState];
                Debug.Log("witch Number : " + witchPowerTemp);
                int diceNum = 6;
                if (witchPowerTemp == 0) diceNum = 0; //능력을 사용하지 않는 경우
                if (witchPowerTemp >= 1 && witchPowerTemp <= 3) diceNum = 1; // 능력이 reroll이어서 주사위 하나만 쓰는 경우
                if (witchPowerTemp >= 4 && witchPowerTemp <=6 ) diceNum = 1; // 능력이 turn이어서 주사위 하나만 쓰는 경우
                if (witchPowerTemp >= 7 && witchPowerTemp <= 9) diceNum = 1; // 능력이 add1이어서 주사위 하나만 쓰는 경우
                if (witchPowerTemp >= 10 && witchPowerTemp <= 12) diceNum = 1; // 능력이 sub1이어서 주사위 하나만 쓰는 경우
                clickedDice[0] = -1;
                clickedDice[1] = -1;
                witchPowerClickState = diceNum;
                
                if (witchPowerTemp %3 ==2) { //적군 만을 대상으로 하는 경우
                    for (int i = 0; i < 4; i++) { 
                        Material material = myDiceUI[i].GetComponent<SpriteRenderer>().material;
                        material.SetFloat("_Transparency", 0.7f);
                    }
                }
                if (witchPowerTemp % 3 == 1) //아군 만을 대상으로 하는 경우
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Material material = enemyDiceUI[i].GetComponent<SpriteRenderer>().material;
                        material.SetFloat("_Transparency", 0.7f);
                    }
                }

                yield return new WaitUntil(() => witchPowerClickState == 0); //요구되는 주사위 수를 모두 채웠을때.

                

                

                if (witchPowerTemp != 0) //능력 사용시만
                {
                    if (witchPowerTemp >= 1 && witchPowerTemp <= 3) //reroll 스킬 사용시 
                    {
                        rerollDice(clickedDice[0]);
                    }
                    if (witchPowerTemp >= 4 && witchPowerTemp <= 6) //turn 스킬 사용시 
                    {
                        Debug.Log("witchPowerClickState : " + witchPowerClickState.ToString());
                        diceArrowSet[clickedDice[0]].SetActive(true); //해당 주사위 화살표 활성화
                        yield return new WaitUntil(() => witchPowerClickState == -1);
                        diceArrowSet[clickedDice[0]].SetActive(false);
                    }
                    if (witchPowerTemp >= 7 && witchPowerTemp <= 9) //add 스킬 사용시 
                    {
                        addDice(clickedDice[0], true);
                    }
                    if (witchPowerTemp >= 10 && witchPowerTemp <= 12) //sub 스킬 사용시 
                    {
                        addDice(clickedDice[0], false);
                    }

                    for (int i = 0; i < 4; i++)
                    {
                        if (i != clickedDice[0] && i != clickedDice[1]){
                            Material material = myDiceUI[i].GetComponent<SpriteRenderer>().material;
                            material.SetFloat("_Transparency", 0.7f);
                        }
                        if (i-4 != clickedDice[0] && i-4 != clickedDice[1]){
                            Material material2 = enemyDiceUI[i].GetComponent<SpriteRenderer>().material;
                            material2.SetFloat("_Transparency", 0.0f);
                        }
                    }

                    //스킬 사용되었으니 변경 값에 대하여 이펙트 생성
                    for (int i = 0; i < 2; i++)
                    {
                        if (clickedDice[i] >= 0 && clickedDice[i] <= 3)
                        {
                            Instantiate(diceRollEff, myDiceUI[clickedDice[i]].transform.position, Quaternion.Euler(0, 0, Random.Range(0, 4) * -90)); //사용된 아이템에 대해 effect
                        }
                        if (clickedDice[i] >= 4 && clickedDice[i] <= 7)
                        {
                            Instantiate(diceRollEff, enemyDiceUI[clickedDice[i] - 4].transform.position, Quaternion.Euler(0, 0, Random.Range(0, 4) * -90)); //사용된 아이템에 대해 effect
                        }
                    }
                    yield return new WaitForSeconds(1f);
                }

                for (int i = 0; i < 4; i++)
                {
                    Material material = myDiceUI[i].GetComponent<SpriteRenderer>().material;
                    material.SetFloat("_Transparency", 0.0f);
                    Material material2 = enemyDiceUI[i].GetComponent<SpriteRenderer>().material;
                    material2.SetFloat("_Transparency", 0.0f);
                }

                witchPowerMoveState = 2;
            }

            yield return new WaitUntil(() => currentMoveUI == 0 && currentLightUI == 0); //

            //주사위 선택 종료시 버튼 이동
            //직관성을 위해 나눔
            if (witchPowerMoveState == 2)
            {

                //다음 페이즈로 넘어가는 부분
                MakeEnemyAttackSet();
                updateEnemyDiceUI();

                curClickSkill = -1;
                witchPowerMoveState = -1;
                curPhase = 3;
            }
        }
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
            if (myDice[idx] != null)
            {
                myDiceNum[idx] = myDice[idx].throwDice();
                myDiceUI[idx].transform.rotation = Quaternion.Euler(0, 0, myDice[idx].dir * -90);
                myDiceUI[idx].GetComponent<SpriteRenderer>().sprite = diceSprite[myDice[idx].getNum() - 1];
            }
        }
        else
        {
            idx -= 4;
            if (enemyDice[idx] != null)
            {
                enemyDiceNum[idx] = enemyDice[idx].throwDice();
                enemyDiceUI[idx].transform.rotation = Quaternion.Euler(0, 0, enemyDice[idx].dir * -90);
                enemyDiceUI[idx].GetComponent<SpriteRenderer>().sprite = diceSprite[enemyDice[idx].getNum() - 1];
            }
        }
        witchPowerClickState = -1;
    }
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
                Debug.Log("turn to sprite index is...! " + (myDice[idx].getNum() - 1).ToString());
                myDiceUI[idx].GetComponent<SpriteRenderer>().sprite = diceSprite[myDice[idx].getNum() - 1]; 
                
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
            }
        }
        witchPowerClickState = -1;
        Debug.Log("Turn Dice Here!");

    }

    //마녀 좌우 선택 UI 천천히 제거
    private IEnumerator deleteWitchPowerUI()
    {
        if (curPhase == 2)
        {
            currentLightUI++;
            Color color = witchPowerObj[1].GetComponent<SpriteRenderer>().color;
            while (color.a > 0.00f)
            {
                //witchPowerObj[0].GetComponent<SpriteRenderer>().color = color;
                witchPowerObj[1].GetComponent<SpriteRenderer>().color = color;
                witchPowerObj[2].GetComponent<SpriteRenderer>().color = color;
                color.a -= 0.2f;
                yield return new WaitForSeconds(0.1f);
            }
            for(int i=1;i<3;i++) witchPowerObj[i].transform.position = new Vector3(witchPowerObj[i].transform.position.x, 300, witchPowerObj[i].transform.position.z);

            witchPowerObj[0].SetActive(false);
            witchPowerObj[1].SetActive(false);
            witchPowerObj[2].SetActive(false);
            currentLightUI--;
        }
    }
    //마녀 좌우 선택 UI 천천히 생성
    private IEnumerator createWitchPowerUI()
    {
        if (curPhase == 2)
        {
            currentLightUI++;
            witchPowerObj[0].SetActive(true);
            witchPowerObj[1].SetActive(true);
            witchPowerObj[2].SetActive(true);
            for (int i = 1; i < 3; i++) witchPowerObj[i].transform.position = new Vector3(witchPowerObj[i].transform.position.x, 50, witchPowerObj[i].transform.position.z);

            Color color = witchPowerObj[1].GetComponent<SpriteRenderer>().color;
            color.a = 0.0f;
            //witchPowerObj[0].GetComponent<SpriteRenderer>().color = color;
            witchPowerObj[1].GetComponent<SpriteRenderer>().color = color;
            witchPowerObj[2].GetComponent<SpriteRenderer>().color = color;
            while (color.a < 1.00f)
            {
                //witchPowerObj[0].GetComponent<SpriteRenderer>().color = color;
                witchPowerObj[1].GetComponent<SpriteRenderer>().color = color;
                witchPowerObj[2].GetComponent<SpriteRenderer>().color = color;
                color.a += 0.2f;
                yield return new WaitForSeconds(0.1f);
            }
            //witchPowerObj[0].GetComponent<SpriteRenderer>().color = color;
            witchPowerObj[1].GetComponent<SpriteRenderer>().color = color;
            witchPowerObj[2].GetComponent<SpriteRenderer>().color = color;
            currentLightUI--;
        }
    }

    // 주사위 선택(다양하게 수정할 수 있어야한다. 지금은 마녀만 해서 이름이 이런데 나중에 수정해야됨.) -> 아니면 주사위 클릭에서 분기 시도 -> 분기했음.(click_dice 함수)
    public void click_witchPower_Dice(int diceIdx)
    {
        if (curPhase == 2 && witchPowerClickState > 0 &&  currentLightUI == 0 && currentMoveUI == 0)
        {
            //아군 캐릭터 중 유효한 주사위가 선택되었고, 능력이 적군 선택이 아닌 경우 
            if ((diceIdx >= 0 && diceIdx <= 3 && myCharacter[diceIdx] != null && myDice[diceIdx] != null) && witchPowerIdx[witchPowerState] % 3 != 2) 
            {
                if (witchPowerClickState == 2)
                {
                    clickedDice[1] = diceIdx;
                }
                else if (witchPowerClickState == 1)
                {
                    clickedDice[0] = diceIdx;
                }
                witchPowerClickState--;
            }
            //적군 캐릭터 중 유효한 주사위가 선택되었고, 능력이 아군 선택이 아닌 경우 
            if ((diceIdx >= 4 && diceIdx <= 7 && enemyCharacter[diceIdx-4] != null && enemyDice[diceIdx-4] != null) && witchPowerIdx[witchPowerState] % 3 != 1)
            {
                if (witchPowerClickState == 2)
                {
                    clickedDice[1] = diceIdx;
                }
                else if (witchPowerClickState == 1)
                {
                    clickedDice[0] = diceIdx;
                }
                witchPowerClickState--;
            }
        }
    }
    /// Witch Power End (Phase 2- witch Power Select)///



    // Character Skill Select Start (Phase 3 - Character Skill Select)///

    private GameObject[] skillSelectUI = new GameObject[9];
    private GameObject[] skillSelectDescUI = new GameObject[7];

    private int curClickSkill = -1; //마지막으로 클릭한 스킬 정보를 저장한다. 저장형식은 characterIdx * 10 + skillIdx의 형태를 띈다. 선택된게 없으면 -1을 갖는다.

    private IEnumerator skillSelectPhase_Coroutine()
    {
       
        StartCoroutine(MoveUI(diceFullUI, 60.0f));
        StartCoroutine(MoveUI(backGroundObj[0], 0.0f)); // 78f : skillSelect  62f: battle
        StartCoroutine(makeBright(backGroundObj[0], 0.0f));
        //StartCoroutine(MoveUI(backGroundObj[1], 10.0f - 108f)); 작게 상단 이동
        StartCoroutine(MoveUI(backGroundObj[1], -300f));
        StartCoroutine(makeDark(backGroundObj[1], 0.7f));

        //정면 보는 마녀
        StartCoroutine(makeDark(backGroundObj[3], 0.7f));
        StartCoroutine(makeDark(backGroundObj[4], 0.7f));
        StartCoroutine(MoveUI(backGroundObj[3], 140f, 0.5f)); //59f

        StartCoroutine(MoveUI(characterUI, 0.0f)); //
        StartCoroutine(MoveUI(skillSelectUI[8], -50.0f)); //

        for (int i = 0; i < 4; i++)
        {
            StartCoroutine(makeBright(myCharacterObjUI[i], 0.0f));
            StartCoroutine(makeBright(enemyCharacterObjUI[i], 0.0f));
        }
        TalkManager.Instance.startTalk(6);
        yield return new WaitUntil(() => currentMoveUI == 0 && currentLightUI == 0); //
    }



    //스킬 선택 중 버튼 클릭에 대한 코드
    public void click_characterSkill_Button(int input)
    {
        Debug.Log(curPhase);
        if (curPhase == 3 && currentLightUI == 0 && currentMoveUI == 0)
        {
            Debug.Log("lets do this");
            int characterIdx = input / 10;
            int skillIdx = input % 10;
            if (myCharacter[characterIdx] != null && myCharacter[characterIdx].getCurState() == 0)
            {
                //현재 선택된게 없는 경우.
                if (curClickSkill == -1)
                {
                    Debug.Log("not select now, so we need to select");
                    //현재 주사위에 배치되어 있는 경우
                    if (mySkillUsed[characterIdx, skillIdx])
                    {
                        //할당된 주사위를 찾아 제거하는 코드
                        for (int i=0;i<4;i++)
                        {
                            if(myDiceTake[i] == input)
                            {
                                diceUIChk[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                                if (i < 3) diceUIChain[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");//연결 체인도 제거
                                //myDiceTake[i] = -999;
                                myDiceChange(i, 0, -999);
                            }
                        }
                        curClickSkill = -1;
                        StartCoroutine(makeBright(skillSelectUI[characterIdx * 2 + skillIdx], 0.0f));
                        mySkillUsed[characterIdx, skillIdx] = false;
                        deleteSkillCommand();
                    }
                    else
                    {
                        curClickSkill = input;
                        StartCoroutine(makeDark(skillSelectUI[characterIdx * 2 + skillIdx], 0.7f));
                        makeSkillCommand(characterIdx, skillIdx);
                    }
                    
                }
                else
                {
                    Debug.Log("select now, so we need to turn off");
                    StartCoroutine(makeBright(skillSelectUI[(curClickSkill / 10) * 2 + (curClickSkill % 10)], 0.0f));
                    deleteSkillCommand();
                    if (mySkillUsed[characterIdx, skillIdx]) //할당이 된 스킬인 경우
                    {
                        curClickSkill = -1;
                    }
                    else if (curClickSkill == input) { //할당이 안된 다른 같은인경우
                        curClickSkill = -1;
                    }
                    else if(curClickSkill != input) //할당이 안된 다른 스킬인 경우
                    {
                        StartCoroutine(makeDark(skillSelectUI[characterIdx * 2 + skillIdx], 0.7f));
                        curClickSkill = input;
                        makeSkillCommand(characterIdx, skillIdx);
                    }
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
                        //diceUIChk[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                        //if(i < 3) diceUIChain[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");//연결 체인도 제거
                        //myDiceTake[i] = -999;
                        myDiceChange(i, 0, -999);
                    }

                }
                updateMyDiceUI();
                //해당 스킬에 대한 버튼 해제
                StartCoroutine(makeBright(skillSelectUI[(deleteSkill / 10) * 2 + (deleteSkill % 10)], 0.0f));
                mySkillUsed[(deleteSkill / 10), (deleteSkill % 10)] = false;
                
                deleteSkillCommand();
            }
            else if (curClickSkill != -1) //스킬 선택을 했으며 해당 주사위가 비어있는 경우.
            {
                int characterIdx = curClickSkill / 10;
                int skillIdx = curClickSkill % 10;

                Skill useSkill = myCharacter[characterIdx].skillUse(skillIdx);
                int needDiceNum = useSkill.getNeedDiceNum();
                //가능한지 확인
                if(MakeMyAttackSet(characterIdx, skillIdx, diceIdx))
                {   //가능한 경우 주사위의 ui를 업데이트
                    updateMyDiceUI();
                    mySkillUsed[characterIdx, skillIdx] = true;
                }
                else //불가능한 경우
                {
                    StartCoroutine(makeBright(skillSelectUI[characterIdx * 2 + skillIdx], 0.0f));
                    makeSkillCommand(characterIdx, skillIdx);
                    Debug.Log("It can't! - wrong Dice Problem");
                }

                curClickSkill = -1;
            }
            //주사위에 할당된 스킬도 클릭된 스킬도 없다면 아무것도 하지 않는다.
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
                    StartCoroutine(makeBright(skillSelectUI[characterIdx * 2 + skillIdx], 0.0f));
                    makeSkillCommand(characterIdx, skillIdx);
                    curClickSkill = -1;
                }
                
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
    void makeSkillCommand(int characterIdx, int skillIdx)
    {
        Skill thisSkill = null;
        if (characterIdx >= 0 && characterIdx < 4) { thisSkill = myCharacter[characterIdx].skillUse(skillIdx); }
        else if (characterIdx >= 4 && characterIdx < 8) {thisSkill = enemyCharacter[characterIdx-4].skillUse(skillIdx);}

        //적군 스킬이면서 본적 없는 스킬인 경우
        if (characterIdx >= 4 && characterIdx < 8 && !jsonDataManager.Instance.getMonsterSkill(enemyCharacter[characterIdx - 4].getDestiny().DestinyIdx, skillIdx))
        {
            skillSelectDescUI[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_noImage");
            skillSelectDescUI[1].GetComponent<TextMeshPro>().text = "아직 몬스터의 스킬을 본적이 없습니다.";
            skillSelectDescUI[6].GetComponent<TextMeshPro>().text = "Not Found";
        }
        else
        {
            if (Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + thisSkill.getSkillName()) != null)
            {
                skillSelectDescUI[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + thisSkill.getSkillName());
            }
            else
            {
                skillSelectDescUI[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_noImage");
            }
            skillSelectDescUI[1].GetComponent<TextMeshPro>().text = thisSkill.getCommand();
            skillSelectDescUI[6].GetComponent<TextMeshPro>().text = thisSkill.getSkillName();
        }
        for (int i=0;i<4;i++)
        {
            skillSelectDescUI[i+2].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_" + thisSkill.getNeedDice(i).ToString());
        }
        
    }
    void deleteSkillCommand()
    {
        skillSelectDescUI[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
        skillSelectDescUI[1].GetComponent<TextMeshPro>().text = "";
        for (int i = 0; i < 4; i++)
        {
            skillSelectDescUI[i + 2].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_0");
        }
        skillSelectDescUI[6].GetComponent<TextMeshPro>().text = "";
    }

    public void flipBag_battle()    // 가방 키고 끄는 함수. 4페이즈, 5페이즈 일땐 끌수 없게 한다.
    {
        if (curPhase != 4 && curPhase != 5)
        {
            itemManager.Instance.flipItemBox_BattleUI();
        }
    }


    //phase넘어가기
    public void moveToBattlePhase()
    {
        if (curPhase == 3 && currentLightUI == 0 && currentMoveUI == 0)
        {
            if (!itemManager.Instance.getItemBoxOpen())
            {
                curPhase = 4;
                itemManager.Instance.flipItemBox_BattleUI(); //넘어갈때 passive칸을 켜준다.
            }
            else
            {
                itemManager.Instance.flipItemBox_BattleUI();// 열려있다면 끄고 넘어간다.
            }
        }
    }

    private IEnumerator moveToBattlePhase_Coroutine()
    {
        if (curPhase == 4 && currentLightUI == 0 && currentMoveUI == 0)
        {
            curPhase = -999;
            //StartCoroutine(MoveUI(diceFullUI, 33.0f));
            StartCoroutine(MoveUI(diceFullUI, 50.0f));

            StartCoroutine(MoveUI(backGroundObj[0], -16.0f)); // 78f : skillSelect  62f: battle
            StartCoroutine(makeBright(backGroundObj[0], 0.0f));
            //StartCoroutine(MoveUI(backGroundObj[1], 10.0f));
            StartCoroutine(MoveUI(backGroundObj[1], -475f));

            StartCoroutine(MoveUI(backGroundObj[3], 59f));

            StartCoroutine(makeBright(backGroundObj[3], 0.3f));
            StartCoroutine(makeBright(backGroundObj[4], 0.3f));

            StartCoroutine(MoveUI(characterUI, -18.0f)); //

            StartCoroutine(MoveUI(skillSelectUI[8], -138.0f)); //


            //StartCoroutine(makeDark(backGroundObj[3], 0.7f));
            //StartCoroutine(MoveUI(backGroundObj[3], 59f));

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 2; j++)
                    StartCoroutine(makeBright(skillSelectUI[i * 2 + j], 0.0f));
            }
            //다음 페이즈로 넘어가는 부분
            yield return new WaitUntil(() => currentMoveUI == 0 && currentLightUI == 0); //

            //스킬 이미지를 각 주사위에 배치
            int curDiceNum = 0;
            string skillNameTake = "";
            for (int i=0;i<4;i++)  //아군 주사위 배치
            {
                if (myDice[i] == null) continue;
                myDiceUI[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                curDiceNum = myDiceTake[i];
                if (curDiceNum == -999)
                {
                    myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                }
                else
                {
                    skillNameTake = myCharacter[curDiceNum / 10].skillUse(curDiceNum % 10).getSkillName();
                    if (Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + skillNameTake) == null)
                    {
                        skillNameTake = "noImage";
                    }
                    myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + skillNameTake);
                }
                yield return new WaitForSeconds(0.2f);
            }
            for (int i = 0; i < 4; i++) //적군 주사위 배치
            {
                if (enemyDice[i] == null) continue;
                enemyDiceUI[i].transform.rotation = Quaternion.Euler(0, 0, 0);
                curDiceNum = enemyDiceTake[i];
                if (curDiceNum == -999)
                {
                    enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                }
                else
                {
                    skillNameTake = enemyCharacter[curDiceNum / 10].skillUse(curDiceNum % 10).getSkillName();
                    if (Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + skillNameTake) == null)
                    {
                        skillNameTake = "noImage";
                    }
                    enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + skillNameTake);
                }
                yield return new WaitForSeconds(0.2f);
            }
            curClickSkill = -1;
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
    private IEnumerator clickEnemy_Coroutine(int clickEnemyNum, int clickAbleTeam) //clickAbleTeam은 0 : 아군 대상 / 1: 적군대상 / 2 : 전체 대상을 의미한다.
    { //캐릭터 클릭을 위한 코루틴(입력된 갯수만큼 반복될 예정)
        characterTargetIdx = 0;   //character인덱스 초기화

        for (int i = 0; i < clickCharacter.Length; i++) { //모든 클릭된 캐릭터 초기화
            clickCharacter[i] = -999;
        }
        //지금 고민중인거는 죽은 캐릭터 위치 클릭가능하게 하나? -> 일단 null일때 조건문 없애놈 -> 근데 아닌거 같아서 걍 null일때 안생기게 해둠
        if (clickAbleTeam != 2) {//아군 선택만 가능한 경우
            for (int i = 0; i < 4; i++)
            {
                if (myCharacter[i] != null && myCharacter[i].getCurState() != 2) {
                    battleTargetUI[i].SetActive(true);
                    characterClickAble[i] = true;
                }
            }
        }
        else if (clickAbleTeam != 1) {//적군 선택만 가능한 경우
            for (int i = 4; i < 8; i++) {
                //if (enemyCharacter[i] != null && enemyCharacter[i].getCurState() != 2) {
                    battleTargetUI[i].SetActive(true);
                    characterClickAble[i] = true; 
                //}  
            }
        }

        while (characterTargetIdx < clickEnemyNum) //클릭된 캐릭터 값을 선택수만큼 배열에 저장
        {
            yield return new WaitUntil(() => clickCharacter[characterTargetIdx] != -999);
            characterTargetIdx++;
        }

        //클릭하지 못하게 바꾸기
        for (int i = 0; i < 8; i++) {
            battleTargetUI[i].SetActive(false);
            characterClickAble[i] = false; 
        }


        //해제해버리면 밖에서 못쓰니 밖에서 해제해줘야합니다!
    }

    
    
    public void click_battle_character(int characterIdxInput)
    {   //캐릭터를 누르면 해당 캐릭터 클릭이 비활성화되고
        if (curPhase == 5 && characterTargetIdx != -999 && characterClickAble[characterIdxInput])
        {
            Debug.Log("hello!");
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
                if (enemyCharacter[i-4] != null && enemyCharacter[i-4].getCurState() == 0) {
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
            for (int i=0;i<clickEnemyNum;i++) {
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
        for (int i=0;i<4;i++){ //초기화
            makeDiceArrToMakePacket[i] = -999;
        }
        int curIdx = 0;
        for (int i=0;i< 4;i++) { //유의미한 길이만큼 길이 생성
            if(startIdx+i < 4 && myDiceNum[startIdx+i] != -999) //유의미한 주사위 값일 경우만
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
        if(characterIdx < 4)
        {
            if (option == 1)
            {
                myCharacterObjUIAnim[characterIdx].Play("Hit");
                Debug.Log(characterIdx.ToString() + " is hit by monster!");
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

    private void DeadCharacterUpdate(int idx) //캐릭터가 죽을 경우(getcurstate가 2를 반환시) 작동한다. 
        //플레이어 죽음으로 맛있는데! 가 아니라 플레이어 받게 되면 애니메이션은 밖에서 해줌.
    {
        changeDiceState(idx, -999);
        if (idx < 4)
        {
            int diceNumTemp = myDiceTake[idx]; //죽은 캐릭터가 지니고 있는 주사위를 사용한 스킬 들 해제

            //해당 스킬 도트 비활성화
            skillSelectUI[idx*2].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.7f);
            skillSelectUI[idx * 2 + 1].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.7f);


            for (int i=0;i<4;i++)   // 죽은 캐릭터가 가지고 있는 스킬 모두 해제.
            {
                if(myDiceTake[i] / 10 == idx)
                {
                    mySkillUsed[myDiceTake[i] / 10, myDiceTake[i] % 10] = false;
                    myDiceChange(i, 0, -999);
                    myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                } 
            }
            
            for (int i=0;i<4;i++) 
            {
                if (myDiceTake[i] == diceNumTemp) {
                    myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                    myDiceChange(i, 0, -999);
                }
            }
            
            myDiceNum[idx] = -999;
            myDice[idx] = null;
            
            updateMyDiceUI();
        }
        else
        {
            idx -= 4;
            int diceNumTemp = enemyDiceTake[idx]; //죽은 캐릭터가 지니고 있는 주사위를 사용한 스킬 들 해제

            for (int i = 0; i < 4; i++)   // 죽은 캐릭터가 가지고 있는 스킬 모두 해제.
            {
                if (enemyDiceTake[i] / 10 == idx )
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

            enemyDiceNum[idx] = -999;
            enemyDice[idx] = null;

            enemySkill[idx] = null;     //적군 스킬 attackset시 포함 안되도록 설정
            enemySkill[idx + 4] = null;
            enemySkillDiceNum[idx] = -999;
            enemySkillDiceNum[idx + 4] = -999;

            int moneyTemp = enemyCharacter[idx].getMoney();
            moneyTemp = Random.Range(moneyTemp - (moneyTemp * 2 / 5), moneyTemp + (moneyTemp * 2 / 5));
            AdventureManager.Instance.addAdventureMoney(moneyTemp);
            for (int i = 0; i < moneyTemp; i++)
            {
                Instantiate(coinEff, enemyCharacterObjUI[idx].transform.position, Quaternion.Euler(0, 0, 0)); //사용된 아이템에 대해 effect
            }

            updateEnemyDiceUI();
        }
    }
    
    
    private string makeBattleFontSize(int input) //상단 텍스트 폰트 사이즈 생성을 담당. 1050뎀 이상일때 520을 최대로 둔다.
    {
        int result;
        if (input < 50) result = 120;
        else result = 120 + (input - 50) * 2 / 5;
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
    private IEnumerator passiveUpdateBeforClick(List<TakeSkillPacket> takeSkillPacketArr, int [] usedDiceArr, bool updateLook, int itemType) 
    {
        passiveItemChk = true;
        for (int takeSkillArrIdx = 0; takeSkillArrIdx < takeSkillPacketArr.Count; takeSkillArrIdx++)
        {
            for (int passiveItemIdx = 0; passiveItemIdx < 11; passiveItemIdx++)
            { //모든 passive 아이템을 확인해서 takeSkillPacket 수정

                if (itemManager.Instance.usePassiveItem(takeSkillPacketArr[takeSkillArrIdx], passiveItemIdx, usedDiceArr, 0) & updateLook) //만약 적용이 되엇으며 그 결과를 보여줄 경우
                {
                    
                    //SoundManager_doremi.Instance.playDoremi(itemUseIdx++);
                    GameObject temp = Instantiate(passiveEffObj, itemManager.Instance.getItemInventoryPosition(passiveItemIdx), new Quaternion(0, 0, 0, 0)); //사용된 아이템에 대해 effect

                    for (int fontSizeIdx = 0; fontSizeIdx < 10; fontSizeIdx++)
                    {
                        battleTextObj.GetComponent<TextMeshPro>().text = "<size=" + makeBattleFontSize(takeSkillPacketArr[takeSkillArrIdx].getVal() + fontSizeIdx * fontSizeIdx * 2) + ">" +
                            takeSkillPacketArr[takeSkillArrIdx].getVal().ToString() //상단부에 적용될 text값 적기
                        + "</size>";
                        yield return new WaitForSeconds(0.02f);
                    }
                    for (int fontSizeIdx = 10; fontSizeIdx > 00; fontSizeIdx--)
                    {
                        battleTextObj.GetComponent<TextMeshPro>().text = "<size=" + makeBattleFontSize(takeSkillPacketArr[takeSkillArrIdx].getVal() + fontSizeIdx * fontSizeIdx * 2) + ">" +
                            takeSkillPacketArr[takeSkillArrIdx].getVal().ToString() //상단부에 적용될 text값 적기
                        + "</size>";
                        yield return new WaitForSeconds(0.02f);
                    }
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
        passiveItemChk = false;
    }
    private void makeHitEffect(int tempTargetIdx)
    {
        Instantiate(hitEff, battleTargetUI[tempTargetIdx].transform.position + new Vector3(Random.Range(-15, 15), Random.Range(-15, 15), 0), Quaternion.Euler(0, 0, Random.Range(0, 4) * -90)); //사용된 아이템에 대해 effect
    }
    private IEnumerator BattlePhase_Coroutine()
    {
        //아직 스킬 애니메이션과의 연동 & 스킬 데미지 연동이 안되어있음.
        if(curPhase == 5)
        {
            int nextDice = 0;
            int nextSkill = -999;
            //아군 스킬 클릭 
            while (nextDice < 4)
            {
                if (myDiceTake[nextDice] != -999)
                {   //주사위 가장 앞에 있는 주사위 클릭을 위해 받아오고 click 기다리기
                    Debug.Log("you should click : " + nextDice.ToString());
                    nextSkill = myDiceTake[nextDice];
                    yield return new WaitUntil(() => clickDice_battlePhase == nextSkill);
                    Debug.Log("My Skill Use : " + clickDice_battlePhase.ToString());


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

                            myDiceChange(i, 0, -999);
                            myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                            diceUIChk[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                            if (i != 3)  diceUIChain[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
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
                    SendSkillPacket sendSkillPacketTemp = new SendSkillPacket(skillUseCharacter, myCharacter[skillUseCharacter].getSkillIdx(skillUseIdx), clickCharacter, makeDiceArrToMakePacket);
                    takeSkillPacketArr.Clear();
                    takeSkillPacketArr = myCharacter[skillUseCharacter].doSkill(sendSkillPacketTemp);
                    
                    //skill기반의 takeSkillPacket의 값 얻고 이벤트 보여주기
                    // 활성화 보여주고, 클릭 전 패시브 대상으로 하며
                    
                    StartCoroutine( passiveUpdateBeforClick(takeSkillPacketArr, usedDiceArr, true, 0));
                    yield return new WaitUntil(() => !passiveItemChk);

                    for (int i=0;i<curSkill.getTargetChance();i++) { // 해당 스킬이 공격하는 숫자
                        characterTargetIdx = 0;

                        //SendSkillPacket sendSkillPacketTemp = new SendSkillPacket(skillUseCharacter, myCharacter[skillUseCharacter].getSkillIdx(skillUseIdx), clickCharacter, makeDiceArrToMakePacket);

                        StartCoroutine(clickEnemy_Coroutine(curSkill.getTargetNum(), curSkill.getTargetTeam())); // 클릭 이벤트 시작
                        yield return new WaitUntil(() => characterTargetIdx == curSkill.getTargetNum()); //필요한 캐릭터만큼 클릭된 경우 click 이벤트 종료!
                        characterTargetIdx = -999;

                        //스킬에 대한 공격용 Packet 생성
                        //makeMyDice_BattlePhase(nextDice, curSkill.getNeedDiceNum() );
                        sendSkillPacketTemp.addClickCharacter(clickCharacter);
                        //SendSkillPacket sendSkillPacketTemp = new SendSkillPacket(skillUseCharacter, myCharacter[skillUseCharacter].getSkillIdx(skillUseIdx), clickCharacter, makeDiceArrToMakePacket);
                        takeSkillPacketArr.Clear();
                        takeSkillPacketArr = myCharacter[skillUseCharacter].doSkill(sendSkillPacketTemp);

                        StartCoroutine(passiveUpdateBeforClick(takeSkillPacketArr, usedDiceArr, false, 0));
                        yield return new WaitUntil(() => !passiveItemChk);

                        int tempTargetIdx;
                        for (int takeSkillArrIdx = 0; takeSkillArrIdx < takeSkillPacketArr.Count; takeSkillArrIdx++)
                        {

                            tempTargetIdx = takeSkillPacketArr[takeSkillArrIdx].getTargetIdx();
                            if (tempTargetIdx < 4) //아군 대상으로 스킬이 들어온 경우
                            {
                                if(myCharacter[tempTargetIdx] != null && myCharacter[tempTargetIdx].getCurState() == 0) //대상 존재시 damage text 출력
                                {
                                    
                                    GameObject temp = Instantiate(damageTextObj, myCharacterObjUI[tempTargetIdx].transform.position + new Vector3(0,45,0), new Quaternion(0, 0, 0, 0)); //적용된 것에 대한 텍스트 생성
                                    temp.GetComponent<damageMove>().textChange(takeSkillPacketArr[takeSkillArrIdx].getVal());
                                    if (myCharacter[tempTargetIdx] != null && myCharacter[tempTargetIdx].TakeSkillPacket(takeSkillPacketArr[takeSkillArrIdx])) //반환 결과가 해당 캐릭터의 죽음 인경우
                                    {
                                        characterDamageMove(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getVal());
                                        makeHitEffect(tempTargetIdx);
                                        battleAnimationControl(tempTargetIdx, 2);
                                        DeadCharacterUpdate(tempTargetIdx);
                                        updateMyDiceUI();
                                    }
                                    else
                                    {
                                        //주사위 상태 변화 실행

                                        changeDiceState(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getStateChange());

                                        if (takeSkillPacketArr[takeSkillArrIdx].getSkillType() == 0)
                                        {  //대미지는 주었지만한 경우(현재 버프에 대한 구분이 없어서 추후 수정필요)
                                            characterDamageMove(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getVal());
                                            makeHitEffect(tempTargetIdx);
                                            battleAnimationControl(tempTargetIdx, 1);
                                        }
                                    }
                                }
                                
                                
                            }
                            else // 적군 대상으로 스킬이 들어온 경우
                            {

                                if (enemyCharacter[tempTargetIdx-4] != null && enemyCharacter[tempTargetIdx-4].getCurState() == 0) //대상 존재시 damage text 출력
                                {
                                    
                                    GameObject temp = Instantiate(damageTextObj, enemyCharacterObjUI[tempTargetIdx-4].transform.position + new Vector3(0, 45, 0), new Quaternion(0, 0, 0, 0)); //적용된 것에 대한 텍스트 생성
                                    temp.GetComponent<damageMove>().textChange(takeSkillPacketArr[takeSkillArrIdx].getVal());
                                    if (enemyCharacter[tempTargetIdx - 4] != null && enemyCharacter[tempTargetIdx - 4].TakeSkillPacket(takeSkillPacketArr[takeSkillArrIdx])) //반환 결과가 해당 캐릭터의 죽음 인경우
                                    {
                                        characterDamageMove(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getVal());
                                        makeHitEffect(tempTargetIdx);
                                        battleAnimationControl(tempTargetIdx, 2);
                                        DeadCharacterUpdate(tempTargetIdx);
                                        updateEnemyDiceUI();
                                    }
                                    else
                                    {
                                        changeDiceState(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getStateChange());
                                        if (takeSkillPacketArr[takeSkillArrIdx].getSkillType() == 0)
                                        { //대미지는 주었지만한 경우(현재 버프에 대한 구분이 없어서 추후 수정필요)
                                            characterDamageMove(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getVal());
                                            makeHitEffect(tempTargetIdx);
                                            battleAnimationControl(tempTargetIdx, 1);
                                        }
                                    }
                                }

                                
                            }
                            


                            updateHp();
                            updateMyDiceUI();
                            updateBattleUI();
                        }
                        skillAnimationControl(true, 3, i, curSkill, skillUseCharacter, -999, skillUseIdx);//타겟팅 전 애니메이션 실행
                        yield return new WaitUntil(() => myCharacterAtkReady[skillUseCharacter] == 0);

                        if (i + 1 < curSkill.getTargetChance()) { //공격 후에 공격기회가 더 남았으면 다시 뒤로 땡기기
                            skillAnimationControl(true, 1, 0, curSkill, skillUseCharacter, -999, skillUseIdx);//타겟팅 전 애니메이션 실행
                            yield return new WaitUntil(() => myCharacterAtkReady[skillUseCharacter] == 2);
                        }

                        battleTextObj.GetComponent<TextMeshPro>().text = "";

                    }

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
                
                if (enemyDiceTake[nextDice] != -999)
                {   //주사위 가장 앞에 있는 주사위 클릭을 위해 받아오고 click 기다리기
                    nextSkill = enemyDiceTake[nextDice];
                    Debug.Log("Enemy Skill Use : " + nextSkill.ToString());
                    for (int i = 0; i < 4; i++)
                    {
                        if (enemyDiceTake[i] == nextSkill)
                        {
                            enemyDiceChange(i, -999);
                            enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                            if (i != 3)
                            {
                                diceUIChain[i + 3].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                            }
                            
                        }
                    }
                    //스킬이 사용 코드 적히는 부분
                    int skillUseCharacter = nextSkill / 10;
                    int skillUseIdx = nextSkill % 10;
                    Skill curSkill = enemyCharacter[skillUseCharacter].skillUse(skillUseIdx); //사용하는 스킬에 대한 정보를 받아온다.

                    skillAnimationControl(false, 1, 0, curSkill, skillUseCharacter, -999, skillUseIdx);//타겟팅 전 애니메이션 실행
                    yield return new WaitUntil(() => enemyCharacterAtkReady[skillUseCharacter] == 2);
                    yield return new WaitForSeconds(0.2f);

                    jsonDataManager.Instance.meetMonsterSkill(enemyCharacter[skillUseCharacter].getDestiny().DestinyIdx, skillUseIdx);
                    
                    for (int i = 0; i < curSkill.getTargetChance(); i++)
                    { // 해당 스킬이 공격하는 숫자

                        makeEnemyClick(curSkill.getTargetNum(), curSkill.getTargetTeam()); // 적군의 공격 대상 만들기

                        //스킬에 대한 공격용 Packet 생성
                        makeEnemyDice_BattlePhase(nextDice, nextDice + curSkill.getNeedDiceNum() - 1);
                        SendSkillPacket sendSkillPacketTemp = new SendSkillPacket(skillUseCharacter, enemyCharacter[skillUseCharacter].getSkillIdx(skillUseIdx), clickCharacter, makeDiceArrToMakePacket);
                        
                        Debug.Log("it is only slime skill idx testLog : "+enemyCharacter[skillUseCharacter].getSkillIdx(skillUseIdx).ToString());
                        
                        takeSkillPacketArr.Clear();
                        takeSkillPacketArr = enemyCharacter[skillUseCharacter].doSkill(sendSkillPacketTemp);
                        Debug.Log("enemy who use skill" + skillUseCharacter);
                        int tempTargetIdx;
                        for (int takeSkillArrIdx = 0; takeSkillArrIdx < takeSkillPacketArr.Count; takeSkillArrIdx++)
                        {
                            
                            tempTargetIdx = takeSkillPacketArr[takeSkillArrIdx].getTargetIdx();
                            Debug.Log("target is...! : " + tempTargetIdx.ToString());
                            if (tempTargetIdx < 4) //아군 대상으로 스킬이 들어온 경우
                            {
                                
                                if (myCharacter[tempTargetIdx] != null && myCharacter[tempTargetIdx].getCurState() == 0) //대상 존재시 damage text 출력
                                {
                                    GameObject temp = Instantiate(damageTextObj, myCharacterObjUI[tempTargetIdx].transform.position + new Vector3(0, 45, 0), new Quaternion(0, 0, 0, 0)); //적용된 것에 대한 텍스트 생성
                                    temp.GetComponent<damageMove>().textChange(takeSkillPacketArr[takeSkillArrIdx].getVal());
                                    //사망 아닐시
                                    if (myCharacter[tempTargetIdx].TakeSkillPacket(takeSkillPacketArr[takeSkillArrIdx]))
                                    {
                                        characterDamageMove(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getVal());
                                        makeHitEffect(tempTargetIdx);
                                        battleAnimationControl(tempTargetIdx, 2);
                                        DeadCharacterUpdate(tempTargetIdx);
                                    }
                                    else // 사망일시
                                    {
                                        changeDiceState(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getStateChange());
                                        if (takeSkillPacketArr[takeSkillArrIdx].getSkillType() == 0)
                                        { //대미지는 주었지만한 경우(현재 버프에 대한 구분이 없어서 추후 수정필요)
                                            characterDamageMove(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getVal());
                                            makeHitEffect(tempTargetIdx);
                                            battleAnimationControl(tempTargetIdx, 1);
                                        }
                                    }
                                }
                                
                            }
                            else // 적군 대상으로 스킬이 들어온 경우
                            {
                                if (enemyCharacter[tempTargetIdx - 4] != null && enemyCharacter[tempTargetIdx - 4].getCurState() == 0) //대상 존재시 damage text 출력
                                {
                                    GameObject temp = Instantiate(damageTextObj, enemyCharacterObjUI[tempTargetIdx - 4].transform.position + new Vector3(0, 45, 0), new Quaternion(0, 0, 0, 0)); //적용된 것에 대한 텍스트 생성
                                    temp.GetComponent<damageMove>().textChange(takeSkillPacketArr[takeSkillArrIdx].getVal());
                                    //사망한경우
                                    if (enemyCharacter[tempTargetIdx - 4].TakeSkillPacket(takeSkillPacketArr[takeSkillArrIdx]))
                                    {
                                        characterDamageMove(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getVal());
                                        makeHitEffect(tempTargetIdx);
                                        battleAnimationControl(tempTargetIdx, 2);
                                        DeadCharacterUpdate(tempTargetIdx);
                                    }
                                    else //사망 하지 않은 경우
                                    {
                                        changeDiceState(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getStateChange());
                                        if (takeSkillPacketArr[takeSkillArrIdx].getSkillType() == 0)
                                        { //대미지는 주었지만한 경우(현재 버프에 대한 구분이 없어서 추후 수정필요)
                                            characterDamageMove(tempTargetIdx, takeSkillPacketArr[takeSkillArrIdx].getVal());
                                            makeHitEffect(tempTargetIdx);
                                            battleAnimationControl(tempTargetIdx, 1);
                                        }
                                    }
                                }

                                

                            }
                            updateHp();
                            updateEnemyDiceUI();
                            updateBattleUI();
                        }
                        skillAnimationControl(false, 3, i, curSkill, skillUseCharacter, -999, skillUseIdx);//타겟팅 전 애니메이션 실행
                        yield return new WaitUntil(() => enemyCharacterAtkReady[skillUseCharacter] == 0);

                        if (i + 1 < curSkill.getTargetChance())
                        { //공격 후에 공격기회가 더 남았으면 다시 뒤로 땡기기
                            yield return new WaitForSeconds(0.2f);
                            skillAnimationControl(false, 1, 0, curSkill, skillUseCharacter, -999, skillUseIdx);//타겟팅 전 애니메이션 실행
                            yield return new WaitUntil(() => enemyCharacterAtkReady[skillUseCharacter] == 2);
                        }

                    }


                    //
                    nextSkill = 0;
                    yield return new WaitForSeconds(1.0f);
                }
                nextDice++;
            }
            nextDice = 0;
            //배틀 끝나서 모두 사용됨.
            for (int i=0;i<4;i++)
            {
                for (int j=0;j<2;j++)
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
        if(input < 4)
        {
            clickDice_battlePhase = myDiceTake[input];
        }
        else
        {
            clickDice_battlePhase = enemyDiceTake[input - 4];
        }
        Debug.Log(clickDice_battlePhase);
        
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
        
        for (int i=0;i<3;i++)
        {
            int j = Random.Range(0, 4);
            int k = Random.Range(1, itemManager.Instance.getItemListCount(j));
            resultItem[i] = itemManager.Instance.getItem(j, k);
        }
    }
    string[] typeArr = { "consume", "dice", "equip", "passive", "destiny" };
    private void printRandomResult(int i, bool pointOn)
    {
        if(pointOn) resultObj[i, 0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/battleResultUI/spr_selectUI_board");
        else resultObj[i, 0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/battleResultUI/spr_selectUI_board_" + resultItem[i].getRare());

        resultObj[i, 1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/" + typeArr[resultItem[i].getType()] + "ItemSprite/spr_item_" + typeArr[resultItem[i].getType()] + "_" + resultItem[i].getItemName());
            resultObj[i, 2].GetComponent<TextMeshPro>().text = resultItem[i].getItemName();
            resultObj[i, 3].GetComponent<TextMeshPro>().text = typeArr[resultItem[i].getType()] + "\n\n" + resultItem[i].getContent();
    }
    public void pointEnterRandomResult(int i){ printRandomResult(i, true);}
    public void pointExitRandomResult(int i) { printRandomResult(i, false); }

    bool bosang_click = false;
    private IEnumerator EndPhase_Coroutine()
    {
        itemManager.Instance.flipItemBox_BattleUI(); //켜진 item box 끄기
        yield return new WaitForSeconds(0.2f);
        int result = winningCheck();
        //아군 전멸
        if(result == 2)
        {
            //itemManager.Instance.endOfBattlePhase();
            Debug.Log("you lose!");

            //AdventureManager.Instance.loseGame();
            CameraManager.Instance.resultScreenActive(0);
            yield return new WaitUntil(() => !(CameraManager.Instance.getLoseScreenActive()));


            if (characterInfoOpen) clickCharacterInfoBox();
            //CameraManager.Instance.loseScreenUnActive();
            AdventureManager.Instance.exitBattleCanvas(false); // 게임이 오버되었음을 전달

            for(int i=0;i<4;i++)
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

            if (bossPhase == 0)
            {
                itemManager.Instance.endOfBattlePhase();
                yield return new WaitForSeconds(0.5f);
                for (int i = 0; i < 4; i++)
                {
                    myHpUI[i].GetComponent<TextMeshPro>().text = "";
                    enemyHpUI[i].GetComponent<TextMeshPro>().text = "";
                }

                //랜덤 아이템 배정하고 출력
                makeRandomResult();
                for (int i = 0; i < 3; i++) printRandomResult(i, false);

                resultObj_all.transform.position = new Vector3(0f, -0f, resultObj_all.transform.position.z);

                CharacterManager.Instance.character_reset();
                for (int i = 0; i < 4; i++) //캐릭터 원래 위치에 character 넣기
                {
                    if (myCharacter[i] == null || myCharacter[i].getCurState() != 0) continue;
                    if (myCharacter[i].getCharacter_battle().getOriginIdx() >= 0 && myCharacter[i].getCharacter_battle().getOriginIdx() <= 3)
                    {
                        CharacterManager.Instance.character_battleEnd_deepCopy(myCharacter[i].getCharacter_battle().getOriginIdx(), myCharacter[i]);
                    }
                }


                Debug.Log("you win!");
                bosang_click = true;
                yield return new WaitUntil(() => !bosang_click); //필요한 캐릭터만큼 클릭된 경우 click 이벤트 종료!
                while (!AdventureManager.Instance.exitBattleCanvas(true))
                {
                    yield return new WaitForSeconds(0.5f);
                }


                resultExitBtn.transform.position = new Vector3(0f, 300f, resultExitBtn.transform.position.z);
                for (int i = 0; i < 4; i++)
                {
                    enemyDiceState[i] = 0;
                    enemyDiceStateAnim[i].Play("0");
                    myDiceState[i] = 0;
                    myDiceStateAnim[i].Play("0");
                }
            }
            else if (bossPhase == 100) //안경 선배가 보스고 1페이즈 인경우
            {
                bossPhase = 0; //보스 페이즈를 0으로 변경
                curPhase = 1;
                setEnemyCharacter(1, 10013);
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

        int result = itemManager.Instance.getItemResult(resultItem[i].getType(), resultItem[i].getIdx());
        if (result == 0)
        {
            resultObj_all.transform.position = new Vector3(0f, 300f, resultObj_all.transform.position.z);

            resultExitBtn.transform.position = new Vector3(171f, -37.5f, resultExitBtn.transform.position.z);
        }
    }
    public void click_backToAdventure()
    {

        if (bosang_click)
        {
            bosang_click = false;
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
       
        //초반 turn 화살표 지우기
        for (int i = 0;i<8;i++) {
            skillSelectUI[i] = GameObject.Find("obj_skillSelect_" + i.ToString());
            
            diceArrowSet[i] = GameObject.Find("arrowSet_" + i.ToString());
            diceUIChk[i] = GameObject.Find("obj_skillChk_" + i.ToString());
            diceArrowSet[i].SetActive(false);
        }
        //

        


        skillSelectDescUI[0] = GameObject.Find("battle_skill_skillImage_selected"); //스킬 이미지
        skillSelectDescUI[1] = GameObject.Find("battle_skill_selected_exp"); //텍스트
        skillSelectDescUI[6] = GameObject.Find("battle_skill_selected_title"); //스킬 명
        for (int i=0;i<4;i++)
        {
            myCharacterSwing[i] = 0;
            enemyCharacterSwing[i] = 0;
            myCharacterPunch[i] = 0;
            enemyCharacterPunch[i] = 0;
            for (int j = 0; j < 2; j++)
            {
                Debug.Log("obj_myCharacter_fire_" + i.ToString() + j.ToString());
                myFireObj[i, j] = GameObject.Find("obj_myCharacter_fire_" + i.ToString() + j.ToString()).GetComponent<ParticleSystem>();
                myFireObj[i, j].Stop();
                enemyFireObj[i, j] = GameObject.Find("obj_enemyCharacter_fire_" + i.ToString() + j.ToString()).GetComponent<ParticleSystem>();
                enemyFireObj[i, j].Stop();
            }
            myDiceStateAnim[i] = GameObject.Find("obj_myDiceState_" + i.ToString()).GetComponent<Animator>();
            enemyDiceStateAnim[i] = GameObject.Find("obj_enemyDiceState_" + (i+4).ToString()).GetComponent<Animator>();

            skillSelectDescUI[i+2] = GameObject.Find("battle_skill_needDice_selected_" + i.ToString()); //스킬에 필요한 주사위 종류

            myCharacterObjEntityUI[i] = GameObject.Find("obj_myCharacter_entity_" + i.ToString());
            enemyCharacterObjEntityUI[i] = GameObject.Find("obj_enemyCharacter_entity_" + i.ToString());

            myCharacterObjUI[i] = GameObject.Find("obj_myCharacter_" + i.ToString());
            enemyCharacterObjUI[i] = GameObject.Find("obj_enemyCharacter_" + i.ToString());

            myCharacterPosition[i] = myCharacterObjEntityUI[i].transform.position;
            enemyCharacterPosition[i] = enemyCharacterObjEntityUI[i].transform.position;

            myCharacterObjUIAnim[i] = myCharacterObjUI[i].GetComponent<Animator>();
            enemyCharacterObjUIAnim[i] = enemyCharacterObjUI[i].GetComponent<Animator>();

            myCharacterShadowObjUI[i] = GameObject.Find("obj_myBattleShadow_" + i.ToString());
            enemyCharacterShadowObjUI[i] = GameObject.Find("obj_enemyBattleShadow_" + i.ToString());

            myCharacterInitPosition[i] = myCharacterObjEntityUI[i].transform.position;
            enemyCharacterInitPosition[i] = enemyCharacterObjEntityUI[i].transform.position;

        }

        resultObj_all = GameObject.Find("bosang_ui");
        resultExitBtn = GameObject.Find("obj_itemUI_battleEndBtn");
        for (int i=0;i<3;i++)
        {
            resultObj[i, 0] = GameObject.Find("obj_resultUI_board_" + i.ToString());
            resultObj[i, 1] = GameObject.Find("obj_resultUI_itemLogo_" + i.ToString());
            resultObj[i, 2] = GameObject.Find("obj_resultUI_itemName_" + i.ToString());
            resultObj[i, 3] = GameObject.Find("obj_resultUI_itemDesc_" + i.ToString());

            diceUIChain[i] = GameObject.Find("obj_myChain_" + i.ToString());
            diceUIChain[i+3] = GameObject.Find("obj_enemyChain_" + i.ToString());
        }

        characterUI = GameObject.Find("CharacterUI");
        skillSelectUI[8] = GameObject.Find("skillSelectUI");

        witchPowerObj[0] = GameObject.Find("obj_witchPower");
        witchPowerObj[1] = GameObject.Find("witchPower_button_left");
        witchPowerObj[2] = GameObject.Find("witchPower_button_right");

        backGroundObj[0] = GameObject.Find("obj_backGround_field");
        backGroundObj[1] = GameObject.Find("obj_backGround_witch_witchPowerSelect");
        backGroundObj[2] = GameObject.Find("obj_backGround_backGround");
        backGroundObj[3] = GameObject.Find("obj_backGround_witch_skillSelect_body");
        backGroundObj[4] = GameObject.Find("obj_backGround_witch_skillSelect_face");

        // Hp 관련 UI, targeting을 위한 object find 
        for (int i = 0; i < 4; i++)
        {
            enemyHpUI[i] = GameObject.Find("obj_enemyCharacterHp_" + i.ToString());
            myHpUI[i] = GameObject.Find("obj_myCharacterHp_" + i.ToString());
            battleTargetUI[i] = GameObject.Find("obj_battleTarget_" + i.ToString());
            battleTargetUI[i + 4] = GameObject.Find("obj_battleTarget_" + (i + 4).ToString());
        }

        battleTextObj = GameObject.Find("obj_battleText");
        battleTextObj.GetComponent<TextMeshPro>().text = "";

        battleTextObj = GameObject.Find("obj_battleText");

        
        battleDescBox = GameObject.Find("board_descBoard");
        battleDescBoxCharacter = GameObject.Find("board_battle_Info_character");
        battleDescBoxName = GameObject.Find("board_battle_Info_name");
        battleDescBoxInfo = GameObject.Find("board_battle_Info_value");
        skillDescBox = GameObject.Find("ui_battle_board_skill");
        equipDescBox = GameObject.Find("ui_battle_board_equip");
        diceDescBox = GameObject.Find("ui_battle_board_dice");
        for (int i=0;i<2;i++)
        { 
            skillDescBox_title[i] = GameObject.Find("board_skillDesc_skillTitle_" + i.ToString());
            skillDescBox_info[i] = GameObject.Find("board_skillDesc_skillInfo_" + i.ToString());
            skillDescBox_image[i] = GameObject.Find("board_skillDesc_skillImage_" + i.ToString());

            equipDescBox_title[i] = GameObject.Find("board_equipDesc_equipTitle_" + i.ToString());
            equipDescBox_info[i] = GameObject.Find("board_equipDesc_equipInfo_" + i.ToString());
            equipDescBox_image[i] = GameObject.Find("board_equipDesc_equipImage_" + i.ToString());

            for (int j=0;j<4;j++) {
                faceDesc[i, j] = GameObject.Find("board_skillDesc_faceImage_" + (i * 4 + j).ToString());
                skillDescBox_dice[i, j] = GameObject.Find("board_skill_needDice_"+i.ToString() +"_"+j.ToString());
            }
        }
        for (int i=0;i<6;i++)
        {
            diceDesc[i] = GameObject.Find("board_skillDesc_dice_" + i.ToString());
        }
        //주사위 정보로 먼저 ui 출력
        curSelectInfo = 0;
        diceDescBox.SetActive(true);
        skillDescBox.SetActive(false);
        equipDescBox.SetActive(false);

        curPhase = 0;
        //마녀 능력 임시 배치
        witchPowerIdx[0] = 0;
        //witchPowerIdx[1] = 1;
        //witchPowerIdx[2] = 2;
    }

    private float[] myCharacterPunch = { 0, 0, 0, 0 };
    private float[] myCharacterSwing = { 0, 0, 0, 0 };
    private float[] enemyCharacterPunch = { 0, 0, 0, 0 };
    private float[] enemyCharacterSwing = { 0, 0, 0, 0 };
    private Vector3[] myCharacterPosition = new Vector3[4];
    private Vector3[] enemyCharacterPosition = new Vector3[4];

    private bool adventureStartChk = false;

    public void takeJsonWitchPower() //json에서 받은 파일 받아오기
    {
        witchPowerIdx[1] = jsonDataManager.Instance.getCurWitchPower(0);
        witchPowerIdx[2] = jsonDataManager.Instance.getCurWitchPower(1);
    }
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
        if (team){ //아군
            myCharacterAtkReady[idx] = changeVal;
        }
        else { 
            enemyCharacterAtkReady[idx] = changeVal;
        }
    }

    void FixedUpdate()
    {
        if (adventureStartChk)
        {
            for (int i = 0; i < 4; i++)
            {
                Debug.Log("bug test");
                Debug.Log(myCharacterPosition[i].x);
                Debug.Log(myCharacterSwing[i]);
                Debug.Log(myCharacterPunch[i]);
                if (myCharacterSwing[i] < 0.0) myCharacterSwing[i] = 0.0f;
                Debug.Log(myCharacterSwing[i] * Mathf.Sin(Mathf.PI * myCharacterPunch[i]));
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
                if (enemyCharacterSwing[i] < 0.0) enemyCharacterSwing[i] = 0.0f;
                enemyCharacterObjEntityUI[i].transform.position = new Vector3(
                        enemyCharacterPosition[i].x + (10 * enemyCharacterSwing[i] * Mathf.Sin(Mathf.PI * enemyCharacterPunch[i])),
                        enemyCharacterObjEntityUI[i].transform.position.y, enemyCharacterObjEntityUI[i].transform.position.z);

                enemyCharacterObjUI[i].transform.rotation = Quaternion.Euler(0, 0, enemyCharacterSwing[i] * Mathf.Sin(Mathf.PI * enemyCharacterPunch[i]) * -90);

                if (enemyCharacterAtkReady[i] == 0) //공격 준비 상태가 아닌경우 
                {
                    enemyCharacterPunch[i] += 0.05f;
                    if (enemyCharacterPunch[i] >= 2) enemyCharacterPunch[i] -= 2.0f;
                    if (enemyCharacterSwing[i] > 0) enemyCharacterSwing[i] -= swingDescVal(enemyCharacterSwing[i]);//0.005f;
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
    }
    private void characterDamageMove(int idx, int damage)
    {
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
        CameraManager.Instance.VibrateForeTime(0.1f, temp * 5);//데미지만큼 더 흔들리게

    }

    public void startBattle_fromAdventure()
    {
        for (int i = 0; i < 4; i++)
        {
            changeDiceState(i, 0);
            changeDiceState(i+4, 0);
            myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
        }
        adventureStartChk = true;
        StartCoroutine(phase_Manage_Coroutine());
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
    private GameObject[] battleTargetUI = new GameObject[8];

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
 
    public void Start_Battle_Phase()
    {

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
            StartCoroutine(makeBright(skillSelectUI[i * 2 + 0], 0.0f));
            StartCoroutine(makeBright(skillSelectUI[i * 2 + 1], 0.0f));

        //적군
        //enemyCharacter[i] = CharacterManager.Instance.getCharacter(false, i); //현재는 null인경우로 체크하는데 나중에 null말고 빈값을 주어야함.
        if (!CharacterManager.Instance.character_deepCopy(ref enemyCharacter[i], CharacterManager.Instance.getCharacter(false, i)))
            {
                enemyCharacter[i] = null;
            }

            if (enemyCharacter[i] != null && enemyCharacter[i].getCurState() == 0)
            {
                
                enemyDice[i] = new Dice();
            }
            else
            {
                enemyDice[i] = null;
                enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            }
            //어군
            //myCharacter[i] = CharacterManager.Instance.getCharacter(true, i);
            if(!CharacterManager.Instance.character_deepCopy(ref myCharacter[i], CharacterManager.Instance.getCharacter(true, i))){
                myCharacter[i] = null;
            }

            if (myCharacter[i] != null && myCharacter[i].getCurState() == 0)
            {
                myCharacter[i].getCharacter_battle().setOriginIdx(i); //돌아갈때 원래 위치 저장하기 위함
                myDice[i] = new Dice(myCharacter[i].getDiceObj());
            }
            else
            {
                myDice[i] = null;
                myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
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
                        skillSelectUI[i * 2 + j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + myCharacter[i].getSkillName(j));
                    }
                    else
                    {
                        skillSelectUI[i * 2 + j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_noImage");
                    }
                }
                else { skillSelectUI[i * 2 + j].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none"); }
            }
        }


        //배틀시 타겟에 대한 UI 비활성
        for (int i=0;i<8;i++)
        {
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

        updateHp();
        InitSetOfEnemySkill();
        updateInfoUIFaceUpdate();
        Debug.Log("StartPhase !");
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
