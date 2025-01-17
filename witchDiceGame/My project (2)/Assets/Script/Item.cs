using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private int idx;
    private int type; // 0. consumable  1. dice   2. equip   3. passive   4.destiny 
    private string itemName;
    private string content;

    public Item(int idx, int type, string itemName, string content)
    {
        this.type = type;
        this.idx = idx;
        this.itemName = itemName;
        this.content = content;
    }
    public Item(ItemReader itemReader)
    {
        this.idx = itemReader.idx;
        this.type=itemReader.type;
        this.itemName = itemReader.itemName;
        this.content=itemReader.content;
    }

    public Item(Item item)
    {
        this.idx = item.idx;
        this.type = item.type;
        this.itemName = item.itemName;
        this.content = item.content;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
