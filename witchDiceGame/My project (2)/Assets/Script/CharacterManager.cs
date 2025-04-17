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
        Debug.Log(skillReaderList.Count);
        Debug.Log(skillList.Count);
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

            if(destinyReaderList[i].DestinyIdx <= 10000) destinyList.Add(new Destiny(destinyReaderList[i], skillArr));
            else if (destinyReaderList[i].DestinyIdx > 10000) destinyList_monster.Add(new Destiny(destinyReaderList[i], skillArr));
        }
        Debug.Log(destinyReaderList.Count);
        Debug.Log(destinyList.Count);

        //캐릭터 테스트 0번에 용사 배치
        setCharacter(0, 0);
        setCharacter(1, 0);
        setCharacter(2, 4);
        setCharacter(3, 4);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public int getCharacterState(int idx)
    {
        if (myCharacter[idx] == null) {
            return 3; 
        }
        return myCharacter[idx].getCurState();
    }
    public void changeDice(int characterIdx, int diceIdx, int diceNum)
    {
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


    //살아있는 캐릭터 배치
    public void setCharacter(int place, int characterIdx)
    {
        if (place > 3) place = 3;
        if (characterIdx == -99999) return;
        //아군
        if (characterIdx <= 10000) {
            switch (characterIdx)
            {
                case 0:
                    myCharacter[place] = new Yongsa(0, destinyList[characterIdx]); break;
                case 1:
                    myCharacter[place] = new Neaco(0, destinyList[characterIdx]); break;
                case 2:
                    myCharacter[place] = new Druid(0, destinyList[characterIdx]); break;
                case 3:
                    myCharacter[place] = new Tom(0, destinyList[characterIdx]); break;
                case 4:
                    myCharacter[place] = new Bob(0, destinyList[characterIdx]); break;

            }
        }
        //몬스터
        if (characterIdx > 10000) {
            characterIdx -= 10001;
            switch (characterIdx)
            {
                case 0:
                    enemyCharacter[place] = new Slime(0, destinyList_monster[characterIdx]); break;
                case 1:
                    enemyCharacter[place] = new Goblin(0, destinyList_monster[characterIdx]); break;
                case 2:
                    enemyCharacter[place] = new RoyalSoldier(0, destinyList_monster[characterIdx]); break;
                case 3:
                    enemyCharacter[place] = new Soldier(0, destinyList_monster[characterIdx]); break;
                case 4:
                    enemyCharacter[place] = new Chicken(0, destinyList_monster[characterIdx]); break;
                case 5:
                    enemyCharacter[place] = new Duck(0, destinyList_monster[characterIdx]); break;
                case 6:
                    enemyCharacter[place] = new Sheep(0, destinyList_monster[characterIdx]); break;
                case 7:
                    enemyCharacter[place] = new Pig(0, destinyList_monster[characterIdx]); break;
                case 8:
                    enemyCharacter[place] = new Wolf(0, destinyList_monster[characterIdx]); break;
            }
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
        myCharacter[idx].upGrade(type, val);
        Debug.Log("heal did!" + idx + " : "+type + " : "+ val);
    }
}
