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
        TakeSkillPacket takeSkillPacket;
        if (sendSkillPacket.useSkillIdx == 0) //용사 기본 스킬
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 3, 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
            packets.Add(new TakeSkillPacket(BattleManager.Instance.getCurSkillInfo().useCharacterIdx, 3, 0, 1));
        }
        if (sendSkillPacket.useSkillIdx == 1) //용사 기본 스킬
        {
            for (int i = 0; i < 4; i++) {
                if (BattleManager.Instance.getCharacter(i + 4) != null && BattleManager.Instance.getCharacter(i +4 ).getCurState() == 0)
                {
                    packets.Add(new TakeSkillPacket(i + 4, 3, 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
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
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 2, 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
            if (Random.Range(0, 2) == 0)
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
                    packets.Add(new TakeSkillPacket(i + 4, 2, 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
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
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], (this.phyAtk + this.character_battle.getAtk()) + 5, 0));
        }
        if (sendSkillPacket.useSkillIdx == 1) //
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], (this.phyAtk + this.character_battle.getAtk()) * 5, 0));
        }
        //}
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
        /*for (int i=sendSkillPacket.targetIdx[0]; i<sendSkillPacket.targetIdx[1] ; i++)
        {
            packets.Add(new TakeSkillPacket(i , sendSkillPacket.useDice[0] , 0));
        }*/
        TakeSkillPacket takeSkillPacket;
        //if (this.destiny.getDestinyIdx() == 0)
        //{
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
                    packets.Add(new TakeSkillPacket(i + 4, 1, 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
                }
            }
        }
        //}
        return packets;
    }
}
