using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Character
{
    public Slime(int curState, Destiny destiny) : base(curState, destiny)
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
        Debug.Log("slime attack " + sendSkillPacket.targetIdx[0].ToString());
        if (sendSkillPacket.useSkillIdx == 0) //슬라임의 첫번째 스킬이 호출된 경우
        {
            Debug.Log("Yes, Iam add!");
            packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0], 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
        }


        return packets;
    }
}
