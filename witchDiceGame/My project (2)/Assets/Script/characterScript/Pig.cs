using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : Character
{
    public Pig(int curState, Destiny destiny) : base(curState, destiny)
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
  
        //if (this.destiny.getDestinyIdx() == 0)
        //{
        if (sendSkillPacket.useSkillIdx == 0) //용사 기본 스킬
        {
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, this.getSkillVal(0, 0), 0, 1)); //자신 1명에게 10만큼 회복
        }
        if (sendSkillPacket.useSkillIdx == 1) 
        {
            
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], this.getSkillVal(1, 0), 0)); //한명에게 3데미지
        }
        //}
        return packets;
    }
}
