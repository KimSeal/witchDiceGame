using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlackEyedRabbit : Character
{
    public BlackEyedRabbit(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();

        if (sendSkillPacket.useSkillIdx == 0)
        { //고블린의 첫번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getPhyAtk() + this.getPhyAtk(), 0));
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, this.getPhyAtk(), 0, 5));
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getSpeed() + this.getPhyAtk(), 0));
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, this.getSpeed(), 0, 2));
        }
        return packets;
    }
}

public class Vamdeer : Character
{
    public Vamdeer(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();

        if (sendSkillPacket.useSkillIdx == 0)
        { //고블린의 첫번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0));
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], -1 * (sendSkillPacket.diceNum[0] + this.getPhyAtk()), 0,2));
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0, 2));
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0));
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(),0, 12));
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0, 2));
        }
        return packets;
    }
}

public class Holemole : Character
{
    public Holemole(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();

        if (sendSkillPacket.useSkillIdx == 0)
        {//고블린의 두번째 스킬이 호출된 경우
            if (BattleManager.Instance.getCharacter(sendSkillPacket.targetIdx[0]) != null &&
                BattleManager.Instance.getCharacter(sendSkillPacket.targetIdx[0]).getCurState() == 0)
            {
                int armorNum = BattleManager.Instance.getCharacter(sendSkillPacket.targetIdx[0]).getArmor();
                for (int i=0;i <= armorNum;i++) {
                    packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0],
                        (sendSkillPacket.diceNum[0] + 1) * this.getPhyAtk(), 0));
                } 
            }
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        { //고블린의 첫번째 스킬이 호출된 경우
            if (BattleManager.Instance.getCharacter(sendSkillPacket.targetIdx[0]) != null &&
                BattleManager.Instance.getCharacter(sendSkillPacket.targetIdx[0]).getCurState() == 0)
            {
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0],
                    BattleManager.Instance.getCharacter(sendSkillPacket.targetIdx[0]).getHp() / 2 + this.getPhyAtk(), 0));
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 1, 0, 6));
            }
        }
        
        return packets;
    }
}


public class Dionaea : Character
{
    public Dionaea(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();

        if (sendSkillPacket.useSkillIdx == 0)
        { //고블린의 첫번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0));
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], -1 * (sendSkillPacket.diceNum[0] + this.getPhyAtk()), 0, 4));
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0, 4));
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0));
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0,14));
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0, 4));
        }
        return packets;
    }
}

public class Drosera : Character
{
    public Drosera(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();

        if (sendSkillPacket.useSkillIdx == 0)
        { //고블린의 첫번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0));
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], -1 * (sendSkillPacket.diceNum[0] + this.getPhyAtk()),0, 5));
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0, 5));
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + sendSkillPacket.diceNum[1] + this.getPhyAtk(), 0));
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + sendSkillPacket.diceNum[1] + this.getPhyAtk(),0, 15));
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, sendSkillPacket.diceNum[0] + sendSkillPacket.diceNum[1] + this.getPhyAtk(), 0, 5));
            if(this.getSpeed() >= 15) {
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + sendSkillPacket.diceNum[1] + this.getPhyAtk(), 0));
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + sendSkillPacket.diceNum[1] + this.getPhyAtk(), 0, 15));
                packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, sendSkillPacket.diceNum[0] + sendSkillPacket.diceNum[1] + this.getPhyAtk(), 0, 5));
            }
        }
        return packets;
    }
}

public class Nepenthes : Character
{
    public Nepenthes(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();

        if (sendSkillPacket.useSkillIdx == 0)
        { //고블린의 첫번째 스킬이 호출된 경우
            for (int i = 0; i < 4; i++)
            {
                if (BattleManager.Instance.getCharacter(i + 4) != null &&
                BattleManager.Instance.getCharacter(i + 4).getCurState() == 0)
                {
                    Character tempCharacter = BattleManager.Instance.getCharacter(i + 4);
                    packets.Add(new TakeSkillPacket(i+4, tempCharacter.getPhyAtk(), 0, 2));
                    packets.Add(new TakeSkillPacket(i + 4, tempCharacter.getMagAtk(), 0, 4));
                    packets.Add(new TakeSkillPacket(i + 4, tempCharacter.getSpeed(), 0, 5));
                }
            }
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
           
            if (BattleManager.Instance.getCharacter(sendSkillPacket.targetIdx[0]) != null &&
                BattleManager.Instance.getCharacter(sendSkillPacket.targetIdx[0]).getCurState() == 0)
            {
                int tempVal = BattleManager.Instance.getCharacter(sendSkillPacket.targetIdx[0]).getHp() / 5;
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], tempVal + this.getPhyAtk(), 0, 0));
                packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx+4, tempVal + this.getPhyAtk(), 0, 1));
                if (this.getPossible(20))
                {
                    tempVal = BattleManager.Instance.getCharacter(sendSkillPacket.targetIdx[0]).getHp() / 5;
                    packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], tempVal + this.getPhyAtk(), 0, 0));
                    packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx+4, tempVal + this.getPhyAtk(), 0, 1));
                }
            }
        }
        return packets;
    }
}

public class Rafflesia : Character
{
    public Rafflesia(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();

        if (sendSkillPacket.useSkillIdx == 0)
        { //고블린의 첫번째 스킬이 호출된 경우
            bool emptyChk = false;
            for (int i = 4; i < 8; i++)
            {
                if (BattleManager.Instance.getCharacter(i) == null ||
                    BattleManager.Instance.getCharacter(i).getCurState() != 0)
                {
                    if (sendSkillPacket.targetIdx[0] >= 0 && sendSkillPacket.targetIdx[0] < 4)
                    {
                        BattleManager.Instance.setEnemyCharacter(i-4, Random.Range(10040, 10042));
                        emptyChk = true;
                        break;
                    }
                }
            }
            if (!emptyChk) {
                for (int i = 0; i < 8; i++)
                {
                    if (i != sendSkillPacket.useCharacterIdx + 4)
                    {
                        if (BattleManager.Instance.getCharacter(i) != null &&
                        BattleManager.Instance.getCharacter(i).getCurState() == 0)
                        {
                            packets.Add(new TakeSkillPacket(i, 10, 0, 10)); //최대체력 제거
                            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 10, 0, 11)); //최대체력 증가
                        }

                    }
                }
            }
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            if (BattleManager.Instance.getCharacter(sendSkillPacket.targetIdx[0]) != null &&
                BattleManager.Instance.getCharacter(sendSkillPacket.targetIdx[0]).getCurState() == 0)
            {
                int tempVal = BattleManager.Instance.getCharacter(sendSkillPacket.targetIdx[0]).getHp() / 5;
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], tempVal, 0, 10));
                packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, tempVal, 0, 11));
            }
        }
        return packets;
    }
}
public class Assassin : Character
{
    public Assassin(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();

        if (sendSkillPacket.useSkillIdx == 0)
        { //고블린의 첫번째 스킬이 호출된 경우
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 1 + this.getPhyAtk(), 0)); 
                packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 1, 0, 6)); 
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j <= this.getArmor(); j++)
                {
                    packets.Add(new TakeSkillPacket(i, this.getPhyAtk() + this.getPhyAtk(), 0));
                }
            }
        }
        return packets;
    }

    public override int getTargetChance(int idx)
    {
        if (idx == 0)
        {
            this.character_battle.setSkillUseCount(0, this.character_battle.getSkillUseCount(0) + 1);
            return this.character_battle.getSkillUseCount(0);
        }
        return 0;
    }
}

public class Grace : Character
{
    public Grace(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();

        if (sendSkillPacket.useSkillIdx == 0)
        { //고블린의 첫번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], (this.getPhyAtk() + this.getMagAtk() + this.getSpeed()) + this.getPhyAtk(), 0)); //최대체력 제거
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], (this.getPhyAtk() + this.getMagAtk() + this.getSpeed()) / 3 + this.getPhyAtk(), 0)); //최대체력 제거
        }
        return packets;
    }

    public override int getTargetChance(int idx)
    {
        if (idx == 1)
        {
            int swordNum = 0;
            for (int i=4;i<8;i++)
            {
                if (BattleManager.Instance.getCharacter(i) != null && BattleManager.Instance.getCharacter(i).getCurState() == 0) swordNum += 1;
            }
            return swordNum;
        }
        return 0;
    }
}

public class BlueSoulRapier : Character
{
    public BlueSoulRapier(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();

        if (sendSkillPacket.useSkillIdx == 0)
        { //고블린의 첫번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getSpeed() + this.getPhyAtk(), 0)); //최대체력 제거
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            for(int i = 4; i < 8; i++)
            {
                packets.Add(new TakeSkillPacket(i, 3, 0, 5)); //최대체력 제거
            }
        }
        return packets;
    }
}
public class PurpleSoulRapier : Character
{
    public PurpleSoulRapier(int curState, Destiny destiny) : base(curState, destiny)
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();

        if (sendSkillPacket.useSkillIdx == 0)
        { //고블린의 첫번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getMagAtk() + this.getPhyAtk(), 0)); //최대체력 제거
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            for (int i = 4; i < 8; i++)
            {
                packets.Add(new TakeSkillPacket(i, 3, 0, 4)); //최대체력 제거
            }
        }
        return packets;
    }
}
public class RedSoulRapier : Character
{
    public RedSoulRapier(int curState, Destiny destiny) : base(curState, destiny)
    {

    }

    public override List<TakeSkillPacket> doSkill(SendSkillPacket sendSkillPacket)
    {
        List<TakeSkillPacket> packets = new List<TakeSkillPacket>();

        if (sendSkillPacket.useSkillIdx == 0)
        { //고블린의 첫번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getPhyAtk() + this.getPhyAtk(), 0)); //최대체력 제거
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            for (int i = 4; i < 8; i++)
            {
                packets.Add(new TakeSkillPacket(i, 3, 0, 2)); //최대체력 제거
            }
        }
        return packets;
    }
}