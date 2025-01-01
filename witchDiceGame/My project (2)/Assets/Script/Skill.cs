using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public string Command;


    public int skillType=0, damage=5;
    public int atkCh=-1, defCh=-1;

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
        this.Command = skillReader.Command;
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
        this.Command = skillReader.Command;
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

}


