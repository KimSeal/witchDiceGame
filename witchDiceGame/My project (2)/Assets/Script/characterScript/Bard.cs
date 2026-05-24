using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodDummy : Character
{
    public WoodDummy(int curState, Destiny destiny) : base(curState, destiny)
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
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 1, 0, 6));
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getArmor() * this.getPhyAtk() + this.getPhyAtk(), 0)); 
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
         packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 2, 0, 6));
        }
        return packets;
    }
}
public class IronDummy : Character
{
    public IronDummy(int curState, Destiny destiny) : base(curState, destiny)
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
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx+4, 1, 0, 6));
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getArmor() * this.getPhyAtk() + this.getPhyAtk(), 0));
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx+4, 3, 0, 6));
        }
        return packets;
    }
}
public class FastFoot : Character
{
    public FastFoot(int curState, Destiny destiny) : base(curState, destiny)
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
            if (sendSkillPacket.targetIdx[0] >= 0 && sendSkillPacket.targetIdx[0] < 4)
            {
                AdventureManager.Instance.addMoney(0, -1 * this.getSpeed());
            }
        }
        return packets;
    }
}
public class BigHammer : Character
{
    public BigHammer(int curState, Destiny destiny) : base(curState, destiny)
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
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 2 * this.getPhyAtk(), 0));
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 2 * this.getPhyAtk(), 0));
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 2 * this.getPhyAtk(), 0));
        }
        return packets;
    }
}
public class Undead : Character
{
    public Undead(int curState, Destiny destiny) : base(curState, destiny)
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
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, sendSkillPacket.diceNum[0], 0, 2));
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 1+ this.getPhyAtk(), 0));
            //packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx+4, this.getPhyAtk(), 0, 2));
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 1 + this.getPhyAtk(), 0));
        }
        return packets;
    }
}
public class DeadChunk : Character
{
    public DeadChunk(int curState, Destiny destiny) : base(curState, destiny)
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
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 10 + this.getPhyAtk(), 0));
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx+4, 10 + this.getPhyAtk(), 0, 1));
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getHp() + this.getPhyAtk(), 0));
        }
        return packets;
    }
}
public class BombHead : Character
{
    public BombHead(int curState, Destiny destiny) : base(curState, destiny)
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
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, this.getHp(), 0, 1));
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            int tempDamage = this.getHp() / 2 + this.getPhyAtk();
            for (int i = 0; i < 4; i++)
            {
                packets.Add(new TakeSkillPacket(i, tempDamage, 0));
            }
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, tempDamage, 0));
        }
        return packets;
    }
}
public class GrandKnight : Character
{
    public GrandKnight(int curState, Destiny destiny) : base(curState, destiny)
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

        if (sendSkillPacket.useSkillIdx == 0) { 
            int atkVal = 0;
            for (int i = 0; i < 8; i++)
            {
                if (i != sendSkillPacket.useCharacterIdx + 4 && 
                    BattleManager.Instance.getCharacter(i) != null && BattleManager.Instance.getCharacter(i).getCurState() == 0)
                {
                    atkVal += 1;
                    packets.Add(new TakeSkillPacket(i, 1 + this.getPhyAtk(), 0));
                }
            }
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, atkVal, 0, 2)); //공격된 대상 숫자 만큼 공격력 업
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            for (int i=0;i<4;i++)
            {
                packets.Add(new TakeSkillPacket(i, this.getPhyAtk() + this.getPhyAtk(), 0));
            }
        }
        return packets;
    }
}

public class NoHead : Character
{
    public NoHead(int curState, Destiny destiny) : base(curState, destiny)
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
                packets.Add(new TakeSkillPacket(i, this.getPhyAtk() + this.getPhyAtk(), 0));
            }
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            for (int i = 4; i < 8; i++)
            {
                packets.Add(new TakeSkillPacket(i, 1, 0, 6));
            }
        }
        return packets;
    }
}



public class Bard : Character
{
    public Bard(int curState, Destiny destiny) : base(curState, destiny)
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
        {
            if (sendSkillPacket.targetIdx[0] >= 0 && sendSkillPacket.targetIdx[0] < 4)
            {
                if (BattleManager.Instance.getCharacter(6) == null ||
                    BattleManager.Instance.getCharacter(6).getCurState() != 0)
                {
                    BattleManager.Instance.setEnemyCharacter(2, 10034);
                }
                else if (BattleManager.Instance.getCharacter(4) == null ||
                BattleManager.Instance.getCharacter(4).getCurState() != 0)
                {
                    BattleManager.Instance.setEnemyCharacter(0, 10035);
                }
    
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getPhyAtk() + this.getPhyAtk(), 0));
            }
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {
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
}

public class SoulShield : Character
{
    public SoulShield(int curState, Destiny destiny) : base(curState, destiny)
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
        {
            for (int i=4;i<8;i++)
            {
                packets.Add(new TakeSkillPacket(i, 0, 1, -999));
            }
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {
            for (int i = 4; i < 8; i++)
            {
                packets.Add(new TakeSkillPacket(i, 1, 0, 6));
            }
        }
        return packets;
    }
}

public class SoulFlag : Character
{
    public SoulFlag(int curState, Destiny destiny) : base(curState, destiny)
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
        { 
            for (int i = 4; i < 8; i++)
            {
                packets.Add(new TakeSkillPacket(i, 0, 1, -999));
            }
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {
            for (int i = 4; i < 8; i++)
            {
                packets.Add(new TakeSkillPacket(i, 1, 0, 2));
            }
        }
        return packets;
    }
}

public class Enzi : Character
{
    public Enzi(int curState, Destiny destiny) : base(curState, destiny)
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
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx, 1, 0, 6));
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getArmor() * this.getPhyAtk() + this.getPhyAtk(), 0));
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx, 2, 0, 6));
        }
        return packets;
    }
}
public class Nubi : Character
{
    public Nubi(int curState, Destiny destiny) : base(curState, destiny)
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
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], AdventureManager.Instance.getDeadEnemyCount() + this.getPhyAtk(), 0));
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
            int emptySpace = -999;
            if (this.getCharacter_battle().getSpecialVal() == 0)
            {
                if (sendSkillPacket.targetIdx[0] >= 4 && sendSkillPacket.targetIdx[0] < 8)
                {
                    for (int i = 4; i < 8; i++)
                    {
                        if (BattleManager.Instance.getCharacter(i) == null ||
                            BattleManager.Instance.getCharacter(i).getCurState() != 0)
                        {
                            emptySpace = i - 4;
                            break;
                        }
                    }
                }
            }
            
            for (int i = 4; i < 8; i++){
                if(emptySpace+4 != i) packets.Add(new TakeSkillPacket(i, 6 + this.getPhyAtk(), 0));
            }
            if (emptySpace >= 0)
            {
                packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx, 1, 0, 3)); //본인 특수 변수를 1으로 만든다.
                BattleManager.Instance.setEnemyCharacter(emptySpace, 10036);
            }
        }
        return packets;
    }
}

public class DeadGuy : Character
{
    public DeadGuy(int curState, Destiny destiny) : base(curState, destiny)
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

        packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, this.hp, 0));
        return packets;
    }
}

public class Munk : Character
{
    public Munk(int curState, Destiny destiny) : base(curState, destiny)
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
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 1, 0, 6));

        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우  
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 5 + this.getPhyAtk(), 0));
        }
        return packets;
    }

    public override int getTargetChance(int idx)
    {
        Debug.Log("Check");

        if (idx == 1)
        {
            Debug.Log(this.getArmor() + 1);
            return this.getArmor() + 1;
        }
        return 0;
    }
}

public class Pimpi : Character
{
    public Pimpi(int curState, Destiny destiny) : base(curState, destiny)
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
            int itemNum = itemManager.Instance.getItemNum(0) + 
                itemManager.Instance.getItemNum(1) +
                itemManager.Instance.getItemNum(2) +
                itemManager.Instance.getItemNum(3);
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0],itemNum + this.getPhyAtk(), 0));
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], itemManager.Instance.getItemNum(sendSkillPacket.curChanceNum) + this.getPhyAtk(), 0));
        }
        return packets;
    }
    public override int getTargetChance(int idx)
    {
        if (idx == 1)
        {
            return 4;
        }
        return 0;
    }
}