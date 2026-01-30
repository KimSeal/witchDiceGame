using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destiny
{ 
    public int DestinyIdx;
    public string Enemy, Name, Sex;
    public int phyAtk, magAtk, phyDef, magDef, speed, maxHp, maxMp, shadow, money;
    //int[] needDiceArr = new int[10];

    public Skill[] skillArr = new Skill[10];
    private int[] skillIdx = new int[10];
    //1-6 일반 값. 7 홀수 8 짝수 9 아무 값이나
    //private int[,] needDice = new int[10,4];

    public Destiny(Destiny destiny)
    {
        this.maxMp = destiny.maxMp;
        this.DestinyIdx = destiny.DestinyIdx;
        this.Enemy = destiny.Enemy; 
        this.Name = destiny.Name;
        this.Sex = destiny.Sex;
        this.phyAtk = destiny.phyAtk;
        this.magAtk = destiny.magAtk;
        this.phyDef = destiny.phyDef;
        this.magDef = destiny.magDef;
        this.speed = destiny.speed;
        this.maxHp = destiny.maxHp;
        this.shadow = destiny.shadow;
        for (int i=0;i<10;i++)
        {
            skillArr[i] = new Skill(destiny.getSkill(i));
            skillIdx[i] = destiny.getSkillIdx(i);
        }
        this.money = destiny.money;
    }

    public Destiny(DestinyReader destinyReader,  Skill[] skillSet)
    {
        this.DestinyIdx = destinyReader.DestinyIdx;
        this.Enemy = destinyReader.Enemy;
        this.Name = destinyReader.Name;
        this.Sex = destinyReader.Sex;
        this.phyAtk = destinyReader.phyAtk;
        this.magAtk = destinyReader.magAtk;
        this.phyDef = destinyReader.phyDef;
        this.magDef = destinyReader.magDef;
        this.speed = destinyReader.Speed;
        this.maxHp = destinyReader.maxHp;
        this.maxMp = destinyReader.maxMp;
        
        this.shadow = destinyReader.shadow;
        for (int i=0;i<10;i++)
        {
            this.skillArr[i] = new Skill(skillSet[i]);
            this.skillIdx[i] = this.skillArr[i].getSkillIdx();
        }
        this.money = destinyReader.money;
        //diceToArr();
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
