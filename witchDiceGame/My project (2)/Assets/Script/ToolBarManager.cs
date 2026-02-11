using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToolBarManager : MonoBehaviour
{

    private static ToolBarManager instance = null;
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
    public static ToolBarManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }

    [SerializeField]
    public GameObject toolBarObj;
    public GameObject toolBarCharacterInfo;
    public GameObject toolBarImage;
    public TextMeshProUGUI toolBarTitle;
    public TextMeshProUGUI toolBarDesc;
    public GameObject[] toolBarDice = new GameObject[6];
    public TextMeshProUGUI[] toolBarCharacterInfoText = new TextMeshProUGUI[5];
    [SerializeField]
    public Sprite[] backgroundSprite = new Sprite[6];

    [SerializeField]
    public GameObject toolBarDiceInfo;
    public TextMeshProUGUI[] toolBarDiceText = new TextMeshProUGUI[6];

    public int toolBarState = 0;
    // Start is called before the first frame update
    void Start()
    {
        toolBarState = 0;
        toolBarOnOff(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        if(toolBarState != 0)
        {
            toolBarObj.GetComponent<RectTransform>().position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        }
    }

    public void toolBarOnOff(int idx)
    {
        toolBarState = idx;
        if (idx == 0){
            toolBarObj.SetActive(false);
            return;
        }
        toolBarObj.SetActive(true);
        toolBarObj.GetComponent<RectTransform>().position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        if (Input.mousePosition.y < Screen.height / 2)
        {
            if (Input.mousePosition.x < Screen.width / 3)
            {
                toolBarObj.GetComponent<RectTransform>().pivot = new Vector2(0, 0);
                toolBarObj.GetComponent<Image>().sprite = backgroundSprite[0];
            }
            else if (Input.mousePosition.x > Screen.width * 2 / 3)
            {
                toolBarObj.GetComponent<RectTransform>().pivot = new Vector2(1, 0);
                toolBarObj.GetComponent<Image>().sprite = backgroundSprite[2];
            }
            else
            {
                toolBarObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
                toolBarObj.GetComponent<Image>().sprite = backgroundSprite[1];
            }
        }
        else
        {
            if (Input.mousePosition.x < Screen.width / 3)
            {
                toolBarObj.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
                toolBarObj.GetComponent<Image>().sprite = backgroundSprite[3];
            }
            else if (Input.mousePosition.x > Screen.width * 2 / 3)
            {
                toolBarObj.GetComponent<RectTransform>().pivot = new Vector2(1, 1);
                toolBarObj.GetComponent<Image>().sprite = backgroundSprite[5];
            }
            else
            {
                toolBarObj.GetComponent<RectTransform>().pivot = new Vector2(0.5f,1);
                toolBarObj.GetComponent<Image>().sprite = backgroundSprite[4];
            }
        }
    }
    public void setToolBar(Character character)
    {
        toolBarOnOff(1);
        toolBarDiceInfo.SetActive(false);
        toolBarCharacterInfo.SetActive(true);

        if (Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + character.getName() + "_face") == null)
        {
            toolBarImage.GetComponent<Image>().sprite =
                Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_enemy_face");
        }
        else
        {
            toolBarImage.GetComponent<Image>().sprite =
            Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + character.getName() + "_face");
        }
        toolBarCharacterInfoText[0].text = character.getHp().ToString() + "/" + character.getMaxHp().ToString();
        toolBarCharacterInfoText[1].text = character.getArmor().ToString();
        toolBarCharacterInfoText[2].text = character.getPhyAtk().ToString();
        toolBarCharacterInfoText[3].text = character.getMagAtk().ToString();
        toolBarCharacterInfoText[4].text = character.getSpeed().ToString();
        toolBarTitle.text = character.getName();
        for (int i=0;i<6;i++)
        {
            toolBarDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_" + character.getDice(i).ToString());
        }
        toolBarDesc.text = "";
    }
    public void setToolBar(Skill skill) {
        toolBarOnOff(1);
        toolBarDiceInfo.SetActive(false);
        toolBarCharacterInfo.SetActive(false);

        toolBarImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + skill.getSkillName());
        toolBarTitle.text = skill.getSkillName();
        for (int i = 0; i < 4; i++)
        {
            toolBarDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_" + skill.getNeedDice(i).ToString());
        }
        toolBarDice[4].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        toolBarDice[5].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        toolBarDesc.text = skill.getCommand();
    }
    public void setToolBar(Item item) {
        toolBarOnOff(1);
        toolBarDiceInfo.SetActive(false);
        toolBarCharacterInfo.SetActive(false);

        if (item.getType() == 0) toolBarImage.GetComponent<Image>().sprite
                 = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_itemType_consume");
        if (item.getType() == 1) toolBarImage.GetComponent<Image>().sprite
                 = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_itemType_dice");
        if (item.getType() == 2) toolBarImage.GetComponent<Image>().sprite
                 = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_itemType_equip");
        if (item.getType() == 3) toolBarImage.GetComponent<Image>().sprite
                 = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_itemType_passive");



        toolBarTitle.text = item.getItemName();
        for (int i = 0; i <6; i++)
        {
            if (item.getRare() >= i) toolBarDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_star1111");
            else toolBarDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
        toolBarDesc.text = item.getContent();
    }
    //공격력, 마법감응력, 스피드, 방어도, HP    
    //전투, 보물, 불운, 행운, 랜덤, 상점, 보스, ??? 이벤트
    private int[] toolBarTitleIdx = { 72,73,74, 83, 85, 100,102,104,106,108,110,112, 114};
    private int[] toolBarContentIdx = { 75,76,77, 84, 86 ,101,103,105,107,109,111,113, 115};
    public void setToolBar(int idx)
    {
        toolBarOnOff(1);
        toolBarDiceInfo.SetActive(false);
        toolBarCharacterInfo.SetActive(false);

        for (int i = 0; i < 6; i++)
        {
            toolBarDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
        toolBarImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_deleteInitBtn");
        toolBarTitle.text = TalkManager.Instance.getDesc(toolBarTitleIdx[idx]);
        toolBarDesc.text = TalkManager.Instance.getDesc(toolBarContentIdx[idx]);
    }
    public void setToolBar(string title, string content, Sprite spriteImage)
    {
        toolBarOnOff(1);
        toolBarDiceInfo.SetActive(false);
        toolBarCharacterInfo.SetActive(false);




        for (int i = 0; i < 6; i++)
        {
            toolBarDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
        toolBarImage.GetComponent<Image>().sprite = spriteImage;
        toolBarTitle.text = title;
        toolBarDesc.text = content;
    }
    public void setToolBarDiceInfo()
    {
        if(!AdventureManager.Instance.getBattleEventChk()){
            return;
        }
        toolBarOnOff(1);
        toolBarDiceInfo.SetActive(true);
        toolBarCharacterInfo.SetActive(false);

        for (int i = 0; i < 6; i++)
        {
            toolBarDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
        toolBarImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_deleteInitBtn");

        toolBarTitle.text = TalkManager.Instance.getDesc(116);
        toolBarDesc.text = "";

        toolBarDiceText[0].text = TalkManager.Instance.getDesc(29);
        toolBarDiceText[1].text = TalkManager.Instance.getDesc(27);
        toolBarDiceText[2].text = TalkManager.Instance.getDesc(28);
        toolBarDiceText[3].text = "3 " + TalkManager.Instance.getDesc(99);
        toolBarDiceText[4].text = "3 " + TalkManager.Instance.getDesc(26);
        toolBarDiceText[5].text = "3 " + TalkManager.Instance.getDesc(25);
    }
}
