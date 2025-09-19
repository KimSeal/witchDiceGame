using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Border : Character
{

    public Border(int curState, Destiny destiny) : base(curState, destiny)
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

        if (sendSkillPacket.useSkillIdx == 0) //보더 기본 스킬
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 8, 0)); //선택한 1개의 대상에게 / 대상이 사용한 주사위 값을 기반으로 Damage / 상태변화 없음
        }

        if (sendSkillPacket.useSkillIdx == 1) //보더 특수 스킬(스킬기억)
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0], 0)); //선택한 1개의 대상에게 / 대상이 사용한 주사위 값을 기반으로 Damage / 상태변화 없음
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx, 0, sendSkillPacket.diceNum[0], -999)); //스킬을 사용한 대상에게 / 0변화 / 상태변화를 현재 주사위 값으로 / 스킬은 버프류

        }

        return packets;
    }

}
