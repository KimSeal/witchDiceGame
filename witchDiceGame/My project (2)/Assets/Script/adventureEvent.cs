using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class adventureEvent_Packet{
    private string[] chooseText = new string[3];
    private string[] resultText = new string[3];
    private int selectType; // 0 : 대화문  1 : 아이템 취득  2: 아이템 버리기  3 : 능력치 감소  4 : 능력치 획득 5 : 능력치 감소 및 획득
                     // 6 : 배틀
    private int[] val = new int[8];
    public int itemExist; // 0 : 결과로 아이템을 주지 않음. 1: 정해진 아이템을 줌. 2: 해당 레벨 내 랜덤한 아이템을 줌.(추후 구현)
    public int[] itemType = new int[4];
    public int[] itemIdx = new int[4]; 
    // 0 : 적군 캐릭터 배치 index
    // 1 : 취득하는 아이템 idx 배열
    // 2 : 아직 미정
    // 3 : 능력치 종류와 감소할 수치
    // 4 : 능력치 종류와 증가할 수치
    // 5 : 능력치 종류와 감소할 수치 + 능력치 종류와 감소할 수치

    private string spriteIndex;
    private string battleBackSprite;

    public adventureEvent_Packet(AdventureEventPacketReader adventureEventPacketReader)
    {

        this.chooseText[0] = adventureEventPacketReader.chooseTextKR.Replace("\\n", "\n"); ;
        this.resultText[0] = adventureEventPacketReader.resultTextKR.Replace("\\n", "\n"); ;

        this.chooseText[1] = adventureEventPacketReader.chooseTextEN.Replace("\\n", "\n"); ;
        this.resultText[1] = adventureEventPacketReader.resultTextEN.Replace("\\n", "\n"); ;

        this.chooseText[2] = adventureEventPacketReader.chooseTextJP.Replace("\\n", "\n"); ;
        this.resultText[2] = adventureEventPacketReader.resultTextJP.Replace("\\n", "\n"); ;

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

        this.itemExist = adventureEventPacketReader.itemExist;
        this.itemIdx[1] = adventureEventPacketReader.itemIdx1;
        this.itemIdx[2] = adventureEventPacketReader.itemIdx2;
        this.itemIdx[3] = adventureEventPacketReader.itemIdx3;
        this.itemIdx[0] = adventureEventPacketReader.itemIdx0;
        this.itemType[0] = adventureEventPacketReader.itemType0;
        this.itemType[1] = adventureEventPacketReader.itemType1;
        this.itemType[2] = adventureEventPacketReader.itemType2;
        this.itemType[3] = adventureEventPacketReader.itemType3;
        this.battleBackSprite = adventureEventPacketReader.battleBackSprite;
}

    public adventureEvent_Packet(adventureEvent_Packet adventureEventPacketReader)
    {
        for (int i = 0; i < chooseText.Length; i++)
        {
            this.chooseText[i] = adventureEventPacketReader.chooseText[i].Replace("\\n", "\n"); ;
            this.resultText[i] = adventureEventPacketReader.resultText[i].Replace("\\n", "\n"); ;
        }
        this.selectType = adventureEventPacketReader.selectType;
        for (int i=0;i<8;i++)
        {
            this.val[i] = adventureEventPacketReader.val[i];
        }
        this.spriteIndex = adventureEventPacketReader.spriteIndex;

        this.itemExist = adventureEventPacketReader.itemExist;
        this.itemIdx[1] = adventureEventPacketReader.itemIdx[1];
        this.itemIdx[2] = adventureEventPacketReader.itemIdx[2];
        this.itemIdx[3] = adventureEventPacketReader.itemIdx[3];
        this.itemIdx[0] = adventureEventPacketReader.itemIdx[0];
        this.itemType[0] = adventureEventPacketReader.itemType[0];
        this.itemType[1] = adventureEventPacketReader.itemType[1];
        this.itemType[2] = adventureEventPacketReader.itemType[2];
        this.itemType[3] = adventureEventPacketReader.itemType[3];
        this.battleBackSprite = adventureEventPacketReader.battleBackSprite;

    }
    public string getBattleBackSprite()
    {
        return battleBackSprite;
    }
    public string getChooseText()
    {
        return chooseText[jsonDataManager.Instance.getLanguage()];
    }
    public string getResultText()
    {
        return resultText[jsonDataManager.Instance.getLanguage()];
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
    public int getItemType(int idx)
    {
        return this.itemType[idx];
    }
    public int getItemIdx(int idx)
    {
        return this.itemIdx[idx];
    }
    public int getItemExist()
    {
        return this.itemExist;
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
    private string[] selectText = new string[3]; // 이벤트 등장시 나오는 text

    adventureEvent_Packet[] packet = new adventureEvent_Packet[6];

    public adventureEvent(AdventureEventReader adventureEventReader, AdventureEventPacketReader[] adventureEventPacketReaders)
    {

        this.diceUse = adventureEventReader.diceUse;
        this.level = adventureEventReader.level;
        this.stageIdx = adventureEventReader.stageIdx;
        this.eventName = adventureEventReader.eventName;
        this.levelIdxStart = adventureEventReader.levelIdxStart;
        this.levelIdxEnd = adventureEventReader.levelIdxEnd;
        this.eventIdx = adventureEventReader.eventIdx;
        this.selectText[0] = adventureEventReader.selectTextKR.Replace("\\n", "\n"); ;
        this.selectText[1] = adventureEventReader.selectTextEN.Replace("\\n", "\n"); ;
        this.selectText[2] = adventureEventReader.selectTextJP.Replace("\\n", "\n"); ;

        this.eventType = adventureEventReader.eventType;
        this.NPCSprite = adventureEventReader.NPCSprite;
        this.backgroundSprite = adventureEventReader.backgroundSprite;

        if (this.eventType == 6) {
            for (int i = 0; i < 6; i++)
            { //초기화
                this.packet[i] = new adventureEvent_Packet(adventureEventPacketReaders[i]);
            }
        }
        else{
            for (int i = 1; i < 6; i++)
            { //초기화
                this.packet[i] = null;
            }
            this.packet[0] = new adventureEvent_Packet(adventureEventPacketReaders[0]);
        }

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
        for(int i=0;i<selectText.Length;i++) this.selectText[i] = adventureEventReader.selectText[i].Replace("\\n", "\n");

        this.eventType = adventureEventReader.eventType;
        this.backgroundSprite = adventureEventReader.backgroundSprite;
        this.NPCSprite = adventureEventReader.NPCSprite;

        this.packet[0] = new adventureEvent_Packet(adventureEventReader.getPacket(0));
        if (eventType == 6)
        {
            for (int i = 1; i < 6; i++)
            {
                this.packet[i] = new adventureEvent_Packet(adventureEventReader.getPacket(i));
            }
        }
    }

    public adventureEvent_Packet getPacket(int idx)
    {
        if (this.eventType != 6) return this.packet[0];
        return this.packet[idx];
    }
    public string getSelectText()
    {
        return this.selectText[jsonDataManager.Instance.getLanguage()];
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
    
    public int getItemType(int selectDiceNum, int itemIdx)
    {
        return this.packet[selectDiceNum].getItemType(itemIdx);
    }
    public int getItemIdx(int selectDiceNum, int itemIdx)
    {
        return this.packet[selectDiceNum].getItemIdx(itemIdx);
    }
}
