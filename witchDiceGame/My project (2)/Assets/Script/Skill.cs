using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendSkillPacket //단일 공격에 대한 Packet이다.
{
    public int useCharacterIdx; //스킬을 사용한 주체
    public int useSkillIdx; //사용된 스킬이 플레이어의 몇번째 스킬인지 Idx
    public int[] targetIdx = new int[8]; // 대상이 되는 캐릭터의 index
    public int[] diceNum = new int[4];  // 현재 사용되는 주사위들에 배치된 값
    public int curChanceNum = 0;
    public SendSkillPacket(int useCharacterIdx, int useSkillIdx, int[] targetIdx,  int[] diceNum2)
    {
        this.useCharacterIdx = useCharacterIdx;
        this.useSkillIdx = useSkillIdx;
        for (int i=0;i<targetIdx.Length;i++)
        {
            this.targetIdx[i] = targetIdx[i];
        }
        int curIdx=0;
        for (int i = 0; i < 4; i++) 
        {
            this.diceNum[i] = -999;
        }
        for (int i=0;i<4;i++)
        {
            if(diceNum2[i] != -999)
            {
                this.diceNum[curIdx] = diceNum2[i];
                curIdx++;
            }
        }
        curChanceNum = 0;
    }
    public int getClickCharacter(int idx)
    {
        return targetIdx[idx];
    }
    public void addClickCharacter(int[] characterArr)
    {
        for (int i = 0; i < targetIdx.Length; i++) { 
            targetIdx[i] = characterArr[i];
        }
    }
    public void addChanceNum()
    {
        curChanceNum += 1;
    }
    
}
public class TakeSkillPacket //각각이 공격 하나하나에 대한 Packet
{
    public TakeSkillPacket(int targetIdx, int val, int stateChange)
    {
        this.targetIdx = targetIdx;
        this.val = val;
        this.stateChange = stateChange;
        this.skillType = 0;
        this.critical = false;
    }

    public TakeSkillPacket(int targetIdx, int val, int stateChange, int skillType)
    {
        this.targetIdx = targetIdx;
        this.val = val;
        this.stateChange = stateChange;
        this.skillType =skillType;
        this.critical = false;
    }


    private int targetIdx;
    private int val;
    private int stateChange;
    private int diceChange;

    private int skillType;
    private bool critical = false;
    // 0 : 데미지. 1: 회복, 2: 단순 상태 변경

    public int getTargetIdx()
    {
        return targetIdx;
    }
    public int getSkillType() { 
        return this.skillType;
    }

    public int getVal () { return val; }
    public int getStateChange() { return stateChange; }
    public void addVal(int val) { this.val += val; }
    public void mulVal(int val) { this.val *= val; Debug.Log(this.val); }
    public void setStateChange(int val) { this.stateChange = val; }
    public bool getCritical() { return critical; }
    public void setCritical() { this.critical = true; }
}


public class Skill 
{

    public int SkillIdx;
    public string SkillName;
    public int NeedDiceNum;
    public int[] NeedDice = new int[4];

    public int TargetAuto;
    public int TargetTeam;
    public int TargetObj;
    public int TargetNum;
    public int TargetChance;
    public int Var0;
    public int Var1;
    public int Var2;
    public string[] Command = new string[3];
    public int Anim;
    public int AnimMove;

    public int skillType=0, damage=5;
    public int atkCh=-1, defCh=-1;
    public int SpecialVal;
    public Skill(SkillReader skillReader)
    {
       this.SkillIdx = skillReader.SkillIdx;
        this.SkillName = skillReader.SkillName;
        this.NeedDiceNum = skillReader.NeedDiceNum;
        this.NeedDice[0] = skillReader.NeedDice0;
        this.NeedDice[1] = skillReader.NeedDice1;
        this.NeedDice[2] = skillReader.NeedDice2;
        this.NeedDice[3] = skillReader.NeedDice3;

        this.TargetAuto = skillReader.TargetAuto;
        this.TargetTeam = skillReader.TargetTeam;
        this.TargetObj = skillReader.TargetObj;
        this.TargetNum = skillReader.TargetNum;
        this.TargetChance = skillReader.TargetChance;
        this.Var0 = skillReader.Var0;
        this.Var1 = skillReader.Var1;
        this.Var2 = skillReader.Var2;
        this.Command[0] = TalkManager.Instance.SpecialTextChange(skillReader.CommandKR);
        this.Command[1] = TalkManager.Instance.SpecialTextChange(skillReader.CommandEN);
        this.Command[2] = TalkManager.Instance.SpecialTextChange(skillReader.CommandJP);

        this.Anim = skillReader.Anim;
        this.AnimMove = skillReader.AnimMove;
        this.SpecialVal = skillReader.SpecialVal;
    }
    public Skill(Skill skillReader)
    {
        this.SkillIdx = skillReader.SkillIdx;
        this.SkillName = skillReader.SkillName;
        this.NeedDiceNum = skillReader.NeedDiceNum;
        this.NeedDice[0] = skillReader.NeedDice[0];
        this.NeedDice[1] = skillReader.NeedDice[1];
        this.NeedDice[2] = skillReader.NeedDice[2];
        this.NeedDice[3] = skillReader.NeedDice[3];

        this.TargetAuto = skillReader.TargetAuto;
        this.TargetTeam = skillReader.TargetTeam;
        this.TargetObj = skillReader.TargetObj;
        this.TargetNum = skillReader.TargetNum;
        this.TargetChance = skillReader.TargetChance;
        this.Var0 = skillReader.Var0;
        this.Var1 = skillReader.Var1;
        this.Var2 = skillReader.Var2;
        for(int i=0;i<Command.Length;i++) this.Command[i] = skillReader.Command[i];

        this.Anim = skillReader.Anim;
        this.AnimMove = skillReader.AnimMove;
        this.SpecialVal = skillReader.SpecialVal;
    }

    //예전 전투 시스템에 사용된것
    public int getNeedDice()
    {
        return 9;
        //return this.NeedDiceNum;
    }

    public int getNeedDice(int idx)
    {
        return this.NeedDice[idx];
    }
    public int getNeedDiceNum()
    {
        return this.NeedDiceNum;
    }

    public string getSkillName()
    {
        return this.SkillName;
    }
    // Start is called before the first frame update
    public int getTargetNum()
    {
        return (int)this.TargetNum;
    }
    public int getTargetChance()
    {
        return ((int)this.TargetChance);
    }
    public int getSpecialVal() { return this.SpecialVal; }
    public int getTargetTeam()
    {
        return this.TargetTeam;
    }
    public int getSkillIdx()
    {
        return SkillIdx;
    }
    public string getCommand()
    {
        if(getSkillIdx() == 79) return this.Command[jsonDataManager.Instance.getLanguage()] + (" ( " + (itemManager.Instance.getItemNum(0) + itemManager.Instance.getItemNum(3)).ToString() + " )"); 
        if (getSkillIdx() == 80) return this.Command[jsonDataManager.Instance.getLanguage()] + (" ( " + itemManager.Instance.getItemNum(0).ToString() + " / " + itemManager.Instance.getItemNum(3).ToString() + " )");
        if (getSkillIdx() == 81) return this.Command[jsonDataManager.Instance.getLanguage()] + (" ( " + AdventureManager.Instance.getDeadEnemyCount().ToString() + " )");
        return this.Command[jsonDataManager.Instance.getLanguage()];
    }
    public int getVal(int idx)
    {
        if (idx == 0) return Var0;
        if (idx == 1) return Var1;
        if (idx == 2) return Var2;
        return -999;
    }
}


