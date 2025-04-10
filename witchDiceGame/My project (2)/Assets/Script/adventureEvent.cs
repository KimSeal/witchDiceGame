using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class adventureEvent_Packet{
    private string chooseText;
    private string resultText;
    private int selectType; // 0 : 대화문  1 : 아이템 취득  2: 아이템 버리기  3 : 능력치 감소  4 : 능력치 획득 5 : 능력치 감소 및 획득
                     // 6 : 배틀
    private int[] val = new int[8];
    // 0 : 적군 캐릭터 배치 index
    // 1 : 취득하는 아이템 idx 배열
    // 2 : 아직 미정
    // 3 : 능력치 종류와 감소할 수치
    // 4 : 능력치 종류와 증가할 수치
    // 5 : 능력치 종류와 감소할 수치 + 능력치 종류와 감소할 수치

    private string spriteIndex;


    public adventureEvent_Packet(AdventureEventPacketReader adventureEventPacketReader)
    {
        this.chooseText = adventureEventPacketReader.chooseText;
        this.resultText = adventureEventPacketReader.resultText;
        this.selectType = adventureEventPacketReader.selectType;
        this.val[0] = adventureEventPacketReader.selectVal0;
        this.val[1] = adventureEventPacketReader.selectVal1;
        this.val[2] = adventureEventPacketReader.selectVal2;
        this.val[3] = adventureEventPacketReader.selectVal3;
        this.val[4] = adventureEventPacketReader.selectVal4;
        this.val[5] = adventureEventPacketReader.selectVal5;    
        this.val[6] = adventureEventPacketReader.selectVal6;
        this.val[7] = adventureEventPacketReader.selectVal7;
        this.spriteIndex = adventureEventPacketReader.spriteIndex;
    }

    public adventureEvent_Packet(adventureEvent_Packet adventureEventPacketReader)
    {
        this.chooseText = adventureEventPacketReader.chooseText;
        this.resultText = adventureEventPacketReader.resultText;
        this.selectType = adventureEventPacketReader.selectType;
        for (int i=0;i<8;i++)
        {
            this.val[i] = adventureEventPacketReader.val[i];
        }
        this.spriteIndex = adventureEventPacketReader.spriteIndex;
    }
    public string getChooseText()
    {
        return chooseText;
    }
    public string getResultText()
    {
        return resultText;
    }
    public int getSelectType()
    {
        return this.selectType;
    }
    public int getVal(int idx)
    {
        return this.val[idx];
    }
    public string getSpriteIndex()
    {
        return spriteIndex;
    }
}

public class adventureEvent
{
    private int eventType;
    private string backgroundSprite;
    private string NPCSprite;

    private int level; // 해당 스테이지의 레벨
    private int diceUse; // 주사위 굴리는 여부
    private int eventIdx; // 해당 이벤트의 idx
    private string eventName; //해당 이벤트의 이벤트 이름
    private int stageIdx; // 해당 이벤트가 나오게 되는 스테이지의 idx
    private int levelIdxStart; //해당 이벤트가 나올 수 있는 스테이지의 단계 최소값
    private int levelIdxEnd; // 해당 이벤가 나올 수 있는 스테이지의 최대값
    private string selectText; // 이벤트 등장시 나오는 text

    adventureEvent_Packet[] packet = new adventureEvent_Packet[6];

    public adventureEvent(AdventureEventReader adventureEventReader, AdventureEventPacketReader[] adventureEventPacketReaders)
    {
        for (int i=0;i<6;i++){ //초기화
            this.packet[i] = new adventureEvent_Packet(adventureEventPacketReaders[i]);
        }
        this.diceUse = adventureEventReader.diceUse;
        this.level = adventureEventReader.level;
        this.stageIdx = adventureEventReader.stageIdx;
        this.eventName = adventureEventReader.eventName;
        this.levelIdxStart = adventureEventReader.levelIdxStart;
        this.levelIdxEnd = adventureEventReader.levelIdxEnd ;
        this.eventIdx = adventureEventReader.eventIdx;
        this.selectText = adventureEventReader.selectText;

        this.eventType = adventureEventReader.eventType;
        this.NPCSprite = adventureEventReader.NPCSprite;
        this.backgroundSprite = adventureEventReader.backgroundSprite;

    }
    public adventureEvent(adventureEvent adventureEventReader)
    {
        this.diceUse = adventureEventReader.diceUse;
        this.level = adventureEventReader.level;
        this.stageIdx = adventureEventReader.stageIdx;
        this.eventName = adventureEventReader.eventName;
        this.levelIdxStart = adventureEventReader.levelIdxStart;
        this.levelIdxEnd = adventureEventReader.levelIdxEnd;
        this.eventIdx = adventureEventReader.eventIdx;
        this.selectText = adventureEventReader.selectText;

        this.eventType = adventureEventReader.eventType;
        this.backgroundSprite = adventureEventReader.backgroundSprite;
        this.NPCSprite = adventureEventReader.NPCSprite;

        for (int i = 0; i < 6; i++)
        {
            this.packet[i] = new adventureEvent_Packet(adventureEventReader.getPacket(i));
        }
    }

    public adventureEvent_Packet getPacket(int idx)
    {
        return this.packet[idx];
    }
    public string getSelectText()
    {
        return this.selectText;
    }
    public string getEventName()
    {
        return this.eventName;
    }
    public int getDiceUse()
    {
        return this.diceUse;
    }
    public int getLevel()
    {
        return this.level;
    }

    public int getEventType()
    {
        return this.eventType;
    }
    public string getBackgroundSprite()
    {
        return this.backgroundSprite;
    }
    public string getNPCSprite()
    {
        return this.NPCSprite;
    }

}
