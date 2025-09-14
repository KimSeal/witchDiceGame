using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManagerBackUp : MonoBehaviour
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
    private static BattleManagerBackUp instance = null;

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

    public GameObject DiceText;

    private GameObject characterUI;
    public GameObject diceFullUI;
    private GameObject[] diceArrowSet = new GameObject[8];

    private GameObject[] myCharacterObjUI = new GameObject[4];
    private Animator[] myCharacterObjUIAnim = new Animator[4];
    private GameObject[] enemyCharacterObjUI = new GameObject[4];
    private Animator[] enemyCharacterObjUIAnim = new Animator[4];

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
    // 4 : Battle 페이즈 - 주사위 세팅
    // 5 : Battle 페이즈 - 스킬에서 주사위 사용
    // 6 : End-Phase
    //-999 : 연타 방지(즉, 버그 차단을 위한 경우의 수)

    private int currentMoveUI = 0;
    private int currentLightUI = 0;

    private GameObject[] witchPowerObj = new GameObject[3];
    private GameObject[] backGroundObj = new GameObject[4];
    
    private int[] clickedDice = new int[2];
    
    // 적군이 주사위를 보고 스킬을 배치하게 하기 위한 변수들//
    Skill[] enemySkill = new Skill[8];
    int[] enemySkillDiceNum = new int[8];
    int[,] enemySkillDiceVal = new int[8, 4];


    //주사위 밑에 HP UI
    private GameObject[] myHpUI = new GameObject[4];
    private GameObject[] enemyHpUI = new GameObject[4];

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
    }
    private void InitSetOfEnemySkill()
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
    private void MakeEnemyAttackSet()
    {
        List<int> liveCharacterList = new List<int>();
        List<int> liveSkillList = new List<int>();
        for (int i = 0; i < 4; i++)
        {
            enemyDiceTake[i] = -999;
        }
        for (int i=0;i<4;i++)
        {
            if(enemyCharacter[i] != null && enemyCharacter[i].getCurState() == 0)
            {
                liveCharacterList.Add(i);
            }
        }

        for (int i=0;i<8;i++)
        {
            if (enemySkillDiceNum[i] != -999)
            {
                liveSkillList.Add(i); ;
            }
            
        }

        for (int skillIdx0=liveSkillList.Count-1; skillIdx0>=0;skillIdx0--)
        {
            int skillIdx = liveSkillList[skillIdx0];
            Debug.Log(skillIdx);
            for (int diceIdx=0; diceIdx <= liveCharacterList.Count - enemySkillDiceNum[skillIdx]; diceIdx++)
            {   //필요 주사위가 1칸인 경우
                
                if(enemySkillDiceNum[skillIdx] == 1)
                {
                    if (condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 0], enemyDiceNum[liveCharacterList[diceIdx]])){ // 첫번쨰주사위가 겹치는 경우
                        enemyDiceTake[liveCharacterList[diceIdx]] = (skillIdx % 4) * 10 + skillIdx /4;
                        liveCharacterList.RemoveAt(diceIdx);
                        break;
                    }

                }
                //필요 주사위가 2칸인 경우
                else if (enemySkillDiceNum[skillIdx] == 2)
                {
                    if (condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 0], enemyDiceNum[liveCharacterList[diceIdx]]) &&
                        condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 1], enemyDiceNum[liveCharacterList[diceIdx + 1]]))
                    { // 첫번쨰주사위가 겹치는 경우
                        enemyDiceTake[liveCharacterList[diceIdx]] = (skillIdx % 4) * 10 + skillIdx / 4;
                        enemyDiceTake[liveCharacterList[diceIdx+1]] = (skillIdx % 4) * 10 + skillIdx / 4;
                        liveCharacterList.RemoveAt(diceIdx);
                        liveCharacterList.RemoveAt(diceIdx);
                        break;
                    }

                }
                else if (enemySkillDiceNum[skillIdx] == 3) //필요 주사위가 3칸인 경우
                {
                    if (condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 0], enemyDiceNum[liveCharacterList[diceIdx]]) &&
                        condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 1], enemyDiceNum[liveCharacterList[diceIdx + 1]]) &&
                        condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 2], enemyDiceNum[liveCharacterList[diceIdx + 2]]))
                    { 
                        enemyDiceTake[liveCharacterList[diceIdx]] = (skillIdx % 4) * 10 + skillIdx / 4;
                        enemyDiceTake[liveCharacterList[diceIdx + 1]] = (skillIdx % 4) * 10 + skillIdx / 4;
                        enemyDiceTake[liveCharacterList[diceIdx + 2]] = (skillIdx % 4) * 10 + skillIdx / 4;
                        liveCharacterList.RemoveAt(diceIdx);
                        liveCharacterList.RemoveAt(diceIdx);
                        liveCharacterList.RemoveAt(diceIdx);
                        break;

                    }

                }
                else if (enemySkillDiceNum[skillIdx] == 4) //필요 주사위가 4칸인 경우
                {
                    if (condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 0], enemyDiceNum[liveCharacterList[diceIdx]]) &&
                        condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 1], enemyDiceNum[liveCharacterList[diceIdx + 1]]) &&
                        condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 2], enemyDiceNum[liveCharacterList[diceIdx + 2]]) &&
                        condition_diceSkillCheck(enemySkillDiceVal[skillIdx, 3], enemyDiceNum[liveCharacterList[diceIdx + 3]]))
                    {
                        enemyDiceTake[liveCharacterList[diceIdx]] = (skillIdx % 4) * 10 + skillIdx / 4;
                        enemyDiceTake[liveCharacterList[diceIdx + 1]] = (skillIdx % 4) * 10 + skillIdx / 4;
                        enemyDiceTake[liveCharacterList[diceIdx + 2]] = (skillIdx % 4) * 10 + skillIdx / 4;
                        enemyDiceTake[liveCharacterList[diceIdx + 3]] = (skillIdx % 4) * 10 + skillIdx / 4;
                        liveCharacterList.RemoveAt(diceIdx);
                        liveCharacterList.RemoveAt(diceIdx);
                        liveCharacterList.RemoveAt(diceIdx);
                        liveCharacterList.RemoveAt(diceIdx);
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
            if (myCharacter[i] != null && myCharacter[i].getCurState() == 0 && myDiceTake[i] == -999)
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
            if (condition_diceSkillCheck(skill.getNeedDice(0), myDiceNum[liveCharacterList[0]]))
            { // 첫번쨰주사위가 겹치는 경우
                myDiceTake[liveCharacterList[0]] = characterIdx  * 10 + skillSelIdx ;
                return true;
            }
        }
                //필요 주사위가 2칸인 경우
        else if (skill.getNeedDiceNum() == 2)
        {
            if (condition_diceSkillCheck(skill.getNeedDice(0), myDiceNum[liveCharacterList[0]]) &&
                condition_diceSkillCheck(skill.getNeedDice(1), myDiceNum[liveCharacterList[1]]))
            { // 첫번쨰주사위가 겹치는 경우
                myDiceTake[liveCharacterList[0]] = characterIdx * 10 + skillSelIdx;
                myDiceTake[liveCharacterList[1]] = characterIdx * 10 + skillSelIdx;
                return true;
            }

        }
        else if (skill.getNeedDiceNum() == 3)
        {
            if (condition_diceSkillCheck(skill.getNeedDice(0), myDiceNum[liveCharacterList[0]]) &&
                condition_diceSkillCheck(skill.getNeedDice(1), myDiceNum[liveCharacterList[1]]) &&
                condition_diceSkillCheck(skill.getNeedDice(2), myDiceNum[liveCharacterList[2]]))
            { // 첫번쨰주사위가 겹치는 경우
                myDiceTake[liveCharacterList[0]] = characterIdx * 10 + skillSelIdx;
                myDiceTake[liveCharacterList[1]] = characterIdx * 10 + skillSelIdx;
                myDiceTake[liveCharacterList[2]] = characterIdx * 10 + skillSelIdx;
                return true;
            }
        }
        else if (skill.getNeedDiceNum() == 4)
        {
            if (condition_diceSkillCheck(skill.getNeedDice(0), myDiceNum[liveCharacterList[0]]) &&
                condition_diceSkillCheck(skill.getNeedDice(1), myDiceNum[liveCharacterList[1]]) &&
                condition_diceSkillCheck(skill.getNeedDice(2), myDiceNum[liveCharacterList[2]]) &&
                condition_diceSkillCheck(skill.getNeedDice(3), myDiceNum[liveCharacterList[3]]))
            { // 첫번쨰주사위가 겹치는 경우
                myDiceTake[liveCharacterList[0]] = characterIdx * 10 + skillSelIdx;
                myDiceTake[liveCharacterList[1]] = characterIdx * 10 + skillSelIdx;
                myDiceTake[liveCharacterList[2]] = characterIdx * 10 + skillSelIdx;
                myDiceTake[liveCharacterList[3]] = characterIdx * 10 + skillSelIdx;
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
            else if (curPhase == 3) click_characterSkill_Dice(diceIdx);
            else if (curPhase == 5) click_BattleSkill_dice(diceIdx);
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

        if (characterIdx < 4)
        {
            mySkillUsed[characterIdx, skillIdx] = true;
        }
        else //적군이 사용할 경우 적군 주사위에 적용
        {
            enemySkillUsed[characterIdx, skillIdx] = true;
        }
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

    //phase관리를 위한 코루틴
    //(코루틴이 너무 중첩해서 생기는 거 방지를 위해 만들어둠)
    private IEnumerator phase_Manage_Coroutine()
    {
        Start_Battle_Phase();
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

        yield return new WaitUntil(() => currentLightUI == 0 && currentMoveUI == 0); //주사위 굴리는 애니메이션 추가 예정

        Dice_Throw_Phase();
        yield return new WaitForSeconds(1f);
        Debug.Log("phase change to 2!");
        curPhase = 2;
    }

    public void Dice_Throw_Phase()
    {
        if (curPhase != 1) { return; }
        //아군 모든 주사위 던지기
        for (int i = 0; i < 4; i++)
        {
            if (myDice[i] != null)
            {
                myDice[i].throwDice();
                myDiceNum[i] = myDice[i].getNum();
                //임시 주사위 UI 변경
                myDiceUI[i].transform.rotation = Quaternion.Euler(0, 0, myDice[i].dir * -90);
                myDiceUI[i].GetComponent<SpriteRenderer>().sprite = diceSprite[myDice[i].curIdx];
            }
        }

        //적군 모든 주사위 던지기
        for (int i = 0; i < 4; i++)
        {
            if (enemyDice[i] != null)
            {
                enemyDice[i].throwDice();
                enemyDiceNum[i] = enemyDice[i].getNum();

                enemyDiceUI[i].transform.rotation = Quaternion.Euler(0, 0, enemyDice[i].dir * -90);
                enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = diceSprite[enemyDice[i].curIdx];
            }
        }

        //임시로 넣어둠. 이곳에 적군 스킬 자동배치 함수가 들어가야 한다!
        
        
        MakeEnemyAttackSet();
        Debug.Log("Dice Throw Make enemy Array : " + enemyDiceTake[0].ToString() + " " + enemyDiceTake[1].ToString() + " " + enemyDiceTake[2].ToString() + " " + enemyDiceTake[3].ToString() + " ");
        updateEnemyDiceUI();
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

    //마녀 파워 선택 (좌우)
    public void witchPowerState_Change(int dir)
    {
        Debug.Log(witchPowerMoveState);
        if (curPhase == 2 && currentLightUI == 0 && currentMoveUI == 0)
        {
            
            if (witchPowerMoveState == 0) //마녀 파워 선택을 하는 경우.
            {
                
                GameObject witchPowerUI = GameObject.Find("obj_witchPower"); //==witchPowerObj[0];
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
                if (witchPowerState == 0) { witchPowerUI.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPower_noUse"); }
                else if (witchPowerState == 1) { witchPowerUI.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPower_turn"); }
                else if (witchPowerState == 2) { witchPowerUI.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPower_turn_blue"); }
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
                int witchPowerTemp = witchPowerState;
                int diceNum = 6;
                if (witchPowerTemp == 0) diceNum = 0; //능력을 사용하지 않는 경우
                if (witchPowerTemp == 1) diceNum = 1; // 능력이 turn이어서 주사위 하나만 쓰는 경우
                clickedDice[0] = -1;
                clickedDice[1] = -1;
                witchPowerClickState = diceNum;

                yield return new WaitUntil(() => witchPowerClickState == 0); //요구되는 주사위 수를 모두 채웠을때.

                if (witchPowerTemp == 1) //turn 스킬 사용시 
                {
                    Debug.Log("witchPowerClickState : " + witchPowerClickState.ToString());
                    diceArrowSet[clickedDice[0]].SetActive(true); //해당 주사위 화살표 활성화
                    yield return new WaitUntil(() => witchPowerClickState == -1);
                    diceArrowSet[clickedDice[0]].SetActive(false);
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
        if (witchPowerClickState > 0 && myCharacter[diceIdx] != null  && myDice[diceIdx] != null && currentLightUI == 0 && currentMoveUI == 0)
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
    /// Witch Power End (Phase 2- witch Power Select)///



    // Character Skill Select Start (Phase 3 - Character Skill Select)///

    private GameObject[] skillSelectUI = new GameObject[9];

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
        StartCoroutine(makeBright(backGroundObj[3], 0.0f));
        StartCoroutine(MoveUI(backGroundObj[3], 59f, 0.5f));

        StartCoroutine(MoveUI(characterUI, 0.0f)); //
        StartCoroutine(MoveUI(skillSelectUI[8], -50.0f)); //

        for (int i = 0; i < 4; i++)
        {
            StartCoroutine(makeBright(myCharacterObjUI[i], 0.0f));
            StartCoroutine(makeBright(enemyCharacterObjUI[i], 0.0f));
        }

        yield return new WaitUntil(() => currentMoveUI == 0 && currentLightUI == 0); //
    }



    //스킬 선택 중 버튼 클릭에 대한 코드
    public void click_characterSkill_Button(int input)
    {
        Debug.Log(curPhase);
        if (curPhase == 3 && currentLightUI == 0 && currentMoveUI == 0)
        {
            int characterIdx = input / 10;
            int skillIdx = input % 10;
            if (myCharacter[characterIdx] != null)
            {
                //현재 선택된게 없는 경우.
                if (curClickSkill == -1)
                {
                    //현재 주사위에 배치되어 있는 경우
                    if(mySkillUsed[characterIdx, skillIdx])
                    {
                        //할당된 주사위를 찾아 제거하는 코드
                        for (int i=0;i<4;i++)
                        {
                            if(myDiceTake[i] == input)
                            {
                                diceUIChk[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                                if (i < 3) diceUIChain[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");//연결 체인도 제거
                                myDiceTake[i] = -999;
                            }
                        }
                        curClickSkill = -1;
                        StartCoroutine(makeBright(skillSelectUI[characterIdx * 2 + skillIdx], 0.0f));
                        mySkillUsed[characterIdx, skillIdx] = false;
                    }
                    else
                    {
                        curClickSkill = input;
                        StartCoroutine(makeDark(skillSelectUI[characterIdx * 2 + skillIdx], 0.7f));
                    }
                    
                }
                else
                {
                    StartCoroutine(makeBright(skillSelectUI[(curClickSkill / 10) * 2 + (curClickSkill % 10)], 0.0f));
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
        return false;
    }

    //스킬 선택 중 주사위 클릭에 대한 코드
    private void click_characterSkill_Dice(int diceIdx)
    {
        if (curPhase == 3 && myCharacter[diceIdx] != null && currentLightUI == 0 && currentMoveUI == 0)
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
                        myDiceTake[i] = -999;
                    }
                }
                updateMyDiceUI();
                //해당 스킬에 대한 버튼 해제
                StartCoroutine(makeBright(skillSelectUI[(deleteSkill / 10) * 2 + (deleteSkill % 10)], 0.0f));
                mySkillUsed[(deleteSkill / 10), (deleteSkill % 10)] = false;
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
                }
                else //불가능한 경우
                {
                    StartCoroutine(makeBright(skillSelectUI[characterIdx * 2 + skillIdx], 0.0f));
                    Debug.Log("It can't! - wrong Dice Problem");
                }

                curClickSkill = -1;
            }
            //주사위에 할당된 스킬도 클릭된 스킬도 없다면 아무것도 하지 않는다.
        }
    }
    
    //phase넘어가기
    public void moveToBattlePhase()
    {
        if (curPhase == 3 && currentLightUI == 0 && currentMoveUI == 0)
        {
            curPhase = 4;
        }
    }

    private IEnumerator moveToBattlePhase_Coroutine()
    {
        if (curPhase == 4 && currentLightUI == 0 && currentMoveUI == 0)
        {
            curPhase = -999;
            StartCoroutine(MoveUI(diceFullUI, 33.0f));
            StartCoroutine(MoveUI(backGroundObj[0], -16.0f)); // 78f : skillSelect  62f: battle
            StartCoroutine(makeBright(backGroundObj[0], 0.0f));
            //StartCoroutine(MoveUI(backGroundObj[1], 10.0f));
            StartCoroutine(MoveUI(backGroundObj[1], -475f));
            StartCoroutine(makeDark(backGroundObj[3], 0.7f));
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
                    myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_noImage");
                }
                else
                {
                    skillNameTake = myCharacter[curDiceNum / 10].skillUse(curDiceNum % 10).getSkillName();
                    if (Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + skillNameTake) == null)
                    {
                        skillNameTake = "null";
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
                    enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_noImage");
                }
                else
                {
                    skillNameTake = enemyCharacter[curDiceNum / 10].skillUse(curDiceNum % 10).getSkillName();
                    if (Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + skillNameTake) == null)
                    {
                        skillNameTake = "null";
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
            Debug.Log("slime attack only 1 person!");
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
    private void makeMyDice_BattlePhase(int startIdx, int endIdx)
    {
        for (int i=0;i<4;i++){ //초기화
            makeDiceArrToMakePacket[i] = -999;
        }

        for (int i=0;i<= endIdx - startIdx;i++) { //유의미한 길이만큼 길이 생성
            makeDiceArrToMakePacket[i] = myDiceNum[startIdx + i];
        }
    }
    private void makeEnemyDice_BattlePhase(int startIdx, int endIdx)
    {
        for (int i = 0; i < 4; i++)
        { //초기화
            makeDiceArrToMakePacket[i] = -999;
        }

        for (int i = 0; i <= endIdx - startIdx; i++)
        { //유의미한 길이만큼 길이 생성
            makeDiceArrToMakePacket[i] = enemyDiceNum[startIdx + i];
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
            if (option == 1) myCharacterObjUIAnim[characterIdx].Play("Hit");
            else if(option == 2) myCharacterObjUIAnim[characterIdx].Play("Dead");
        }
        else
        {
            characterIdx -= 4;
            if (option == 1) enemyCharacterObjUIAnim[characterIdx].Play("Hit");
            else if (option == 2) enemyCharacterObjUIAnim[characterIdx].Play("Dead");
        }
    }

    private void DeadCharacterUpdate(int idx) //캐릭터가 죽을 경우(getcurstate가 2를 반환시) 작동한다. 
        //플레이어 죽음으로 맛있는데! 가 아니라 플레이어 받게 되면 애니메이션은 밖에서 해줌.
    {
        if (idx < 4)
        {
            int diceNumTemp = myDiceTake[idx]; //죽은 캐릭터가 지니고 있는 주사위를 사용한 스킬 들 해제


            for (int i=0;i<4;i++)   // 죽은 캐릭터가 가지고 있는 스킬 모두 해제.
            {
                if(myDiceTake[i] / 10 == idx)
                {
                    mySkillUsed[myDiceTake[i] / 10, myDiceTake[i] % 10] = false;
                    myDiceTake[i] = -999;
                    myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                } 
            }
            
            for (int i=0;i<4;i++) 
            {
                if (myDiceTake[i] == diceNumTemp) {
                    myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                    myDiceTake[i] = -999; 
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
                    enemyDiceTake[i] = -999;
                    enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                }
            }

            
            for (int i = 0; i < 4; i++)
            {
                if (enemyDiceTake[i] == diceNumTemp)
                {
                    enemyDiceTake[i] = -999;
                    enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                }
            }

            enemyDiceNum[idx] = -999;
            enemyDice[idx] = null;

            enemySkill[idx] = null;     //적군 스킬 attackset시 포함 안되도록 설정
            enemySkill[idx + 4] = null;
            enemySkillDiceNum[idx] = -999;
            enemySkillDiceNum[idx + 4] = -999;
            updateEnemyDiceUI();
        }
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
                    for (int i = 0; i < 4; i++)
                    {
                        if (myDiceTake[i] == nextSkill)
                        {
                            myDiceTake[i] = -999;
                            myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_null");
                            diceUIChk[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                            if (i != 3)  diceUIChain[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
                        }
                    }
                    //스킬이 사용 코드 적히는 부분
                    int skillUseCharacter = nextSkill / 10;
                    int skillUseIdx = nextSkill % 10;
                    Skill curSkill = myCharacter[skillUseCharacter].skillUse(skillUseIdx); //사용하는 스킬에 대한 정보를 받아온다.
                    
                    characterTargetIdx = 0;
                    for (int i=0;i<curSkill.getTargetChance();i++) { // 해당 스킬이 공격하는 숫자
                        characterTargetIdx = 0;
                        StartCoroutine(clickEnemy_Coroutine(curSkill.getTargetNum(), curSkill.getTargetTeam())); // 클릭 이벤트 시작
                        yield return new WaitUntil(() => characterTargetIdx == curSkill.getTargetNum()); //필요한 캐릭터만큼 클릭된 경우 click 이벤트 종료!
                        characterTargetIdx = -999;

                        //스킬에 대한 공격용 Packet 생성
                        makeMyDice_BattlePhase(nextDice, nextDice + curSkill.getNeedDiceNum() - 1);
                        SendSkillPacket sendSkillPacketTemp = new SendSkillPacket(skillUseCharacter, myCharacter[skillUseCharacter].getSkillIdx(skillUseIdx), clickCharacter, makeDiceArrToMakePacket);
                        takeSkillPacketArr.Clear();
                        takeSkillPacketArr = myCharacter[skillUseCharacter].doSkill(sendSkillPacketTemp);

                        int tempTargetIdx;
                        for (int takeSkillArrIdx = 0; takeSkillArrIdx < takeSkillPacketArr.Count; takeSkillArrIdx++)
                        {
                            tempTargetIdx = takeSkillPacketArr[takeSkillArrIdx].getTargetIdx();
                            if (tempTargetIdx < 4) //아군 대상으로 스킬이 들어온 경우
                            {
                                if (myCharacter[tempTargetIdx].TakeSkillPacket(takeSkillPacketArr[takeSkillArrIdx])) //반환 결과가 해당 캐릭터의 죽음 인경우
                                {
                                    battleAnimationControl(tempTargetIdx, 2);
                                    DeadCharacterUpdate(tempTargetIdx);
                                    updateMyDiceUI();
                                }
                                else
                                {  //대미지는 주었지만한 경우(현재 버프에 대한 구분이 없어서 추후 수정필요)
                                    battleAnimationControl(tempTargetIdx, 1);
                                }
                            }
                            else // 적군 대상으로 스킬이 들어온 경우
                            {
                                if (enemyCharacter[tempTargetIdx - 4].TakeSkillPacket(takeSkillPacketArr[takeSkillArrIdx])) //반환 결과가 해당 캐릭터의 죽음 인경우
                                {
                                    battleAnimationControl(tempTargetIdx, 2);
                                    DeadCharacterUpdate(tempTargetIdx);
                                    updateEnemyDiceUI();
                                }
                                else
                                { //대미지는 주었지만한 경우(현재 버프에 대한 구분이 없어서 추후 수정필요)
                                    battleAnimationControl(tempTargetIdx, 1);
                                }
                            }
                            updateHp();
                            updateMyDiceUI();
                        }

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
                            enemyDiceTake[i] = -999;
                            enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_null");
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

                    for (int i = 0; i < curSkill.getTargetChance(); i++)
                    { // 해당 스킬이 공격하는 숫자

                        makeEnemyClick(curSkill.getTargetNum(), curSkill.getTargetTeam()); // 적군의 공격 대상 만들기

                        //스킬에 대한 공격용 Packet 생성
                        makeEnemyDice_BattlePhase(nextDice, nextDice + curSkill.getNeedDiceNum() - 1);
                        SendSkillPacket sendSkillPacketTemp = new SendSkillPacket(skillUseCharacter, enemyCharacter[skillUseCharacter].getSkillIdx(skillUseIdx), clickCharacter, makeDiceArrToMakePacket);
                        
                        Debug.Log("it is only slime skill idx testLog : "+enemyCharacter[skillUseCharacter].getSkillIdx(skillUseIdx).ToString());
                        
                        takeSkillPacketArr.Clear();
                        takeSkillPacketArr = enemyCharacter[skillUseCharacter].doSkill(sendSkillPacketTemp);

                        int tempTargetIdx;
                        for (int takeSkillArrIdx = 0; takeSkillArrIdx < takeSkillPacketArr.Count; takeSkillArrIdx++)
                        {
                            
                            tempTargetIdx = takeSkillPacketArr[takeSkillArrIdx].getTargetIdx();
                            Debug.Log("target is...! : " + tempTargetIdx.ToString());
                            if (tempTargetIdx < 4) //아군 대상으로 스킬이 들어온 경우
                            {
                                if (myCharacter[tempTargetIdx].TakeSkillPacket(takeSkillPacketArr[takeSkillArrIdx]))
                                {
                                    battleAnimationControl(tempTargetIdx, 2);
                                    DeadCharacterUpdate(tempTargetIdx);
                                }
                                else { battleAnimationControl(tempTargetIdx, 1); }
                                
                            }
                            else // 적군 대상으로 스킬이 들어온 경우
                            {
                                if(enemyCharacter[tempTargetIdx - 4].TakeSkillPacket(takeSkillPacketArr[takeSkillArrIdx]))
                                {
                                    battleAnimationControl(tempTargetIdx, 2);
                                    DeadCharacterUpdate(tempTargetIdx);
                                }
                                else { battleAnimationControl(tempTargetIdx, 1); }
                                
                            }
                            updateHp();

                        }

                    }


                    //
                    nextSkill = 0;
                    yield return new WaitForSeconds(0.2f);
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
    private IEnumerator EndPhase_Coroutine()
    {
        int result = winningCheck();
        //아군 전멸
        if(result == 2)
        {
            Debug.Log("you lose!");
        }
        //적군 전멸
        else if (result == 1)
        {
            Debug.Log("you win!");
        }
        //전투 지속 필요
        else
        {
            curPhase = 1;
        }
        yield return new WaitForSeconds(0.2f);
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

    public static BattleManagerBackUp Instance
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

        for (int i=0;i<4;i++)
        {
            myCharacterObjUI[i] = GameObject.Find("obj_myCharacter_" + i.ToString());
            enemyCharacterObjUI[i] = GameObject.Find("obj_enemyCharacter_" + i.ToString());
            myCharacterObjUIAnim[i] = myCharacterObjUI[i].GetComponent<Animator>();
            enemyCharacterObjUIAnim[i] = enemyCharacterObjUI[i].GetComponent<Animator>();
        }

        for (int i=0;i<3;i++)
        {
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
        backGroundObj[3] = GameObject.Find("obj_backGround_witch_skillSelect");

        

        curPhase = 0;

    }

    // Update is called once per frame
    void Update()
    {
        DiceText.GetComponent<TextMeshProUGUI>().text = curPhase.ToString();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(phase_Manage_Coroutine());
        }
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
        if (alphaVal == 0.0f)
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
            //AdventureManager.Instance.adventureFadeClick();
            
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
            //AdventureManager.Instance.stageClear();
            //AdventureManager.Instance.adventureFadeClick();
            
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
    */
    private GameObject[] battleTargetUI = new GameObject[8];
    public void Start_Battle_Phase()
    {
        

        //선택된 주사위 이미지 초기화
        chooseDiceObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        //플레이를 위한 장치
        CharacterManager.Instance.setCharacter(0, 0);
        CharacterManager.Instance.setCharacter(1, 0);
        CharacterManager.Instance.setCharacter(2, 0);
        CharacterManager.Instance.setCharacter(3, 0);

        CharacterManager.Instance.setCharacter(0, 10001);
        CharacterManager.Instance.setCharacter(1, 10001);
        CharacterManager.Instance.setCharacter(2, 10001);


        //battleTimer = skillDo();

        DiceText = GameObject.Find("DiceCurText");
        //테스트를 위한 Character 세팅


        enemyHpUI[0] = GameObject.Find("obj_enemyCharacterHp_0");

        if (enemyHpUI[0] == null) Debug.Log("why? why aren't you come?");
        enemyHpUI[1] = GameObject.Find("obj_enemyCharacterHp_1");
        enemyHpUI[2] = GameObject.Find("obj_enemyCharacterHp_2");
        enemyHpUI[3] = GameObject.Find("obj_enemyCharacterHp_3");

        //UI test
        for (int i = 0; i < 4; i++)
        {
            //enemyHpUI[i] = GameObject.Find("obj_enemyCharacterHp_" + i.ToString());
            enemyCharacter[i] = CharacterManager.Instance.getCharacter(false, i);
            if (enemyCharacter[i] != null)
            {
                
                enemyDice[i] = new Dice();
            }
            else enemyDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            
        }
        for (int i = 0; i < 4; i++)
        {
            myHpUI[i] = GameObject.Find("obj_myCharacterHp_" + i.ToString());
            myCharacter[i] = CharacterManager.Instance.getCharacter(true, i);
            if (myCharacter[i] != null)
            {
                myDice[i] = new Dice();
                
            }
            else myDiceUI[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            
        }
        for (int i = 0; i < 4; i++)
        {
            //myDiceUI[i] = GameObject.Find("obj_dice_my_" + i.ToString());
            //enemyDiceUI[i] = GameObject.Find("obj_dice_enemy_" + i.ToString());
            
            //makeBtnText(i);
            //makeCharacterObj(i);
        }
        //배틀시 타겟에 대한 UI 비활성
        for (int i=0;i<8;i++)
        {
            battleTargetUI[i] = GameObject.Find("obj_battleTarget_" + i.ToString());
            battleTargetUI[i].SetActive(false);
        }

        for (int i=0;i<4;i++)
        {
            
            if (myCharacter[i] == null || myCharacter[i].getCurState() == 2)
            {
                //추후 null로 바꿀것
                myCharacterObjUIAnim[i].runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/Yongsa/animator_Yongsa");
            }
            else
            {
                string temp = myCharacter[i].getDestiny().getName();
                myCharacterObjUIAnim[i].runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + temp + "/animator_" + temp);
            }
            
            if (enemyCharacter[i] == null || enemyCharacter[i].getCurState() == 2)
            {
                //추후 null로 바꿀것
                enemyCharacterObjUIAnim[i].runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/Yongsa/animator_Yongsa");
            }
            else
            {
                string temp = enemyCharacter[i].getDestiny().getName();
                enemyCharacterObjUIAnim[i].runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + temp + "/animator_" + temp);
            }
            
        }


        witchPowerObj[0].SetActive(false);
        witchPowerObj[1].SetActive(false);
        witchPowerObj[2].SetActive(false);

        updateHp();
        InitSetOfEnemySkill();

        firstAttackTeam = Random.Range(1, 3);
        Debug.Log("StartPhase : firstAttackTeam is " + firstAttackTeam.ToString());
        curPhase = 1;
        
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

    /*
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
        chooseDiceObj.GetComponent<SpriteRenderer>().sprite = diceSprite[chooseDice.curIdx];
        chooseDiceIdx = diceIdx;
    }
    
    public void setDice()
    {
        if (chooseDice != null)
        {
            if (chooseDiceIdx < 4)
            {
                myDice[chooseDiceIdx] = new Dice(chooseDice); //아군일때
                myDiceUI[chooseDiceIdx].transform.rotation = Quaternion.Euler(0, 0, myDice[chooseDiceIdx].dir * -90);
                myDiceUI[chooseDiceIdx].GetComponent<SpriteRenderer>().sprite = diceSprite[myDice[chooseDiceIdx].curIdx];
                myDiceNum[chooseDiceIdx] = myDice[chooseDiceIdx].getNum();
            }
            else
            {
                chooseDiceIdx -= 4;
                enemyDice[chooseDiceIdx] = new Dice(chooseDice);//적군일때
                enemyDiceUI[chooseDiceIdx].transform.rotation = Quaternion.Euler(0, 0, enemyDice[chooseDiceIdx].dir * -90);
                enemyDiceUI[chooseDiceIdx].GetComponent<SpriteRenderer>().sprite = diceSprite[enemyDice[chooseDiceIdx].curIdx];
                enemyDiceNum[chooseDiceIdx] = enemyDice[chooseDiceIdx].getNum();
            }
            chooseDiceObj.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
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
    */
}
