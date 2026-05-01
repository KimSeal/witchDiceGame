using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wawa : Character
{

    public Wawa(int curState, Destiny destiny) : base(curState, destiny)
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

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 0) //와와 기본 스킬
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx, 0, 6, -999)); //스킬을 사용한 대상에게 / 0변화 / 상태변화를 6으로 / 스킬은 버프류
        }

        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 1) //와와 특수 스킬(스킬기억)
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], (this.getPhyAtk()) * this.skillUse(sendSkillPacket.useSkillIdx).getVal(0) + this.getPhyAtk(), 0)); //선택한 1개의 대상에게 / 공격력 * 10 Damage / 상태변화 없음
        }

        return packets;
    }

}
