using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neaco : Character
{
    public Neaco(int curState, Destiny destiny) : base(curState, destiny)
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
        if (sendSkillPacket.useSkillIdx == 0) //용사 기본 스킬
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 3 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
            packets.Add(new TakeSkillPacket(BattleManager.Instance.getCurSkillInfo().useCharacterIdx, 3, 0, 1));
        }
        if (sendSkillPacket.useSkillIdx == 1) //용사 기본 스킬
        {
            for (int i = 0; i < 4; i++) {
                if (BattleManager.Instance.getCharacter(i + 4) != null && BattleManager.Instance.getCharacter(i +4 ).getCurState() == 0)
                {
                    packets.Add(new TakeSkillPacket(i + 4, 3 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
                }
                if (BattleManager.Instance.getCharacter(i ) != null && BattleManager.Instance.getCharacter(i ).getCurState() == 0)
                {
                    packets.Add(new TakeSkillPacket(i, 3, 0, 1)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
                }
                
            }
        }
        //}
        return packets;
    }
}

public class Raco : Character
{
    public Raco(int curState, Destiny destiny) : base(curState, destiny)
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
        if (sendSkillPacket.useSkillIdx == 0) //용사 기본 스킬
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 2 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
            if (getPossible(50))
            {
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 0, 2, -999));
            }
        }
        if (sendSkillPacket.useSkillIdx == 1) //
        {
            for (int i = 0; i < 4; i++)
            {
                if (BattleManager.Instance.getCharacter(i + 4) != null && BattleManager.Instance.getCharacter(i + 4).getCurState() == 0)
                {
                    packets.Add(new TakeSkillPacket(i + 4, 2 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
                    packets.Add(new TakeSkillPacket(i + 4, 0, 2, -999)); // 적 대상의 다음 주사위 2로 만들기.
                }
            }
        }
        //}
        return packets;
    }
}

public class LemGol : Character
{
    public LemGol(int curState, Destiny destiny) : base(curState, destiny)
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
        if (sendSkillPacket.useSkillIdx == 0) //용사 기본 스킬
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getPhyAtk() + this.getPhyAtk(), 0));
        }
        if (sendSkillPacket.useSkillIdx == 1) //
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getPhyAtk() * 5 + this.getPhyAtk(), 0));
        }
        //}
        return packets;
    }
}
public class SHA : Character
{
    public SHA(int curState, Destiny destiny) : base(curState, destiny)
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
        if (sendSkillPacket.useSkillIdx == 0) //용사 기본 스킬
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 3 + this.getPhyAtk(), 0));
            if (getPossible(33)) {
                packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx, 3, 0, 2));
            }
            if (getPossible(33)) {
                packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx, 0, 3,-999));
            }
        }
        if (sendSkillPacket.useSkillIdx == 1) // 스피드 만큼 데미지
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 3 + this.getPhyAtk(), 0));
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx, 0, 3, -999));
        }
        return packets;
    }

}
public class Peck : Character
{
    public Peck(int curState, Destiny destiny) : base(curState, destiny)
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
        if (sendSkillPacket.useSkillIdx == 0) //용사 기본 스킬
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getMagAtk() + this.getPhyAtk(), sendSkillPacket.diceNum[0]));
        }
        if (sendSkillPacket.useSkillIdx == 1) // 스피드 만큼 데미지
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getSkillVal(1, 0), 0, 4));
        }
        return packets;
    }
}
public class Flap : Character
{
    public Flap(int curState, Destiny destiny) : base(curState, destiny)
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
        if (sendSkillPacket.useSkillIdx == 0) //용사 기본 스킬
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), sendSkillPacket.diceNum[0]));
        }
        if (sendSkillPacket.useSkillIdx == 1) // 스피드 만큼 데미지
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getSpeed() + this.getPhyAtk(), 0));
            if (sendSkillPacket.targetIdx[0] != -999 && BattleManager.Instance.getDiceNum(sendSkillPacket.targetIdx[0]) == sendSkillPacket.diceNum[0])
            {
                Debug.Log("It is Active!");
                packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx, 1, 0, 5));
            }
        }
        return packets;
    }
}

public class Betaca : Character
{
    public Betaca(int curState, Destiny destiny) : base(curState, destiny)
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
                packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx, this.getSkillVal(0, 1), 0, 2)); //자신에게 공격력 추가
            }
        }
        else if (sendSkillPacket.useSkillIdx == 1) //고블린의 두번째 스킬이 호출된 경우
        {
            if (sendSkillPacket.targetIdx[0] >= 4 && sendSkillPacket.targetIdx[0] < 8)
            {
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getPhyAtk() + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
            }
            else if (sendSkillPacket.targetIdx[0] >= 0 && sendSkillPacket.targetIdx[0] < 4)
            {
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getSkillVal(1, 0), 0, 2)); //아군 대상에게 공격력 1 추가
            }
        }

        return packets;
    }
}
public class Unu : Character
{
    public Unu(int curState, Destiny destiny) : base(curState, destiny)
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

        if (sendSkillPacket.useSkillIdx == 0) //용사 기본 스킬
        {
            packets.Add(new TakeSkillPacket(BattleManager.Instance.getCurSkillInfo().useCharacterIdx, 10, 0, 1)); //10회복하고
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx, 0, 1, -999));  //캐릭터 상태 1로 바꾸기
        }
        if (sendSkillPacket.useSkillIdx == 1) //용사 기본 스킬
        {
            for (int i = 0; i < 4; i++)
            {
                if (BattleManager.Instance.getCharacter(i + 4) != null && BattleManager.Instance.getCharacter(i + 4).getCurState() == 0)
                {
                    packets.Add(new TakeSkillPacket(i + 4, 1000 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
                }
            }
        }
        //}
        return packets;
    }
}
