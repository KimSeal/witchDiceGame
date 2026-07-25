using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
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
    public GameObject[] optionBtn = new GameObject[4];
    [SerializeField]
    public TextMeshProUGUI[] languageBigText = new TextMeshProUGUI[2];
    public GameObject[] languageBtn = new GameObject[3];
    public GameObject[] underTextSizeButton = new GameObject[3];
    public TextMeshProUGUI[] underTextSizeButtonText = new TextMeshProUGUI[3];

    [SerializeField]
    public GameObject[] screenSizeBtn = new GameObject[5];
    [SerializeField]
    public GameObject[] soundBtn = new GameObject[2];

    [SerializeField]
    public GameObject[] battleZoomOptButton = new GameObject[4];
    public TextMeshProUGUI[] battleZoomOptButtonText = new TextMeshProUGUI[4];
    public TextMeshProUGUI battleZoomOptText;

    [SerializeField]
    public GameObject optionBackBoard;
    [SerializeField]
    public GameObject[] optionBoards = new GameObject[4];
   
    [SerializeField]
    public TextMeshProUGUI[] optionBtnText = new TextMeshProUGUI[4];
    public TextMeshProUGUI[] soundText = new TextMeshProUGUI[2];
    public TextMeshProUGUI fullScreenText;

    [SerializeField]
    public GameObject[] optionImage = new GameObject[4];
    public Sprite[] optionLogo = new Sprite[5];

    private bool optionOn = false;
    public bool getOptionOn()
    {
        return optionOn;
    }
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
        upDownManager.Instance.clickWasteItemNoButton();
        upDownManager.Instance.clickNoSkillNoButton();
        if (optionOn)
        {
            unactiveOptionBoard();
        }
        else
        {
            activeOptionBoard();
        }
        upDownManager.Instance.hoverOutOptionButton();
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
            if (i == optionIdx)
            {
                optionBoards[i].SetActive(true);
            }
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

        for (int i = 0; i < optionImage.Length; i++)
        {
            if(i == idx) optionImage[i].GetComponent<Image>().sprite = optionLogo[0];
            else optionImage[i].GetComponent<Image>().sprite = optionLogo[i + 1];
        }

        for (int i = 0; i < optionBoards.Length; i++)
        {
            if (idx == i)
            {
                optionBtn[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color32(134, 229, 127, 255);
                optionBoards[i].SetActive(true);
            }
            else
            {
                optionBtn[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, 255);
                optionBoards[i].SetActive(false);
            }
        }
        if (idx == 0) { changeLanguage(jsonDataManager.Instance.getLanguage()); changeTextSize(jsonDataManager.Instance.getFontSize()); }
        else if (idx == 1) changeScreenSize(jsonDataManager.Instance.getScreenSize());
        else if (idx == 2) changeSound();
        else if (idx == 3) changePlay();
    }
    public void changePlay()
    {
        if (optionIdx == 3)
        {
            battleZoomOptText.text = TalkManager.Instance.getDesc(197);
            for (int i = 0; i < 4; i++)
            {
                battleZoomOptButtonText[i].text = "- "+ TalkManager.Instance.getDesc(201 - i);
            }

            changeBattleZoom(jsonDataManager.Instance.getBattleShakeOpt());
            updateOptionButtonText();
        }
    }
    public void changeBattleZoom(int val)
    {
        jsonDataManager.Instance.setBattleShakeOpt(val);
        for (int i = 0; i < battleZoomOptButton.Length; i++)
        {
            if (val == i)
            {
                battleZoomOptButton[i].GetComponent<hoverRotateUI>().setLanguageActive(true);
            }
            else
            {
                battleZoomOptButton[i].GetComponent<hoverRotateUI>().setLanguageActive(false);
            }
        }
    }

    public void updateOptionButtonText()
    {
        optionBtnText[0].text = TalkManager.Instance.getDesc(192);
        optionBtnText[1].text = TalkManager.Instance.getDesc(94);
        optionBtnText[2].text = TalkManager.Instance.getDesc(95);
        optionBtnText[3].text = TalkManager.Instance.getDesc(202);
    }

    public void changeTextSize(int val)
    {
        if(optionIdx == 0)
        {
            TalkManager.Instance.clickFontSize(val);
            for (int i = 0; i < underTextSizeButton.Length; i++)
            {
                if (val == i)
                {
                    underTextSizeButton[i].GetComponent<hoverRotateUI>().setLanguageActive(true);
                }
                else
                {
                    underTextSizeButton[i].GetComponent<hoverRotateUI>().setLanguageActive(false);
                }
            }
        }
    }
    public void changeLanguage(int idx)
    {
        if (optionIdx == 0)
        {
            AdventureManager.Instance.changeLanguage();
            jsonDataManager.Instance.setLanguage(idx);

            languageBigText[0].text = TalkManager.Instance.getDesc(93);
            languageBigText[1].text = TalkManager.Instance.getDesc(193);

            underTextSizeButtonText[0].text = "- "+TalkManager.Instance.getDesc(194);
            underTextSizeButtonText[1].text = "- " + TalkManager.Instance.getDesc(195);
            underTextSizeButtonText[2].text = "- " + TalkManager.Instance.getDesc(196);

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
            changeTextSize(jsonDataManager.Instance.getFontSize());
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
