using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager0 : MonoBehaviour
{
    [SerializeField]
    public Sprite[] diceSprite = new Sprite[6];
    [SerializeField]
    public GameObject[] myChkDice = new GameObject[4];
    [SerializeField]
    public GameObject[] enemyChkDice = new GameObject[4];
    [SerializeField]
    public GameObject chooseDiceObj;

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
    private static BattleManager0 instance = null;

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
    public GameObject DiceText;



    // 타겟팅시 일시정지를 위한 코루틴 저장함수.
    private IEnumerator battleTimer = null;

    //phase버튼 누를수 있는지
    private bool clickAble = true;
    public int curPhase = -1;
    // -1 : 아직 시작안함
    // 0 : Stage-Start 페이즈
    // 1 : Dice-Throw Phase
    // 2 : Dice-Fix Phase
    // 3 : Skill-Select 페이즈
    // 4 : Battle 페이즈
    // 5 : End-Phase


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

    public static BattleManager0 Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }

    void Start()
    {

    }
    void makeCharacterObj(int chrIdx)
    {
        if (myCharacter[chrIdx] != null && myCharacter[chrIdx].getCurState() == 0) {
            GameObject.Find("obj_character_my_" + chrIdx.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/man_0");
            GameObject.Find("obj_character_my_" + chrIdx.ToString()).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                   myCharacter[chrIdx].getDestiny().Name + "\n"
                   + myCharacter[chrIdx].getHp().ToString() + " / " + myCharacter[chrIdx].getMaxHp().ToString() + "\n";
        }
        else
        {
            GameObject.Find("obj_character_my_" + chrIdx.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            GameObject.Find("obj_character_my_" + chrIdx.ToString()).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                   " ";
        }
        if (enemyCharacter[chrIdx] != null && enemyCharacter[chrIdx].getCurState() == 0)
        {
            GameObject.Find("obj_character_enemy_" + chrIdx.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/slime_0");
            GameObject.Find("obj_character_enemy_" + chrIdx.ToString()).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                   enemyCharacter[chrIdx].getDestiny().Name + "\n"
                   + enemyCharacter[chrIdx].getHp().ToString() + " / " + enemyCharacter[chrIdx].getMaxHp().ToString() + "\n";
        }
        else
        {
            GameObject.Find("obj_character_enemy_" + chrIdx.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            GameObject.Find("obj_character_enemy_" + chrIdx.ToString()).transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                   " ";
        }
    }
    //UI 테스트
    void makeBtnText(int hello)
    {
        if (myCharacter[hello] != null)
        {
            GameObject.Find("Button_my_" + hello.ToString() + "0").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                myCharacter[hello].skillUse(0).SkillName + " : " + myCharacter[hello].needDice(0).ToString();
            GameObject.Find("Button_my_" + hello.ToString() + "1").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                 myCharacter[hello].skillUse(1).SkillName + " : " + myCharacter[hello].needDice(1).ToString();
        }
        else
        {
            GameObject.Find("Button_my_" + hello.ToString() + "0").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "no Hero Here!";
            GameObject.Find("Button_my_" + hello.ToString() + "1").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "no Hero Here!";
        }

        if (enemyCharacter[hello] != null)
        {
            GameObject.Find("Button_enemy_" + hello.ToString() + "0").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                enemyCharacter[hello].skillUse(0).SkillName + " : " + enemyCharacter[hello].needDice(0).ToString();
            GameObject.Find("Button_enemy_" + hello.ToString() + "1").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =
                 enemyCharacter[hello].skillUse(1).SkillName + " : " + enemyCharacter[hello].needDice(1).ToString();
        }
        else
        {
            GameObject.Find("Button_enemy_" + hello.ToString() + "0").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "no Enemy Here!";
            GameObject.Find("Button_enemy_" + hello.ToString() + "1").transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "no Enemy Here!";
        }
    }

    // Update is called once per frame
    void Update()
    {
        DiceText.GetComponent<TextMeshProUGUI>().text = curPhase.ToString();
    }
    public void debugDice()
    {
        string outputString = "";
        for (int i = 0; i < 4; i += 2)
        {
            outputString += i.ToString() + "th dice's state is " + myDiceTake[i].ToString() + " ";
            outputString += (i + 1).ToString() + "th dice's state is " + myDiceTake[i + 1].ToString() + "\n";
        }
        Debug.Log(outputString);
    }

    public void moveToNextPhase()
    {
        if (!clickAble) return;

        curPhase++;
        if (curPhase == 0) { Start_Battle_Phase(); }
        else if (curPhase == 1) { Dice_Throw_Phase(); }
        else if (curPhase == 2) { Dice_Fix_Phase(); }
        else if (curPhase == 3) { Skill_Select_Phase(); }
        else if (curPhase == 4) { Battle_Phase(); }
        else if (curPhase == 5)
        { // 추후 End Phase추가 필요
            End_Phase();
        }
    }

    public void End_Phase()
    {
        

        if (myCharacter[0] == null && myCharacter[1] == null && myCharacter[2] == null && myCharacter[3] == null)
        {
            for (int i = 0; i < 4; i++)
            {
                CharacterManager.Instance.setcharacterHp(i, 0);
            }
            Debug.Log("Enemy Team Win!");
            curPhase++;
            AdventureManager.Instance.adventureFadeClick();
            
            clearBattle();
            return;
        }
        if (enemyCharacter[0] == null && enemyCharacter[1] == null && enemyCharacter[2] == null && enemyCharacter[3] == null)
        {
            for (int i = 0; i < 4; i++)
            {
                if(myCharacter[i] == null) CharacterManager.Instance.setcharacterHp(i, 0);
                else CharacterManager.Instance.setcharacterHp(i, myCharacter[i].getHp());
            }
            Debug.Log("Our Team Win!");
            AdventureManager.Instance.stageClear();
            AdventureManager.Instance.adventureFadeClick();
            
            curPhase++;
            clearBattle();
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                mySkillUsed[i, j] = false;
                enemySkillUsed[i, j] = false;
            }
        }
        curPhase = 1;
        Dice_Throw_Phase();
        firstAttackTeam = (firstAttackTeam == 1) ? 2 : 1;
        Debug.Log("End Phase : firstAttackTeam is " + firstAttackTeam.ToString());
    }


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


    public void Start_Battle_Phase()
    {

        //선택된 주사위 이미지 초기화
        chooseDiceObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        //플레이를 위한 장치
        CharacterManager.Instance.setCharacter(0, 0);
        CharacterManager.Instance.setCharacter(1, 1);
        CharacterManager.Instance.setCharacter(2, 3);
        battleTimer = skillDo();

        DiceText = GameObject.Find("DiceCurText");
        //테스트를 위한 Character 세팅

        //UI test
        for (int i = 0; i < 3; i++)
        {
            enemyCharacter[i] = new Character(0, CharacterManager.Instance.destinyList[2]);
            if (enemyCharacter[i] != null) enemyDice[i] = new Dice();
        }
        for (int i = 0; i < 4; i++)
        {
            myCharacter[i] = CharacterManager.Instance.getCharacter(i);
            if (myCharacter[i] != null) myDice[i] = new Dice();
        }
        for (int i = 0; i < 4; i++)
        {
            myDiceUI[i] = GameObject.Find("obj_dice_my_" + i.ToString());
            enemyDiceUI[i] = GameObject.Find("obj_dice_enemy_" + i.ToString());
            makeBtnText(i);
            makeCharacterObj(i);
        }

        firstAttackTeam = Random.Range(1, 3);
        Debug.Log("StartPhase : firstAttackTeam is " + firstAttackTeam.ToString());

    }

    public void Dice_Throw_Phase()
    {
        if (curPhase != 1) { return; }
        //아군 모든 주사위 던지기
        for (int i = 0; i < 4; i++) {
            if (myDice[i] != null) {
                myDice[i].throwDice();
                myDiceNum[i] = myDice[i].getNum();
                //임시 주사위 UI 변경
                myDiceUI[i].transform.rotation = Quaternion.Euler(0, 0, myDice[i].dir * -90);
                myDiceUI[i].GetComponent<Image>().sprite = diceSprite[myDice[i].curIdx];
            }
        }
        //적군 모든 주사위 던지기
        for (int i = 0; i < 4; i++) {
            if (enemyDice[i] != null) {
                enemyDice[i].throwDice();
                enemyDiceNum[i] = enemyDice[i].getNum();

                enemyDiceUI[i].transform.rotation = Quaternion.Euler(0, 0, enemyDice[i].dir * -90);
                enemyDiceUI[i].GetComponent<Image>().sprite = diceSprite[enemyDice[i].curIdx];
            }
        }
        
    }

    void Dice_Fix_Phase()
    {
        if (curPhase == 3) return;
        //아군 후공일시 스킬이 제시되고 주사위가 수정된다.
        if (firstAttackTeam == 2){
            make_enemy_attack(7, 0);
        }
    }

    public void Skill_Select_Phase()
    {
        //아군 선공
        
    }


    public void Battle_Phase()
    {
        StartCoroutine(Battle_Phase_sub());
    }
    public IEnumerator Battle_Phase_sub()
    {
        clickAble = false;

        make_enemy_attack(7, 0);
        //아군 선공

        if (firstAttackTeam == 1)
        {
            //아군이 선공일때는 스킬을 다 세팅한 후에야 스킬 정해짐!

            battlePhaseState = 5;
            StartCoroutine(skillDo());
            yield return new WaitUntil(() => battlePhaseState != 5);

            battlePhaseState = 6;
            StartCoroutine(skillDo_enemy());
            yield return new WaitUntil(() => battlePhaseState != 6);

        }
        //적군 선공
        else if (firstAttackTeam == 2)
        {
            
            battlePhaseState = 6;
            StartCoroutine(skillDo_enemy());
            yield return new WaitUntil(() => battlePhaseState != 6);

            battlePhaseState = 5;
            StartCoroutine(skillDo());
            yield return new WaitUntil(() => battlePhaseState != 5);

        }
        clickAble=true;

    }

    public IEnumerator skillDo()
    {
        int hitManTemp = 0;
        int chooseSkill = 0;
        Skill useSkillTemp;

        for (int i = 0; i < 4; i++)
        {
            if (myDiceTake[i] != -999 && myCharacter[myDiceTake[i] / 10].getCurState() == 0)
            {
                hitManTemp = myDiceTake[i] / 10;
                chooseSkill = myDiceTake[i] % 10;
                useSkillTemp = myCharacter[hitManTemp].skillUse(chooseSkill);
                Debug.Log("Character " + hitManTemp.ToString() + " Use Skill " + useSkillTemp.SkillName);

                clickState = 1;
                StartCoroutine(SkillUse(useSkillTemp, hitManTemp));
                yield return new WaitUntil(() => clickState != 1);

                for (int k = 0; k < 4; k++) { makeCharacterObj(k); } //변동된 정보 등록
                for (int j = 3; j >= i; j--) if (myDiceTake[j] == myDiceTake[i])
                    {
                        myDiceTake[j] = -999;  //할당되었던 주사위 해제
                        myChkDice[j].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                        //GameObject.Find("obj_character_enemy_" + j.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                        mySkillUsed[hitManTemp, chooseSkill] = false;
                    }
            }
        }
        battlePhaseState = 0;
    }
    public IEnumerator skillDo_enemy()
    {
        int hitManTemp = 0;
        int chooseSkill = 0;
        Skill useSkillTemp;

        for (int i = 0; i < 4; i++)
        {
            if (enemyDiceTake[i] != -999 && enemyCharacter[enemyDiceTake[i] / 10].getCurState() == 0)
            {
                hitManTemp = enemyDiceTake[i] / 10;
                chooseSkill = enemyDiceTake[i] % 10;
                useSkillTemp = enemyCharacter[hitManTemp].skillUse(chooseSkill);
                Debug.Log("Character " + hitManTemp.ToString() + " Use Skill " + useSkillTemp.SkillName);

                clickState = 101;
                StartCoroutine(SkillUse_enemy(useSkillTemp, hitManTemp));
                yield return new WaitUntil(() => clickState != 101);

                for (int k = 0; k < 4; k++) { makeCharacterObj(k); } //변동된 정보 등록
                for (int j = 3; j >= i; j--) if (enemyDiceTake[j] == enemyDiceTake[i])
                    {
                        enemyDiceTake[j] = -999;  //할당되었던 주사위 해제
                        enemyChkDice[j].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                        //GameObject.Find("obj_character_enemy_" + j.ToString()).GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                        enemySkillUsed[hitManTemp, chooseSkill] = false;
                    }
            }
        }
        battlePhaseState = 0;
    }


    public int enemy_target_auto()
    {
        int characterNum = 0;
        int targetNum = 0;
        for (int i=0;i<4;i++)
        {
            if (myCharacter[i] != null) characterNum++;
        }
        if (characterNum == 1) targetNum = 0;
        if (characterNum == 2)
        {
            targetNum = Random.Range(0, 3);
            if (targetNum > 0) targetNum = 1;
        }
        if (characterNum == 3)
        {
            targetNum = Random.Range(0, 6);
            if (targetNum > 2) targetNum = 2;
            else if (targetNum > 0) targetNum = 1;
        }
        if (characterNum == 4)
        {
            targetNum = Random.Range(0, 10);
            if (targetNum > 5) targetNum = 3;
            else if (targetNum > 2) targetNum = 2;
            else if (targetNum > 0) targetNum = 1;
        }
        for (int i=3;i>=0;i--)
        {
            if (myCharacter[i] != null)
            {
                targetNum--;
                if (targetNum < 0) return i;
            }
        }
        return 0;
    }
    //적군 공격 타킷

    public IEnumerator SkillUse(Skill skill, int atkMan)
    {
        Debug.Log("Stop battle!");
        if (skill.skillType == 0)
        {

            clickMonster = -1;
            Debug.Log("select Enemy!");
            yield return new WaitUntil(() => clickMonster != -1);

            Debug.Log("back To Battle!");
            //죽으면 이렇게 됨.
            if (enemyCharacter[clickMonster].damage(myCharacter[atkMan].getPhyAtk()) == 1) {
                Debug.Log("Character " + clickMonster + " Dead!");
                deadEventEnemy(clickMonster);
            }
            for (int i = 0; i < enemyCharacter.Length; i++)
            {
                makeBtnText(i);
                makeCharacterObj(i);
            }
        }
        else if (skill.skillType == 1)
        {

        }
        clickState = 0;

    }

    public IEnumerator SkillUse_enemy(Skill skill, int atkMan)
    {
        Debug.Log("Stop battle!");
        if (skill.skillType == 0)
        {

            clickSelf = -1;
            Debug.Log("select Own Character!");

            yield return new WaitForSeconds(2.0f);
            clickSelf = enemy_target_auto(); //이거 없애면 지정 타깃으로 테스트 가능
            
            yield return new WaitUntil(() => clickSelf != -1);

            Debug.Log("back To Battle!");
            //죽으면 이렇게 됨.
            if (myCharacter[clickSelf].damage(enemyCharacter[atkMan].getPhyAtk()) == 1)
            {
                Debug.Log("Character " + clickSelf + " Dead!");
                deadEvent(clickSelf);
            }
            for (int i = 0; i < myCharacter.Length; i++)
            {
                makeBtnText(i);
                makeCharacterObj(i);
            }
        }
        else if (skill.skillType == 1)
        {

        }
        clickState = 0;

    }

    //적이 죽었을때의 이벤트
    void deadEventEnemy(int clickMonster0)
    {
        //죽은 캐릭터의 주사위에 할당되었던 스킬의 해제
        int skillCur = enemyDiceTake[clickMonster0];
        if (skillCur != -999)
        {
            for (int i = 0; i < enemyCharacter.Length; i++)
            {
                if (enemyDiceTake[i] == skillCur)
                {
                    enemyChkDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                    enemyDiceTake[i] = -999;
                }
            }
            mySkillUsed[skillCur / 10, skillCur % 10] = false;
        }

        //해당 캐릭터로 할당된 주사위 전체 해제
        for (int i = 0; i < enemyCharacter.Length; i++)
        {
            if (enemyDiceTake[i] / 10 == clickMonster0)
            {
                enemyDiceTake[i] = -999;
                enemyChkDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            }
        }
        mySkillUsed[clickMonster0, 0] = false; mySkillUsed[clickMonster0, 1] = false;
        enemyDice[clickMonster0] = null;

        enemyCharacter[clickMonster0] = null;

    }
    void deadEvent(int clickMonster0)
    {
        //죽은 캐릭터의 주사위에 할당되었던 스킬의 해제
        int skillCur = myDiceTake[clickMonster0];
        if (skillCur != -999)
        {
            for (int i = 0; i < myCharacter.Length; i++)
            {
                if (myDiceTake[i] == skillCur)
                {
                    myChkDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                    myDiceTake[i] = -999;
                }
            }
            enemySkillUsed[skillCur / 10, skillCur % 10] = false;
        }

        //해당 캐릭터로 할당된 주사위 전체 해제
        for (int i = 0; i < myCharacter.Length; i++)
        {
            if (myDiceTake[i] / 10 == clickMonster0)
            {
                myDiceTake[i] = -999;
                myChkDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            }
        }
        enemySkillUsed[clickMonster0, 0] = false; enemySkillUsed[clickMonster0, 1] = false;
        myDice[clickMonster0] = null;

        myCharacter[clickMonster0] = null;

    }
    public void targetMonster_0(int monsterIdx)
    {
        if (clickState == 1 && enemyCharacter[monsterIdx] != null && enemyCharacter[monsterIdx].getCurState() == 0)
        {
            clickMonster = monsterIdx;
        }
    }
    public void targetSelf_0(int monsterIdx)
    {
        if (clickState == 101 && myCharacter[monsterIdx] != null && myCharacter[monsterIdx].getCurState() == 0)
        {
            clickSelf = monsterIdx;
        }
    }
    //주사위 회전을 위한 함수들
    public void clickDice(int diceIdx)
    {
        chooseDice = (diceIdx < 4) ? myDice[diceIdx] : enemyDice[diceIdx - 4];
        chooseDiceObj.transform.rotation = Quaternion.Euler(0, 0, chooseDice.dir * -90);
        chooseDiceObj.GetComponent<Image>().sprite = diceSprite[chooseDice.curIdx];
        chooseDiceIdx = diceIdx;
    }
    public void turnDice(int idx)
    {
        if (chooseDice != null)
        {
            chooseDice.turnDice(idx);
            chooseDiceObj.transform.rotation = Quaternion.Euler(0, 0, chooseDice.dir * -90);
            chooseDiceObj.GetComponent<Image>().sprite = diceSprite[chooseDice.curIdx];
        }
    }
    public void setDice()
    {
        if (chooseDice != null)
        {
            if (chooseDiceIdx < 4)
            {
                myDice[chooseDiceIdx] = new Dice(chooseDice); //아군일때
                myDiceUI[chooseDiceIdx].transform.rotation = Quaternion.Euler(0, 0, myDice[chooseDiceIdx].dir * -90);
                myDiceUI[chooseDiceIdx].GetComponent<Image>().sprite = diceSprite[myDice[chooseDiceIdx].curIdx];
                myDiceNum[chooseDiceIdx] = myDice[chooseDiceIdx].getNum();
            }
            else
            {
                chooseDiceIdx -= 4;
                enemyDice[chooseDiceIdx] = new Dice(chooseDice);//적군일때
                enemyDiceUI[chooseDiceIdx].transform.rotation = Quaternion.Euler(0, 0, enemyDice[chooseDiceIdx].dir * -90);
                enemyDiceUI[chooseDiceIdx].GetComponent<Image>().sprite = diceSprite[enemyDice[chooseDiceIdx].curIdx];
                enemyDiceNum[chooseDiceIdx] = enemyDice[chooseDiceIdx].getNum();
            }
            chooseDiceObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            chooseDiceIdx = -999;
            chooseDice = null;
        }
    }

    /// 아군 적군 배틀시 스킬 사용 가능 여부 판단 함수들(start)
    public bool selectSkill_cal_con_sub(int opt, int a, int b)
    {
        if (opt == 0) return a == b;   //1-6 사이의 눈이 일치
        if (opt == 1) return a % 2 == 1;  //홀수 눈
        if (opt == 2) return a % 2 == 0; //짝수 눈
        return true;                    //주사위 눈 무관 사용
    }    //일치 홀수 짝수 무관 조건 확인  
    public int selectSkill_cal_con(int i, int[] needSkill, bool[] usedDiceTemp, int[] skillDice, int opt)
    {
        for (int j = 0; j < 4; j++)
        {
            //주사위 주인이 살아있고, 주사위 쓰고 있는 사람 없고, 이번 탐색에서도 선택된적 없으면서, 내 스킬에 필요한 눈인 경우
            if (!(myCharacter[j] == null || myCharacter[j].getCurState() != 0) &&
                myDice[j] != null && myDiceTake[j] < 0 && !usedDiceTemp[j] && selectSkill_cal_con_sub(opt, myDiceNum[j], needSkill[i]))
            {
                skillDice[i] = j;
                usedDiceTemp[j] = true;
                return 1;
            }
        }
        return 0;
    }  //아군 주사위 사용가능 여부 확인
    public int selectSkill_enemy_cal_con(int i, int[] needSkill, bool[] usedDiceTemp, int[] skillDice, int opt)
    {
        for (int j = 0; j < 4; j++)
        {
            //주사위 주인이 살아있고, 주사위 쓰고 있는 사람 없고, 이번 탐색에서도 선택된적 없으면서, 내 스킬에 필요한 눈인 경우
            if (!(enemyCharacter[j] == null || enemyCharacter[j].getCurState() != 0) &&
                enemyDice[j] != null && enemyDiceTake[j] < 0 && !usedDiceTemp[j] && selectSkill_cal_con_sub(opt, enemyDiceNum[j], needSkill[i]))
            {
                skillDice[i] = j;
                usedDiceTemp[j] = true;
                return 1;
            }
        }
        return 0;
    }  //적군 주사위 사용가능 여부 확인
    public void selectSkill_cal(int selCharacter, int selSkillNum)
    {
        int needDiceArr = myCharacter[selCharacter].needDice(selSkillNum);
        Debug.Log("We need " + needDiceArr.ToString());
        int[] needSkill = new int[4];
        needSkill[0] = needDiceArr / 1000;
        needSkill[1] = needDiceArr % 1000 / 100;
        needSkill[2] = needDiceArr % 100 / 10;
        needSkill[3] = needDiceArr % 10;

        //스킬 사용가능 여부 검색할때 쓰는 배열
        bool[] usedDiceTemp = new bool[4] { false, false, false, false };

        //스킬에 사용되는 주사위 인덱스
        int[] skillDice = new int[4] { -999, -999, -999, -999 };
        //스킬 성공여부 판단 변수
        int skillSuccess = 0;
        //스킬 사용시 중복 여부 제거

        for (int i = 0; i < 4; i++)
        {
            //주사위 미사용 / 주사위 1-6 / 짝수,홀수,무관 의 경우 
            if (needSkill[i] == 0) skillSuccess++;
            else if (needSkill[i] > 0 && needSkill[i] <= 6) { skillSuccess += selectSkill_cal_con(i, needSkill, usedDiceTemp, skillDice, 0); }
            else { skillSuccess += selectSkill_cal_con(i, needSkill, usedDiceTemp, skillDice, needSkill[i] % 6); }
        }
        if (skillSuccess == 4)
        {
            //가능한 경우!
            for (int i = 0; i < 4; i++)
            {
                if (skillDice[i] != -999)
                {
                    Debug.Log("Select Dice : " + skillDice[i]);
                    myDiceTake[skillDice[i]] = selCharacter * 10 + selSkillNum;
                    myChkDice[skillDice[i]].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_" + (selCharacter * 10 + selSkillNum).ToString());
                }
            }

            mySkillUsed[selCharacter, selSkillNum] = true;

        }
        else
        {
            Debug.Log(skillSuccess.ToString() + "You cant use your skill now!");
        }
    }   //아군 스킬 사용가능 여부 확인
    public void selectSkill_enemy_cal(int selCharacter, int selSkillNum)
    {
        int needDiceArr = enemyCharacter[selCharacter].needDice(selSkillNum);
        Debug.Log("We need " + needDiceArr.ToString());
        int[] needSkill = new int[4];
        needSkill[0] = needDiceArr / 1000;
        needSkill[1] = needDiceArr % 1000 / 100;
        needSkill[2] = needDiceArr % 100 / 10;
        needSkill[3] = needDiceArr % 10;

        //스킬 사용가능 여부 검색할때 쓰는 배열
        bool[] usedDiceTemp = new bool[4] { false, false, false, false };

        //스킬에 사용되는 주사위 인덱스
        int[] skillDice = new int[4] { -999, -999, -999, -999 };
        //스킬 성공여부 판단 변수
        int skillSuccess = 0;
        //스킬 사용시 중복 여부 제거

        for (int i = 0; i < 4; i++)
        {
            //주사위 미사용 / 주사위 1-6 / 짝수,홀수,무관 의 경우 
            if (needSkill[i] == 0) skillSuccess++;
            else if (needSkill[i] > 0 && needSkill[i] <= 6) { skillSuccess += selectSkill_enemy_cal_con(i, needSkill, usedDiceTemp, skillDice, 0); }
            else { skillSuccess += selectSkill_enemy_cal_con(i, needSkill, usedDiceTemp, skillDice, needSkill[i] % 6); }
        }
        if (skillSuccess == 4)
        {
            //가능한 경우!
            for (int i = 0; i < 4; i++)
            {
                if (skillDice[i] != -999)
                {
                    Debug.Log("Select Dice : " + skillDice[i]);
                    enemyDiceTake[skillDice[i]] = selCharacter * 10 + selSkillNum;
                    enemyChkDice[skillDice[i]].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_" + (selCharacter * 10 + selSkillNum).ToString());
                }
            }

            enemySkillUsed[selCharacter, selSkillNum] = true;

        }
        else
        {
            Debug.Log(skillSuccess.ToString() + "You cant use your skill now!");
        }
    }   //적군 스킬 사용가능 여부 확인
    public void selectSkill(int selSkill)
    {
        if (curPhase != 3) return;
        //각 스킬은 00 10 20 30
        //          01 11 21 31 의 번호를 가진다.(아군 기준)
        Debug.Log("selSkill : " + selSkill.ToString());
        int selCharacter = selSkill / 10; //선택한 아군
        int selSkillNum = selSkill % 10;  // 선택된 n번째의 스킬

        //캐릭터가 배치되지 않았거나 활성화상태가 아닌경우.
        if (myCharacter[selCharacter] == null || myCharacter[selCharacter].getCurState() != 0)
        {
            Debug.Log("No Character!");
            return;
        }


        if (!mySkillUsed[selCharacter, selSkillNum])
        {  //스킬이 아직 사용되지 않은 경우
            selectSkill_cal(selCharacter, selSkillNum);//본인 스킬에 필요한 주사위 구해오기
        }
        else
        {   //스킬이 이미 사용된 경우
            Debug.Log("Realease Button : " + selSkill.ToString());
            for (int i = 0; i < 4; i++)
            {
                if (myDiceTake[i] == selSkill)
                {
                    myDiceTake[i] = -999;
                    myChkDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                }
            }
            mySkillUsed[selCharacter, selSkillNum] = false;
        }
    }   //스킬 선택
    public void selectSkill_enemy(int selSkill)
    {
        //각 스킬은 00 10 20 30
        //          01 11 21 31 의 번호를 가진다.(적군 기준)
        Debug.Log("selSkill : " + selSkill.ToString());
        int selCharacter = selSkill / 10; //선택한 아군
        int selSkillNum = selSkill % 10;  // 선택된 n번째의 스킬

        //캐릭터가 배치되지 않았거나 활성화상태가 아닌경우.
        if (enemyCharacter[selCharacter] == null || enemyCharacter[selCharacter].getCurState() != 0)
        {
            Debug.Log("No Character!");
            return;
        }


        if (!enemySkillUsed[selCharacter, selSkillNum])
        {  //스킬이 아직 사용되지 않은 경우
            selectSkill_enemy_cal(selCharacter, selSkillNum);//본인 스킬에 필요한 주사위 구해오기
        }
        else
        {   //스킬이 이미 사용된 경우
            Debug.Log("Realease Button : " + selSkill.ToString());
            for (int i = 0; i < 4; i++)
            {
                if (enemyDiceTake[i] == selSkill)
                {
                    enemyDiceTake[i] = -999;
                    enemyChkDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                }
            }
            enemySkillUsed[selCharacter, selSkillNum] = false;
        }
    }   //적군 스킬 선택


    public bool selectSkill_enemy_cal_auto(int selCharacter, int selSkillNum)
    {
        int needDiceArr = enemyCharacter[selCharacter].needDice(selSkillNum);
        Debug.Log("We need " + needDiceArr.ToString());
        int[] needSkill = new int[4];
        needSkill[0] = needDiceArr / 1000;
        needSkill[1] = needDiceArr % 1000 / 100;
        needSkill[2] = needDiceArr % 100 / 10;
        needSkill[3] = needDiceArr % 10;

        //스킬 사용가능 여부 검색할때 쓰는 배열
        bool[] usedDiceTemp = new bool[4] { false, false, false, false };

        //스킬에 사용되는 주사위 인덱스
        int[] skillDice = new int[4] { -999, -999, -999, -999 };
        //스킬 성공여부 판단 변수
        int skillSuccess = 0;
        //스킬 사용시 중복 여부 제거

        for (int i = 0; i < 4; i++)
        {
            //주사위 미사용 / 주사위 1-6 / 짝수,홀수,무관 의 경우 
            if (needSkill[i] == 0) skillSuccess++;
            else if (needSkill[i] > 0 && needSkill[i] <= 6) { skillSuccess += selectSkill_enemy_cal_con(i, needSkill, usedDiceTemp, skillDice, 0); }
            else { skillSuccess += selectSkill_enemy_cal_con(i, needSkill, usedDiceTemp, skillDice, needSkill[i] % 6); }
        }
        if (skillSuccess == 4)
        {
            //가능한 경우!
            for (int i = 0; i < 4; i++)
            {
                if (skillDice[i] != -999)
                {
                    Debug.Log("Select Dice : " + skillDice[i]);
                    enemyDiceTake[skillDice[i]] = selCharacter * 10 + selSkillNum;
                }
            }
            enemySkillUsed[selCharacter, selSkillNum] = true;
            return true;
        }
        else
        {
            return false;
        }
    }   //적군 스킬 사용가능 여부 확인
    public bool selectSkill_enemy_auto(int selSkill)
    {
        //각 스킬은 00 10 20 30
        //          01 11 21 31 의 번호를 가진다.(적군 기준)
        Debug.Log("selSkill : " + selSkill.ToString());
        int selCharacter = selSkill / 10; //선택한 아군
        int selSkillNum = selSkill % 10;  // 선택된 n번째의 스킬

        //캐릭터가 배치되지 않았거나 활성화상태가 아닌경우.
        if (enemyCharacter[selCharacter] == null || enemyCharacter[selCharacter].getCurState() != 0)
        {
            Debug.Log("No Character!");
            return false;
        }


        if (!enemySkillUsed[selCharacter, selSkillNum])
        {  //스킬이 아직 사용되지 않은 경우
            return selectSkill_enemy_cal_auto(selCharacter, selSkillNum);//본인 스킬에 필요한 주사위 구해오기
        }
        else
        {   //스킬이 이미 사용된 경우
            Debug.Log("Realease Button : " + selSkill.ToString());
            for (int i = 0; i < 4; i++)
            {
                if (enemyDiceTake[i] == selSkill)
                {
                    enemyDiceTake[i] = -999;
                    enemyChkDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
                }
            }
            enemySkillUsed[selCharacter, selSkillNum] = false;
            return false;
        }
    }   //적군 스킬 선택

    ///스킬 자동 배치를 위한 변수 & 함수
    private int enemyDiceSelAuto = 0; // 적군 자동 스킬 배정시 사용되는 스킬들
    private int enemyDiceSelNumAuto = 0; // 적군 자동 스킬 배정시 사용되는 주사위 
    public void make_enemy_attack(int idx, int saveVal)
    {
        
        //시작할 경우 선택한 dice초기화
        if (idx == 7) {
            for (int i = 0; i < 4; i++)
            {
                if (enemyDiceTake[i] != -999)
                {

                    selectSkill_enemy(enemyDiceTake[i]);
                }
            }
            enemyDiceSelAuto = -999;
            enemyDiceSelNumAuto = -999;
        }
        bool nextSkillChk = false;
        for (int temp = idx; temp >= 0; temp--)
        {
            if (selectSkill_enemy_auto((temp / 2) * 10 + temp % 2))
            {
                nextSkillChk = true;
                make_enemy_attack(temp - 1, saveVal * 10 + temp+1);
                selectSkill_enemy_auto((temp / 2) * 10 + temp % 2);
            }
        }
        if (!nextSkillChk)// 더 쓸수 있는 스킬이 없는 경우
        { // 지금은 가장 많이 주사위를 사용하는 경우의 수를 찾는다.
            int diceSelNum = 0;
            for (int i=0;i<4;i++){
                if (enemyDiceTake[i] != -999) diceSelNum++;
            }
            if(enemyDiceSelNumAuto < diceSelNum)
            {
                enemyDiceSelNumAuto = diceSelNum;
                enemyDiceSelAuto = saveVal;
            }
        }
        //최대 경우 구한뒤 패턴따라 배치
        if(idx == 7)
        {
            Debug.Log("make Enemy Attack Pattern : " + enemyDiceSelAuto.ToString());
            int temp = 1;
            while (enemyDiceSelAuto >= temp)
            {
                temp *= 10;
            }
            temp /= 10;

            while (enemyDiceSelAuto > 0)
            {
                Debug.Log("sel Auto Skill" + ((enemyDiceSelAuto / temp - 1) / 2 * 10) + ((enemyDiceSelAuto / temp - 1) % 2).ToString());
                selectSkill_enemy(((enemyDiceSelAuto / temp - 1) / 2 * 10) + ((enemyDiceSelAuto / temp - 1) % 2));
                enemyDiceSelAuto %= temp;
                temp /= 10;
            }

        }

    }
}
