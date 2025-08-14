using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item 
{
    private int idx;
    private int type; // 0. consumable  1. dice   2. equip   3. passive   4.destiny 
    private int rare; // 0 : 일단 1 : 희귀 2 : 영웅 3 : 전설 4 : 그 이상(아직 못정함) 
    private string itemName;
    private string[] content = new string[10];
    private int[] val = new int[8];

    public Item(int idx, int type, string itemName, string contentKR, string contentEN, string contentJP, int val0, int val1, int val2, int val3, int val4, int val5, int val6, int val7)
    {
        this.type = type;
        this.idx = idx;
        this.itemName = itemName;
        this.content[0] = contentKR;
        this.content[1]= contentEN;
        this.content[2] = contentJP;
        this.val[0] = val0;
        this.val[1] = val1;
        this.val[2] = val2;
        this.val[3] = val3;
        this.val[4] = val4;
        this.val[5] = val5;
        this.val[6] = val6;
        this.val[7] = val7;
    }
    public Item(ItemReader itemReader)
    {
        this.idx = itemReader.idx;
        this.type=itemReader.type;
        this.itemName = itemReader.itemName;

        this.content[0] = TalkManager.Instance.SpecialTextChange(itemReader.contentKR);
        this.content[1] = TalkManager.Instance.SpecialTextChange(itemReader.contentEN);
        this.content[2] = TalkManager.Instance.SpecialTextChange(itemReader.contentJP);

        this.val[0] = itemReader.val0;
        this.val[1] = itemReader.val1;
        this.val[2] = itemReader.val2;
        this.val[3] = itemReader.val3;
        this.val[4] = itemReader.val4;
        this.val[5] = itemReader.val5;
        this.val[6] = itemReader.val6;
        this.val[7] = itemReader.val7;
        this.rare = itemReader.rare;
    }

    public Item(Item item)
    {
        this.idx = item.idx;
        this.type = item.type;
        this.itemName = item.itemName;
        for(int i=0;i<content.Length;i++) this.content[i] = item.content[i];
        for (int i=0;i<8;i++)
        {
            this.val[i] = item.val[i];
        }
        this.rare = item.rare;
    }

    public int getRare()
    {
        return this.rare;
    }
    public int getIdx()
    {
        return this.idx;
    }
    public int getType()
    {
        return type;
    }
    public string getItemName()
    {
        return itemName;
    }
    public string getContent()
    {
        return content[jsonDataManager.Instance.getLanguage()];
    }
    public int getVal(int idx)
    {
        return this.val[idx];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
