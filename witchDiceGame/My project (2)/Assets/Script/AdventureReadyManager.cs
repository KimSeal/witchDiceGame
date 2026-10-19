using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [SerializeField]
    public GameObject warningObj;
    public TextMeshPro warningText;

    [SerializeField]
    public GameObject ChapterNoGoEntity;
    public TextMeshProUGUI ChapterNoGoText;

    [SerializeField]
    public Sprite[] levelSelectSprite = new Sprite[4];
    public Sprite[] activeButtonSprite = new Sprite[4];
    public TextMeshPro[] levelSelectText = new TextMeshPro[10]; // normal level, infinity level, curlevelText, val0, val1, val2, val3, 
                                                                //curLevelText2
    public GameObject levelSelectBoard;
    public GameObject levelSelectActiveButton;
    public GameObject levelSelectActiveOutline;
    public GameObject levelSelectArrow;

    public GameObject levelSelectImage;
    public GameObject levelMask;
    
    private int curLevelMode = 0;
    private bool levelSelectOnOff = false;

    public void hoverInLevelSelectSlider(int idx)
    {
        if (idx == 0) ToolBarManager.Instance.setToolBar(31);
        if (idx == 1) ToolBarManager.Instance.setToolBar(32);
        if (idx == 2) ToolBarManager.Instance.setToolBar(33);
        if (idx == 3) ToolBarManager.Instance.setToolBar(34);
    }
    public void hoverOutLevelSelectSlider(int opt)
    {
        ToolBarManager.Instance.toolBarOnOff(0);
    }
    public void setAdventureLevel(int opt, float val)
    {
        Debug.Log(val);
    }

    public void hoverInLevelSelect(int opt)
    {
        if (opt != curLevelMode)
        {
            levelSelectBoard.GetComponent<SpriteRenderer>().sprite = levelSelectSprite[curLevelMode + 2];
        }
        
    }
    public void hoverOutLevelSelect()
    {
        if (curLevelMode == 0) { levelSelectText[0].color = Color.black; levelSelectText[1].color = Color.white; }
        if (curLevelMode == 1) { levelSelectText[0].color = Color.white; levelSelectText[1].color = Color.black; }
        levelSelectBoard.GetComponent<SpriteRenderer>().sprite = levelSelectSprite[curLevelMode];
        levelSelectArrow.transform.position = new Vector3(-1075f + curLevelMode * 150f, levelSelectArrow.transform.position.y, levelSelectArrow.transform.position.z);
    }
    public void clickLevelSelect(int opt)
    {
        if (false)//demo version lock
        {
            curLevelMode = opt;
        }
        else if(opt == 1)
        {
            fullUI.showFull(5);
        }
        hoverOutLevelSelect();
    }
    public void hoverInActiveLevelSelect() {
        levelSelectActiveOutline.GetComponent<SpriteRenderer>().sprite = activeButtonSprite[2];
    }
    public void hoverOutActiveLevelSelect() {
        levelSelectActiveOutline.GetComponent<SpriteRenderer>().sprite = activeButtonSprite[3];
    }
    public void clickActiveLevelSelect()
    {
        activeLevelSelect(!levelSelectOnOff);
    }
    public void activeLevelSelect(bool onOff)
    {
        if (!onOff) {
            if (levelSelectOnOff == true) { hoverOutLevelSelect(); hoverOutActiveLevelSelect(); } 
            levelSelectActiveButton.GetComponent<SpriteRenderer>().sprite = activeButtonSprite[0];
            levelSelectText[7].text = TalkManager.Instance.getDesc(273) + ":\n-Easy-\n" + TalkManager.Instance.getDesc(271);
        }

        levelSelectBoard.SetActive(onOff);
        
        if (onOff) { 
            hoverOutLevelSelect(); hoverOutActiveLevelSelect(); levelSelectActiveButton.GetComponent<SpriteRenderer>().sprite = activeButtonSprite[1];
            levelSelectText[0].text = TalkManager.Instance.getDesc(271);
            levelSelectText[1].text = TalkManager.Instance.getDesc(272);
            levelSelectText[3].text = TalkManager.Instance.getDesc(263) + "\n*1.0";
            levelSelectText[4].text = TalkManager.Instance.getDesc(265) + "\n+0";
            levelSelectText[5].text = TalkManager.Instance.getDesc(267) + "\n" + TalkManager.Instance.getDesc(242);
            levelSelectText[6].text = TalkManager.Instance.getDesc(269) + "\n+0.0";
            levelSelectText[7].text = "";

        }
        levelSelectOnOff = onOff;
    }

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
        clickChapterNoGoButton();
        activeLevelSelect(false);
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

    public bool warningClickAble(int opt)
    {
        if (opt == 0)
        {
            for (int i=0;i<6;i++)
            {
                if (jsonDataManager.Instance.getChapterRead(i, 2) == 1) return true;
            }
            return false;
        }
        if (opt == 1) {
            if(jsonDataManager.Instance.getChapterRead(0,2) == 2 && 
                jsonDataManager.Instance.getCharacterSelect(0) == 0) return true;
        }
        if(opt == 2)
        {
            if (jsonDataManager.Instance.getChapterRead(1, 2) != 2) return false; // town 미개방시 return false 
            int curChapter = 1;
            for (int i=0;i<6;i++)
            {
                if (jsonDataManager.Instance.getChapterRead(i, 2) == 2) curChapter += 1; //현재 챕터 확인
            }
            if (jsonDataManager.Instance.getFoodStreetStat(0) +
                jsonDataManager.Instance.getFoodStreetStat(1) +
                jsonDataManager.Instance.getFoodStreetStat(2) +
                jsonDataManager.Instance.getFoodStreetStat(3) < curChapter) return true; // 현재 챕터만큼 마을 스탯을 다 안썼다면
        }
        return false;
    }
    public void hoverInWarning(int opt)
    {
        warningObj.GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
    }
    public void hoverOutWarning()
    {
        
        warningObj.GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
        
    }
    public void clickWarning()
    {
        int curOpt = -1;
        for (int i=0;i<warningTextArr.Length;i++)
        {
            if (warningClickAble(i))
            {
                curOpt = i;
                break;
            }
        }

        if(curOpt == 0)
        {
            upDownManager.Instance.clickUnderTownButton(1);
        }
        else if (curOpt == 1){
            upDownManager.Instance.clickUnderTownButton(2);
        }
        else if (curOpt == 2) {
            upDownManager.Instance.clickUnderTownButton(4);
        }
    }
    public int[] warningTextArr = { 176,177,0};
    public void updateWarningImage()
    {
        int warningExist = -1;
        for(int i = 0; i < warningTextArr.Length; i++)
        {
            if (warningClickAble(i)){
                warningExist = i;
                break;
            }
        }
        if (warningExist >= 0)
        {
            warningObj.GetComponent<Animator>().Play("Warning");
            warningText.text = TalkManager.Instance.getDesc(warningTextArr[warningExist]);
        }
        else {
            warningObj.GetComponent<Animator>().Play("Empty");
            warningText.text = "";
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
    public void clickAdventureTower()
    {
        if (warningClickAble(0))
        {
            ChapterNoGoEntity.SetActive(true);
            ChapterNoGoText.text = TalkManager.Instance.getDesc(259);
        }
        else
        {
            activeLevelSelect(false);
            startAdventure();
        }
    }
    public void clickChapterNoGoButton()
    {
        ChapterNoGoEntity.SetActive(false);
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

        if (jsonDataManager.Instance.getChapterRead(0, 0) == 0)
        {
            //TalkManager.Instance.makeTutorialLine(60f, 0f);
        }
        if (!jsonDataManager.Instance.getTowerEntry()) {
            
            newMark.GetComponent<Animator>().Play("NewEvent");
        }
        else newMark.GetComponent<Animator>().Play("Empty");

        hoverOutWarning();
        warningTextArr[0] = 184;
        warningTextArr[1] = 176;
        warningTextArr[2] = 177;
        updateWarningImage();
        activeLevelSelect(false);
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

        if (jsonDataManager.Instance.getCharacterSelect(0) == 0 )
        {
            enterCharacterMove[1] = 200;
            enterCharacterObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        CameraManager.Instance.updateInitPosition(new Vector3(-1000f, -500f, CameraManager.Instance.cameraPointZ()));
    }
    public void exitAdventureReady() {
        hoverOutWarning();
        SoundManager_Sfx.Instance.stopSound(17);
        spark[0] = -1;
        spark[1] = -1;
        enterCharacterMove[0] = 200;
        enterCharacterMove[1] = 200;
        //TownManager.Instance.backToTownUI();
        enterCharacterObj[0].transform.position = new Vector3(-1032f, -514f, 0f);
        enterCharacterObj[1].transform.position = new Vector3(-1032f, -514f, 0f);
        activeLevelSelect(false);
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
