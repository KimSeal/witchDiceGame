using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LibraryManager : MonoBehaviour
{
    private static LibraryManager instance = null;
    [SerializeField] private GameObject[] BtnArr = new GameObject[13];//obj_library_btn_(number)
    private Sprite[] spriteArr = new Sprite[12];

    [SerializeField] private GameObject curPowerDesc; //obj_library_witchPower_curSelect
    [SerializeField] private GameObject curPowerDescInfo; //obj_library_desc
    [SerializeField] private GameObject[] curPowerArr = new GameObject[2]; //obj_library_witchPower_Select_(number)

    private List<WitchPowerReader> witchPowerInfoList = new List<WitchPowerReader>();

    [SerializeField] private GameObject[] buyUI = new GameObject[3]; //순서대로 전체, 스프라이트, text를 받을 예정 obj_ui_library_buy_  /sprite/text

    private int savePreScreen = 0;
    //어디서 왓는지 확인. 0 : 마을 지도창  1: 모험 시작 창
    [SerializeField] private GameObject[] buyButton = new GameObject[2]; //spr_ui_library_yes/no Btn
    [SerializeField]
    public GameObject Owl;

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

    public static LibraryManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }

    public void hoverRotateAble(GameObject gameObjectTemp, int eventType, bool onOff)
    {
        if (eventType == 0) gameObjectTemp.GetComponent<hoverRotate>().shakeAble(onOff);
        else if (eventType == 1) gameObjectTemp.GetComponent<hoverRotate>().expandAble(onOff);
        else if (eventType == 2) gameObjectTemp.GetComponent<hoverRotate>().clickShakeAble(onOff); //에러있음.
    }

    public void shakeObject(GameObject gameObjectTemp)
    {
        gameObjectTemp.GetComponent<hoverRotate>().shakeStart();
    }
    /*
    private void makeDarkBtn(int i) {
        Material material = BtnArr[i].GetComponent<SpriteRenderer>().material;
        material.SetFloat("_Transparency", 0.7f);
    }
    private void makeBrightBtn(int i)
    {
        Material material = BtnArr[i].GetComponent<SpriteRenderer>().material;
        material.SetFloat("_Transparency", 0.0f);
    }
    */
    private string[] powerType = { "reroll", "turn", "add", "sub"};
    private string[] targetType = { "my", "enemy", "any" };

    private int buyPowerVal = 0;

    

    public void buyPower()
    {
        int buyChk = jsonDataManager.Instance.checkWitchPower(buyPowerVal);
        if (buyChk == 1)
        {
            SoundManager_Sfx.Instance.playSound(1);
            jsonDataManager.Instance.buyWitchPower(buyPowerVal);
            drawPowerByLock(buyPowerVal);
            deleteBuyUI();
            //buyUI[0].SetActive(false);
        }
        else if (buyChk == 0)
        {
            shakeObject(buyUI[0]);
            SoundManager_Sfx.Instance.playSound(7);
            buyUI[2].GetComponent<TextMeshPro>().text = TalkManager.Instance.getDesc(21);
        }
        else if (buyChk == 2) {
            shakeObject(buyUI[0]);
            SoundManager_Sfx.Instance.playSound(7);
            buyUI[2].GetComponent<TextMeshPro>().text = TalkManager.Instance.getDesc(20);
        }
    }
    public void noBuyPower()
    {
        SoundManager_Sfx.Instance.playSound(1);
        deleteBuyUI();
        //buyUI[0].SetActive(false);
    }
    public void tryBuyPower(int idx)
    {
        //if (jsonDataManager.Instance.checkWitchPower(idx) != 0) {
            buyPowerVal = idx;
            
            //buyUI[0].SetActive(true);
            makeBuyUI();
            buyUI[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPowerUI/spr_witchUI_" + powerType[(idx - 1) / 3] + "_" + targetType[(idx - 1) % 3]);
            buyUI[2].GetComponent<TextMeshPro>().text = TalkManager.Instance.getDesc(10) + " : " + jsonDataManager.Instance.getPowerPrice(idx).ToString() +
                "\n" + TalkManager.Instance.getDesc(11) + " : " + jsonDataManager.Instance.getMoney().ToString() + " -> " + (jsonDataManager.Instance.getMoney() - jsonDataManager.Instance.getPowerPrice(idx)).ToString();
        //} 
    }
    //보유 여부 확인후 Lock인지 아닌지 바꾸기
    private void drawPowerByLock(int power)
    {
        if (jsonDataManager.Instance.checkWitchPower(power) == 0)
        {
            BtnArr[power].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPowerUI/spr_witchUI_" + powerType[(power - 1) / 3] + "_" + targetType[(power - 1) % 3]);
        }
        else {
            BtnArr[power].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPowerUI/spr_witchUI_lock");
        }
    }
    //현재 선택한 능력 반영 함수
    private void drawSelectPower(int idx, int power) //
    {
        string witchStr = "";
        if (idx == -1)
        {
            witchStr = witchPowerInfoList[power].PowerName + "\n";

            if(jsonDataManager.Instance.getLanguage() == 0) witchStr+= witchPowerInfoList[power].KR;
            else if (jsonDataManager.Instance.getLanguage() == 1) witchStr += witchPowerInfoList[power].EN;
            else if (jsonDataManager.Instance.getLanguage() == 2) witchStr += witchPowerInfoList[power].JP;
        }
        else
        {
            witchStr = witchPowerInfoList[0].PowerName + "\n";

            if (jsonDataManager.Instance.getLanguage() == 0) witchStr += witchPowerInfoList[0].KR;
            else if (jsonDataManager.Instance.getLanguage() == 1) witchStr += witchPowerInfoList[0].EN;
            else if (jsonDataManager.Instance.getLanguage() == 2) witchStr += witchPowerInfoList[0].JP;
        }

        witchStr = TalkManager.Instance.SpecialTextChange(witchStr);
        TalkManager.Instance.setDescString(witchStr);
    }

    public void hoverInBtn(int i)
    {
        drawSelectPower(-1, i);
        //makeDarkBtn(i);
        
    }
    public void hoverOutBtn(int i)
    {
        TalkManager.Instance.setDescString("");
        //drawSelectPower(-1, 0);
        /*
        if (curWitchPower[1] != i && curWitchPower[2] != i)
        {
            makeBrightBtn(i);
        }
        */
    }

    private void makeBuyUI()
    {
        for (int i = 1; i<BtnArr.Length;i++) {
            hoverRotateAble(BtnArr[i], 1, false);
        }
        buyUI[0].SetActive(true);
        buyButton[0].GetComponent<hoverRotate>().expandEnd();
        buyButton[1].GetComponent<hoverRotate>().expandEnd();
        hoverRotateAble(buyButton[0], 1, true);
        hoverRotateAble(buyButton[1], 1, true);
        shakeObject(buyUI[0]);
    }
    private void deleteBuyUI()
    {
        for (int i = 1; i < BtnArr.Length; i++)
        {
            hoverRotateAble(BtnArr[i], 1, true);
        }
        buyUI[0].SetActive(false);
        buyButton[0].GetComponent<hoverRotate>().expandEnd();
        buyButton[1].GetComponent<hoverRotate>().expandEnd();
    }

    // Start is called before the first frame update
    void Start()
    {
        witchPowerInfoList = CSVReader.Read<WitchPowerReader>("witchPower");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clickWitchPower(int input)
    {
        shakeObject(BtnArr[input]);
        if (jsonDataManager.Instance.checkWitchPower(input) != 0)
        {
            SoundManager_Sfx.Instance.playSound(0);
            tryBuyPower(input);
            return;
        }
        SoundManager_Sfx.Instance.playSound(7);
    }

    public void enterLibrary(int idx)
    {
        // 올빼미 수인 보여주기
        if (jsonDataManager.Instance.getChapterRead(1,2) == 2) { 
            Owl.GetComponent<Animator>().Play(Random.Range(1,3).ToString()); 
        }
        else Owl.GetComponent<Animator>().Play("0");

        //hoverRotateAble(curPowerArr[0], 1, true);
        //hoverRotateAble(curPowerArr[0], 2, true);
        //hoverRotateAble(curPowerArr[1], 1, true);
        //hoverRotateAble(curPowerArr[1], 2, true);
        savePreScreen = idx;
        //CameraManager.Instance.zoomEvent();
        CameraManager.Instance.updateInitPosition(new Vector3(-1500f, 0f, CameraManager.Instance.cameraPointZ()));
        SoundManager_Main.Instance.playSound(1);
        SoundManager_Main.Instance.stopSound(0);
        SoundManager_Main.Instance.stopSound(7);
        jsonDataManager.Instance.addMoney(0);

        for (int i=1;i<BtnArr.Length;i++)
        {
            drawPowerByLock(i);
        }
        deleteBuyUI();
        //buyUI[0].SetActive(false);
        
    }
    public void exitLibrary() {
        //둘다 선택이 되었을 경우에만 나갈 수 있도록


            if(savePreScreen == 0) TownManager.Instance.backToTownUI();
            if (savePreScreen == 1) AdventureReadyManager.Instance.enterAdventureReady();
            SoundManager_Main.Instance.stopSound(1);
            //buyUI[0].SetActive(false);
            deleteBuyUI();

    }
}
