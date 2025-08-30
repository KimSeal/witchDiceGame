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
        /*for (int i=sendSkillPacket.targetIdx[0]; i<sendSkillPacket.targetIdx[1] ; i++)
        {
            packets.Add(new TakeSkillPacket(i , sendSkillPacket.useDice[0] , 0));
        }*/
        TakeSkillPacket takeSkillPacket;
        //if (this.destiny.getDestinyIdx() == 0)
        //{
        if (sendSkillPacket.useSkillIdx == 0) //용사 기본 스킬
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], 3, 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
            packets.Add(new TakeSkillPacket(BattleManager.Instance.getCurSkillInfo().useCharacterIdx, 3, 0, 1));
        }
        if (sendSkillPacket.useSkillIdx == 1) //용사 기본 스킬
        {
            for (int i = 0; i < 4; i++) {
                packets.Add(new TakeSkillPacket(i + 4, 3, 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
                packets.Add(new TakeSkillPacket(i, 3, 0, 1));
            }
        }
        //}
        return packets;
    }
}
