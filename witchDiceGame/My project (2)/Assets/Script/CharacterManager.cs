using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    //ΩÃ≈¨≈Ê
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

    public List<Destiny> destinyList = new List<Destiny>();
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

            destinyList.Add( new Destiny(destinyReaderList[i], skillArr) );
        }
        Debug.Log(destinyReaderList.Count);
        Debug.Log(destinyList.Count);

        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setCharacter(int place, int characterIdx)
    {
        myCharacter[place] = new Character(0, destinyList[characterIdx]);
    }
    public Character getCharacter(int idx)
    {
        return myCharacter[idx];
    }
    public void setcharacterHp(int idx, int hp)
    {
        myCharacter[idx].setHp(hp);
    }
}
