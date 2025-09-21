using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harfy : Character
{

    public Harfy(int curState, Destiny destiny) : base(curState, destiny)
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
        
        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 0){ //다 같은 눈으로 만들고,
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx +4, 0, sendSkillPacket.diceNum[0], 2));

            packets.Add(new TakeSkillPacket(0, sendSkillPacket.diceNum[0], sendSkillPacket.diceNum[0], 0));
            packets.Add(new TakeSkillPacket(1, sendSkillPacket.diceNum[0], sendSkillPacket.diceNum[0], 0));
            packets.Add(new TakeSkillPacket(2, sendSkillPacket.diceNum[0], sendSkillPacket.diceNum[0], 0));
            packets.Add(new TakeSkillPacket(3, sendSkillPacket.diceNum[0], sendSkillPacket.diceNum[0], 0));
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 1, 0, 3)); //본인 특수 변수를 1으로 만든다.
        }
        if (this.skillIdx[sendSkillPacket.useSkillIdx] == 1)
        {
            Debug.Log("target, diceNum, and targetDiceNum");
            Debug.Log(sendSkillPacket.targetIdx[0]);
            Debug.Log(sendSkillPacket.diceNum[0]);
            Debug.Log(BattleManager.Instance.getDiceNum(sendSkillPacket.targetIdx[0]));
            //타겟이 있고, 주사위 숫자가 같으면 강한 공격!
            if (sendSkillPacket.targetIdx[0] != -999 && sendSkillPacket.diceNum[0] == BattleManager.Instance.getDiceNum(sendSkillPacket.targetIdx[0]))
            {
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0] * this.phyAtk, 0)); //선택한 1개의 대상에게 / 대상이 사용한 주사위 값을 기반으로 Damage / 상태변화 없음
            }
            else
            {
                packets.Add(new TakeSkillPacket(sendSkillPacket.targetIdx[0], sendSkillPacket.diceNum[0], 0)); //선택한 1개의 대상에게 / 대상이 사용한 주사위 값을 기반으로 Damage / 상태변화 없음
            }
            packets.Add(new TakeSkillPacket(sendSkillPacket.useCharacterIdx + 4, 0, 0, 3)); //본인 특수 변수를 0으로 만든다.
        }
        return packets;
    }

}
