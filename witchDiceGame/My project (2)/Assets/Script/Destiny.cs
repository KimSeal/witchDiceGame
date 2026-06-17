using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destiny
{ 
    public int DestinyIdx;
    public string Enemy, Name, Sex;
    public int phyAtk, magAtk, speed, maxHp, shadow, money, armor;
    public int[] baseDiceNum = new int[6];
    //int[] needDiceArr = new int[10];

    public int[] skillMotion = new int[2];
    public Skill[] skillArr = new Skill[10];
    private int[] skillIdx = new int[10];
    //1-6 일반 값. 7 홀수 8 짝수 9 아무 값이나
    //private int[,] needDice = new int[10,4];

    public Destiny(Destiny destiny)
    {
        this.DestinyIdx = destiny.DestinyIdx;
        this.Enemy = destiny.Enemy; 
        this.Name = destiny.Name;
        this.Sex = destiny.Sex;
        this.phyAtk = destiny.phyAtk;
        this.magAtk = destiny.magAtk;
        this.speed = destiny.speed;
        this.maxHp = destiny.maxHp;
        this.shadow = destiny.shadow;
        this.armor = destiny.armor;
        for (int i=0;i<10;i++)
        {
            skillArr[i] = new Skill(destiny.getSkill(i));
            skillIdx[i] = destiny.getSkillIdx(i);
        }
        for (int i=0;i<skillMotion.Length;i++)
        {
            skillMotion[i] = destiny.skillMotion[i];
        }
        this.money = destiny.money;
        for (int i=0;i<baseDiceNum.Length;i++)
        {
            baseDiceNum[i] = destiny.baseDiceNum[i];
        }
    }

    public Destiny(DestinyReader destinyReader,  Skill[] skillSet)
    {
        this.DestinyIdx = destinyReader.DestinyIdx;
        this.Enemy = destinyReader.Enemy;
        this.Name = destinyReader.Name;
        this.Sex = destinyReader.Sex;
        this.phyAtk = destinyReader.phyAtk;
        this.magAtk = destinyReader.magAtk;
        this.speed = destinyReader.Speed;
        this.maxHp = destinyReader.maxHp;
        this.armor = destinyReader.Armor;
        
        this.shadow = destinyReader.shadow;
        for (int i=0;i<10;i++)
        {
            this.skillArr[i] = new Skill(skillSet[i]);
            this.skillIdx[i] = this.skillArr[i].getSkillIdx();
        }

        baseDiceNum[0] = destinyReader.BaseDiceNum0;
        baseDiceNum[1] = destinyReader.BaseDiceNum1;
        baseDiceNum[2] = destinyReader.BaseDiceNum2;
        baseDiceNum[3] = destinyReader.BaseDiceNum3;
        baseDiceNum[4] = destinyReader.BaseDiceNum4;
        baseDiceNum[5] = destinyReader.BaseDiceNum5;

        skillMotion[0] = destinyReader.skillMotion0;
        skillMotion[1] = destinyReader.skillMotion1;
        this.money = destinyReader.money;
        //diceToArr();
    }
    public int getArmor()
    {
        return this.armor;
    }
    public int getBaseDiceNum(int diceIdx)
    {
        return baseDiceNum[diceIdx];
    }
    public int getSkillMotion(int idx)
    {
        return this.skillMotion[idx];
    }
    public int getMoney() {
        return this.money;
    }
    public int getShadow()
    {
        return this.shadow;
    }
    public int getDestinyIdx()
    {
        return this.DestinyIdx;
    }
    public int getNeedDice(int idx)
    {
        return this.skillArr[idx].getNeedDice(); 
    }
    public Skill findSkill(int curSel)
    {
        return this.skillArr[curSel];
    }


    public string getName()
    {
        return this.Name;
    }
    public int getSkillIdx(int idx)
    {
        return this.skillIdx[idx];
    }
    public Skill getSkill(int idx)
    {
        return this.skillArr[idx];
    }
}
