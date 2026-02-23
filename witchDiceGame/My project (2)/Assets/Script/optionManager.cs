using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class optionManager : MonoBehaviour
{
    private static optionManager instance = null;
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

    private int optionIdx = 0;
    //0: language 1: screen Size

    [SerializeField]
    public GameObject optionBoard;
    [SerializeField]
    public GameObject[] optionBtn = new GameObject[3];
    [SerializeField]
    public GameObject[] languageBtn = new GameObject[3];
    [SerializeField]
    public GameObject[] screenSizeBtn = new GameObject[5];
    [SerializeField]
    public GameObject[] soundBtn = new GameObject[2];

    [SerializeField]
    public GameObject optionBackBoard;
    [SerializeField]
    public GameObject[] optionBoards = new GameObject[3];
   
    [SerializeField]
    public TextMeshProUGUI[] optionBtnText = new TextMeshProUGUI[3];
    public TextMeshProUGUI[] soundText = new TextMeshProUGUI[2];
    public TextMeshProUGUI fullScreenText;

    private bool optionOn = false;
    public static optionManager Instance
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
        optionIdx = 0;
        optionBoard.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            clickOptionButton();
        }
    }

    public void clickOptionButton()
    {
        AdventureManager.Instance.activeGiveUpBoard(false);
        if (optionOn)
        {
            unactiveOptionBoard();
        }
        else
        {
            activeOptionBoard();
        }
    }
    public void clickBlack()
    {
        Debug.Log("double click");
    }
    public void activeOptionBoard()
    {
        optionOn = true;
        optionBoard.SetActive(true);
        optionBackBoard.SetActive(true);
        optionBackBoard.transform.position = new Vector3(CameraManager.Instance.cameraPointX(), CameraManager.Instance.cameraPointY(), 0);
        
        for (int i = 0; i < optionBoards.Length; i++)
        {
            if (i == optionIdx) optionBoards[i].SetActive(true);
            else optionBoards[i].SetActive(false);
        }
        changeOption(optionIdx);
        if (AdventureManager.Instance.getTutorial() == 0)
        {
            AdventureManager.Instance.changeLanguage();
            upDownManager.Instance.clickItem(-1);
            upDownManager.Instance.clickItem(-1);
            upDownManager.Instance.hoverOutWitchPowerButton();
            BattleManager.Instance.setCurClickSkill(-1);
        }
        updateOptionButtonText();
    }
    public void unactiveOptionBoard()
    {
        optionOn = false ;
        optionBoard.SetActive(false);
        optionBackBoard.SetActive(false);
    }

    public void changeOption(int idx)
    {
        optionIdx = idx;
        for (int i = 0; i < optionBoards.Length; i++)
        {
            if (idx == i)
            {
                optionBtn[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(255, 255, 255, 125);
                optionBoards[i].SetActive(true);
            }
            else
            {
                optionBtn[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, 255);
                optionBoards[i].SetActive(false);
            }
        }
        if (idx == 0) { changeLanguage(jsonDataManager.Instance.getLanguage()); }
        else if (idx == 1) changeScreenSize(jsonDataManager.Instance.getScreenSize());
        else if (idx == 2) changeSound();
    }

    public void updateOptionButtonText()
    {
        optionBtnText[0].text = TalkManager.Instance.getDesc(93);
        optionBtnText[1].text = TalkManager.Instance.getDesc(94);
        optionBtnText[2].text = TalkManager.Instance.getDesc(95);
    }

    public void changeLanguage(int idx)
    {
        if (optionIdx == 0)
        {
            AdventureManager.Instance.changeLanguage();
            jsonDataManager.Instance.setLanguage(idx);
            for (int i = 0; i < languageBtn.Length; i++)
            {
                if (idx == i)
                {
                    languageBtn[i].GetComponent<hoverRotateUI>().setLanguageActive(true);
                }
                else
                {
                    languageBtn[i].GetComponent<hoverRotateUI>().setLanguageActive(false);
                }
            }
            TalkManager.Instance.changeLan();
            updateOptionButtonText();
        }
    }

    public void changeScreenSize(int idx) {
        if (optionIdx == 1) {
            CameraManager.Instance.changeScreenSize(idx);
            soundBtn[0].GetComponent<soundDragAndDrop>().changeSoundMinMaxVal();
            soundBtn[1].GetComponent<soundDragAndDrop>().changeSoundMinMaxVal();
            for (int i = 0; i < screenSizeBtn.Length; i++)
            {
                if (idx == i)
                {
                    screenSizeBtn[i].GetComponent<hoverRotateUI>().setLanguageActive(true);
                }
                else
                {
                    screenSizeBtn[i].GetComponent<hoverRotateUI>().setLanguageActive(false);
                }
            }
            fullScreenText.text = "- " + TalkManager.Instance.getDesc(98);
        }
    }
    public void changeSound()
    {
        soundText[0].text = TalkManager.Instance.getDesc(96);
        soundText[1].text = TalkManager.Instance.getDesc(97);
        for (int i=0;i<soundBtn.Length;i++)
        {
            soundBtn[i].GetComponent<soundDragAndDrop>().setUIButton();
        }
    }
}
