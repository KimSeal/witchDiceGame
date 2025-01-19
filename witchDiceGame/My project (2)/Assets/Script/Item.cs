using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private int idx;
    private int type; // 0. consumable  1. dice   2. equip   3. passive   4.destiny 
    private string itemName;
    private string content;
    private int val1;

    public Item(int idx, int type, string itemName, string content, int val1)
    {
        this.type = type;
        this.idx = idx;
        this.itemName = itemName;
        this.content = content;
        this.val1 = val1;
    }
    public Item(ItemReader itemReader)
    {
        this.idx = itemReader.idx;
        this.type=itemReader.type;
        this.itemName = itemReader.itemName;
        this.content=itemReader.content;
        this.val1 = itemReader.val1;
    }

    public Item(Item item)
    {
        this.idx = item.idx;
        this.type = item.type;
        this.itemName = item.itemName;
        this.content = item.content;
        this.val1 = item.val1;
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
        return content;
    }
    public int getVal1()
    {
        return val1;
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
