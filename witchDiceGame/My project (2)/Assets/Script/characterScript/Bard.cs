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
            //packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
         // packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 8 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
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
            //packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
         // packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 8 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
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
            //packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
         // packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 8 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
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
            //packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
         // packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 8 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
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
            //packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
         // packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 8 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
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
            //packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
         // packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 8 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
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
        { //고블린의 첫번째 스킬이 호출된 경우
            //packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
         // packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 8 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
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

        if (sendSkillPacket.useSkillIdx == 0)
        { //고블린의 첫번째 스킬이 호출된 경우
            //packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
         // packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 8 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
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
            //packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
         // packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 8 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
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
        { //고블린의 첫번째 스킬이 호출된 경우
            //packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
           // packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 8 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
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
        { //고블린의 첫번째 스킬이 호출된 경우
            //packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
         // packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 8 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
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
        { //고블린의 첫번째 스킬이 호출된 경우
            //packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }
        else if (sendSkillPacket.useSkillIdx == 1)
        {//고블린의 두번째 스킬이 호출된 경우
         // packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 8 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }
        return packets;
    }
}