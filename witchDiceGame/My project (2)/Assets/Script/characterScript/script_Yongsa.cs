using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yongsa : Character
{

    public Yongsa(int curState, Destiny destiny) : base(curState, destiny)
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

        if (this.destiny.getDestinyIdx() == 0)
        {
            if (sendSkillPacket.useSkillIdx == 0) //용사 기본 스킬
            {
                /*
                for (int i = 0; i < 4; i++)
                {
                    if (BattleManager.Instance.getCharacter(i + 4) != null && BattleManager.Instance.getCharacter(i + 4).getCurState() == 0)
                    {
                        packets.Add(new TakeSkillPacket(i + 4, 2000 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
                        packets.Add(new TakeSkillPacket(i + 4, 2000 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
                        packets.Add(new TakeSkillPacket(i + 4, 2000 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
                        packets.Add(new TakeSkillPacket(i + 4, 2000 + this.getPhyAtk(), 0)); //대상이 사용한 주사위 값을 기반으로 Damage를 기반으로
                    }
                }
                */
                //packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx, 1, 0, 6));
                //원래 스킬
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] + this.getPhyAtk(), 0)); //선택한 1개의 대상에게 / 대상이 사용한 주사위 값을 기반으로 Damage / 상태변화 없음
            }
            if (sendSkillPacket.useSkillIdx == 1) //용사 특수 스킬
            {
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] * sendSkillPacket.diceNum[1] + this.getPhyAtk(), 0)); //선택한 1개의 대상에게 / 대상이 사용한 주사위 값을 기반으로 Damage / 상태변화 없음
            }
        }
        return packets;
    }

}
