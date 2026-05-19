using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureReadyManager : MonoBehaviour
{

    private static AdventureReadyManager instance = null;
    private GameObject[] witchPowerObj = new GameObject[2];
    [SerializeField]
    public GameObject[] enterCharacterObj = new GameObject[2];
    private float[] enterCharacterMove = new float[2];

    public GameObject enterButton;
    public GameObject newMark;

    [SerializeField]
    public GameObject backgroundLine;
    public GameObject[] backgroundSpark = new GameObject[9];

    public Sprite[] towerSprite = new Sprite[2];
    private int[] spark = new int[2];
    private float[] sparkVal = new float [2];
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

    public static AdventureReadyManager Instance
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
        enterCharacterMove[0] = 200f;
        enterCharacterMove[1] = 200f;
        for(int i=0;i<2;i++) witchPowerObj[i] = GameObject.Find("obj_adventureReady_witchPower_Select_" +i.ToString());

        for (int i=0;i<9;i++)
        {
            backgroundSpark[i].GetComponent<SpriteRenderer>().color = new Color(255f,255f,255f,0f);
        }
        spark[0] = -1; spark[1] = -1;
        sparkVal[0] = 0;  sparkVal[1] = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [SerializeField]
    public float addVal = 0f;

    private void FixedUpdate(){
        if (enterCharacterMove[0] < 200f)
        {
            enterCharacterMove[0] += 2.5f;
            if (enterCharacterMove[0] % 50f == 25) SoundManager_Sfx.Instance.playSound(17);
            enterCharacterObj[0].GetComponent<Transform>().position = new Vector3(-1185 + enterCharacterMove[0], -530 + 12 * Mathf.Abs(Mathf.Sin((enterCharacterMove[0]/50) * Mathf.PI)), 0f);
        }

        if (enterCharacterMove[1] < 200)
        {
            enterCharacterMove[1] += 2.5f;
            if (enterCharacterMove[1] % 50f == 25) SoundManager_Sfx.Instance.playSound(17);
            enterCharacterObj[1].GetComponent<Transform>().position = new Vector3(-1260 + enterCharacterMove[1], -530 + 12 * Mathf.Abs(Mathf.Sin((enterCharacterMove[1] / 50) * Mathf.PI)), 0f);
        }

        if (spark[0] >= 0)
        {
            for (int i = 0; i < 2; i++)
            {
                if (sparkVal[i] >= 0.0f && sparkVal[i] <= 1.0f)
                {
                    backgroundSpark[spark[i]].GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, sparkVal[i]);
                    sparkVal[i] += addVal;
                }
                else if (sparkVal[i] > 1.0f && sparkVal[i] <= 2.0f)
                {
                    backgroundSpark[spark[i]].GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, (2.0f - sparkVal[i]));
                    sparkVal[i] += addVal;
                    if (sparkVal[i] >= 2.0f)
                    {
                        backgroundSpark[spark[i]].GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 0f);
                        sparkVal[i] = Random.Range(-2.0f, -0.5f);
                        spark[i] = Random.Range(0, 9);
                        if (spark[i] == spark[(i + 1) % 2]) spark[i] = (spark[i] + 1) % 9;
                    }
                }
                else
                {
                    sparkVal[i] += addVal;
                }

            }
        }
    }
    
    public void hoverInTower()
    {
        enterButton.GetComponent<SpriteRenderer>().sprite = towerSprite[1];
        if (!backgroundLine.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("lineDraw"))
        {
            backgroundLine.GetComponent<Animator>().Play("lineDraw");
        }
    }
    public void hoverOutTower()
    {
        enterButton.GetComponent<SpriteRenderer>().sprite = towerSprite[0];
    }
    public void startAdventure()
    {

        jsonDataManager.Instance.towerEntry();
        enterCharacterMove[0] = 200f;
        enterCharacterMove[1] = 200f;
        SoundManager_Sfx.Instance.stopSound(17);

        SoundManager_Main.Instance.stopSound(7);
        SoundManager_Sfx.Instance.playSound(0);
        TownManager.Instance.setTownActive(false);
        upDownManager.Instance.clickCharacterButton(-1);
        AdventureManager.Instance.startAdventure();
    }
    public void enterAdventureReady()
    {
        if (!jsonDataManager.Instance.getTowerEntry()) {
            newMark.GetComponent<Animator>().Play("NewEvent");
        }
        else newMark.GetComponent<Animator>().Play("Empty");

        
        CharacterManager.Instance.setCharacter(2, jsonDataManager.Instance.getCharacterSelect(1));
        CharacterManager.Instance.setFoodStreetInfo();

        if (jsonDataManager.Instance.getCharacterSelect(0) > 0) CharacterManager.Instance.setCharacter(1, jsonDataManager.Instance.getCharacterSelect(0));
        else CharacterManager.Instance.emptyMyCharacter(1);

       

        //FoodStreetManager.Instance.upgradeInitStat(ref CharacterManager.Instance.getCharacter(2) );

        hoverOutTower();

        enterCharacterObj[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/backImage/spr_" + CharacterManager.Instance.getDestiny(jsonDataManager.Instance.getCharacterSelect(1)).getName() + "_back");
        enterCharacterObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/backImage/spr_" + CharacterManager.Instance.getDestiny(jsonDataManager.Instance.getCharacterSelect(0)).getName() + "_back");

        enterCharacterMove[0] = 0;
        enterCharacterMove[1] = -20;

        spark[0] = Random.Range(0, 9);
        spark[1] = (spark[0] + 1) % 9;
        for (int i = 0; i < 9; i++)
        {
            backgroundSpark[i].GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 0f);
        }

        if (jsonDataManager.Instance.getCharacterSelect(0) == 0 || jsonDataManager.Instance.getChapterRead(0,2) != 2)
        {
            enterCharacterMove[1] = 200;
            enterCharacterObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        CameraManager.Instance.updateInitPosition(new Vector3(-1000f, -500f, CameraManager.Instance.cameraPointZ()));
    }
    public void exitAdventureReady() {
        SoundManager_Sfx.Instance.stopSound(17);
        spark[0] = -1;
        spark[1] = -1;
        enterCharacterMove[0] = 200;
        enterCharacterMove[1] = 200;
        //TownManager.Instance.backToTownUI();
        enterCharacterObj[0].transform.position = new Vector3(-1032f, -514f, 0f);
        enterCharacterObj[1].transform.position = new Vector3(-1032f, -514f, 0f);

        //enterCharacterObj[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        //enterCharacterObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
    }

    public void hoverInCurPower(int i)
    {
        Material material = witchPowerObj[i].GetComponent<SpriteRenderer>().material;
        material.SetFloat("_Transparency", 0.7f);

    }
    public void hoverOutCurPower(int i)
    {
        Material material = witchPowerObj[i].GetComponent<SpriteRenderer>().material;
        material.SetFloat("_Transparency", 0.0f);
    }
    public void clickCurPower()
    {
        if (!jsonDataManager.Instance.getLibraryMeet())
        {
            jsonDataManager.Instance.libraryMeet();
            //TalkManager.Instance.startTalk(1);
        }
        SoundManager_Sfx.Instance.playSound(0);
        LibraryManager.Instance.enterLibrary(1);
    }
    private string[] powerType = { "reroll", "turn", "add", "sub" };
    private string[] targetType = { "my", "enemy", "any" };

}
