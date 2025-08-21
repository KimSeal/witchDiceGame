using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;

public class WitchPowerReader
{
    public string PowerIdx;
    public string PowerName;
    public string KR;
    public string EN;
    public string JP;
    public string SC;
}

public class SkillReader
{
    public int SkillIdx;
    public string SkillName;

    public int SpecialVal;

    public int NeedDiceNum;
    public int NeedDice0;
    public int NeedDice1;
    public int NeedDice2;
    public int NeedDice3;

    public int TargetAuto;
    public int TargetTeam;
    public int TargetObj;
    public int TargetNum;
    public int TargetChance;
    public int Var0;
    public int Var1;
    public int Var2;
    public string CommandKR;
    public string CommandEN;
    public string CommandJP;
    public int Anim;
    public int AnimMove;

    
}

public class DestinyReader
{
    public int DestinyIdx;
    public string Enemy;
    public string Name;
    public string Sex;
    public int phyAtk;
    public int magAtk;
    public int phyDef;
    public int magDef;
    public int maxHp;
    public int maxMp;
    public int shadow;
    public int skill0;
    public int skill1;
    public int skill2;
    public int skill3;
    public int skill4;
    public int skill5;
    public int skill6;
    public int skill7;
    public int skill8;
    public int skill9;
    public int money;
}

public class ItemReader
{
    public int idx;
    public int type;
    public int rare;
    public string itemName;
    
    public int val0, val1, val2, val3, val4, val5, val6, val7;
    public int activeTiming;
    public string contentKR, contentEN, contentJP;
}

public class AdventureEventReader
{
    public int level;
    public string NPCSprite;
    public string backgroundSprite;
    public int eventType;
    public int diceUse;
    public int eventIdx;
    public string eventName;
    public int stageIdx; // 해당 이벤트가 나오게 되는 스테이지의 idx
    public int levelIdxStart; //해당 이벤트가 나올 수 있는 스테이지의 단계 최소값
    public int levelIdxEnd; // 해당 이벤가 나올 수 있는 스테이지의 최대값
    public string selectTextKR, selectTextEN, selectTextJP; //이벤트 전체에 대한 설명
}
public class AdventureEventPacketReader
{
    public int eventIdx, diceNum, selectType,
        selectVal0, selectVal1, selectVal2, selectVal3, selectVal4, selectVal5, selectVal6, selectVal7;
    public string chooseTextKR, chooseTextEN, chooseTextJP, resultTextKR, resultTextEN, resultTextJP;
    public string spriteIndex;
    public int itemExist, itemType0, itemIdx0, itemType1, itemIdx1, itemType2, itemIdx2, itemType3, itemIdx3;
    public string battleBackSprite;
}

public class DescReader
{
    public int index;
    public string KR;
    public string EN;
    public string JP;
}

public class TalkReader{
    public int talkIdx;
    public int Idx;
    public string characterLeft;
    public string characterLeftFace;
    public string characterRight;
    public string characterRightFace;

    public string characterLeftest;
    public string characterLeftestFace;
    public string characterRightest;
    public string characterRightestFace;

    public float characterLeftX;
    public float characterLeftestX;
    public float characterRightX;
    public float characterRightestX;

    public string brightCharacter;
    public int imagePlace;
    public int imageIdx;
    public int eventType;
    public string Name;
    public string backGround;

    public string TextKR;
    public string TextEN;
    public string TextJP;
    
}

public class CSVReader
{
    //static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    //static char[] TRIM_CHARS = { '\"' };

    public static List<T> Read<T>(string file) where T : new()
    {
        List<T> list = new List<T>();
        TextAsset data = Resources.Load(file) as TextAsset;
        
        //byte[] byteData = File.ReadAllBytes(Application.dataPath + "/" + file + ".csv");


        string[] lines = Regex.Split(data.text, LINE_SPLIT_RE);
        //string[] lines = data.text.Split("\r\n");

        //if (lines.Length <= 1) return list;

        /* test DebugLog)
        Debug.Log(data.text);
        Debug.Log("Test : " + lines[0]);
        Debug.Log("this line Num is " + lines.Length);
        */

        string[] header = lines[0].Split(",");
        for (int i = 1; i < lines.Length; i++)
        {
            if(i == lines.Length - 1) { continue; }
            string[] values = lines[i].Split(",");
            //string[] values = Regex.Split(lines[i], SPLIT_RE);
            //if (values.Length == 0 || values[0] == "") continue;

            T entry = new T();
            for (int j = 0; j < values.Length && j < header.Length; j++)
            {
                //Debug.Log(values[j] + " : i check this");
                System.Reflection.FieldInfo fieldInfo = typeof(T).GetField(header[j]);
                //Debug.Log(header[j]);
                //Debug.Log(fieldInfo.FieldType);
                TypeConverter typeConverter = TypeDescriptor.GetConverter(fieldInfo.FieldType);
                fieldInfo.SetValue(entry, typeConverter.ConvertFrom(values[j]));
            }
            //Debug.Log(i + " : " + entry);
            list.Add(entry);
        }
        //Debug.Log("CSV Reader : " + list.Count.ToString());
        return list;
    }

}