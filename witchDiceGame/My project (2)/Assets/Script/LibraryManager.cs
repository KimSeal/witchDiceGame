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

    private void drawSelectPower(int idx, int power) //
    {
        Debug.Log("draw point, man");
        Debug.Log(idx + "/" + power);
        if (idx == -1)
        {
            Debug.Log((power - 1) / 3);
            Debug.Log((power - 1) % 3);
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

 

    //나중에 여기다가 능력유효여부 확인
    private bool checkLock(int witchPowerIdx) {
        return true;
    }

    public void clickCurWitchPower(int idx)
    {
        //이미 배치된 파워 해제용
        if (curWitchPower[idx] != -1)
        {
            clickWitchPower(curWitchPower[idx]);
        }
    }
    public void clickWitchPower(int input)
    {
        if (!checkLock(input)) {
            Debug.Log("no it is locking now!");
            return;
        }
        for (int idx = 1; idx < curWitchPower.Length; idx++) {
            if (curWitchPower[idx] == input)
            {
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
            if (curWitchPower[idx] == -1) {
                CameraManager.Instance.VibrateForeTime(0.1f);
                
                curWitchPower[idx] = input;
                makeDarkBtn(input);
                drawSelectPower(idx-1, input);
                return;
            }
        }

    }

    public void enterLibrary()
    {
        CameraManager.Instance.zoomEvent();
        //배틀 매니져에서 받아오기
        for (int i = 1; i < curWitchPower.Length; i++)
        {
            Debug.Log("we draw at " + i.ToString() + "  about curWitch power " + curWitchPower[i]);
            curWitchPower[i] = BattleManager.Instance.getWitchPower(i);
            drawSelectPower(i-1, curWitchPower[i]);
            makeDarkBtn(curWitchPower[i]);
        }
    }
    public void exitLibrary() {
        //둘다 선택이 되었을 경우에만 나갈 수 있도록
        if (curWitchPower[1] != -1 && curWitchPower[2] != -1)
        {
            //배틀 메니져에 선택 세팅하기
            BattleManager.Instance.setWitchPower(1, curWitchPower[1]);
            BattleManager.Instance.setWitchPower(2, curWitchPower[2]);
            TownManager.Instance.backToTownUI();
        }
        else //지금은 못나가게 하는 게 다지만, 기존 마녀 능력 유지하는거 공지 화면과 함께 나갈껀지 물어보고, 그래도 나간다 그러면 이전 마녀능력으로 돌리기
        {
            
        }
    }
}
