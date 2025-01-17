using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.ComponentModel;

public class SkillReader
{
    public int SkillIdx;
    public string SkillName;
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
    public string Command;
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
}

public class ItemReader
{
    public int idx;
    public int type;
    public string itemName;
    public string content;
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