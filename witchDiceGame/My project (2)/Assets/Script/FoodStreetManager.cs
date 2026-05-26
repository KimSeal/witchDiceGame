using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FoodStreetManager : MonoBehaviour
{
    private static FoodStreetManager instance = null;


    [SerializeField] public GameObject[] foodTower = new GameObject[4];
    [SerializeField] public GameObject[] foodTowerBowl = new GameObject[4]; 
    [SerializeField] public GameObject[] statText = new GameObject[4];

    [SerializeField] public GameObject foodInHand;
    [SerializeField] public GameObject womanSayRemainFood;

    [SerializeField] public Sprite[] foodInHandSpriteArr = new Sprite[8];

    [SerializeField] public GameObject[] upgradeAnim = new GameObject[4];

    [SerializeField] public GameObject ailBodyObj;
    [SerializeField] public GameObject ailFaceObj;
    [SerializeField] public GameObject grinFaceObj;
    [SerializeField] public GameObject womanFaceObj;

    [SerializeField] public GameObject[] skillObj = new GameObject[2];
    [SerializeField] public GameObject[] skillOutline = new GameObject[2];

    private float FoodTowerInitY = 0;
    public int[] statArr = { 0, 0, 0, 0 };
    public int[] preArr = { 0,0,0,0};
    public int maxRemainFood = 0;
    public int curRemainFood = 0;

    public Character ailCharacter;

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

    public static FoodStreetManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        FoodTowerInitY = foodTower[0].transform.position.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        

    }

    public void makePreArr()
    {
        for (int i = 0; i < preArr.Length; i++) preArr[i] = statArr[i];
    }
    public void clickFoodTower(int idx)
    {
        makePreArr();

        if (curRemainFood == 0) {
            curRemainFood = statArr[idx];
            statArr[idx] = 0;
        }
        else{
            curRemainFood -= 1;
            statArr[idx] += 1;
        }

        jsonDataManager.Instance.setFoodStreetStat(idx, statArr[idx]);
        animAboutWoman();
        updateFoodTowerChange();
    }
    public void hoverInFoodTower(int idx)
    {
        foodTower[idx].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
        foodTowerBowl[idx].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
        if (idx == 0) itemManager.Instance.hoverInInfo(4);
        else itemManager.Instance.hoverInInfo(idx - 1);
    }
    public void hoverOutFoodTower(int idx)
    {
        foodTower[idx].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
        foodTowerBowl[idx].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
        ToolBarManager.Instance.toolBarOnOff(0);
    }
    public void clickWoman()
    {
        makePreArr();
        for (int i = 0; i < 4; i++) {
            statArr[i] = 0; jsonDataManager.Instance.setFoodStreetStat(i, 0);
            curRemainFood = maxRemainFood;
            jsonDataManager.Instance.setFoodStreetStat(i, 0);
        }
        animAboutWoman();
        updateFoodTowerChange();
    }
    public void hoverInWoman()
    {
        foodInHand.GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
        ToolBarManager.Instance.setToolBarStat(6);

    }
    public void hoverOutWoman()
    {
        foodInHand.GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
        ToolBarManager.Instance.toolBarOnOff(0);
    }

    public void hoverInAil()
    {
        ailBodyObj.GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
        ToolBarManager.Instance.setToolBar(ailCharacter);
    }
    public void hoverOutAil()
    {
        ailBodyObj.GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
        ToolBarManager.Instance.toolBarOnOff(0);
    }
    public void clickAil()
    {

    }

    public void hoverInSkill(int skillIdx)
    {
        Skill thisSkill = null;
        thisSkill = ailCharacter.skillUse(skillIdx);
        ToolBarManager.Instance.setToolBar(thisSkill);
        //skillObj[skillIdx].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
        skillOutline[skillIdx].SetActive(true);
    }
    public void hoverOutSkill()
    {
        ToolBarManager.Instance.toolBarOnOff(0);
        skillOutline[0].SetActive(false);
        skillOutline[1].SetActive(false);
        //skillObj[0].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
        //skillObj[1].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
    }
    public void enterFoodStreet()
    {
        
        
        for (int i = 5; i >= 0; i--)
        {
            if (jsonDataManager.Instance.getChapterRead(i, 2) == 2) { maxRemainFood = i + 2; break; }
        }
        curRemainFood = maxRemainFood;
        for (int i = 0; i < statArr.Length; i++)
        {
            statArr[i] = jsonDataManager.Instance.getFoodStreetStat(i);
            curRemainFood -= statArr[i]; 
        }
        for (int i = 0; i < skillOutline.Length; i++) skillOutline[i].SetActive(false);
        updateFoodTowerChange();
    }
    public void exitFoodStreet()
    {
        for (int i = 0; i < statArr.Length; i++)
        {
            statArr[i] = jsonDataManager.Instance.getFoodStreetStat(i);
        }
    }

    public void upgradeInitStat(ref Character character)
    {
        character.upGrade(1, statArr[0] * 5);
        character.upGrade(5, statArr[1]);
        character.upGrade(6, statArr[2]);
        character.upGrade(7, statArr[3]);
    }
    public void updateFoodTowerChange()
    {
        CharacterManager.Instance.setCharacter_destinyBase(ref ailCharacter, jsonDataManager.Instance.getCharacterSelect(1));
        upgradeInitStat(ref ailCharacter);

        for (int i=0;i<statArr.Length;i++){ //food tower Y update
            foodTower[i].transform.position = new Vector3(foodTower[i].transform.position.x,
                FoodTowerInitY + (-15f * (7 - statArr[i])), foodTower[i].transform.position.z);
            if(i==0) statText[i].GetComponent<TextMeshPro>().text = ailCharacter.getMaxHp().ToString() + "(+" + (statArr[i]*5).ToString() + ")";
            else if(i==1) statText[i].GetComponent<TextMeshPro>().text    = ailCharacter.getPhyAtk() + "(+" + statArr[i].ToString() + ")";
            else if (i == 2) statText[i].GetComponent<TextMeshPro>().text = ailCharacter.getMagAtk() + "(+" + statArr[i].ToString() + ")";
            else if (i == 3) statText[i].GetComponent<TextMeshPro>().text = ailCharacter.getSpeed() + "(+" + statArr[i].ToString() + ")";
        }

        

        ailFaceObj.GetComponent<Animator>().Play((maxRemainFood - curRemainFood).ToString());
        grinFaceObj.GetComponent<Animator>().Play((maxRemainFood - curRemainFood).ToString());
        foodInHand.GetComponent<SpriteRenderer>().sprite = foodInHandSpriteArr[curRemainFood];
        animAboutWoman();

        skillObj[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + ailCharacter.skillUse(0).getSkillName());
        skillObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + ailCharacter.skillUse(1).getSkillName());
    }
    public void animAboutWoman()
    {
        
        if (statArr[0] > preArr[0] || statArr[1] > preArr[1] || statArr[2] > preArr[2] || statArr[3] > preArr[3])
        {
            womanSayRemainFood.GetComponent<Animator>().Play("happy " + curRemainFood.ToString());
            womanFaceObj.GetComponent<Animator>().Play("womanHappy");
        }
        else {
            womanSayRemainFood.GetComponent<Animator>().Play(curRemainFood.ToString());
        }
        for (int i=0;i<4;i++)
        {
            //if(statArr[i] > preArr[i]) upgradeAnim[i].GetComponent<Animator>().Play("2");
            //else if(statArr[i] < preArr[i]) upgradeAnim[i].GetComponent<Animator>().Play("3");
        }
    }
}
