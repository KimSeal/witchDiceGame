using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
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

    private Item[,] ItemArr = new Item[5,11];
    private bool[,] ItemExistArr = new bool[5, 11];

    private GameObject[] characterBoardState = new GameObject[4]; //캐릭터 보드의 선택버튼에 대한 object
    private GameObject[] itemBoardState = new GameObject[5]; //item 보드 선택버튼에 대한 object

    private GameObject[] CharacterUIArr = new GameObject[4]; //상단부 캐릭터 선택에 대한 오브젝트 모음
    private GameObject[] inventoryUIArr = new GameObject[11]; // 하단부 인벤토리에 대한 오브젝트 모음

    private GameObject[] infoBoardObj = new GameObject[5]; //이미지, 제목, 서브 설명, hp수치, mp 수치
    private GameObject[] diceBoardObj = new GameObject[7]; //주사위 각 면에 대한 이미지 처리를 위해 사용될 object
    private GameObject[,] skillBoardObj = new GameObject[2,7]; //스킬에 대한 이미지 처리를 위해 사용될 object 뒤줄은 메인이미지,제목,설명,필요주사위4개 순으로 index를 갖는다.
    private GameObject[] equipBoardObj = new GameObject[6]; //취득 아이템에 대한 이미지 처리를 위해 사용될 object


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
            if (curSelectItemIndex != -1)
            {
                changeAlpha(inventoryUIArr[curSelectItemIndex], 0.0f);
            }
            curSelectItemIndex = -1;
        } 
        
    }

    public void click_item_trash()
    {
        if(curSelectItemIndex != -1)
        {
            //주사위 클릭해서 바뀐후 아이템 삭제 및 선택한거 초기화(일단 item은 안건들이긴합니다. 나중에 빈 아이템 만들어서 배정해야할듯?)
            useItem();
        }
    }

    public void click_selectCharacter(int idx) //캐릭터 선택
    {
        if(CharacterManager.Instance.getCharacterState(idx) == 0) //캐릭터 전환이 되는 경우(생존해 있는 캐릭터!)
        {
            characterSelectIdx = idx;
            click_characterInfoType_selectButton(curSelectCharacterInfoType);
            for (int i=0;i<4;i++)
            {
                changeAlpha(CharacterUIArr[i], 0.7f);
            }
            changeAlpha(CharacterUIArr[idx], 0.0f);
        }
    }

    public void click_itemType_selectButton(int idx) // 중단부 아이템 종류 선택 버튼 클릭하는 경우
    {

        if(idx != curSelectItemType) //같으면 이동 안함. 다르니까 이동하는 경우에 대한 처리
        {
            curSelectItemIndex = -1; //택한 아이템 초기화
            curSelectItemType = idx; //타입 변경   
            for (int i=0;i < 11;i++)
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
            if (i == idx)
            {
                characterBoardState[i].SetActive(true);
                characterBoard_update(idx);
            }
            else characterBoardState[i].SetActive(false);
        }
    }
    private void characterBoard_update(int idx) //board 변경시 업데이트를 하기 위한 함수. character board 변경이나 character idx가 변경될 경우 사용하게 된다.
    {
        Character tempCharacter = CharacterManager.Instance.getCharacter(characterSelectIdx);
        if (tempCharacter == null || tempCharacter.getCurState() != 0) {
            return;
        }
        if (idx == 0) //개인 정보
        {
            

            if (Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + tempCharacter.getName() + "_face") != null)
            {
                infoBoardObj[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + tempCharacter.getName() + "_face");
            }
            else { infoBoardObj[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_noImage_face"); }
            infoBoardObj[1].GetComponent<TextMeshPro>().text = tempCharacter.getName();
            infoBoardObj[3].GetComponent<TextMeshPro>().text = tempCharacter.getHp().ToString() + "/" + tempCharacter.getMaxHp().ToString();
            infoBoardObj[4].GetComponent<TextMeshPro>().text = tempCharacter.getHp().ToString() + "/" + tempCharacter.getMaxHp().ToString(); //이후 Mp로 수정할것
        }
        else if (idx == 1) // 주사위
        {
            for (int i = 0; i < 6; i++)
            {
                diceBoardObj[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + CharacterManager.Instance.getDiceNum(characterSelectIdx, i).ToString());
            }
        }
        else if (idx == 2) //skill
        {
            Skill temp;
            for (int i=0;i<2;i++)
            {
                temp = CharacterManager.Instance.getCharacterSkill(characterSelectIdx, i);
                if(Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + temp.getSkillName()) == null)
                {
                    skillBoardObj[i, 0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                }
                else
                {
                    skillBoardObj[i, 0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + temp.getSkillName());
                }
                
                skillBoardObj[i, 1].GetComponent<TextMeshPro>().text = temp.getSkillName();
                skillBoardObj[i, 2].GetComponent<TextMeshPro>().text = temp.getCommand();
                for (int j=0;j<4; j++)
                {
                    Debug.Log("sprite/TestSprite/diceImage/needDice_" + temp.getNeedDice(j).ToString());
                    skillBoardObj[i, j + 3].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_" + temp.getNeedDice(j).ToString());
                }
            }
        }
        else if (idx == 3) // item(equip)
        {
            Item temp;
            for (int i = 0; i < 2; i++)
            {
                temp = CharacterManager.Instance.getCharacterItem(characterSelectIdx, i);
                equipBoardObj[i*3 + 0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/equipItemSprite/spr_item_equip_" + temp.getItemName());
                equipBoardObj[i * 3 + 1].GetComponent<TextMeshPro>().text = temp.getItemName();
                equipBoardObj[i * 3 + 2].GetComponent<TextMeshPro>().text = temp.getContent();
            }
        }
    }


    public void click_dice_changeNum(int idx) //
    {   //주사위 변수 값은 val1으로 변경했습니다
        if (curSelectItemType == 1 && curSelectItemIndex != -1) {
            CharacterManager.Instance.changeDice(characterSelectIdx, idx, ItemArr[1,curSelectItemIndex].getVal1());
            Debug.Log(diceBoardObj[idx]);

            diceBoardObj[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + ItemArr[1, curSelectItemIndex].getVal1().ToString());

            //주사위 클릭해서 바뀐후 아이템 삭제 및 선택한거 초기화(일단 item은 안건들이긴합니다. 나중에 빈 아이템 만들어서 배정해야할듯?)
            useItem();

        }
    }

    public void click_equip_changeNum(int idx) //
    {   //주사위 변수 값은 val1으로 변경했습니다
        if (curSelectItemType == 2 && curSelectItemIndex != -1)
        {
            Debug.Log("test Debug : " + curSelectItemIndex + "/" + ItemArr[2, curSelectItemIndex].getIdx());
            CharacterManager.Instance.changeEquip(characterSelectIdx, idx, 2, ItemArr[2, curSelectItemIndex].getIdx());

            equipBoardObj[idx * 3].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/equipItemSprite/spr_item_equip_" + ItemArr[2, curSelectItemIndex].getItemName().ToString());
            equipBoardObj[idx * 3 + 1].GetComponent<TextMeshPro>().text = ItemArr[2, curSelectItemIndex].getItemName();
            equipBoardObj[idx * 3 + 2].GetComponent<TextMeshPro>().text = ItemArr[2, curSelectItemIndex].getContent();

            //주사위 클릭해서 바뀐후 아이템 삭제 및 선택한거 초기화(일단 item은 안건들이긴합니다. 나중에 빈 아이템 만들어서 배정해야할듯?)
            useItem();

        }
    }

    private void useItem()
    {
        changeAlpha(inventoryUIArr[curSelectItemIndex], 0.0f);
        inventoryUIArr[curSelectItemIndex].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
        ItemArr[curSelectItemType, curSelectItemIndex] = null;
        ItemExistArr[curSelectItemType, curSelectItemIndex] = false;
        curSelectItemIndex = -1;
    }

    public Item getItem(int itemType, int itemIndex)
    {
        return itemList[itemType][itemIndex];
    }


    string [] typeArr = { "consume", "dice", "equip", "passive", "destiny"}; //item type string 

    private void updateInventory() //전체 inventory 업데이트
    {
        for (int i = 0; i < 11; i++)
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

    public void click_upgradeCanvas_start()
    {
        //초반 캐릭터는 살아있는 친구로 선택하는 코드 나중에 넘어올떄마다 실행시킬수 있도록 코드
        
        for (int i = 0; i < 4; i++)
        {
            changeAlpha(CharacterUIArr[i], 0.7f);
        }
        for (int i = 0; i < 4; i++)
        {
            if (CharacterManager.Instance.getCharacter(i).getCurState() == 0)
            {
                characterSelectIdx = i;
                changeAlpha(CharacterUIArr[i], 0.0f);
                break;
            }
        }
        characterBoard_update(0);
        //mainCamera.transform.position = new Vector3(-1000f,mainCamera.transform.position.y, mainCamera.transform.position.z);
    }
    
    public void updateCharacterUIBtn()
    {
        //CharacterUIArr[i]
        for (int characterSelectIdx = 0; characterSelectIdx < 4; characterSelectIdx++) {
            Character tempCharacter = CharacterManager.Instance.getCharacter(characterSelectIdx);
            if (tempCharacter == null || tempCharacter.getCurState() != 0)
            {
                CharacterUIArr[characterSelectIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_no_face");
            }
            else
            {
                if (Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + tempCharacter.getName() + "_face") != null)
                {
                    CharacterUIArr[characterSelectIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + tempCharacter.getName() + "_face");
                }
                else { CharacterUIArr[characterSelectIdx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_noImage_face"); }
            }
        }

    }

    GameObject mainCamera;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
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
            itemList[itemReaderList[i].type].Add(new Item(itemReaderList[i]));
        }

        characterBoardState[0] = GameObject.Find("itemUI_board_infoBoard");
        characterBoardState[1] = GameObject.Find("itemUI_board_diceBoard");
        characterBoardState[2] = GameObject.Find("itemUI_board_skillBoard");
        characterBoardState[3] = GameObject.Find("itemUI_board_itemBoard");

        //캐릭터 정보 칸을 위한 object 받기
        infoBoardObj[0] = GameObject.Find("board_info_characterImage");
        infoBoardObj[1] = GameObject.Find("board_info_characterName");
        infoBoardObj[2] = GameObject.Find("board_info_subExp");
        infoBoardObj[3] = GameObject.Find("board_info_HPVal");
        infoBoardObj[4] = GameObject.Find("board_info_MPVal");

        CharacterUIArr[0] = GameObject.Find("obj_itemUI_characterBtn_0");
        CharacterUIArr[1] = GameObject.Find("obj_itemUI_characterBtn_1");
        CharacterUIArr[2] = GameObject.Find("obj_itemUI_characterBtn_2");
        CharacterUIArr[3] = GameObject.Find("obj_itemUI_characterBtn_3");

        for (int i = 0; i < 11; i++)
        {
            for(int j=0;j<5;j++) ItemExistArr[j, i] = false; //아이템 없다는 것을 초기화를 통해 배정
            inventoryUIArr[i] = GameObject.Find("obj_inventory_" + i.ToString()); //inventory 오브젝트 설정
        }
        for (int i = 0; i < 6; i++)
        {
            diceBoardObj[i] = GameObject.Find("itemUI_board_diceBoard_diceBtn_" + i.ToString()); //inventory 오브젝트 설정
        }
        diceBoardObj[6] = GameObject.Find("itemUI_board_diceInfo");



        for (int i=0;i<2;i++)
        {
            skillBoardObj[i,0] = GameObject.Find("board_skill_skillImage_" + i.ToString());
            skillBoardObj[i,1] = GameObject.Find("board_skill_skillTitle_" + i.ToString());
            skillBoardObj[i,2] = GameObject.Find("board_skill_skillInfo_" + i.ToString());
            for (int j=0;j<4;j++) {
                skillBoardObj[i, j+3] = GameObject.Find("board_skill_needDice_" + i.ToString() + "_" + j.ToString());
            }
            
            equipBoardObj[0 + i * 3] = GameObject.Find("board_equip_equipImage_" + i.ToString());
            equipBoardObj[1 + i * 3] = GameObject.Find("board_equip_equipTitle_" + i.ToString());
            equipBoardObj[2 + i * 3] = GameObject.Find("board_equip_equipInfo_" + i.ToString());
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

        ItemExistArr[2, 0] = true;
        ItemArr[2, 0] = new Item(itemList[2][1]);
        ItemExistArr[2, 1] = true;
        ItemArr[2, 1] = new Item(itemList[2][2]);

        updateInventory();
    }

    public void setItem(int type, int index)
    {
        ItemExistArr[type, index] = true;
        ItemArr[type, index] = new Item(itemList[1][6]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
