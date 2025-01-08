using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character
{
    // 0 : 활성화 1: 미배정 2: 비활성화 3 : 사용불가
    protected int curState = 3;
    protected int level =0, exp = 0, phyAtk =0 , magAtk = 0, phyDef =0, magDef=0, 
        hp =0, maxHp = 0, armor=0, item1=-1, item2=-1;
    //버프, 디버프, 상태이상, 패시브, 지닌 주사위
    protected int[] skillIdx = new int[2] {0,1};
    protected Destiny destiny; //할당된 운명에 대한 클래스.

    
    public Character(int curState, Destiny destiny)
    {
        this.destiny = destiny;
        this.curState = curState;
        if (curState == 0 || curState == 2)
        {
            
            this.level = 1;
            this.exp = 0;

            this.phyAtk = destiny.phyAtk;
            this.magAtk = destiny.magAtk;
            this.phyDef = destiny.phyDef;
            this.magDef = destiny.magDef;
            this.maxHp = destiny.maxHp;
            this.hp = maxHp;

            skillIdx[0] = destiny.getSkillIdx(0); 
            skillIdx[1] = destiny.getSkillIdx(1);
        }
    }

    public int needDice(int skillNum)
    {
        return destiny.getNeedDice(skillIdx[skillNum]);
    } 

    public Destiny getDestiny()
    {
        return destiny;
    }
    public int getCurState(){ return curState; }
    public int getHp() {return hp; }
    public int getMaxHp() { return maxHp; }
    public Skill skillUse(int selNum)
    {
        return destiny.findSkill(skillIdx[selNum]);
    }

    public int getSkillIdx(int num)
    {
        return skillIdx[num];
    }

    public abstract List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket);

    public bool TakeSkillPacket(TakeSkillPacket takeSkillPacket)
    {
        this.hp -= takeSkillPacket.getDamage();
        Debug.Log("this damage is : " + takeSkillPacket.getDamage());
        Debug.Log("my remain Hp is : " + this.hp);

        if (this.hp <= 0)
        {
            this.hp = 0;
            this.curState = 2;
            return true;
        }
        return false;
    }

    public int getPhyAtk(){ return phyAtk; }
    public int getMagAtk() { return magAtk; }
    public int damage(int damage)
    {
        this.hp -= damage;
        if (this.hp <= 0)
        {
            this.curState = 2;
            return 1;
        }
        return 0;
    }
    public void setHp(int hp)
    {
        this.hp = hp;
        if (hp == 0 )
        {
            if(this.curState == 0) this.curState = 2;
        }
    }
    
}

