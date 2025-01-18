using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemManager : MonoBehaviour
{

    private static itemManager instance = null;
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
    public static itemManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }

    private void changeAlpha(GameObject gameobj, float alphaVal)
    {
        Material material = gameobj.GetComponent<SpriteRenderer>().material;
        material.SetFloat("_Transparency", alphaVal);
        /*
        if (alphaVal == 0.7f)
        {
            //currentLightUI++;
            Material material = gameobj.GetComponent<SpriteRenderer>().material;
            material.SetFloat("_Transparency", 0.0f);
            
            float curAlpha = material.GetFloat("_Transparency");

            while (curAlpha < alphaVal)
            {
                material.SetFloat("_Transparency", curAlpha);
                curAlpha += 0.05f;
                yield return new WaitForSeconds(0.01f);
            }
            //currentLightUI--;
        }*/
     
    }


    public List<Item>[] itemList = new List<Item>[5];
    public List<ItemReader> itemReaderList = new List<ItemReader>();

    private Item[,] ItemArr = new Item[5,12];
    private bool[,] ItemExistArr = new bool[5, 12];

    private GameObject[] characterBoardState = new GameObject[4]; //캐릭터 보드의 선택버튼에 대한 object
    private GameObject[] itemBoardState = new GameObject[5]; //item 보드 선택버튼에 대한 object

    private GameObject[] inventoryUIArr = new GameObject[12]; // 하단부 인벤토리에 대한 오브젝트 모음

    private GameObject[] diceBoardButton = new GameObject[6]; //주사위 각 면에 대한 이미지 처리를 위해 사용될 object

    

    private int curSelectItemType = 0;  // 현재 선택한 아이템 종류 선택
    private int curSelectItemIndex = -1; // 현재 선택한 아이템의 인덱스

    private int characterSelectIdx = 0;//현재 선택된 캐릭터의 idx
    private int curSelectCharacterInfoType = 0; //현재 선택한 캐릭터 정보 창 종류

    public void click_item_bagButton(int idx) //하단부 아이템 박스에서 아이템 클릭하는 경우
    {
        if(ItemExistArr[curSelectItemType, idx]) //아이템이 있는 경우 
        {
            
            if (curSelectItemIndex == idx) {// 이미 자신이 선택한 아이템인 경우 해제
                changeAlpha(inventoryUIArr[curSelectItemIndex], 0.0f);
                curSelectItemIndex = -1;

            }
            else //다른 경우 선택한 대상으로 변경
            {
                if(curSelectItemIndex != -1) changeAlpha(inventoryUIArr[curSelectItemIndex], 0.0f); //-1이면 색 바꿀게 없다.
                curSelectItemIndex = idx;
                changeAlpha(inventoryUIArr[curSelectItemIndex], 0.7f);
            }
        }
        else //아이템이 없는 경우 해제
        {
            changeAlpha(inventoryUIArr[curSelectItemIndex], 0.0f);
            curSelectItemIndex = -1;
        } 
        
    }
    public void click_selectCharacter(int idx)
    {
        characterSelectIdx = idx;// 이거 character있는지 없는지 확인해야함. + info도 수정해줘야함.
    }

    public void click_itemType_selectButton(int idx) // 중단부 아이템 종류 선택 버튼 클릭하는 경우
    {

        if(idx != curSelectItemType) //같으면 이동 안함. 다르니까 이동하는 경우에 대한 처리
        {
            curSelectItemIndex = -1; //택한 아이템 초기화
            curSelectItemType = idx; //타입 변경   
            for (int i=0;i < 12;i++)
            {
                changeAlpha(inventoryUIArr[i], 0.0f);
            }
            updateInventory();
        }
    }

    public void click_characterInfoType_selectButton(int idx) // 캐릭터 정보 창에서 선택한 정보
    {
        curSelectCharacterInfoType = idx;
        for (int i=0;i<4;i++)
        {
            if(i == idx) characterBoardState[i].SetActive(true);
            else characterBoardState[i].SetActive(false);
        }
    }

    public void click_dice_changeNum(int idx) //
    {   //주의! 현재는 변경될 주사위 num에 대한 정보가 없어서 idx로 처리중이다. 추후 val 1이런거 추가하면 수정필요
        if (curSelectItemType == 1 && curSelectItemIndex != -1) {
            CharacterManager.Instance.changeDice(characterSelectIdx, idx, ItemArr[1,curSelectItemIndex].getIdx());
            Debug.Log(diceBoardButton[idx] );

            diceBoardButton[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + ItemArr[1, curSelectItemIndex].getIdx().ToString());

            //주사위 클릭해서 바뀐후 아이템 삭제 및 선택한거 초기화(일단 item은 안건들이긴합니다. 나중에 빈 아이템 만들어서 배정해야할듯?)
            changeAlpha(inventoryUIArr[curSelectItemIndex], 0.0f);
            inventoryUIArr[curSelectItemIndex].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            ItemExistArr[curSelectItemType, curSelectItemIndex] = false;
            curSelectItemIndex = -1;

        }
    }

    string [] typeArr = { "consume", "dice", "equip", "passive", "destiny"}; //item type string 

    private void updateInventory() //전체 inventory 업데이트
    {
        for (int i = 0; i < 12; i++)
        {
            if (ItemExistArr[curSelectItemType, i]) //아이템이 있는 경우 해당 아이템으로 변경
            {
                inventoryUIArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/"+ typeArr[curSelectItemType]+"ItemSprite/spr_item_" + typeArr[curSelectItemType]+
                    "_" + ItemArr[curSelectItemType, i].getItemName());
            }
            else
            {
                inventoryUIArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {

        curSelectItemType = 0;  // 현재 선택한 아이템 종류 선택
        curSelectItemIndex = -1; // 현재 선택한 아이템의 인덱스
        curSelectCharacterInfoType = 0;

        for (int i=0;i<5;i++)
        {
            itemList[i] = new List<Item>();
        }

        itemReaderList = CSVReader.Read<ItemReader>("Item");    //csv 읽고 해당 Item을 타입에 맞춰 넣는 모습
        Debug.Log("ItemReadList Count is : " + itemReaderList.Count);
        for (int i=0;i<itemReaderList.Count;i++)
        {
            Debug.Log("type say : " + itemReaderList[i].type);
            itemList[itemReaderList[i].type].Add(new Item(itemReaderList[i]));
        }

        characterBoardState[0] = GameObject.Find("itemUI_board_infoBoard");
        characterBoardState[1] = GameObject.Find("itemUI_board_diceBoard");
        characterBoardState[2] = GameObject.Find("itemUI_board_skillBoard");
        characterBoardState[3] = GameObject.Find("itemUI_board_itemBoard");
        //itemBoardState[0]

        for (int i = 0; i < 12; i++)
        {
            for(int j=0;j<5;j++) ItemExistArr[j, i] = false; //아이템 없다는 것을 초기화를 통해 배정
            inventoryUIArr[i] = GameObject.Find("obj_inventory_" + i.ToString()); //inventory 오브젝트 설정
        }
        for (int i = 0; i < 6; i++)
        {
            diceBoardButton[i] = GameObject.Find("itemUI_board_diceBoard_diceBtn_" + i.ToString()); //inventory 오브젝트 설정
        }
        for (int i = 1; i < 4; i++)
        {
            characterBoardState[i].SetActive(false);
        }

        

        //test Sample
        for (int i=0;i<7;i++) {
            ItemExistArr[1, i] = true;
            ItemArr[1,i] = new Item(itemList[1][i]);
        }
        ItemExistArr[0, 0] = true;
        ItemArr[0, 0] = new Item(itemList[0][0]);
        ItemExistArr[0, 1] = true;
        ItemArr[0, 1] = new Item(itemList[0][1]);

        updateInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
