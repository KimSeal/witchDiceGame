using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alphaca : Character
{
    public Alphaca(int curState, Destiny destiny) : base(curState, destiny)
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

        if (sendSkillPacket.useSkillIdx == 0) //고블린의 첫번째 스킬이 호출된 경우
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getPhyAtk() + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }
        else if (sendSkillPacket.useSkillIdx == 1) //고블린의 두번째 스킬이 호출된 경우
        {
            for (int i = 0; i < 4; i++)
            {
                packets.Add(new TakeSkillPacket(i, this.getPhyAtk() + this.getPhyAtk(), 0)); //적 모두에게 공격력 데미지
            }
        }

        return packets;
    }
}

public class MaybeAlphaca : Character
{
    public MaybeAlphaca(int curState, Destiny destiny) : base(curState, destiny)
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

        if (sendSkillPacket.useSkillIdx == 0) //고블린의 첫번째 스킬이 호출된 경우
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getSkillVal(0, 0) + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
            if (sendSkillPacket.targetIdx[0] != -999 && BattleManager.Instance.getDiceNum(sendSkillPacket.targetIdx[0]) <= 3)
            {
                packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, this.getSkillVal(0, 1), 0, 2)); //자신에게 공격력 추가
            }
        }
        else if (sendSkillPacket.useSkillIdx == 1) //고블린의 두번째 스킬이 호출된 경우
        {
            if (sendSkillPacket.targetIdx[0] >= 0 && sendSkillPacket.targetIdx[0] < 4)
            {
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getPhyAtk() + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
            }
            else if (sendSkillPacket.targetIdx[0] >= 4 && sendSkillPacket.targetIdx[0] < 8) 
            {
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getSkillVal(1, 0), 0, 2)); //아군 대상에게 공격력 1 추가
            }
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 0, Random.Range(1,4), -999));
        }

        return packets;
    }
}

public class NestEater : Character
{
    public NestEater(int curState, Destiny destiny) : base(curState, destiny)
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

        if (sendSkillPacket.useSkillIdx == 0) //고블린의 첫번째 스킬이 호출된 경우
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }
        else if (sendSkillPacket.useSkillIdx == 1) //고블린의 두번째 스킬이 호출된 경우
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, this.getSkillVal(1, 0), 0, 2)); //자신에게 공격력 추가
        }

        return packets;
    }
}

public class NestAter : Character
{
    public NestAter(int curState, Destiny destiny) : base(curState, destiny)
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

        if (sendSkillPacket.useSkillIdx == 0) //고블린의 첫번째 스킬이 호출된 경우
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getHp() + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }
        else if (sendSkillPacket.useSkillIdx == 1) //고블린의 두번째 스킬이 호출된 경우
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 2, 0, 1)); //자신에게 공격력 추가
        }

        return packets;
    }
}

public class FlyingPaper : Character
{
    public FlyingPaper(int curState, Destiny destiny) : base(curState, destiny)
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

        if (sendSkillPacket.useSkillIdx == 0) //고블린의 첫번째 스킬이 호출된 경우
        {
            if (getPossible(50))
            {
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0));
            }
            else
            {
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] * 2 + this.getPhyAtk(), 0));
            }
        }
        else if (sendSkillPacket.useSkillIdx == 1) //고블린의 두번째 스킬이 호출된 경우
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getSkillVal(1, 0), 0, 4)); //자신에게 공격력 추가
        }
        return packets;
    }
}

public class FlyingTreatise : Character
{
    public FlyingTreatise(int curState, Destiny destiny) : base(curState, destiny)
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

        if (sendSkillPacket.useSkillIdx == 0) //고블린의 첫번째 스킬이 호출된 경우
        {
            if (getPossible(25)) 
            {
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0));
            }
            else
            {
                for(int i=0;i<4;i++) packets.Add(new TakeSkillPacket(i, sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0));
            }
        }
        else if (sendSkillPacket.useSkillIdx == 1) //고블린의 두번째 스킬이 호출된 경우
        {
            int useCharacterIdx = sendSkillPacket.useCharacterIdx;
            if(useCharacterIdx != 0) packets.Add(new TakeSkillPacket(useCharacterIdx + 3, this.getSkillVal(1, 0), 0, 4)); //자신에게 공격력 추가
            if (useCharacterIdx != 3) packets.Add(new TakeSkillPacket(useCharacterIdx + 5, this.getSkillVal(1, 0), 0, 4)); //자신에게 공격력 추가
        }
        return packets;
    }
}

public class FlyingBook : Character
{
    public FlyingBook(int curState, Destiny destiny) : base(curState, destiny)
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

        if (sendSkillPacket.useSkillIdx == 0) //고블린의 첫번째 스킬이 호출된 경우
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getMagAtk() + this.getPhyAtk(), 0));
        }
        else if (sendSkillPacket.useSkillIdx == 1) //고블린의 두번째 스킬이 호출된 경우
        {
            for (int i=4;i<8;i++)
            {
                if (sendSkillPacket.useCharacterIdx + 4 != i) packets.Add(new TakeSkillPacket(i, this.getSkillVal(1, 0), 0, 4)); //자신에게 공격력 추가
            }
        }
        return packets;
    }
}

public class EagleWizard : Character
{
    public EagleWizard(int curState, Destiny destiny) : base(curState, destiny)
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

        if (sendSkillPacket.useSkillIdx == 0) //고블린의 첫번째 스킬이 호출된 경우
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getMagAtk() + this.getPhyAtk(), 0));
            if (getPossible(50))
            {
                for (int i = 0; i < 4; i++)
                {
                    if (BattleManager.Instance.getCharacter(i) != null && BattleManager.Instance.getCharacter(i).getCurState() == 0)
                    {
                        packets.Add(new TakeSkillPacket(i, 0, 5, -999));
                    }
                }
                packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 5, -999));
            }
        }
        else if (sendSkillPacket.useSkillIdx == 1) //고블린의 두번째 스킬이 호출된 경우
        {
            for (int i = 0; i < 4; i++) {
                packets.Add(new TakeSkillPacket(i, 5 + this.getPhyAtk(), 0));
                if(BattleManager.Instance.getDiceNum(i) == 5) packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx +4, this.getSkillVal(1, 0), 0, 4)); //자신에게 감응력 추가
            }
        }
        return packets;
    }
}