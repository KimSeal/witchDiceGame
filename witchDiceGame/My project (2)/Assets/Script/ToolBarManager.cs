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
        toolBarCharacterInfoText[2].text = character.getPhyAtk().ToString() + "(" + character.getPhyAtk(0) + "/" + character.getPhyAtk(1) + ")";
        toolBarCharacterInfoText[3].text = character.getMagAtk().ToString() + "(" + character.getMagAtk(0) + "/" + character.getMagAtk(1) + ")"; ;
        toolBarCharacterInfoText[4].text = character.getSpeed().ToString() + "(" + character.getSpeed(0) + "/" + character.getSpeed(1) + ")"; ;
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


        if(item == null)
        {
            toolBarImage.GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            toolBarTitle.text = "Empty";
            toolBarDesc.text = " ";
            for (int i = 0; i < 6; i++)
            {
                toolBarDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            }
            return;
        }

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
    //공격력, 마법감응력, 스피드, 방어도, HP, 최대 HP
    //전투, 보물, 불운, 행운, 랜덤, 상점, 보스, ???, 미확인 이벤트, 우연, 성장,
    //(11, 12, 13 - 21)
    //아카이브수직이동, 아카이브수직이동, 아카이브종료, 지도화살표, 모험선택지, 가방버튼, 전투시작버튼, 대화 이전버튼, 대화 스킵버튼, 대화 폰트변경버튼, 전투 보상 리롤 버튼
    //(22, 23, 24)
    //주사위 배치 가능여부 설명
    private int[] toolBarStatTitleIdx = { 72, 73, 74, 83, 85, 124, 155 };
    private int[] toolBarStatContentIdx = { 75, 76, 77, 84, 86, 125, 154 };

    private int[] toolBarTitleIdx = {   100, 102, 104, 106, 108, 110, 112, 114, 117, 119, 121,
                                     156,156,159, 161,163,165,167, 170,172,174, 178,
                                     183, 183, 183};
    private int[] toolBarContentIdx = {101, 103, 105, 107, 109, 111, 113, 115 ,118, 120, 122,
                                     157,158,160, 162,164,166,168, 171,173,175, 179,
                                    180,181,182};

    private int[] toolBarItemTypeTitleIdx = { 78,79,80,81};
    private int[] toolBarItemTypeContentIdx = { 134,135,136, 137};
    public void setToolBarRandom(int idx)
    {
        toolBarOnOff(1);
        toolBarDiceInfo.SetActive(false);
        toolBarCharacterInfo.SetActive(false);

        for (int i = 0; i < 6; i++)
        {
            toolBarDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
        toolBarImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_knowButton");

        toolBarTitle.text = TalkManager.Instance.getDesc(128 + idx);
        toolBarDesc.text = TalkManager.Instance.getDesc(130 + idx);
    }

    public void setToolBar(int idx)
    {
        toolBarOnOff(1);
        toolBarDiceInfo.SetActive(false);
        toolBarCharacterInfo.SetActive(false);

        for (int i = 0; i < 6; i++)
        {
            toolBarDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
        toolBarImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_knowButton");
        toolBarTitle.text = TalkManager.Instance.getDesc(toolBarTitleIdx[idx]);
        toolBarDesc.text = TalkManager.Instance.getDesc(toolBarContentIdx[idx]);
        if (idx == 21) toolBarDesc.text += ("\n( $" + BattleManager.Instance.getRerollNeedGold().ToString() + ")");
    }
    public void setToolBarStat(int idx)
    {
        toolBarOnOff(1);
        toolBarDiceInfo.SetActive(false);
        toolBarCharacterInfo.SetActive(false);

        for (int i = 0; i < 6; i++)
        {
            toolBarDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
        toolBarImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_knowButton");
        toolBarTitle.text = TalkManager.Instance.getDesc(toolBarStatTitleIdx[idx]);
       
        toolBarDesc.text = TalkManager.Instance.getDesc(toolBarStatContentIdx[idx]);
        if (idx == 0) toolBarDesc.text += AdventureManager.Instance.getAtkMaxVal() + " )";
        if (idx == 1) toolBarDesc.text += AdventureManager.Instance.getMagMaxVal() + " )";
        if (idx == 2) toolBarDesc.text += AdventureManager.Instance.getSpdMaxVal() + " )";
        if (idx == 3) toolBarDesc.text += AdventureManager.Instance.getArmorMaxVal() + " )";
    }
    public void setToolBarItemType(int idx)
    {
        if (idx == 0) toolBarImage.GetComponent<Image>().sprite
                 = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_itemType_consume");
        if (idx == 1) toolBarImage.GetComponent<Image>().sprite
                 = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_itemType_dice");
        if (idx == 2) toolBarImage.GetComponent<Image>().sprite
                 = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_itemType_equip");
        if (idx == 3) toolBarImage.GetComponent<Image>().sprite
                 = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_itemType_passive");
        toolBarOnOff(1);
        toolBarDiceInfo.SetActive(false);
        toolBarCharacterInfo.SetActive(false);

        for (int i = 0; i < 6; i++)
        {
            toolBarDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        }
        toolBarTitle.text = TalkManager.Instance.getDesc(toolBarItemTypeTitleIdx[idx]);
        toolBarDesc.text = TalkManager.Instance.getDesc(toolBarItemTypeContentIdx[idx]);
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
    public void setToolBarJewelImage(int num)
    {
        for (int i=0;i<num;i++)
        {
            toolBarDice[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_jewel_11_11");
        }
        
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
        toolBarImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/extraUIButton/spr_knowButton");

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
