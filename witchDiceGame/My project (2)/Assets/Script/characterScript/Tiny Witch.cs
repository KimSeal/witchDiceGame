using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TinyWitch : Character
{

    public TinyWitch(int curState, Destiny destiny) : base(curState, destiny)
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
        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 0) {
            //if(BattleManager.Instance.getCharacter(sendSkillPacket.targetIdx[0]) == )
            if (sendSkillPacket.targetIdx[0] == -999 || sendSkillPacket.diceNum[0] != BattleManager.Instance.getDiceNum(sendSkillPacket.targetIdx[0]))
            {
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] * this.getPhyAtk() + this.getPhyAtk(), 0)); //선택한 1개의 대상에게 / 대상이 사용한 주사위 값을 기반으로 Damage / 상태변화 없음
            }
            else
            {
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] * 10 + this.getPhyAtk(), 0)); //선택한 1개의 대상에게 / 대상이 사용한 주사위 값을 기반으로 Damage / 상태변화 없음
            }
        }
        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 1){
            packets.Add(new TakeSkillPacket(4, 0, sendSkillPacket.diceNum[0], 2));
            packets.Add(new TakeSkillPacket(5, 0, sendSkillPacket.diceNum[0], 2));
            packets.Add(new TakeSkillPacket(6, 0, sendSkillPacket.diceNum[0], 2));
            packets.Add(new TakeSkillPacket(7, 0, sendSkillPacket.diceNum[0], 2));
        }
        return packets;
    }

}
