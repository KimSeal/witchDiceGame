using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    //싱클톤
    private static CharacterManager instance = null;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static CharacterManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }


    private Character[] myCharacter = new Character[4];
    private Character[] enemyCharacter = new Character[4];

    public List<Destiny> destinyList = new List<Destiny>();
    public List<Destiny> destinyList_monster = new List<Destiny>();
    public List<Skill> skillList = new List<Skill>();

    public List<DestinyReader> destinyReaderList = new List<DestinyReader>();
    public List<SkillReader> skillReaderList = new List<SkillReader>();

    // Start is called before the first frame update
    void Start()
    {
        destinyReaderList = CSVReader.Read<DestinyReader>("Destiny");
        skillReaderList = CSVReader.Read<SkillReader>("Skill");
        Skill[] skillArr = new Skill[10];

        for (int i = 0; i < skillReaderList.Count; i++)
        {
            skillList.Add(new Skill(skillReaderList[i]) );
        }

        for (int i=0;i < destinyReaderList.Count;i++)
        {
            skillArr[0] = skillList[destinyReaderList[i].skill0];
            skillArr[1] = skillList[destinyReaderList[i].skill1];
            skillArr[2] = skillList[destinyReaderList[i].skill2];
            skillArr[3] = skillList[destinyReaderList[i].skill3];
            skillArr[4] = skillList[destinyReaderList[i].skill4];
            skillArr[5] = skillList[destinyReaderList[i].skill5];
            skillArr[6] = skillList[destinyReaderList[i].skill6];
            skillArr[7] = skillList[destinyReaderList[i].skill7];
            skillArr[8] = skillList[destinyReaderList[i].skill8];
            skillArr[9] = skillList[destinyReaderList[i].skill9];

            if(destinyReaderList[i].DestinyIdx <= 10000 && destinyReaderList[i].DestinyIdx >= 0 ) destinyList.Add(new Destiny(destinyReaderList[i], skillArr));
            else if (destinyReaderList[i].DestinyIdx > 10000) destinyList_monster.Add(new Destiny(destinyReaderList[i], skillArr));
        }


        //캐릭터 테스트 0번에 용사 배치
        emptyMyCharacter(0);
        emptyMyCharacter(1);
        emptyMyCharacter(2);
        emptyMyCharacter(3);
        setCharacter(0, 0);
        myCharacter[0].setReviveUnit(true);

        //setCharacter(3, 4);
    }
    public void setTurotialCharacterSet()
    {
        emptyMyCharacter(0);
        emptyMyCharacter(1);
        emptyMyCharacter(2);
        emptyMyCharacter(3);
        setCharacter(0, 0);
        myCharacter[0].setReviveUnit(true);
    }
    public void setTestCharacterSet()
    {
        emptyMyCharacter(0);
        emptyMyCharacter(1);
        emptyMyCharacter(2);
        emptyMyCharacter(3);
        setCharacter(0, 0);
        myCharacter[0].setReviveUnit(true);
        setCharacter(1, 1);
        setCharacter(2, 2);
        //setCharacter(3, 3);
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void resetCharacterManager()
    {
        
        for (int i=0;i<4;i++)
        {
            myCharacter[i] = null;
        }
    }

    public void startGame(int characterIdx, int point)
    {
        setCharacter(point, characterIdx);
    }

    public Destiny getDestiny(int idx)
    {
        return destinyList[idx];
    }
    public int getRandomCharacterDestinyIdx()
    {
        return Random.Range(1, destinyList.Count) ;
    }
    public int getCharacterState(int idx)
    {
        
        if (idx<0 || idx>3 || myCharacter[idx] == null) {
            return 3; 
        }
        return myCharacter[idx].getCurState();
    }
    public void changeDice(int characterIdx, int diceIdx, int diceNum)
    {
        if (diceNum > 6) diceNum = 6;
        if (diceNum < 1) diceNum = 1;
        myCharacter[characterIdx].changeDiceNum(diceIdx, diceNum);
    }

    public void throwDice(int characterIdx)
    {
        myCharacter[characterIdx].throwDice();
    }

    public void changeEquip(int characterIdx, int itemNum, int itemType, int itemIdx)
    {
        myCharacter[characterIdx].changeEquip(itemNum, itemType, itemIdx);
    }

    public void character_reset()
    {
        for (int i = 0; i < 4; i++)
        {
            if (myCharacter[i] != null && myCharacter[i].getReviveUnit()){ //부활해야 하는 유닛인 경우. 플레이어의 운명을 다시 배치하고 hp를 1로 만든다.
                setCharacter(i, myCharacter[i].getDestiny().getDestinyIdx());
                myCharacter[i].setHp(1);
                myCharacter[i].setReviveUnit(true);
            }
            else{//부활 유닛이 아닌경우
                myCharacter[i] = null;
            }
            enemyCharacter[i] = null;
        }
    }
    public void character_battleEnd_deepCopy(int idx, Character character) //배틀 종료 후 다시 character manager로 가져온다.
    {
        character_deepCopy(ref myCharacter[idx], character);
        if (myCharacter[idx].getCurState() != 0) { //죽은 경우에 대한 처리 필요.
        }
    }
    public bool character_deepCopy(ref Character A, Character character) {
        if (character == null) return false; //복사 불가시 false 리턴

        setCharacter_destinyBase(ref A, character.getDestiny().getDestinyIdx());
        A.characterDeepCopy(character);
        
        return true;
    }

    public bool deleteCharacter(int idx)
    {
        if (myCharacter[idx].getReviveUnit())
        {
            Debug.Log("no, this is main character!");
            return false;
        }
        else
        {
            myCharacter[idx] = null;
        }
        return true;
    }

    public void setCharacter_destinyBase(ref Character character, int characterIdx) //캐릭터 각각에 대해 초기 new를 해주는 함수
    {
        if (characterIdx == -99999) return;
        //아군
        if (characterIdx <= 10000)
        {
            switch (characterIdx)
            {
                case 0:
                    character = new Yongsa(0, destinyList[characterIdx]); break;
                case 1:
                    character = new Neaco(0, destinyList[characterIdx]); break;
                case 2:
                    character = new Druid(0, destinyList[characterIdx]); break;
                case 3:
                    character = new Tom(0, destinyList[characterIdx]); break;
                case 4:
                    character = new Bob(0, destinyList[characterIdx]); break;
                case 5:
                    character = new Border(0, destinyList[characterIdx]); break;
                case 6:
                    character = new Wawa(0, destinyList[characterIdx]); break;
                case 7:
                    character = new Unu(0, destinyList[characterIdx]); break;
                case 8:
                    character = new Raco(0, destinyList[characterIdx]); break;
                case 9:
                    character = new LemGol(0, destinyList[characterIdx]); break;

            }
        }
        //몬스터
        if (characterIdx > 10000)
        {
            characterIdx -= 10001;
            switch (characterIdx)
            {
                case 0:
                    character = new Slime(0, destinyList_monster[characterIdx]); break;
                case 1:
                    character = new Goblin(0, destinyList_monster[characterIdx]); break;
                case 2:
                    character = new RoyalSoldier(0, destinyList_monster[characterIdx]); break;
                case 3:
                    character = new Soldier(0, destinyList_monster[characterIdx]); break;
                case 4:
                    character = new Chicken(0, destinyList_monster[characterIdx]); break;
                case 5:
                    character = new Duck(0, destinyList_monster[characterIdx]); break;
                case 6:
                    character = new Sheep(0, destinyList_monster[characterIdx]); break;
                case 7:
                    character = new Pig(0, destinyList_monster[characterIdx]); break;
                case 8:
                    character = new Wolf(0, destinyList_monster[characterIdx]); break;
                case 9:
                    character = new WolfQueen(0, destinyList_monster[characterIdx]); break;
                case 10:
                    character = new Chihuahua(0, destinyList_monster[characterIdx]); break;
                case 11:
                    character = new TinyWitch(0, destinyList_monster[characterIdx]); break;
                case 12:
                    character = new Harfy(0, destinyList_monster[characterIdx]); break;
                case 13:
                    character = new Schnauzer(0, destinyList_monster[characterIdx]); break;
                case 14:
                    character = new WolfSheep(0, destinyList_monster[characterIdx]); break;
            }
        }
    }

    //살아있는 캐릭터 배치
    public void setCharacter(int place, int characterIdx)
    {
        if (place > 3) place = 3;
        if (characterIdx == -99999) return;
        //아군
        if (characterIdx <= 10000) {
            setCharacter_destinyBase(ref myCharacter[place], characterIdx);
        }
        //몬스터
        if (characterIdx > 10000) {
            setCharacter_destinyBase(ref enemyCharacter[place], characterIdx);
        }
    }

    
    public void setCharacter(int place, Character character) {
        //아군
        myCharacter[place] = character; 
    }
    public void emptyEnemyCharacter(int place)
    {
        enemyCharacter[place] = null;
    }
    public void emptyMyCharacter(int place)
    {
        myCharacter[place] = null;
    }
    public Character getCharacter(int idx)
    {
        return myCharacter[idx];
    }

    public Character getCharacter(bool myTeam, int idx)
    {
        if(myTeam) return myCharacter[idx];
        return enemyCharacter[idx];
    }


    public string getName_itemManager(int idx)
    {
        return myCharacter[idx].getName();
    }


    public void setcharacterHp(int idx, int hp)
    {
        myCharacter[idx].setHp(hp);
    }

    public void setDiceNum(int idx, int diceIdx, int val) //해당 캐릭터의 주사위 면의 숫자를 바꾸는 함수
    {
        myCharacter[idx].setDice(diceIdx, val);
    }

    public int getDiceNum(int idx) //해당 캐릭터의 주사위 면의 숫자를 가져오는 함수
    {
        return myCharacter[idx].getDice();
    }
    public int getDiceDir(int idx) //해당 캐릭터의 주사위의 각도를 가져오는 함수
    {
        return myCharacter[idx].getDiceDir();
    }

    public int getDiceNum(int idx, int diceIdx) //해당 캐릭터의 주사위 면의 숫자를 가져오는 함수
    {
        return myCharacter[idx].getDice(diceIdx);
    }
    public Item getCharacterItem(int characterIdx, int itemIdx)
    {
        return myCharacter[characterIdx].getItem(itemIdx);
    }
    public Skill getCharacterSkill(int characterIdx, int skillIdx)
    {
        return myCharacter[characterIdx].skillUse(skillIdx);
    }
    public void CharacterUpgrade(int idx, int type, int val)
    {
        
        if(val >= 0) myCharacter[idx].upGrade(type, val);
        else myCharacter[idx].downGrade(type, val);
    }
}
