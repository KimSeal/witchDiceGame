using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class passiveReturn
{
    public bool used = false;
    public string cal = "";
    public int val = 0;
    public passiveReturn(bool used, string cal, int val)
    {
        this.used = used;
        this.cal = cal;
        this.val = val;
    }
}
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
    [SerializeField]
    public GameObject effObj;

    [SerializeField]
    public GameObject getOutButton;

    public List<Item>[] itemList = new List<Item>[5];
    public List<Item>[,] itemListChapter = new List<Item>[5, 10];
    public List<ItemReader> itemReaderList = new List<ItemReader>();
    public List<Item>[] specialItemList = new List<Item>[10];

    private Item[,] ItemArr = new Item[5,11];
    private bool[,] ItemExistArr = new bool[5, 11];

    /*
    [SerializeField] private GameObject[] characterBoardState = new GameObject[5]; //캐릭터 보드의 선택버튼에 대한 object
    private GameObject[] itemBoardState = new GameObject[5]; //item 보드 선택버튼에 대한 object

    [SerializeField] private GameObject[] CharacterUIArr = new GameObject[4]; //상단부 캐릭터 선택에 대한 오브젝트 모음
    [SerializeField] private GameObject[] CharacterStandArrInit = new GameObject[4];// 중단부 캐릭터 스탠딩 애니메이션 오브젝트 모음
    private Animator[] CharacterStandArr = new Animator[4];// 중단부 캐릭터 스탠딩 애니메이션 오브젝트 모음
    [SerializeField]private GameObject[] inventoryUIArr = new GameObject[11]; // 하단부 인벤토리에 대한 오브젝트 모음, obj_inventory_(number)
    [SerializeField] private GameObject inventoryUI; //하단부 인벤토리 전체에 대한 오브젝트

    [SerializeField] private GameObject[] infoBoardObj = new GameObject[5]; //이미지, 제목, 서브 설명, hp수치, mp 수치
    [SerializeField] private GameObject[] diceBoardObj = new GameObject[7]; //주사위 각 면에 대한 이미지 처리를 위해 사용될 object itemUI_board_diceBoard_diceBtn_(number)
    [SerializeField] private GameObject[] skillBoardObjInit = new GameObject[14];
    private GameObject[,] skillBoardObj = new GameObject[2,7]; //스킬에 대한 이미지 처리를 위해 사용될 object 뒤줄은 메인이미지,제목,설명,필요주사위4개 순으로 index를 갖는다.
    [SerializeField] private GameObject[] equipBoardObj = new GameObject[6]; //취득 아이템에 대한 이미지 처리를 위해 사용될 object
    */
    [SerializeField]private GameObject bagBtnObj; //item_btn

    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject[] itemBoxInitPointInit = new GameObject[12]; //obj_inventory_box_(number)
    private Vector3[] itemBoxInitPoint = new Vector3[12];

    [SerializeField] private GameObject [] descObj = new GameObject[4]; //obj_ui_item_Desc_ board/logo/name/desc



    [SerializeField]
    public GameObject characterUIEntity;
    public GameObject characterSprite;
    public GameObject characterOrigin;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI[] characterHp;
    [SerializeField]
    public GameObject characterInfoEntity;
    public TextMeshProUGUI characterAtk;
    public TextMeshProUGUI characterMag;
    public TextMeshProUGUI characterSpd;
    public TextMeshProUGUI characterArmor;
    public GameObject[] characterDice = new GameObject[6];
    public GameObject[] characterSkill = new GameObject[2];
    public GameObject[] characterEquip = new GameObject[2];
    public GameObject[] characterDiceOutline = new GameObject[6];
    public GameObject[] characterSkillOutline = new GameObject[2];
    public GameObject[] characterEquipOutline = new GameObject[2];
    public GameObject characterDescImage;
    public TextMeshProUGUI characterDescTitle;
    public TextMeshProUGUI characterDescText;
    public GameObject[] characterDescDice = new GameObject[4];

    //이거 아래는 일단 대기. 없애기에는 리스크가 있다.
    private int curSelectItemType = 0;  // 현재 선택한 아이템 종류 선택
    private int curSelectItemIndex = -1; // 현재 선택한 아이템의 인덱스

    private int characterSelectIdx = -1;//현재 선택된 캐릭터의 idx -> 얘는 둬야 할듯

    private int[,] itemArrLengthByChapter = new int[5,10];

    private int itemMaxNum = 6;

    //string[] typeArr = { "consume", "dice", "equip", "passive", "destiny" };
    [SerializeField]
    public GameObject changeDiceEff;

    void Update()
    {
        characterSprite.GetComponent<Image>().sprite = characterOrigin.GetComponent<SpriteRenderer>().sprite;
        characterSprite.GetComponent<RectTransform>().sizeDelta = new Vector2(
            characterOrigin.GetComponent<SpriteRenderer>().sprite.bounds.size.x,
           characterOrigin.GetComponent<SpriteRenderer>().sprite.bounds.size.y);
    }

    public int getItemMaxNum()
    {
        return itemMaxNum;
    }
    public void setItemMaxNum(int number)
    {
        if (itemMaxNum > number) {
            for (int i = itemMaxNum - 1; i >= number; i--) {
                upDownManager.Instance.activePassiveItem(i, -1);
            }
        }
        if (itemMaxNum < number) {
            for (int i=itemMaxNum; i<number; i++) {
                upDownManager.Instance.activePassiveItem(i, -1);
            } 
        }
        itemMaxNum = number;
        updateInventory();
    }
    public bool getItemExistArr(int idx)
    {
        return ItemExistArr[curSelectItemType, idx];
    }

    public int getItemNum(int itemType)
    {
        int result = 0;
        for (int i=0;i<11;i++)
        {
            if (ItemExistArr[itemType, i]) result++;
        }
        return result;
    }
    public Item getCurItem(int idx)
    {
        return ItemArr[curSelectItemType, idx];
    }
    public int getCurSelectItemType()
    {
        return curSelectItemType;
    }
    public void hoverInItem(int idx)
    {
        if (idx == 11) {
            if (descObj[0].activeSelf == false) descObj[0].SetActive(true);
            upDownManager.Instance.updateUpperHoverBar(1, null);
        }
        else if (ItemExistArr[curSelectItemType, idx]) //아이템이 있는 경우 해당 아이템으로 변경
        {
            upDownManager.Instance.updateUpperHoverBar(0,ItemArr[curSelectItemType, idx] );
            /*
            if (descObj[0].activeSelf == false) descObj[0].SetActive(true);
            Item hoverItem = ItemArr[curSelectItemType, idx];
            descObj[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/battleResultUI/spr_selectUI_board_" + hoverItem.getRare() + "_90");
            descObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/" + typeArr[curSelectItemType] + "ItemSprite/spr_item_" + typeArr[curSelectItemType] + "_" + hoverItem.getItemName());
            descObj[2].GetComponent<TextMeshPro>().text = hoverItem.getItemName();
            descObj[3].GetComponent<TextMeshPro>().text = typeArr2[curSelectItemType] + "\n" + hoverItem.getContent();
            */
        }
        else
        {
            upDownManager.Instance.updateUpperHoverBar(0, null);
            /*
            if (descObj[0].activeSelf == true)
            {
                descObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                descObj[2].GetComponent<TextMeshPro>().text = "";
                descObj[3].GetComponent<TextMeshPro>().text = "";
                descObj[0].SetActive(false);
            }
            */
        }
    }
    public void hoverOutItem(int idx)
    {
        upDownManager.Instance.updateUpperHoverBar(0, null);
        /*
        descObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
        descObj[2].GetComponent<TextMeshPro>().text = "";
        descObj[3].GetComponent<TextMeshPro>().text = "";
        if (descObj[0].activeSelf == true) descObj[0].SetActive(false);
        */
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
            if (!ItemExistArr[type, i] && i <itemMaxNum) return i;
        }
        return 999; // 공간 내 넣을 자리가 없음.
    }
    public int getItemResult(int type, int idx) //이벤트 결과로 부터 아이템을 받아왔을 경우, 가능한지 여부를 확인해 받아온다.
    {
        if (type == -99999 || idx == -99999 || type>4 || idx >= itemList[type].Count || itemList[type][idx] == null) {
            return 2; //존재 하지 않는 아이템 호출
        }

        int originType = type;
        if (type < 3) type = 0; //임시 테스트를 위해 배치. 만약 원래대로 아이템마다 배치할거면 이거 지우면 됩니다.

        int emptyIdx = findEmptyIdx(type);
        if (emptyIdx == 999) {
            return 1; //빈 공간이 없는 경우.
        }

        
        setItem(type, emptyIdx, originType, idx); //공간이 존재하면 가장 왼쪽에 아이템을 배치한다.
        upDownManager.Instance.activePassiveItem(emptyIdx, -1);
        upDownManager.Instance.clickItemTypeButton(type);
        updateInventory();
        
        return 0;
    }

    public Item getRandomItemByChapter(int exceptItem0, int exceptItem1)
    {
        int curChapter = jsonDataManager.Instance.getCurChapter();
        int randomType = 0;
        if (AdventureManager.Instance.getTutorial() > 0) randomType = 3;
        else// if (curChapter <= 1)
        { // before chapter 2 clear( equip lock )
            randomType = Random.Range(0, 3);
            if (randomType == 2) randomType++;
        }
        /*
        else
        {
            randomType = Random.Range(0, 4);
        }
        */
        int randomItemIdx = Random.Range(1, itemArrLengthByChapter[randomType, curChapter]); //현재 챕터에서 얻을 수 있는, 해당 종류의 모든 아이템 수 
        //escape same item idx
        if (itemListChapter[randomType, curChapter][randomItemIdx].getIdx() == exceptItem0 ||
            itemListChapter[randomType, curChapter][randomItemIdx].getIdx() == exceptItem1) randomItemIdx = (randomItemIdx + 1) % itemArrLengthByChapter[randomType, curChapter];
        if (randomItemIdx == 0) randomItemIdx = 1;

        if (itemListChapter[randomType, curChapter][randomItemIdx].getIdx() == exceptItem0 ||
            itemListChapter[randomType, curChapter][randomItemIdx].getIdx() == exceptItem1) randomItemIdx = (randomItemIdx + 1) % itemArrLengthByChapter[randomType, curChapter];
        if (randomItemIdx == 0) randomItemIdx = 1;

        return itemListChapter[randomType, curChapter][randomItemIdx];
    }
    public Item[] get3RandomItemByChapter()
    {
        Item[] resultArr = new Item[3];
        resultArr[0] = getRandomItemByChapter(-1, -1);
        resultArr[1] = getRandomItemByChapter(resultArr[0].getIdx(), -1);
        resultArr[2] = getRandomItemByChapter(resultArr[0].getIdx(), resultArr[1].getIdx());
        return resultArr;
    }

    public Item getRandomSpecialItemByChapter(int exceptItem0, int exceptItem1)
    {
        int curChapter = jsonDataManager.Instance.getCurChapter();

        int randomItemIdx = Random.Range(0, specialItemList[curChapter].Count); //현재 챕터에서 얻을 수 있는, 해당 종류의 모든 아이템 수 
        //escape same item idx
        if (specialItemList[curChapter][randomItemIdx].getIdx() == exceptItem0 ||
            specialItemList[curChapter][randomItemIdx].getIdx() == exceptItem1) randomItemIdx = (randomItemIdx + 1) % specialItemList[curChapter].Count;

        if (specialItemList[curChapter][randomItemIdx].getIdx() == exceptItem0 ||
            specialItemList[curChapter][randomItemIdx].getIdx() == exceptItem1) randomItemIdx = (randomItemIdx + 1) % specialItemList[curChapter].Count;

        return specialItemList[curChapter][randomItemIdx];
    }

    public Item[] get3RandomSpecialItemByChapter()
    {
        Item[] resultArr = new Item[3];
        resultArr[0] = getRandomSpecialItemByChapter(-1, -1);
        resultArr[1] = getRandomSpecialItemByChapter(resultArr[0].getIdx(), -1);
        resultArr[2] = getRandomSpecialItemByChapter(resultArr[0].getIdx(), resultArr[1].getIdx());
        return resultArr;
    }

    public void turnOffItemCollider_item()
    {
        /*
        for (int i=0;i<11;i++)
        {
            inventoryUIArr[i].GetComponent<BoxCollider2D>().enabled = false;
        }
        */
    }
    public void turnOnItemCollider_item()
    {
        /*
        for (int i = 0; i < 11; i++)
        {
            inventoryUIArr[i].GetComponent<BoxCollider2D>().enabled = true;
        }
        */
    }

    /*
    public void swapItem()
    {
        if(AdventureManager.Instance.curCanvasItemCanvas && !(dragObjEndNum == -1 || dragObjStartNum == -1))
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
    */
    /*
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
    */
    public void swapCharacter(int idx, int idx2)
    {
        int originDiceIdx = AdventureManager.Instance.getSelectDiceCharacterIdx();
        int endDiceIdx = AdventureManager.Instance.getSelectDiceCharacterIdx();
        if (originDiceIdx == idx) { endDiceIdx = idx2; }
        if (originDiceIdx == idx2) { endDiceIdx = idx; }

            Character playerA = CharacterManager.Instance.getCharacter(idx);
            Character playerB = CharacterManager.Instance.getCharacter(idx2);
            CharacterManager.Instance.setCharacter(idx, playerB);
            CharacterManager.Instance.setCharacter(idx2, playerA);
            AdventureManager.Instance.resetDice(endDiceIdx);
            SoundManager_Sfx.Instance.playSound(72);
            AdventureManager.Instance.smokeCharacter(idx);
            AdventureManager.Instance.smokeCharacter(idx2);
            //click_characterInfoType_selectButton(curSelectCharacterInfoType);
            updateCharacterBar();
    }

    public void deleteCharacter()
    {
        bool deleteResult = false;
        if (CharacterManager.Instance.getCharacterState(characterSelectIdx) == 0) {
            deleteResult = CharacterManager.Instance.deleteCharacter(characterSelectIdx);
        }
        if (deleteResult) //캐릭터가 무사히 제거된 경우
        {
            if (AdventureManager.Instance.getSelectDiceCharacterIdx() == characterSelectIdx) {
                AdventureManager.Instance.resetDice();
            }
            else
            {
                AdventureManager.Instance.resetDice(AdventureManager.Instance.getSelectDiceCharacterIdx());
            }
            
            updateCharacterBar();
        }
    }

    public void enterBattlePhase()
    {
        //click_itemType_selectButton(3);
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
        updateInventory();
    }
    public void setCurSelectItemType(int idx)
    {
        curSelectItemType = idx;
    }
    public void setCurSelectItemIndex(int idx)
    {
        curSelectItemIndex = idx;
    }


    public void click_item_trash()
    {
        if(curSelectItemIndex != -1)
        {
            SoundManager_Sfx.Instance.playSound(6);
            //주사위 클릭해서 바뀐후 아이템 삭제 및 선택한거 초기화(일단 item은 안건들이긴합니다. 나중에 빈 아이템 만들어서 배정해야할듯?)
            useItem();
        }
    }
     
    public void click_itemType_selectButton(int idx) // 중단부 아이템 종류 선택 버튼 클릭하는 경우
    {

        if(idx != curSelectItemType) //같으면 이동 안함. 다르니까 이동하는 경우에 대한 처리
        {
            SoundManager_Sfx.Instance.playSound(0);
            curSelectItemIndex = -1; //택한 아이템 초기화
            curSelectItemType = idx; //타입 변경   
            updateInventory();
        }
        //else { SoundManager_Sfx.Instance.playSound(7); }
    }
    
    public int click_Character(int idx)
    {
        characterSelectIdx = idx;
        if (idx == -1) {//character UI delete 
            characterUIEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(-85f, 390f, 0f);
        }
        else if (CharacterManager.Instance.getCharacter(idx) != null && CharacterManager.Instance.getCharacterState(idx) == 0) //캐릭터 전환이 되는 경우(생존해 있는 캐릭터!)
        {
            characterUIEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(-85f, 89f, 0f);
            characterBoard_update();
        }
        else
        {
            characterUIEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(-85f, 390f, 0f);
            characterSelectIdx = -1;
        }
        return characterSelectIdx;
    }
    public int click_Character_battle(int idx)
    {
        characterSelectIdx = idx;
        if (idx == -1)
        {//character UI delete 
            characterUIEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(-85f, 390f, 0f);
        }
        else if (BattleManager.Instance.getCharacter(idx) != null && BattleManager.Instance.getCharacter(idx).getCurState() == 0) //캐릭터 전환이 되는 경우(생존해 있는 캐릭터!)
        {
            characterUIEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(-85f, 89f, 0f);
            characterBoard_update();
        }
        else
        {
            characterUIEntity.GetComponent<RectTransform>().anchoredPosition = new Vector3(-85f, 390f, 0f);
            characterSelectIdx = -1;
        }
        return characterSelectIdx;
    }
    public void characterBoard_update() //board 변경시 업데이트를 하기 위한 함수. character board 변경이나 character idx가 변경될 경우 사용하게 된다.
    {
        Character tempCharacter;
        if (AdventureManager.Instance.getBattleEventChk()) {
            tempCharacter = BattleManager.Instance.getCharacter(characterSelectIdx);
        }
        else
        {
            tempCharacter = CharacterManager.Instance.getCharacter(characterSelectIdx);
        }

        if (tempCharacter == null || tempCharacter.getCurState() != 0) {
            return;
        }

        characterInfoEntity.SetActive(true);
        characterName.text = tempCharacter.getName();
        characterAtk.text = tempCharacter.getPhyAtk().ToString() + "(" + tempCharacter.getPhyAtk(0).ToString() + "/" + tempCharacter.getPhyAtk(1) + ")";
        characterMag.text = tempCharacter.getMagAtk().ToString() + "(" + tempCharacter.getMagAtk(0).ToString() + "/" + tempCharacter.getMagAtk(1) + ")"; ;
        characterSpd.text = tempCharacter.getSpeed().ToString() + "(" + tempCharacter.getSpeed(0).ToString() + "/" + tempCharacter.getSpeed(1) + ")"; ;
        characterArmor.text = tempCharacter.getArmor().ToString();
        characterDescTitle.text = "";
        for (int i = 0; i < 5; i++) characterHp[i].text = tempCharacter.getHp().ToString() + "/" + tempCharacter.getMaxHp().ToString();
        characterOrigin.GetComponent<Animator>().runtimeAnimatorController = tempCharacter.getAnimator(false);
        characterSprite.GetComponent<Image>().sprite = characterOrigin.GetComponent<SpriteRenderer>().sprite;
        //주사위 업데이트
        for (int i = 0; i < 6; i++)
        {
            characterDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + tempCharacter.getDice(i).ToString());
        }
        //스킬, 장비 이미지 업데이트
        for (int i = 0; i < 2; i++)
        {
            characterSkill[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + tempCharacter.getSkillName(i));
        }
        for (int i = 0; i < 2; i++)
        {
            characterEquip[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/equipItemSprite/spr_item_equip_" + tempCharacter.getItem(i).getItemName());
        }
    }

    public void hoverInDice(int i)
    {
        characterDiceOutline[i].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
    }

    public void hoverInSkill(int i)
    {
        characterInfoEntity.SetActive(false);
        Skill temp;
        
        if (AdventureManager.Instance.getBattleEventChk())
        {
            temp = BattleManager.Instance.getCharacter(characterSelectIdx).skillUse(i);
        }
        else
        {
            temp = CharacterManager.Instance.getCharacterSkill(characterSelectIdx, i);
        }

        if (Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + temp.getSkillName()) == null)
        {
            characterDescImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            //skillBoardObj[i, 0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
        }
        else
        {
            characterDescImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + temp.getSkillName());
            //skillBoardObj[i, 0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + temp.getSkillName());
        }
        characterSkillOutline[i].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
        characterDescTitle.text = temp.getSkillName();
        characterDescText.text = temp.getCommand();
        for (int j = 0; j < 4; j++)
        {
            characterDescDice[j].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_" + temp.getNeedDice(j).ToString());
        }
    }
    public void hoverInSwapOrDelete(int idx)
    {
        characterInfoEntity.SetActive(false);
        if (idx == 0) {
            characterDescTitle.text = TalkManager.Instance.getDesc(87);
            characterDescText.text = TalkManager.Instance.getDesc(89);
            characterDescImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_changeInitBtn");
        }
        if (idx == 1)
        {
            characterDescTitle.text = TalkManager.Instance.getDesc(88);
            characterDescText.text = TalkManager.Instance.getDesc(90);
            characterDescImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_getOutButton_active");
        }
        
        for (int j = 0; j < 4; j++)
        {
            characterDescDice[j].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
    }
    public void hoverInEquip(int i)
    {
        characterInfoEntity.SetActive(false);
        Item temp;
        if (AdventureManager.Instance.getBattleEventChk())
        {
            temp = BattleManager.Instance.getCharacter(characterSelectIdx).getItem(i);
        }
        else
        {
            temp = CharacterManager.Instance.getCharacterItem(characterSelectIdx, i);
        }
        

        characterEquipOutline[i].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");

        characterDescImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/equipItemSprite/spr_item_equip_" + temp.getItemName());
        characterDescTitle.text = temp.getItemName();
        characterDescText.text = temp.getContent();
        for (int j = 0; j < 4; j++)
        {
            characterDescDice[j].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_0");
        }

    }
    public void hoverInInfo(int i)
    {
        ToolBarManager.Instance.setToolBarStat(i);
    }
    public void hoverOutDesc()
    {
        characterInfoEntity.SetActive(true);
        for (int i=0;i<6;i++)
        {
            characterDiceOutline[i].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        for(int i = 0; i < 2; i++)
        {
            characterSkillOutline[i].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            characterEquipOutline[i].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }

        characterDescImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        characterDescTitle.text = "";
        characterDescText.text = "";
        for (int j = 0; j < 4; j++)
        {
            characterDescDice[j].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
    }
    public bool getItemUseAble(int characterIdx, int itemType, int itemIdx)
    {
        if(CharacterManager.Instance.getCharacter(characterIdx) == null || CharacterManager.Instance.getCharacterState(characterIdx) != 0 ||
            itemIdx == -1 || ItemArr[itemType, itemIdx] == null || !ItemExistArr[itemType, itemIdx])
        {
            return false;
        }
        return true;
    }
    public void useConsumeItem(int characterIdx, int itemType, int itemIdx)
    {
        if (getItemUseAble(characterIdx, itemType, itemIdx))
        {
            if (AdventureManager.Instance.getTutorial() == 5) AdventureManager.Instance.setTutorial(6);
            useItemToUpgrade(characterIdx, itemIdx);
            SoundManager_Sfx.Instance.playSound(2);
            useItem(itemType, itemIdx);
            //click_characterInfoType_selectButton(0);
        }
        else
        {
            SoundManager_Sfx.Instance.playSound(7);
        }
    }

    public void click_info_useItem()
    {
        if (curSelectItemType == 0 && curSelectItemIndex != -1 && characterSelectIdx >=0 )
        {
            //CharacterManager.Instance.CharacterUpgrade(characterSelectIdx, ItemArr[0, curSelectItemIndex].getVal(0), ItemArr[0, curSelectItemIndex].getVal(1));
            //useItemToUpgrade();
            SoundManager_Sfx.Instance.playSound(2);
            useItem();
        }
        else
        {
            SoundManager_Sfx.Instance.playSound(7);
        }
       
    }

    public void useItemToUpgrade(int characterIdx, int itemIdx)
    {
        Item useItem = ItemArr[0, itemIdx];
        int useItemIdx = useItem.getIdx();
        //단일
        if (useItem.getActiveTiming() == 1){
            CharacterManager.Instance.CharacterUpgrade(characterIdx, ItemArr[0, itemIdx].getVal(0), ItemArr[0, itemIdx].getVal(1));
            upDownManager.Instance.makeItemCharacterEff(characterIdx, ItemArr[0, itemIdx].getVal(0), ItemArr[0, itemIdx].getVal(1), 0);
        }
        if (useItem.getActiveTiming() == 2)
        {// 2개의 stat에 대하여 업그레이드
            CharacterManager.Instance.CharacterUpgrade(characterIdx, ItemArr[0, itemIdx].getVal(0), ItemArr[0, itemIdx].getVal(1));
            CharacterManager.Instance.CharacterUpgrade(characterIdx, ItemArr[0, itemIdx].getVal(2), ItemArr[0, itemIdx].getVal(3));
            upDownManager.Instance.makeItemCharacterEff(characterIdx, ItemArr[0, itemIdx].getVal(0), ItemArr[0, itemIdx].getVal(1), 0);
            upDownManager.Instance.makeItemCharacterEff(characterIdx, -999, ItemArr[0, itemIdx].getVal(3), 1);
        }
        if (useItem.getActiveTiming() == 41)
        { //모든 캐릭터에 대하여 1개 stat 업그레이드
            for (int i = 0; i < 4; i++)
            {
                if (CharacterManager.Instance.getCharacterState(i) == 0)
                {
                    CharacterManager.Instance.CharacterUpgrade(i, ItemArr[0, itemIdx].getVal(0), ItemArr[0, itemIdx].getVal(1));
                    upDownManager.Instance.makeItemCharacterEff(i, ItemArr[0, itemIdx].getVal(0), ItemArr[0, itemIdx].getVal(1), 0);
                }
            }
        }
        if (useItem.getActiveTiming() == 42)
        { //모든 캐릭터에 대하여 2개 stat 업그레이드
            for (int i = 0; i < 4; i++)
            {
                if (CharacterManager.Instance.getCharacterState(i) == 0)
                {
                    CharacterManager.Instance.CharacterUpgrade(i, ItemArr[0, itemIdx].getVal(0), ItemArr[0, itemIdx].getVal(1));
                    CharacterManager.Instance.CharacterUpgrade(i, ItemArr[0, itemIdx].getVal(2), ItemArr[0, itemIdx].getVal(3));
                    upDownManager.Instance.makeItemCharacterEff(i, ItemArr[0, itemIdx].getVal(0), ItemArr[0, itemIdx].getVal(1), 0);
                    upDownManager.Instance.makeItemCharacterEff(i, -999, ItemArr[0, itemIdx].getVal(3), 1);
                }
            }
        }
    }

    private void changeDice(int characterIdx, int idx, int number)
    {
        if (number < 1) number = 1;
        if (number > 6) number = 6;
        SoundManager_Sfx.Instance.playSound(1);
        CharacterManager.Instance.changeDice(characterIdx, idx, number);
        
    }
    /*
    private void changeDice(int characterIdx, int idx, int number)
    {
        if (number < 1) number = 1;
        if (number > 6) number = 6;
        CharacterManager.Instance.changeDice(characterIdx, idx, number);
        if (characterSelectIdx == characterIdx) //같은 경우만 변경
        {
                Instantiate(changeDiceEff, diceBoardObj[idx].transform.position, new Quaternion(0, 0, 0, 0));
                diceBoardObj[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/" + number.ToString());
        }
    }
    */
    public void setTutorialInitDice()
    {
        CharacterManager.Instance.changeDice(2, 0, 1);
        CharacterManager.Instance.changeDice(2, 1, 1);
        CharacterManager.Instance.changeDice(2, 2, 1);
        CharacterManager.Instance.changeDice(2, 3, 1);
        CharacterManager.Instance.changeDice(2, 4, 1);
        CharacterManager.Instance.changeDice(2, 5, 1);
        //for(int i=0;i<6;i++) CharacterManager.Instance.changeDice(0, i, 6);
    }
    public void click_dice_changeNum(int characterIdx, int idx,int itemBagIdx) //
    {   //주사위 변수 값은 val1으로 변경했습니다
        
        if (itemBagIdx != -1 && idx != -1 && ItemArr[curSelectItemType, itemBagIdx] != null && ItemExistArr[curSelectItemType, itemBagIdx]) {
            int itemIdx = ItemArr[curSelectItemType, itemBagIdx].getIdx();
            if (AdventureManager.Instance.getTutorial() == 12) AdventureManager.Instance.setTutorial(13);
            if (itemIdx == 1)
            { //랜덤한 숫자로 변경 
                changeDice(characterIdx, idx, Random.Range(1, 7));
                upDownManager.Instance.makeDiceItemCharacterEff(characterIdx);
                upDownManager.Instance.makeItemCharacterDiceEff(idx);
            }
            else if (itemIdx >= 2 && itemIdx <= 7) //해당 숫자로 변경
            {
                changeDice(characterIdx, idx, ItemArr[curSelectItemType, itemBagIdx].getVal(0));
                upDownManager.Instance.makeDiceItemCharacterEff(characterIdx);
                upDownManager.Instance.makeItemCharacterDiceEff(idx);
            }
            else if (itemIdx == 8)
            { //현재 선택한 캐릭터에 대해 보통 주사위로 변경
                AdventureManager.Instance.setUseFairDice(true);
                upDownManager.Instance.makeDiceItemCharacterEff(characterIdx);
                for (int i = 0; i < 6; i++)
                {
                    upDownManager.Instance.makeItemCharacterDiceEff(i);
                    changeDice(characterIdx, i, i + 1);
                }
            }
            else if (itemIdx == 9)
            {//4명의 아군들에 대해 살아있으면 보통 주사위로 변경
                for (int chIdx = 0; chIdx < 4; chIdx++)
                {
                    upDownManager.Instance.makeDiceItemCharacterEff(chIdx);
                    for (int i = 0; i < 6; i++)
                    {
                        changeDice(chIdx, i, i + 1);
                        upDownManager.Instance.makeItemCharacterDiceEff(i);
                    }
                }
            }
            else if (itemIdx == 10)
            {
                upDownManager.Instance.makeDiceItemCharacterEff(characterIdx);
                for (int i = 0; i < 6; i++)
                {
                    changeDice(characterIdx, i, Random.Range(1, 7));
                    upDownManager.Instance.makeItemCharacterDiceEff(i);
                }
            }
            else if (itemIdx == 11)
            { //4명의 아군들에 대해 살아있으면 다 랜덤한 주사위 값으로 변경
                for (int chIdx = 0; chIdx < 4; chIdx++)
                {
                    upDownManager.Instance.makeDiceItemCharacterEff(chIdx);
                    for (int i = 0; i < 6; i++)
                    {
                        changeDice(chIdx, i, Random.Range(1, 7));
                        upDownManager.Instance.makeItemCharacterDiceEff(i);
                    }
                }
            }
            else if (itemIdx == 12)
            {
                upDownManager.Instance.makeDiceItemCharacterEff(characterIdx);
                int tempRandom = Random.Range(1, 7);
                for (int i = 0; i < 6; i++)
                {
                    changeDice(characterIdx, i, tempRandom);
                    upDownManager.Instance.makeItemCharacterDiceEff(i);
                }
            }
            else if (itemIdx >= 13 && itemIdx <= 18)
            {
                upDownManager.Instance.makeDiceItemCharacterEff(characterIdx);
                for (int i = 0; i < 6; i++)
                {
                    changeDice(characterIdx, i, itemIdx - 12);
                    upDownManager.Instance.makeItemCharacterDiceEff(i);
                }
            }
            else if (itemIdx == 19)
            {
                if (!AdventureManager.Instance.getBattleEventChk())
                {
                    if (CharacterManager.Instance.getCharacter(characterIdx) != null && CharacterManager.Instance.getCharacter(characterIdx).getCurState() == 0)
                    {
                        upDownManager.Instance.makeDiceItemCharacterEff(characterIdx);
                        changeDice(characterIdx, idx, CharacterManager.Instance.getDiceNum(characterIdx, idx) + 1);
                        upDownManager.Instance.makeItemCharacterDiceEff(idx);
                    }
                }
                else
                {
                    if (BattleManager.Instance.getCharacter(characterIdx) != null && BattleManager.Instance.getCharacter(characterIdx).getCurState() == 0)
                    {
                        upDownManager.Instance.makeDiceItemCharacterEff(characterIdx);
                        changeDice(characterIdx, idx, BattleManager.Instance.getCharacter(characterIdx).getDice(idx) + 1);
                        upDownManager.Instance.makeItemCharacterDiceEff(idx);
                    }
                }
            }
            else if (itemIdx == 20)
            {
                if (!AdventureManager.Instance.getBattleEventChk())
                {
                    if (CharacterManager.Instance.getCharacter(characterIdx) != null && CharacterManager.Instance.getCharacter(characterIdx).getCurState() == 0)
                    {
                        upDownManager.Instance.makeDiceItemCharacterEff(characterIdx);
                        changeDice(characterIdx, idx, CharacterManager.Instance.getDiceNum(characterIdx, idx) - 1);
                        upDownManager.Instance.makeItemCharacterDiceEff(idx);
                    }
                }
                else
                {
                    if (BattleManager.Instance.getCharacter(characterIdx) != null && BattleManager.Instance.getCharacter(characterIdx).getCurState() == 0)
                    {
                        upDownManager.Instance.makeDiceItemCharacterEff(characterIdx);
                        changeDice(characterIdx, idx, BattleManager.Instance.getCharacter(characterIdx).getDice(idx) - 1);
                        upDownManager.Instance.makeItemCharacterDiceEff(idx);
                    }
                }
            }
            else if (itemIdx == 21)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (!AdventureManager.Instance.getBattleEventChk())
                    {
                        if (CharacterManager.Instance.getCharacter(characterIdx) != null && CharacterManager.Instance.getCharacter(characterIdx).getCurState() == 0)
                        {
                            upDownManager.Instance.makeDiceItemCharacterEff(characterIdx);
                            changeDice(characterIdx, i, CharacterManager.Instance.getDiceNum(characterIdx, i) + 1);
                            upDownManager.Instance.makeItemCharacterDiceEff(i);
                        }
                    }
                    else
                    {
                        if (BattleManager.Instance.getCharacter(characterIdx) != null && BattleManager.Instance.getCharacter(characterIdx).getCurState() == 0)
                        {
                            upDownManager.Instance.makeDiceItemCharacterEff(characterIdx);
                            changeDice(characterIdx, i, BattleManager.Instance.getCharacter(characterIdx).getDice(i) + 1);
                            upDownManager.Instance.makeItemCharacterDiceEff(i);
                        }
                    }
                }
            }
            else if (itemIdx == 22)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (!AdventureManager.Instance.getBattleEventChk())
                    {
                        if (CharacterManager.Instance.getCharacter(characterIdx) != null && CharacterManager.Instance.getCharacter(characterIdx).getCurState() == 0)
                        {
                            upDownManager.Instance.makeDiceItemCharacterEff(characterIdx);
                            changeDice(characterIdx, i, CharacterManager.Instance.getDiceNum(characterIdx, i) - 1);
                            upDownManager.Instance.makeItemCharacterDiceEff(i);
                        }
                    }
                    else
                    {
                        if (BattleManager.Instance.getCharacter(characterIdx) != null && BattleManager.Instance.getCharacter(characterIdx).getCurState() == 0)
                        {
                            upDownManager.Instance.makeDiceItemCharacterEff(characterIdx);
                            changeDice(characterIdx, i, BattleManager.Instance.getCharacter(characterIdx).getDice(i) - 1);
                            upDownManager.Instance.makeItemCharacterDiceEff(i);
                        }
                    }
                }
            }
            //주사위 클릭해서 바뀐후 아이템 삭제 및 선택한거 초기화(일단 item은 안건들이긴합니다. 나중에 빈 아이템 만들어서 배정해야할듯?)
            useItem(curSelectItemType, itemBagIdx);

        }
    }

    
    public void click_equip_changeNum(int characterIdx, int itemBagIdx, int idx) //
    {   //주사위 변수 값은 val1으로 변경했습니다
        if (itemBagIdx != -1&& idx != -1 && ItemArr[curSelectItemType, itemBagIdx] != null && ItemExistArr[curSelectItemType, itemBagIdx]) 
        {
            CharacterManager.Instance.changeEquip(characterIdx, idx, ItemArr[curSelectItemType, itemBagIdx].getIdx());
            upDownManager.Instance.makeItemCharacterEquipEff(idx);
            //equipBoardObj[idx * 3].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/equipItemSprite/spr_item_equip_" + ItemArr[2, itemBagIdx].getItemName().ToString());
            //equipBoardObj[idx * 3 + 1].GetComponent<TextMeshPro>().text = ItemArr[2, itemBagIdx].getItemName();
            //equipBoardObj[idx * 3 + 2].GetComponent<TextMeshPro>().text = ItemArr[2, itemBagIdx].getContent();

            //주사위 클릭해서 바뀐후 아이템 삭제 및 선택한거 초기화(일단 item은 안건들이긴합니다. 나중에 빈 아이템 만들어서 배정해야할듯?)
            useItem(curSelectItemType, itemBagIdx);
        }
    }
    private void useItem(int itemIdx)
    {
        //changeAlpha(inventoryUIArr[curSelectItemIndex], 0.0f);
        upDownManager.Instance.updateUpperItem(1, itemIdx, -1, "0");
        //inventoryUIArr[curSelectItemIndex].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
        ItemArr[curSelectItemType, itemIdx] = null;
        ItemExistArr[curSelectItemType, itemIdx] = false;

    }
    private void useItem()
    {
        //changeAlpha(inventoryUIArr[curSelectItemIndex], 0.0f);
        upDownManager.Instance.updateUpperItem(1, curSelectItemIndex, -1, "0");
        //inventoryUIArr[curSelectItemIndex].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
        ItemArr[curSelectItemType, curSelectItemIndex] = null;
        ItemExistArr[curSelectItemType, curSelectItemIndex] = false;
        curSelectItemIndex = -1;

    }
    public void useItem(int itemType, int itemIdx)
    {
        upDownManager.Instance.updateUpperItem(1, itemIdx, itemType, "0");
        //inventoryUIArr[curSelectItemIndex].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
        ItemArr[itemType, itemIdx] = null;
        ItemExistArr[itemType, itemIdx] = false;
    }

    public Item getItem(int itemType, int itemIndex)
    {
        return itemList[itemType][itemIndex];
    }
    public Item getItemFromBag(int type, int idx)
    {
        if (ItemExistArr[type, idx] == true)
        {
            return ItemArr[type, idx];
        }
        return null;
    }

    string [] typeArr = { "consume", "dice", "equip", "passive", "destiny"}; //item type string 
    string[] typeArr2 = { "- CONSUME -", "- DICE -", "- EQUIP -", "- PASSIVE -", "- DESTINY -" };
    private void updateInventory() //전체 inventory 업데이트
    {
        for (int i = 0; i < 11; i++)
        {
            if (i >= itemMaxNum)
            {
                upDownManager.Instance.updateUpperItem(2, i, -1, "0");
            }
            else if (ItemExistArr[curSelectItemType, i]) //아이템이 있는 경우 해당 아이템으로 변경
            {
                upDownManager.Instance.updateUpperItem(0, i, curSelectItemType, ItemArr[curSelectItemType, i].getItemName());
                //inventoryUIArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/"+ typeArr[curSelectItemType]+"ItemSprite/spr_item_" + typeArr[curSelectItemType]+
                //    "_" + ItemArr[curSelectItemType, i].getItemName());
            }
            else
            {
                upDownManager.Instance.updateUpperItem(1, i, -1, "0");
                //inventoryUIArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            }
            //inventoryUIArr[i].transform.position = itemBoxInitPoint[i];
        }
    }

    public void updateTypeInventory()
    {
        for (int typeIdx = 0; typeIdx < 4; typeIdx++)
        {
            for (int i = 0; i < 11; i++)
            {
                if (i>=itemMaxNum)
                {
                    upDownManager.Instance.updateUpperTypeItem(2, i, typeIdx, "0");
                }
                else if (ItemExistArr[typeIdx, i]) //아이템이 있는 경우 해당 아이템으로 변경
                {
                    upDownManager.Instance.updateUpperTypeItem(0, i, typeIdx, ItemArr[typeIdx, i].getItemName());
                    //inventoryUIArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/itemSprite/"+ typeArr[curSelectItemType]+"ItemSprite/spr_item_" + typeArr[curSelectItemType]+
                    //    "_" + ItemArr[curSelectItemType, i].getItemName());
                }
                else
                {
                    upDownManager.Instance.updateUpperTypeItem(1, i, typeIdx, "0");
                    //inventoryUIArr[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
                }
                //inventoryUIArr[i].transform.position = itemBoxInitPoint[i];
            }
        }
    }

    void updateCharacterBar()
    {
        for (int i = 0; i < 4; i++)
        {
            if (CharacterManager.Instance.getCharacter(i) != null && CharacterManager.Instance.getCharacter(i).getCurState() == 0)
            {
                characterSelectIdx = i;
                break;
            }
        }
        //characterBoard_update(0);
    }
    public void click_upgradeCanvas_start()
    {
        //초반 캐릭터는 살아있는 친구로 선택하는 코드 나중에 넘어올떄마다 실행시킬수 있도록 코드

        updateCharacterBar();

        //mainCamera.transform.position = new Vector3(-1000f,mainCamera.transform.position.y, mainCamera.transform.position.z);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        curSelectItemType = 0;  // 현재 선택한 아이템 종류 선택
        curSelectItemIndex = -1; // 현재 선택한 아이템의 인덱스
        itemMaxNum = 6;
        
        for (int i = 0; i < 5; i++)
        {
            itemList[i] = new List<Item>();
            for (int j=0;j < 10;j++)
            {
                itemListChapter[i, j] = new List<Item>();
            }
        }
        for (int i=0;i<specialItemList.Length;i++)
        {
            specialItemList[i] = new List<Item>();
        }

        itemReaderList = CSVReader.Read<ItemReader>("Item");    //csv 읽고 해당 Item을 타입에 맞춰 넣는 모습
        for (int i = 0; i < itemReaderList.Count; i++)
        {
            itemList[itemReaderList[i].type].Add(new Item(itemReaderList[i]));
            if (itemReaderList[i].chapterClear < 0)
            {
                for (int j = (itemReaderList[i].chapterClear * -1) - 1; j < 10; j++)
                {
                    specialItemList[j].Add(new Item(itemReaderList[i]));
                }
            }
            else
            {
                for (int j = itemReaderList[i].chapterClear; j < 10; j++)
                {
                    itemListChapter[itemReaderList[i].type, j].Add(new Item(itemReaderList[i]));
                }
            }
        }

        for (int i = 0; i < 11; i++)
        {
            for(int j=0;j<5;j++) ItemExistArr[j, i] = false; //아이템 없다는 것을 초기화를 통해 배정
            itemBoxInitPoint[i] = itemBoxInitPointInit[i].transform.position;
        }

        itemBoxInitPoint[11] = itemBoxInitPointInit[11].transform.position;
        descObj[0].SetActive(false);

        for (int i = 0; i < 5; i++) {
            for (int j = 0; j < 10; j++){
                itemArrLengthByChapter[i, j] = itemListChapter[i, j].Count;
            }
        }
        
        for (int i = 0; i < 6; i++)
        {
            ItemExistArr[0, i] = true;
            ItemArr[0, i] = new Item(itemList[1][Random.Range(1,22)]);
        }
        
        
        ItemExistArr[3, 0] = true;
        ItemArr[3, 0] = new Item(itemList[3][8]);

        ItemExistArr[3, 1] = true;
        ItemArr[3, 1] = new Item(itemList[3][7]);

        ItemExistArr[3, 2] = true;
        ItemArr[3, 2] = new Item(itemList[3][19]);

        ItemExistArr[3, 3] = true;
        ItemArr[3, 3] = new Item(itemList[3][9]);

        ItemExistArr[3, 4] = true;
        ItemArr[3, 4] = new Item(itemList[3][23]);

        ItemExistArr[3, 5] = true;
        ItemArr[3, 5] = new Item(itemList[3][14]);
        
        //test Sample
        /*
        
         ItemExistArr[1, 3] = true;
        ItemArr[1, 3] = new Item(itemList[1][18]);
          
        for (int i = 0; i < 10; i++)
        {
            ItemExistArr[1, i] = true;
            ItemArr[1, i] = new Item(itemList[1][i+13]);
        }
        
        for (int i = 0; i < 10; i++)
        {
            ItemExistArr[3, i] = true;
            ItemArr[3, i] = new Item(itemList[3][i + 24]);
        }
        
        for (int i=0;i<7;i++) {
            ItemExistArr[3, i] = true;
            ItemArr[3, i] = new Item(itemList[3][i+1]);
        }
        ItemArr[3, 0] = new Item(itemList[3][21]);
        ItemArr[3, 1] = new Item(itemList[3][8]);
        ItemArr[3, 2] = new Item(itemList[3][7]);
        ItemArr[3, 3] = new Item(itemList[3][10]);
        ItemArr[3, 4] = new Item(itemList[3][11]);
        ItemArr[3, 5] = new Item(itemList[3][12]);
        ItemArr[3, 6] = new Item(itemList[3][1]);
        ItemExistArr[3, 7] = true;
        ItemArr[3, 7] = new Item(itemList[3][23]);
        ItemExistArr[3, 8] = true;
        ItemArr[3, 8] = new Item(itemList[3][12]);
        ItemExistArr[3, 9] = true;
        ItemArr[3, 9] = new Item(itemList[3][1]);
        ItemExistArr[3, 10] = true;
        ItemArr[3, 10] = new Item(itemList[3][23]);

        for (int i = 0; i < 10; i++)
        {
            ItemExistArr[0, i] = true;
            ItemArr[0, i] = new Item(itemList[0][i + 6]);
        }


        ItemExistArr[2, 0] = true;
        ItemArr[2, 0] = new Item(itemList[2][1]);
        ItemExistArr[2, 1] = true;
        ItemArr[2, 1] = new Item(itemList[2][2]);

        ItemArr[0, 1] = new Item(itemList[1][2]);
        ItemArr[0, 2] = new Item(itemList[2][1]);
        ItemArr[0, 3] = new Item(itemList[2][1]);
        ItemArr[0, 4] = new Item(itemList[2][1]);
       */
        updateInventory();
    }

    private void setItem(int type, int index, int itemType, int ItemIndex) //실제로 아이템을 배치하는 코드
    {
        ItemExistArr[type, index] = true;
        ItemArr[type, index] = new Item(itemList[itemType][ItemIndex]);
    }

    public Vector3 getItemInventoryPosition(int idx)
    {
        return new Vector3(0,0,0);
        //return inventoryUIArr[idx].transform.position;
    }

    //0 : 공격/회복/공격력... 스킬 사용시 능력치 변화.
    //100: 데미지를 입을 시
    public void useEquipItem(int[] heightArr, int timing, int useCharacterIdx, TakeSkillPacket takeSkillPacket)
    {
        for (int i=0;i<2;i++) {
            activeEquipItem(heightArr, timing, useCharacterIdx, i, takeSkillPacket);
        }
    }
    public int activeEquipItem(int[] heightArr, int timing, int useCharacterIdx, int itemNum, TakeSkillPacket takeSkillPacket)
    {
        int returnValue = 0;
        Character useCharacter = BattleManager.Instance.getCharacter(useCharacterIdx);
        if (useCharacter == null || useCharacter.getCurState() != 0) return 0;
        Item item = useCharacter.getItem(itemNum);
        if (item == null || item.getIdx() == 0 || timing != item.getActiveTiming()) return 0;

        bool myTeam = false;
        if (useCharacterIdx < 4) myTeam = true;

        switch (item.getIdx())
        {
            case 1:
                BattleManager.Instance.makeCalculateText(myTeam, 2, useCharacterIdx % 4, 1, heightArr[useCharacterIdx]++);
                useCharacter.getCharacter_battle().upgrade(2,1);
                break;
            case 2:
                BattleManager.Instance.makeCalculateText(myTeam, 2, useCharacterIdx % 4, 1, heightArr[useCharacterIdx]++);
                useCharacter.getCharacter_battle().upgrade(6, 1);
                break;
            case 3:
                BattleManager.Instance.makeCalculateText(myTeam, 2, useCharacterIdx % 4, 1, heightArr[useCharacterIdx]++);
                useCharacter.getCharacter_battle().upgrade(2, 1);
                break;
            case 4:
                BattleManager.Instance.makeCalculateText(myTeam, 6, useCharacterIdx % 4, 1, heightArr[useCharacterIdx]++);
                useCharacter.getCharacter_battle().upgrade(6, 1);
                break;
        }

        item.useDurability();
        if (item.getDurability() == 0) {
            useCharacter.changeEquip(itemNum, 0);
            BattleManager.Instance.updateEquipUI(useCharacterIdx, itemNum);
        }
        return returnValue;
    }
    //passive Item use function start
    public int[] passiveItemActiveByHover(int characterIdx, int skillIdx, int diceStartIdx, int activeTime, int targetCharacter)
    {
        int[] resultArr = { 0,0,0,0,0,0,
                            0,0,0,0,0};
        if(activeTime == 0 && 0 == BattleManager.Instance.MakeMyAttackSet(true, characterIdx, skillIdx, diceStartIdx)){
            return resultArr;
        }

        int[] clickCharacter = { -999, -999, -999, -999 };
        if (targetCharacter >= 0) clickCharacter[0] = targetCharacter;

        Character characterTemp = CharacterManager.Instance.getCharacter(characterIdx);
        int[] diceArr = BattleManager.Instance.getMakeDiceArrToMakePacket(diceStartIdx, characterTemp.skillUse(skillIdx).getNeedDiceNum());
        SendSkillPacket sendSkillPacketTemp = new SendSkillPacket(characterIdx, characterTemp.getSkillIdx(skillIdx), clickCharacter , diceArr);
        List<TakeSkillPacket> takeSkillPacketList = characterTemp.doSkill(sendSkillPacketTemp);
        BattleManager.Instance.takeSkillPacketLastFix(characterIdx, takeSkillPacketList);

        for (int idx = 0; idx < 11; idx++)
        {
            if (!ItemExistArr[3, idx]) { continue; } // 아이템이 없으면 그냥 스킵
            Item item = ItemArr[3, idx];
            int activeTiming = item.getActiveTiming();
            if (activeTiming != activeTime) { continue; } //원하는 타이밍이 아니면 생략

            for (int i=0;i<takeSkillPacketList.Count;i++) {
                if (item.getVal(0) == takeSkillPacketList[i].getSkillType()) {  //스킬이 아이템 타입하고 안맞으면 종료 
                                                                         //클릭 이전 타이밍 이면서 주사위 조건이 안맞으면 return
                    if (activeTiming == 0 && conditionCheck_dice(diceArr, item.getVal(1), item.getVal(2), item.getVal(3), item.getVal(4), item.getVal(5))) {
                        resultArr[idx] = 1;
                        break;
                    }
                    //클릭 이후 대상. 이건 몬스터 정보 같은거도 받아야해서 조건 추가될 예정
                    if (activeTiming == 1 && conditionCheck_target(takeSkillPacketList[i], item)) {
                        resultArr[idx] = 1;
                        break;
                    }
                }
            }
        }
        return resultArr;
    }
    public passiveReturn usePassiveItem(List<TakeSkillPacket> takeSkillPacketList, TakeSkillPacket takeSkillPacket, int idx, int[] diceArr, int activeTime)
    {
        if (!ItemExistArr[3, idx]) {return new passiveReturn(false, "", 0);} // 아이템이 없으면 그냥 스킵
        Item item = ItemArr[3, idx];
        int activeTiming = item.getActiveTiming();
        if (activeTiming != activeTime) { return new passiveReturn(false, "", 0); } //원하는 타이밍이 아니면 생략

        if (item.getVal(0) != takeSkillPacket.getSkillType()) { return new passiveReturn(false, "", 0); };  //스킬이 아이템 타입하고 안맞으면 종료 
        
        //클릭 이전 타이밍 이면서 주사위 조건이 안맞으면 return
        if (activeTiming == 0 && !conditionCheck_dice(diceArr, item.getVal(1), item.getVal(2), item.getVal(3), item.getVal(4), item.getVal(5))) { return new passiveReturn(false, "", 0); }
        //클릭 이후 대상. 이건 몬스터 정보 같은거도 받아야해서 조건 추가될 예정
        if (activeTiming == 1 && !conditionCheck_target(takeSkillPacket, item)) { return new passiveReturn(false, "", 0); }

        passiveReturn returnVal = new passiveReturn(true, "+", item.getVal(3));
        int sumDiceVal = 0;
        switch (item.getIdx())
        {
            case 1:
                takeSkillPacket.addVal(item.getVal(3)); returnVal = new passiveReturn(true, "+", item.getVal(3)); break;
            case 2:
                takeSkillPacket.addVal(item.getVal(3)); returnVal = new passiveReturn(true, "+", item.getVal(3)); break;
            case 3:
                takeSkillPacket.mulVal(item.getVal(3)); returnVal = new passiveReturn(true, "X", item.getVal(3)); break;
            case 4:
                takeSkillPacket.mulVal(item.getVal(3)); returnVal = new passiveReturn(true, "X", item.getVal(3)); break;
            case 5:
                takeSkillPacket.addVal(item.getVal(3)); returnVal = new passiveReturn(true, "+", item.getVal(3)); break;
            case 6:
                takeSkillPacket.mulVal(item.getVal(3)); returnVal = new passiveReturn(true, "X", item.getVal(3)); break;
            case 7:
                takeSkillPacket.addVal(item.getVal(3)); returnVal = new passiveReturn(true, "+", item.getVal(3)); break;
            case 8:
                takeSkillPacket.addVal(item.getVal(3)); returnVal = new passiveReturn(true, "+", item.getVal(3)); break;
            case 9:
                takeSkillPacket.mulVal(item.getVal(3)); returnVal = new passiveReturn(true, "X", item.getVal(3)); break;
            case 10:
                takeSkillPacket.addVal(item.getVal(3)); returnVal = new passiveReturn(true, "+", item.getVal(3)); break;
            case 11:
                takeSkillPacket.addVal(item.getVal(3)); returnVal = new passiveReturn(true, "+", item.getVal(3)); break;
            case 12:
                takeSkillPacket.addVal(item.getVal(3)); returnVal = new passiveReturn(true, "+", item.getVal(3)); break;
            case 13:
                takeSkillPacket.mulVal(item.getVal(3)); returnVal = new passiveReturn(true, "X", item.getVal(3)); break;
            case 14:
                takeSkillPacket.mulVal(item.getVal(3)); returnVal = new passiveReturn(true, "X", item.getVal(3)); break;
            case 15:
                takeSkillPacket.setStateChange(Random.Range(1, 3) * 2); returnVal = new passiveReturn(true, "none", item.getVal(3)); break;
            case 16:
                takeSkillPacket.setStateChange(Random.Range(0, 2) * 2 + 1); returnVal = new passiveReturn(true, "none", item.getVal(3)); break;
            case 17:
                takeSkillPacketList.Add(new TakeSkillPacket(BattleManager.Instance.getCurSkillInfo().useCharacterIdx, 0, Random.Range(0, 2) * 2 + 1, -999)); returnVal = new passiveReturn(true, "none", item.getVal(3)); break;
            case 18:
                takeSkillPacketList.Add(new TakeSkillPacket(BattleManager.Instance.getCurSkillInfo().useCharacterIdx, 0, Random.Range(1, 3) * 2, -999)); returnVal = new passiveReturn(true, "none", item.getVal(3)); break;
            case 19:
                for (int i = 0; i < 4; i++) if (diceArr[i] >= 1 && diceArr[i] < 7) sumDiceVal += diceArr[i];
                takeSkillPacket.addVal(sumDiceVal); returnVal = new passiveReturn(true, "+", sumDiceVal); break;
            case 20:
                for (int i = 0; i < 4; i++) if (diceArr[i] >= 1 && diceArr[i] < 7) sumDiceVal += diceArr[i];
                takeSkillPacket.addVal(sumDiceVal); returnVal = new passiveReturn(true, "+", sumDiceVal); break;
            case 21:
                takeSkillPacketList.Add(new TakeSkillPacket(BattleManager.Instance.getCurSkillInfo().useCharacterIdx, item.getVal(3), 0, 2));
                returnVal = new passiveReturn(true, "none", item.getVal(3)); break;//자신에게 공격력 추가
            case 22:
                takeSkillPacket.mulVal(item.getVal(3));
                takeSkillPacketList.Add(new TakeSkillPacket(BattleManager.Instance.getCurSkillInfo().useCharacterIdx, 0, item.getVal(4), -999));
                returnVal = new passiveReturn(true, "X", item.getVal(3)); break;
            case 23:
                takeSkillPacket.mulVal(item.getVal(3)); returnVal = new passiveReturn(true, "X", item.getVal(3)); break;
            case 24:
                takeSkillPacketList.Add(new TakeSkillPacket(BattleManager.Instance.getCurSkillInfo().useCharacterIdx, 0, item.getVal(3), -999)); returnVal = new passiveReturn(true, "none", item.getVal(3)); break;
            case 25:
                takeSkillPacketList.Add(new TakeSkillPacket(BattleManager.Instance.getCurSkillInfo().useCharacterIdx, 0, item.getVal(3), -999)); returnVal = new passiveReturn(true, "none", item.getVal(3)); break;
            case 26:
                takeSkillPacketList.Add(new TakeSkillPacket(BattleManager.Instance.getCurSkillInfo().useCharacterIdx, 0, item.getVal(3), -999)); returnVal = new passiveReturn(true, "none", item.getVal(3)); break;
            case 27:
                sumDiceVal = 1;
                for (int i = 0; i < 4; i++)
                {
                    if (diceArr[i] == 4) { 
                        sumDiceVal *= 2;
                        takeSkillPacket.mulVal(sumDiceVal);
                        break;
                    }
                }
                returnVal = new passiveReturn(true, "X", sumDiceVal); break;
            case 28:
                takeSkillPacket.addVal(item.getVal(3)); returnVal = new passiveReturn(true, "+", item.getVal(3)); break;
            case 29:
                takeSkillPacket.addVal(item.getVal(3)); returnVal = new passiveReturn(true, "+", item.getVal(3));
                takeSkillPacketList.Add(new TakeSkillPacket(takeSkillPacket.getTargetIdx(), 0, item.getVal(4), -999));
                break;
            case 30:
                takeSkillPacket.addVal(item.getVal(3)); returnVal = new passiveReturn(true, "+", item.getVal(3)); break;
            case 31:
                takeSkillPacket.mulVal(item.getVal(3));
                takeSkillPacketList.Add(new TakeSkillPacket(BattleManager.Instance.getCurSkillInfo().useCharacterIdx, 0, item.getVal(4), -999));
                returnVal = new passiveReturn(true, "X", item.getVal(3)); break;
            case 32:
                takeSkillPacketList.Add(new TakeSkillPacket(BattleManager.Instance.getCurSkillInfo().useCharacterIdx, item.getVal(4), 0, 1)); //아군 한명에게 10만큼 회복
                for (int i = 0; i < 8; i++) {
                    int temp = BattleManager.Instance.getCurSkillInfo().getClickCharacter(i);
                    if (temp == -999) break;
                    takeSkillPacketList.Add(new TakeSkillPacket(temp, 0, item.getVal(3), -999));
                } 
                returnVal = new passiveReturn(true, "none", item.getVal(4)); break;
            case 33:
                Debug.Log("Item Test :");
                Debug.Log(BattleManager.Instance.getCurSkillInfo().useCharacterIdx);
                takeSkillPacketList.Add(new TakeSkillPacket(BattleManager.Instance.getCurSkillInfo().useCharacterIdx, item.getVal(4), 0, 1)); //아군 한명에게 10만큼 회복
                takeSkillPacketList.Add(new TakeSkillPacket(BattleManager.Instance.getCurSkillInfo().useCharacterIdx, 0, item.getVal(3), -999));
                returnVal = new passiveReturn(true, "none", item.getVal(4)); break;
        }

        return returnVal;
    }

    private bool conditionCheck_target(TakeSkillPacket takeSkillPacket, Item item)
    {
        if (takeSkillPacket.getSkillType() < 1000)
        {
            int characterIdx = takeSkillPacket.getTargetIdx();

            return conditionCheck_target_detail(takeSkillPacket, item, characterIdx);
        }
        else {
            bool chkTrue = false;
            for (int i = 0; i < 8; i++){
                int characterIdx = BattleManager.Instance.getCurSkillInfo().getClickCharacter(i);
                if (characterIdx == -999) continue;
                if (conditionCheck_target_detail(takeSkillPacket, item, characterIdx)) {chkTrue = true; break; }
            }
            return chkTrue;
        }
    }

    private bool conditionCheck_target_detail(TakeSkillPacket takeSkillPacket, Item item, int characterIdx) {
        if (!(characterIdx >= 0 && characterIdx < 8)) return false; //캐릭터가 타겟팅 되지 않았거나
        if (BattleManager.Instance.getCharacter(characterIdx) == null || BattleManager.Instance.getCharacter(characterIdx).getCurState() != 0) return false; //캐릭터가 존재하지 않으면 false 

        if (item.getVal(1) == 1)
        {//단순 주사위 확인
            int diceNum = BattleManager.Instance.getDiceNum(characterIdx);
            if (item.getVal(2) == 0)
            { //어떤 주사위든 상관없이
                return true;
            }
            if (item.getVal(2) >= 1 && item.getVal(2) <= 6 && diceNum == item.getVal(2))
            {//캐릭터와 주사위 일치
                return true;
            }
            if ((item.getVal(2) == 7 && diceNum % 2 == 1) || (item.getVal(2) == 8 && diceNum % 2 == 0)) //짝수 홀수 체크
            {//캐릭터와 주사위 일치
                return true;
            }
        }
        return false;
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


        if (condition0 == 0) return true;
        //존재 개수를 기반하는 조건들의 계산
        if(condition0 >=1 && condition0 <= 4){ return condition0 <= sumOfNumber(arr, condition1); } //한개일때 숫자
        if ((condition0 >= 11 && condition0 <= 13) || condition0 == 22) return (condition0 / 10 <= sumOfNumber(arr, condition1)) && (condition0 % 10 <= sumOfNumber(arr, condition2));
        if (condition0 >= 111 && condition0 <= 112) return (condition0 / 100 <= sumOfNumber(arr, condition1)) && ((condition0 % 100) / 10 <= sumOfNumber(arr, condition2)) && (condition0 % 10 <= sumOfNumber(arr, condition3));
        if (condition0 == 1111) return (condition0 / 1000 <= sumOfNumber(arr, condition1)) && ((condition0 % 1000) / 100 <= sumOfNumber(arr, condition2)) && ((condition0 % 100) / 10 <= sumOfNumber(arr, condition3)) && (condition0 % 10 <= sumOfNumber(arr, condition4)) ;
        if (condition0 == 5) return condition1 == sumOfNumber(arr, 0); //사용된 주사위 수와 일치, 보다 적게, 보다 크게 사용된 경우
        if (condition0 == 6) return condition1 <= sumOfNumber(arr, 0);
        if (condition0 == 7) return condition1 >= sumOfNumber(arr, 0);
        return false;
    }
    private int sumOfNumber(int[] arr, int opt) //각 option에 해당하는 주사위의 수 확인
    {
        int result = 0;
        if (opt == 0) return arr[1] + arr[3] + arr[5] + arr[2] + arr[4] + arr[6];
        if (opt > 0 && opt <= 6) return arr[opt];
        if(opt == 7) return arr[1] + arr[3] + arr[5]; //홀수
        if (opt == 8) return arr[2] + arr[4] + arr[6]; //짝수
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

}
