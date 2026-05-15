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

    [SerializeField] public GameObject ailFaceObj;
    [SerializeField] public GameObject grinFaceObj;
    [SerializeField] public GameObject womanFaceObj;

    private float FoodTowerInitY = 0;
    public int[] statArr = { 0, 0, 0, 0 };
    public int[] preArr = { 0,0,0,0};
    public int maxRemainFood = 0;
    public int curRemainFood = 0;

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
    }
    public void hoverOutFoodTower(int idx)
    {
        foodTower[idx].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
        foodTowerBowl[idx].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
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
    }
    public void hoverOutWoman()
    {
        foodInHand.GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
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
        updateFoodTowerChange();
    }
    public void exitFoodStreet()
    {

    }
    public void updateFoodTowerChange()
    {
        for (int i=0;i<statArr.Length;i++){ //food tower Y update
            foodTower[i].transform.position = new Vector3(foodTower[i].transform.position.x,
                FoodTowerInitY + (-15f * (7 - statArr[i])), foodTower[i].transform.position.z);
            if(i==0) statText[i].GetComponent<TextMeshPro>().text = "+" + (statArr[i]*10).ToString();
            else statText[i].GetComponent<TextMeshPro>().text = "+" + statArr[i].ToString();
        }

        ailFaceObj.GetComponent<Animator>().Play((maxRemainFood - curRemainFood).ToString());
        grinFaceObj.GetComponent<Animator>().Play((maxRemainFood - curRemainFood).ToString());
        foodInHand.GetComponent<SpriteRenderer>().sprite = foodInHandSpriteArr[curRemainFood];
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
            if(statArr[i] > preArr[i]) upgradeAnim[i].GetComponent<Animator>().Play("2");
            else if(statArr[i] < preArr[i]) upgradeAnim[i].GetComponent<Animator>().Play("3");
        }
    }
}
