using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill 
{

    public int SkillIdx;
    public string SkillName;
    public int NeedDice;

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
        this.NeedDice = skillReader.NeedDice;
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
        this.NeedDice = skillReader.NeedDice;
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


    public int getNeedDice()
    {
        return this.NeedDice;
    }
    // Start is called before the first frame update

}


