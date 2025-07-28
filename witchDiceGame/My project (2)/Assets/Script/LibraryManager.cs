using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LibraryManager : MonoBehaviour
{
    private static LibraryManager instance = null;
    private int[] curWitchPower = new int[3];
    private GameObject[] BtnArr = new GameObject[13];
    private Sprite[] spriteArr = new Sprite[12];

    private GameObject curPowerDesc;
    private GameObject curPowerDescInfo;
    private GameObject[] curPowerArr = new GameObject[2];

    private List<WitchPowerReader> witchPowerInfoList = new List<WitchPowerReader>();

    private GameObject[] buyUI = new GameObject[3]; //순서대로 전체, 스프라이트, text를 받을 예정

    private int savePreScreen = 0;
    //어디서 왓는지 확인. 0 : 마을 지도창  1: 모험 시작 창

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

    private void makeDarkBtn(int i) {
        Material material = BtnArr[i].GetComponent<SpriteRenderer>().material;
        material.SetFloat("_Transparency", 0.7f);
    }
    private void makeBrightBtn(int i)
    {
        Material material = BtnArr[i].GetComponent<SpriteRenderer>().material;
        material.SetFloat("_Transparency", 0.0f);
    }
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
            buyUI[0].SetActive(false);
        }
        else if (buyChk == 0)
        {
            SoundManager_Sfx.Instance.playSound(7);
            buyUI[2].GetComponent<TextMeshPro>().text = "You have this!";
        }
        else if (buyChk == 2) {
            SoundManager_Sfx.Instance.playSound(7);
            buyUI[2].GetComponent<TextMeshPro>().text = "You need To more money!";
        }
    }
    public void noBuyPower()
    {
        SoundManager_Sfx.Instance.playSound(1);
        buyUI[0].SetActive(false);
    }
    public void tryBuyPower(int idx)
    {
        if (jsonDataManager.Instance.checkWitchPower(idx) != 0) {
            buyPowerVal = idx;
            buyUI[0].SetActive(true);
            buyUI[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPowerUI/spr_witchUI_" + powerType[(idx - 1) / 3] + "_" + targetType[(idx - 1) % 3]);
            buyUI[2].GetComponent<TextMeshPro>().text = "능력 가격 : " + jsonDataManager.Instance.getPowerPrice(idx).ToString() +
                "\n현재 금액" + jsonDataManager.Instance.getMoney().ToString() + " -> " + (jsonDataManager.Instance.getMoney() - jsonDataManager.Instance.getPowerPrice(idx)).ToString();
        } 
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
        Debug.Log("draw point, man");
        Debug.Log(idx + "/" + power);
        if (idx == -1)
        {
            if (power == 0)
            {
                curPowerDesc.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPowerUI/spr_witchUI_nothing");
            }
            else
            {
                curPowerDesc.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPowerUI/spr_witchUI_" + powerType[(power - 1) / 3] + "_" + targetType[(power - 1) % 3]);
            }
            curPowerDescInfo.GetComponent<TextMeshPro>().text = witchPowerInfoList[power].PowerName + "\n" + witchPowerInfoList[power].KR;
        }
        else
        {
            if (power == 0) curPowerArr[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPowerUI/spr_witchUI_nothing");
            else curPowerArr[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPowerUI/spr_witchUI_" + powerType[(power - 1) / 3] + "_" + targetType[(power - 1) % 3]);
            curPowerDescInfo.GetComponent<TextMeshPro>().text = witchPowerInfoList[0].PowerName + "\n" + witchPowerInfoList[0].KR;
        }
    }

    public void hoverInCurPower(int i)
    {
        if (curWitchPower[i] != -1)
        {
            Material material = curPowerArr[i - 1].GetComponent<SpriteRenderer>().material;
            material.SetFloat("_Transparency", 0.7f);
            drawSelectPower(-1, curWitchPower[i]);
        }
    }
    public void hoverOutCurPower(int i)
    {
        if (curWitchPower[i] != -1) {
            Material material = curPowerArr[i - 1].GetComponent<SpriteRenderer>().material;
            material.SetFloat("_Transparency", 0.0f);
            drawSelectPower(-1, 0);
        }
    }

    public void hoverInBtn(int i)
    {
        drawSelectPower(-1, i);
        makeDarkBtn(i);
    }
    public void hoverOutBtn(int i)
    {
        drawSelectPower(-1, 0);
        if (curWitchPower[1] != i && curWitchPower[2] != i)
        {
            makeBrightBtn(i);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        curPowerDescInfo = GameObject.Find("obj_library_desc");
        for (int i = 1; i < BtnArr.Length; i++) {
            BtnArr[i] = GameObject.Find("obj_library_btn_" + i.ToString());
        }

        curPowerDesc = GameObject.Find("obj_library_witchPower_curSelect");
        for (int i = 0; i < 2; i++) {
            curPowerArr[i] = GameObject.Find("obj_library_witchPower_Select_" + i.ToString());
        }
        witchPowerInfoList = CSVReader.Read<WitchPowerReader>("witchPower");

        buyUI[0] = GameObject.Find("obj_ui_library_buy");
        buyUI[1] = GameObject.Find("obj_ui_library_buy_sprite");
        buyUI[2] = GameObject.Find("obj_ui_library_buy_text");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clickCurWitchPower(int idx)
    {
        //이미 배치된 파워 해제용
        if (curWitchPower[idx] != -1)
        {
            SoundManager_Sfx.Instance.playSound(7);
            clickWitchPower(curWitchPower[idx]);
        }
    }
    public void clickWitchPower(int input)
    {
        int chk = jsonDataManager.Instance.checkWitchPower(input);
        if (chk == 0) //가지고 있는 경우
        {
            for (int idx = 1; idx < curWitchPower.Length; idx++)
            {
                if (curWitchPower[idx] == input)
                {
                    SoundManager_Sfx.Instance.playSound(7);
                    //이미 사용중인 거 선택시 해제
                    Debug.Log("make empty about current box");
                    hoverOutCurPower(idx);
                    curWitchPower[idx] = -1;
                    makeBrightBtn(input);
                    drawSelectPower(idx - 1, 0);
                    return;
                }
            }
            //배틀 매니져에 선택 세팅하기
            for (int idx = 1; idx < curWitchPower.Length; idx++)
            {
                
                //mainCamera.GetComponent<CameraShake>().updateInitPosition(new Vector3(-1000f, mainCamera.transform.position.y, mainCamera.transform.position.z));
                if (curWitchPower[idx] == -1)
                {
                    SoundManager_Sfx.Instance.playSound(4);
                    CameraManager.Instance.VibrateForeTime(0.1f);

                    curWitchPower[idx] = input;
                    makeDarkBtn(input);
                    drawSelectPower(idx - 1, input);
                    return;
                }
            }
        }
        else
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
        if (jsonDataManager.Instance.getChapterDid(0) == 6) { 
            Owl.GetComponent<Animator>().Play(Random.Range(1,3).ToString()); 
        }
        else Owl.GetComponent<Animator>().Play("0");

        savePreScreen = idx;
        //CameraManager.Instance.zoomEvent();
        CameraManager.Instance.updateInitPosition(new Vector3(-1500f, 0f, CameraManager.Instance.cameraPointZ()));
        SoundManager_Main.Instance.playSound(1);
        SoundManager_Main.Instance.stopSound(0);
        SoundManager_Main.Instance.stopSound(7);
        jsonDataManager.Instance.addMoney(0);
        //배틀 매니져에서 받아오기
        for (int i = 1; i < curWitchPower.Length; i++)
        {
            Debug.Log("we draw at " + i.ToString() + "  about curWitch power " + curWitchPower[i]);
            curWitchPower[i] = BattleManager.Instance.getWitchPower(i);
            drawSelectPower(i-1, curWitchPower[i]);
            makeDarkBtn(curWitchPower[i]);
        }
        for (int i=1;i<BtnArr.Length;i++)
        {
            drawPowerByLock(i);
        }
        buyUI[0].SetActive(false);
    }
    public void exitLibrary() {
        //둘다 선택이 되었을 경우에만 나갈 수 있도록
        if (curWitchPower[1] != -1 && curWitchPower[2] != -1)
        {
            //배틀 메니져에 선택 세팅하기
            BattleManager.Instance.setWitchPower(1, curWitchPower[1]);
            BattleManager.Instance.setWitchPower(2, curWitchPower[2]);
            if(savePreScreen == 0) TownManager.Instance.backToTownUI();
            if (savePreScreen == 1) AdventureReadyManager.Instance.enterAdventureReady();
            SoundManager_Main.Instance.stopSound(1);
            jsonDataManager.Instance.changeWitchPower(curWitchPower[1], curWitchPower[2]);
            buyUI[0].SetActive(false);
        }
        else //지금은 못나가게 하는 게 다지만, 기존 마녀 능력 유지하는거 공지 화면과 함께 나갈껀지 물어보고, 그래도 나간다 그러면 이전 마녀능력으로 돌리기
        {
            
        }
    }
}
