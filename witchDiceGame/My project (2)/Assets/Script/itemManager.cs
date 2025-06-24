using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    private GameObject[] characterBoardState = new GameObject[5]; //캐릭터 보드의 선택버튼에 대한 object
    private GameObject[] itemBoardState = new GameObject[5]; //item 보드 선택버튼에 대한 object

    private GameObject[] CharacterUIArr = new GameObject[4]; //상단부 캐릭터 선택에 대한 오브젝트 모음
    private Animator[] CharacterStandArr = new Animator[4];// 중단부 캐릭터 스탠딩 애니메이션 오브젝트 모음
    private GameObject[] inventoryUIArr = new GameObject[11]; // 하단부 인벤토리에 대한 오브젝트 모음
    private GameObject inventoryUI; //하단부 인벤토리 전체에 대한 오브젝트

    private GameObject[] infoBoardObj = new GameObject[5]; //이미지, 제목, 서브 설명, hp수치, mp 수치
    private GameObject[] diceBoardObj = new GameObject[7]; //주사위 각 면에 대한 이미지 처리를 위해 사용될 object
    private GameObject[,] skillBoardObj = new GameObject[2,7]; //스킬에 대한 이미지 처리를 위해 사용될 object 뒤줄은 메인이미지,제목,설명,필요주사위4개 순으로 index를 갖는다.
    private GameObject[] equipBoardObj = new GameObject[6]; //취득 아이템에 대한 이미지 처리를 위해 사용될 object

    private GameObject bagBtnObj;

    private GameObject [] descObj = new GameObject[4];

    private int curSelectItemType = 0;  // 현재 선택한 아이템 종류 선택
    private int curSelectItemIndex = -1; // 현재 선택한 아이템의 인덱스

    private int characterSelectIdx = 0;//현재 선택된 캐릭터의 idx
    private int curSelectCharacterInfoType = 0; //현재 선택한 캐릭터 정보 창 종류

    private bool itemBoxMove = false;
    private bool itemBoxOpen = false;
    private int itemBoxOpenPoint = -1;


    private int dragObjStartNum =-1;
    private int dragObjEndNum = -1;


    private int dragCharacterStartNum = -1;
    private int dragCharacterEndNum = -1;

    //string[] typeArr = { "consume", "dice", "equip", "passive", "destiny" };

    public void hoverInItem(int idx)
    {
        Debug.Log("hover!");
        if (idx == 11) {
            if (descObj[0].activeSelf == false) descObj[0].SetActive(true);
            descObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            descObj[2].GetComponent<TextMeshPro>().text = "Delete Box";
            descObj[3].GetComponent<TextMeshPro>().text = "이곳으로 아이템을 드래그하면 버릴 수 있습니다.";
        }
        else if (ItemExistArr[curSelectItemType, idx]) //아이템이 있는 경우 해당 아이템으로 변경
        {
            if (descObj[0].activeSelf == false) descObj[0].SetActive(true);
            Item hoverItem = ItemArr[curSelectItemType, idx];
            descObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/" + typeArr[curSelectItemType] + "ItemSprite/spr_item_" + typeArr[curSelectItemType] + "_" + hoverItem.getItemName());
            descObj[2].GetComponent<TextMeshPro>().text = hoverItem.getItemName();
            descObj[3].GetComponent<TextMeshPro>().text = typeArr[curSelectItemType] + "\n" + hoverItem.getContent();

        }
        else
        {
            descObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            descObj[2].GetComponent<TextMeshPro>().text = "";
            descObj[3].GetComponent<TextMeshPro>().text = "";
            if (descObj[0].activeSelf == true) descObj[0].SetActive(false);

        }
    }
    public void hoverOutItem(int idx)
    {
        Debug.Log("hover out!");
        descObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
        descObj[2].GetComponent<TextMeshPro>().text = "";
        descObj[3].GetComponent<TextMeshPro>().text = "";
        if (descObj[0].activeSelf == true) descObj[0].SetActive(false);
    }
    public int getItemListCount(int idx)
    {
        return itemList[idx].Count;
    }
    public string getItemSprite(int type, int idx)
    {
        if (type == -99999 || idx == -99999 || type > 4 || idx >= itemList[type].Count || itemList[type][idx] == null){
            return "sprite/TestSprite/characterSkill/spr_skill_none";
        }
        return "sprite/TestSprite/itemSprite/" + typeArr[type] + "ItemSprite/spr_item_" + typeArr[type] + "_" + itemList[type][idx].getItemName();
    }
    public int findEmptyIdx(int type)
    {
        for (int i=0;i<11;i++)
        {
            if (!ItemExistArr[type, i]) return i;
        }
        return 999; // 공간 내 넣을 자리가 없음.
    }
    public int getItemResult(int type, int idx) //이벤트 결과로 부터 아이템을 받아왔을 경우, 가능한지 여부를 확인해 받아온다.
    {
        if (type == -99999 || idx == -99999 || type>4 || idx >= itemList[type].Count || itemList[type][idx] == null) {
            return 2; //존재 하지 않는 아이템 호출
        }
        int emptyIdx = findEmptyIdx(type);
        if (emptyIdx == 999) {
            return 1; //빈 공간이 없는 경우.
        }
        setItem(type, emptyIdx, idx); //공간이 존재하면 가장 왼쪽에 아이템을 배치한다.
        updateInventory();
        return 0;
    }

    public void turnOffItemCollider_item()
    {
        for (int i=0;i<11;i++)
        {
            inventoryUIArr[i].GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    public void turnOnItemCollider_item()
    {
        for (int i = 0; i < 11; i++)
        {
            inventoryUIArr[i].GetComponent<BoxCollider2D>().enabled = true;
        }
    }
    public void turnOffCharacterCollider_item()
    {
        for (int i = 0; i < 4; i++)
        {
            CharacterUIArr[i].GetComponent<BoxCollider2D>().enabled = false;
        }
    }
    public void turnOnCharacterCollider_item()
    {
        for (int i = 0; i < 4; i++)
        {
            CharacterUIArr[i].GetComponent<BoxCollider2D>().enabled = true;
        }
    }


    public void swapItem()
    {
        if(! (dragObjEndNum == -1 || dragObjStartNum == -1))
        {
            int a = dragObjStartNum;
            int b = dragObjEndNum;
            Item itemA = null;
            Item itemB = null;
            bool tempExistA = ItemExistArr[curSelectItemType, a];
            bool tempExistB = ItemExistArr[curSelectItemType, b];
            if (!tempExistA) itemA = null;
            else itemA = new Item(ItemArr[curSelectItemType, a]);
            if (!tempExistB) itemB = null;
            else itemB = new Item(ItemArr[curSelectItemType, b]);

            ItemExistArr[curSelectItemType, b] = tempExistA;
            ItemExistArr[curSelectItemType, a] = tempExistB;
            ItemArr[curSelectItemType, b] = itemA;
            ItemArr[curSelectItemType, a] = itemB;
            dragObjStartNum = -1;
            dragObjEndNum = -1;
            updateInventory();
            click_item_bagButton(curSelectItemIndex);
        }
    }

    public void swapCharacter()
    {
        if (!(dragCharacterStartNum == -1 || dragCharacterEndNum == -1)) {
        Character playerA = CharacterManager.Instance.getCharacter(dragCharacterStartNum);
        Character playerB = CharacterManager.Instance.getCharacter(dragCharacterEndNum);
        CharacterManager.Instance.setCharacter(dragCharacterStartNum, playerB);
        CharacterManager.Instance.setCharacter(dragCharacterEndNum, playerA);
            AdventureManager.Instance.resetDice();
            dragCharacterStartNum = -1;
        dragCharacterEndNum = -1;
        updateCharacterUIBtn();
        setUpAnimator();
        click_characterInfoType_selectButton(curSelectCharacterInfoType);
        updateCharacterBar();
        }
    }

    public void deleteCharacter()
    {
        bool deleteResult = false;
        if (CharacterManager.Instance.getCharacterState(characterSelectIdx) == 0) {
            deleteResult = CharacterManager.Instance.deleteCharacter(characterSelectIdx);
        }
        if (deleteResult) //캐릭터가 무사히 제거된 경우
        {
            AdventureManager.Instance.resetDice();
            updateCharacterUIBtn();
            setUpAnimator();
            click_characterInfoType_selectButton(curSelectCharacterInfoType);
            updateCharacterBar();
        }
    }

    //drag 관련 함수 모음
    #region
    public int getDragObjStartNum()
    {
        return dragObjStartNum;
    }
    public int getDragObjEndNum()
    {
        return dragObjEndNum;
    }

    public void setDragObjStartNum(int input)
    {
        dragObjStartNum = input;
    }
    public void setDragObjEndNum(int input)
    {
        dragObjEndNum = input;
    }

    public int getDragCharacterStartNum()
    {
        return dragCharacterStartNum;
    }
    public int getDragCharacterEndNum()
    {
        return dragCharacterEndNum;
    }

    public void setDragCharacterStartNum(int input)
    {
        dragCharacterStartNum = input;
    }
    public void setDragCharacterEndNum(int input)
    {
        dragCharacterEndNum = input;
    }
    #endregion

    private IEnumerator ItemMoveUI(GameObject gameObjTemp, int opt, int onOff) // onoff : 위로 올라갈때 0 아래로 내려갈때 1
    {
        float[] tempPointX = { -1148f, -648f, -148f};
        float[] tempPointY = { -200f, -88f };
        
        if (!itemBoxMove) { //혹시 모르니 한번더 이동중인지 확인
            gameObjTemp.transform.position = new Vector3(tempPointX[opt], tempPointY[onOff], 0);

            itemBoxMove = true;
            float termY = 0.3f;
            Vector3 destination = new Vector3(gameObjTemp.transform.position.x, tempPointY[(onOff+1)%2], 0);

            
            if (onOff == 0 && !itemBoxOpen) //상자가 잠겨있고 올라가는 경우
            {
                itemBoxOpenPoint = opt; //현재 포인트 변경
                termY *= -1;

                while (gameObjTemp.transform.position.y < destination.y + termY )
                {
                    gameObjTemp.transform.position = Vector3.Lerp(gameObjTemp.transform.position, destination, 0.05f);
                    yield return new WaitForSeconds(0.01f);
                    
                }
                itemBoxOpen = true;
                
            }
            else if(onOff == 1 && itemBoxOpen) // 상자가 열려있고 내려가는 경우
            {
                termY = 5.0f;
                while (gameObjTemp.transform.position.y > destination.y + termY*10)//inputY + termY)
                {
                    gameObjTemp.transform.position = Vector3.Lerp(gameObjTemp.transform.position, destination, 0.05f);
                    yield return new WaitForSeconds(0.01f);
                }
                itemBoxOpen =  false;
                itemBoxOpenPoint = -1; //종료 후 포인트 변경
            }
            gameObjTemp.transform.position = destination;
            Debug.Log("Arrive!");
            
            itemBoxMove = false;
        }
    }

    public bool getItemBoxMove()
    {
        return itemBoxMove;
    }
    public bool getItemBoxOpen()
    {
        return itemBoxOpen;
    }

    public void flipItemBox(int placeIdx, int onOff)
    {
        if (!itemBoxMove) { //box가 움직이지 않을때
            if (onOff == 0 && !itemBoxOpen) //열려있지 않으면 열수있게
            {
                StartCoroutine(ItemMoveUI(inventoryUI, placeIdx, 0));
            }
            else if (onOff == 1 && itemBoxOpen) // 열려있다면 닫을 수 있게.
            {   //이미 열려있다면 닫을 수 있게 한다
                StartCoroutine(ItemMoveUI(inventoryUI, placeIdx, 1));
            }
        }
    }

    public void flipItemBox_AdventureUI(bool onOff)
    {
        if (!itemBoxOpen) flipItemBox(1, 0);
        else flipItemBox(1, 1);
    }

    public void flipItemBox_BattleUI()
    {
        if (!itemBoxOpen) flipItemBox(2, 0);
        else flipItemBox(2, 1);
    }
    public void enterBattlePhase()
    {
        click_itemType_selectButton(3);
        bagBtnObj.SetActive(false);
    }
    public void endOfBattlePhase()
    {
        bagBtnObj.SetActive(true);
    }
    public void resetItemManager()
    {
        for (int i=0;i<5;i++)
        {
            for (int j = 0; j < 11; j++)
            {
                ItemArr[i, j] = null;
                ItemExistArr[i, j] = false;
            }

        }
    }


    public void click_item_bagButton(int idx) //하단부 아이템 박스에서 아이템 클릭하는 경우
    {
        if (idx != -1)
        {
            if (ItemExistArr[curSelectItemType, idx]) //아이템이 있는 경우 
            {

                if (curSelectItemIndex == idx)
                {// 이미 자신이 선택한 아이템인 경우 해제
                    changeAlpha(inventoryUIArr[curSelectItemIndex], 0.0f);
                    curSelectItemIndex = -1;

                }
                else //다른 경우 선택한 대상으로 변경
                {
                    if (curSelectItemIndex != -1) changeAlpha(inventoryUIArr[curSelectItemIndex], 0.0f); //-1이면 색 바꿀게 없다.
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
        for (int i=0;i<5;i++)
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
            infoBoardObj[4].GetComponent<TextMeshPro>().text = tempCharacter.getMp().ToString() + "/" + tempCharacter.getMaxMp().ToString(); //이후 Mp로 수정할것
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

    public void click_info_useItem()
    {
        if (curSelectItemType == 0 && curSelectItemIndex != -1 && characterSelectIdx >=0 && CharacterManager.Instance.getCharacter(characterSelectIdx) != null)
        {
            CharacterManager.Instance.CharacterUpgrade(characterSelectIdx, ItemArr[0, curSelectItemIndex].getVal(0), ItemArr[0, curSelectItemIndex].getVal(1));
            Debug.Log("click item! : " + characterSelectIdx + " : " + ItemArr[0, curSelectItemIndex].getVal(0) + " : " + ItemArr[0, curSelectItemIndex].getVal(1));
            useItem();
            click_characterInfoType_selectButton(0);
        }
       
    }

    private void changeDice(int idx, int number)
    {
        CharacterManager.Instance.changeDice(characterSelectIdx, idx, number);
        diceBoardObj[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + number.ToString());
        Debug.Log("change Dice "+ idx.ToString() + " to number " + number.ToString());
    }
    public void click_dice_changeNum(int idx) //
    {   //주사위 변수 값은 val1으로 변경했습니다
        if (curSelectItemType == 1 && curSelectItemIndex != -1) {
            int itemIdx = ItemArr[1, curSelectItemIndex].getIdx();
            
            if (itemIdx == 1) { //랜덤한 숫자로 변경 
                changeDice(idx, Random.Range(1,7));
            }
            else if (itemIdx >= 2 && itemIdx <= 7) //해당 숫자로 변경
            {
                changeDice(idx, ItemArr[1, curSelectItemIndex].getVal(0));
            }
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

        setUpAnimator();

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

    void updateCharacterBar()
    {
        for (int i = 0; i < 4; i++)
        {
            changeAlpha(CharacterUIArr[i], 0.7f);
        }
        for (int i = 0; i < 4; i++)
        {
            if (CharacterManager.Instance.getCharacter(i) != null && CharacterManager.Instance.getCharacter(i).getCurState() == 0)
            {
                characterSelectIdx = i;
                changeAlpha(CharacterUIArr[i], 0.0f);
                break;
            }
        }
        characterBoard_update(0);
    }
    public void click_upgradeCanvas_start()
    {
        //초반 캐릭터는 살아있는 친구로 선택하는 코드 나중에 넘어올떄마다 실행시킬수 있도록 코드

        updateCharacterBar();
    
        StartCoroutine(ItemMoveUI(inventoryUI, 0, 0));

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
        characterBoardState[4] = GameObject.Find("itemUI_board_exitBoard");

        //battle phase시 아이템 선택 제거
        bagBtnObj = GameObject.Find("item_btn");

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

        CharacterStandArr[0] = GameObject.Find("obj_itemUI_character_0").GetComponent<Animator>();
        CharacterStandArr[1] = GameObject.Find("obj_itemUI_character_1").GetComponent<Animator>();
        CharacterStandArr[2] = GameObject.Find("obj_itemUI_character_2").GetComponent<Animator>();
        CharacterStandArr[3] = GameObject.Find("obj_itemUI_character_3").GetComponent<Animator>();

        CharacterStandArr[0].Play("Idle");
        CharacterStandArr[1].Play("Idle");
        CharacterStandArr[2].Play("Idle");
        CharacterStandArr[3].Play("Idle");

        inventoryUI = GameObject.Find("obj_inventory");
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

        for (int i = 1; i < 5; i++)
        {
            characterBoardState[i].SetActive(false);
        }

        setUpAnimator();

        descObj[0] = GameObject.Find("obj_ui_item_Desc_board");
        descObj[1] = GameObject.Find("obj_ui_item_Desc_logo");
        descObj[2] = GameObject.Find("obj_ui_item_Desc_name");
        descObj[3] = GameObject.Find("obj_ui_item_Desc_desc");
        descObj[0].SetActive(false);

        
        //test Sample
        for (int i=0;i<7;i++) {
            ItemExistArr[1, i] = true;
            ItemArr[1,i] = new Item(itemList[1][i+1]);
            ItemExistArr[3, i] = true;
            ItemArr[3, i] = new Item(itemList[3][i+1]);
        }
        ItemExistArr[0, 0] = true;
        ItemArr[0, 0] = new Item(itemList[0][1]);
        ItemExistArr[0, 1] = true;
        ItemArr[0, 1] = new Item(itemList[0][2]);

        ItemExistArr[2, 0] = true;
        ItemArr[2, 0] = new Item(itemList[2][1]);
        ItemExistArr[2, 1] = true;
        ItemArr[2, 1] = new Item(itemList[2][2]);
        
        updateInventory();
    }

    private void setItem(int type, int index, int ItemIndex) //실제로 아이템을 배치하는 코드
    {
        ItemExistArr[type, index] = true;
        ItemArr[type, index] = new Item(itemList[type][ItemIndex]);
    }

    public Vector3 getItemInventoryPosition(int idx)
    {
        return inventoryUIArr[idx].transform.position;
    }
    public void setUpAnimator()
    {
        for (int i=0;i<4;i++)
        {
            
            if (CharacterManager.Instance.getCharacterState(i) == 0) {
                CharacterStandArr[i].runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + CharacterManager.Instance.getName_itemManager(i) + "/animator_" + CharacterManager.Instance.getName_itemManager(i));
            }
            else
            {
                CharacterStandArr[i].runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/animator_noneCharacter");
            }
        }

    }

    //passive Item use function start
    public bool usePassiveItem(TakeSkillPacket takeSkillPacket, int idx, int[] diceArr)
    {
        if (!ItemExistArr[3, idx]) {return false;} // 아이템이 없으면 그냥 스킵
        Item item = ItemArr[3, idx];
        if (item.getVal(0) != takeSkillPacket.getSkillType()) return false; //스킬이 아이템 타입하고 안맞으면 종료 
        if (!conditionCheck_dice(diceArr, item.getVal(1), item.getVal(2), item.getVal(3), item.getVal(4), item.getVal(5))) return false; //조건이 안맞으면 return

        switch (item.getIdx())
        {
            case 1:
                takeSkillPacket.addVal(item.getVal(3)); break;
            case 2:
                takeSkillPacket.addVal(item.getVal(3)); break;
            case 3:
                takeSkillPacket.mulVal(item.getVal(3)); break;
            case 4:
                takeSkillPacket.mulVal(item.getVal(3)); break;
            case 5:
                takeSkillPacket.addVal(item.getVal(3)); break;
            case 6:
                takeSkillPacket.mulVal(item.getVal(3)); break;
            case 7:
                takeSkillPacket.addVal(item.getVal(3)); break;
            case 8:
                takeSkillPacket.addVal(item.getVal(3)); break;
        }


        return true;
    }

    private bool conditionCheck_dice(int[] diceArr, int condition0, int condition1, int condition2, int condition3, int condition4)
    {   //diceArr : 주사위에 대한 조건 확인
        //condition0 : 조건,
        //condition1, 2, 3, 4는 조건에 대한 값.
        int[] arr = { 0,0, 0, 0, 0, 0, 0 }; //각 주사위의 가진 수
        for (int i=0;i<4;i++)
        {
            if (diceArr[i] >=1 &&  diceArr[i] < 7)
            {
                arr[diceArr[i]]++;
            }
        }

        //존재 개수를 기반하는 조건들의 계산
        if(condition0 >=1 && condition0 <= 4){ return condition0 <= sumOfNumber(arr, condition1); } //한개일때 숫자
        if ((condition0 >= 11 && condition0 <= 13) || condition0 == 22) return (condition0 / 10 <= sumOfNumber(arr, condition1)) && (condition0 % 10 <= sumOfNumber(arr, condition2));
        if (condition0 >= 111 && condition0 <= 112) return (condition0 / 100 <= sumOfNumber(arr, condition1)) && ((condition0 % 100) / 10 <= sumOfNumber(arr, condition2)) && (condition0 % 10 <= sumOfNumber(arr, condition3));
        if (condition0 == 1111) return (condition0 / 1000 <= sumOfNumber(arr, condition1)) && ((condition0 % 1000) / 100 <= sumOfNumber(arr, condition2)) && ((condition0 % 100) / 10 <= sumOfNumber(arr, condition3)) && (condition0 % 10 <= sumOfNumber(arr, condition4)) ;
        if (condition0 == 5) return condition1 == sumOfNumber(arr, 0);
        if (condition0 == 6) return condition1 <= sumOfNumber(arr, 0);
        if (condition0 == 7) return condition1 >= sumOfNumber(arr, 0);
        return false;
    }
    private int sumOfNumber(int[] arr, int opt) //각 option에 해당하는 주사위의 수 확인
    {
        int result = 0;
        if (opt == 0) return arr[1] + arr[3] + arr[5] + arr[2] + arr[4] + arr[6];
        if (opt > 0 && opt <= 6) return arr[opt];
        if(opt == 7) return arr[1] + arr[3] + arr[5];
        if (opt == 8) return arr[2] + arr[4] + arr[6];
        if(opt >= 11 && opt <= 16) {    //이하
            for (int i=1;i<=opt%10;i++) {result += arr[i];} //1부터 해당 값 도달할때 까지
            return result;
        }
        if (opt >= 21 && opt <= 26)
        {    //이상
            for (int i = 6; i >= opt % 10; i--) { result += arr[i]; } //6부터 해당 값 도달할때 까지
            return result;
        }
        return result;
    }

    // passive Item use function End


    // Update is called once per frame
    void Update()
    {
        
    }
}
