using System;
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
    public GameObject[] optionBtn = new GameObject[6];
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
    public GameObject[] optionBoards = new GameObject[6];
   
    [SerializeField]
    public TextMeshProUGUI[] optionBtnText = new TextMeshProUGUI[6];
    public TextMeshProUGUI[] soundText = new TextMeshProUGUI[2];
    public TextMeshProUGUI fullScreenText;

    [SerializeField]
    public GameObject[] optionImage = new GameObject[6];
    public Sprite[] optionLogo = new Sprite[7];

    [SerializeField]
    public TextMeshProUGUI storyAutoSpeedText;
    public GameObject storyAutoSpeedButton;

    [SerializeField]
    public GameObject[] itemKeyBoard = new GameObject[13];
    public TextMeshProUGUI[] itemKeyBoardDesc = new TextMeshProUGUI[14];
    public TextMeshProUGUI[] itemKeyBoardText = new TextMeshProUGUI[13];
    public GameObject[] underKeyBoard = new GameObject[9];
    public TextMeshProUGUI[] underKeyBoardDesc = new TextMeshProUGUI[10];
    public TextMeshProUGUI[] underKeyBoardText = new TextMeshProUGUI[9];

    private int curSelectKeyBoard = -1;

    private readonly Dictionary<KeyCode, string> KeyNameMap = new Dictionary<KeyCode, string>
    {
        // 알파벳 기호 & 특수문자 키
        { KeyCode.Equals, "=" },
        { KeyCode.Plus, "+" },
        { KeyCode.Minus, "-" },
        { KeyCode.Period, "." },
        { KeyCode.Comma, "," },
        { KeyCode.Semicolon, ";" },
        { KeyCode.Colon, ":" },
        { KeyCode.Quote, "'" },
        { KeyCode.DoubleQuote, "\"" },
        { KeyCode.Slash, "/" },
        { KeyCode.Backslash, "\\" },
        { KeyCode.LeftBracket, "[" },
        { KeyCode.RightBracket, "]" },
        { KeyCode.BackQuote, "`" },
        { KeyCode.Tilde, "~" },
        { KeyCode.Question, "?" },
        { KeyCode.Exclaim, "!" },
        { KeyCode.At, "@" },
        { KeyCode.Hash, "#" },
        { KeyCode.Dollar, "$" },
        { KeyCode.Percent, "%" },
        { KeyCode.Ampersand, "&" },
        { KeyCode.Asterisk, "*" },
        { KeyCode.LeftParen, "(" },
        { KeyCode.RightParen, ")" },
        { KeyCode.Underscore, "_" },

        // 숫자 키보드 상단 (Alpha0 ~ Alpha9 -> "0" ~ "9")
        { KeyCode.Alpha0, "0" }, { KeyCode.Alpha1, "1" }, { KeyCode.Alpha2, "2" },
        { KeyCode.Alpha3, "3" }, { KeyCode.Alpha4, "4" }, { KeyCode.Alpha5, "5" },
        { KeyCode.Alpha6, "6" }, { KeyCode.Alpha7, "7" }, { KeyCode.Alpha8, "8" },
        { KeyCode.Alpha9, "9" },

        // 키패드(우측 숫자패드)
        { KeyCode.Keypad0, "Num 0" }, { KeyCode.Keypad1, "Num 1" }, { KeyCode.Keypad2, "Num 2" },
        { KeyCode.Keypad3, "Num 3" }, { KeyCode.Keypad4, "Num 4" }, { KeyCode.Keypad5, "Num 5" },
        { KeyCode.Keypad6, "Num 6" }, { KeyCode.Keypad7, "Num 7" }, { KeyCode.Keypad8, "Num 8" },
        { KeyCode.Keypad9, "Num 9" },
        { KeyCode.KeypadDivide, "Num /" }, { KeyCode.KeypadMultiply, "Num *" },
        { KeyCode.KeypadMinus, "Num -" },  { KeyCode.KeypadPlus, "Num +" },
        { KeyCode.KeypadPeriod, "Num ." }, { KeyCode.KeypadEnter, "Num Enter" },

        // 주요 특수 기능 키
        { KeyCode.Escape, "ESC" },
        { KeyCode.Return, "Enter" },
        { KeyCode.Space, "Space" },
        { KeyCode.Backspace, "Backspace" },
        { KeyCode.Tab, "Tab" },
        { KeyCode.CapsLock, "Caps Lock" },
        
        // Modifier 키 (Shift, Ctrl, Alt)
        { KeyCode.LeftShift, "L-Shift" }, { KeyCode.RightShift, "R-Shift" },
        { KeyCode.LeftControl, "L-Ctrl" }, { KeyCode.RightControl, "R-Ctrl" },
        { KeyCode.LeftAlt, "L-Alt" },     { KeyCode.RightAlt, "R-Alt" },

        // 화살표 방향키
        { KeyCode.UpArrow, "▲" },    { KeyCode.DownArrow, "▼" },
        { KeyCode.LeftArrow, "◀" },  { KeyCode.RightArrow, "▶" },

        // 마우스 버튼
        { KeyCode.Mouse0, "Mouse Left" },
        { KeyCode.Mouse1, "Mouse Right" },
        { KeyCode.Mouse2, "Mouse Middle" }
    };
    public string ToReadableString(KeyCode key)
    {
        // 1. 사전(Dictionary)에 정의된 특수 기호/키명이면 해당 문자열 반환
        if (KeyNameMap.TryGetValue(key, out string readableName))
        {
            return readableName;
        }

        // 2. 사전에 없으면 기본 ToString() 사용 (A~Z 알파벳, F1~F12 등은 그대로 출력됨)
        return key.ToString();
    }

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
        curSelectKeyBoard = -1;
        optionBoard.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            clickOptionButton();
        }
        if(curSelectKeyBoard >= 0 && Input.anyKeyDown)
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                // 실제 키보드/마우스 키로 감지되는 KeyCode만 필터링
                if (Input.GetKeyDown(key))
                {
                    // 마우스 좌/우 클릭(Mouse0, Mouse1 등)을 키 변경에서 제외하고 싶다면 조건 추가 가능
                    if (key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6)
                    {
                        continue;
                    }
                    else
                    {
                        jsonDataManager.Instance.setClickKey(curSelectKeyBoard / 100, curSelectKeyBoard % 100, key);
                        TalkManager.Instance.setKeyBoardBinding(curSelectKeyBoard / 100, curSelectKeyBoard % 100, key);
                        clickKeyMapping(curSelectKeyBoard);
                        changeKeyBoard();
                        curSelectKeyBoard = -1;
                        break;
                    }
                }
            }
            
        }
    }

    public void clickOptionButton()
    {
        curSelectKeyBoard = -1;
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
        curSelectKeyBoard = -1;
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
        else if (idx == 4) changeStory();
        else if (idx == 5) changeKeyBoard();
    }
    public void changeStory()
    {
        storyAutoSpeedText.text = TalkManager.Instance.getDesc(235);
        storyAutoSpeedButton.GetComponent<soundDragAndDrop>().setUIButton();
    }
    public void changeKeyBoard()
    {
        updateKeyMappingOutline();
        itemKeyBoardDesc[0].text = TalkManager.Instance.getDesc(211);
        for (int i=1;i < itemKeyBoardDesc.Length;i++)
        {
            itemKeyBoardDesc[i].text = TalkManager.Instance.getDesc(211+i);
            itemKeyBoardText[i - 1].text = ToReadableString(jsonDataManager.Instance.getClickKey(0, i - 1));

        }
        underKeyBoardDesc[0].text = TalkManager.Instance.getDesc(225);
        for (int i = 1; i < underKeyBoardDesc.Length; i++)
        {
            underKeyBoardDesc[i].text = TalkManager.Instance.getDesc(225 + i);
            underKeyBoardText[i - 1].text = ToReadableString(jsonDataManager.Instance.getClickKey(1, i - 1));
        }
    }

    public void clickKeyMapping(int idx)
    {
        if (idx < 0) return;
        
        int temp = curSelectKeyBoard;
        if (curSelectKeyBoard == idx) curSelectKeyBoard = -1;
        else curSelectKeyBoard = idx;

        updateKeyMappingOutline();
    }
    public void hoverInKeyMapping(int idx) {
        updateKeyMappingOutline();
        if (idx < 100 && idx >= 0)
        {
            itemKeyBoard[idx].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/spr_keyMappingOutline");
        }
        else
        {
            underKeyBoard[idx-100].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/spr_keyMappingOutline");
        }
    }
    public void hoverOutKeyMapping(int idx) {
        updateKeyMappingOutline();
    }

    public void updateKeyMappingOutline()
    {

        for(int i=0;i<itemKeyBoard.Length;i++) itemKeyBoard[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        for (int i = 0; i < underKeyBoard.Length; i++) underKeyBoard[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
        
        if(curSelectKeyBoard < 100 && curSelectKeyBoard >= 0)
        {
            itemKeyBoard[curSelectKeyBoard].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/spr_keyMappingOutline");
        }
        else
        {
            underKeyBoard[curSelectKeyBoard-100].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/spr_keyMappingOutline");
        }
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
