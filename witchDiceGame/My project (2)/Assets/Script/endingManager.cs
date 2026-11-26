using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class endingManager : MonoBehaviour
{
    //싱클톤
    private static endingManager instance = null;
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
    public static endingManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }

    //about select phase
    [SerializeField]
    public GameObject[] endingSelectObj = new GameObject[5]; //entity, button-saveAil, button-killAil, save outline, kill Outline
    public TextMeshProUGUI[] endingSelectText = new TextMeshProUGUI[2]; // saveText, killText
    public Sprite[] endingSelectSpriteArr = new Sprite[8]; // 0-5 : button by num, 6 : none outline 7 : outline
    private bool endingType = false;
    private bool endingSelect = false;
    private bool endingTiming = false;
    public bool getEndingSelect()
    {
        return endingSelect;
    }
    public void setEndingSelect(bool opt)
    {
        endingSelect = opt;
    }
    public bool getEndingTiming()
    {
        return endingTiming;
    }
    public void setEndingTiming(bool onOff)
    {
        endingTiming = onOff;
    }
    public void selectEndlingType(bool sel)
    {
        endingType = sel; //false : bad true : harfy or happy
        endingSelect = true;
        onOffEndingSelect(false);
    }
    public bool getEndingType()
    {
        return endingType;
    }
    public void setEndingType(bool opt) { endingType = opt; }
    public void onOffEndingSelect(bool onOff)
    {
        if (!onOff)
        {
            endingSelectObj[0].SetActive(false);
        }
        else
        {
            hoverOutEndingSelect(0);
            endingSelectObj[0].SetActive(true);
            endingSelectText[0].text = TalkManager.Instance.getDesc(279);
            endingSelectText[1].text = TalkManager.Instance.getDesc(280);
        }
    }

    public void hoverInEndingSelect(int idx)
    {
        endingSelectObj[3 + idx].GetComponent<Image>().sprite = endingSelectSpriteArr[7];
    }
    public void hoverOutEndingSelect(int idx)
    {
        endingSelectObj[3].GetComponent<Image>().sprite = endingSelectSpriteArr[6];
        endingSelectObj[4].GetComponent<Image>().sprite = endingSelectSpriteArr[6];
    }

    private Vector3 harfyPoint;
    private float harfyMoveVal;
    private float harfyMoveMoveVal;

    [SerializeField]
    public GameObject ailObj;
    public GameObject jewelObj;
    public GameObject witchObj;
    public GameObject harfyObj;
    public GameObject fieldObj;
    public GameObject backgroundObj;
    public GameObject[] dropObj = new GameObject[5];
    public GameObject[] dropObj2 = new GameObject[5];
    public GameObject[] shadowObj = new GameObject[2];

    public GameObject[] brokenFieldObj = new GameObject[2];
    public GameObject[] brokenBackObj = new GameObject[2];

    public GameObject whiteFullObj;

    public Sprite[] fieldSprite = new Sprite[2];
    public Sprite[] backSprite = new Sprite[2];

    public Sprite[] brokenFieldSprite = new Sprite[5];
    public Sprite[] brokenBackSprite = new Sprite[5];

    private bool jewelMove = false;
    private bool harfyMove = false;
    private bool whiteMove = false;
    private float whiteSpeed = 0f;
    [SerializeField]
    private float whiteChangeVal = 0.05f;

    [SerializeField]
    private float fieldSpeedX;
    [SerializeField]
    private float backSpeedX;

    [SerializeField]
    private float[] dropRockSpeedY = new float[4]; //0-1 small random y speed, 2-3 big random y speed
    [SerializeField]
    private float[] dropRockSpeedX = new float[2]; // 
    [SerializeField]
    private float[] dropRockPointX = new float[8]; // 0-1: small stop 2-3 smallmove
    [SerializeField]
    private float[] dropRockPointY = new float[8];// 4-5 big stop 6-7 big move

    private int harfyTake = 0;

    private int rockDropState =0;

    private float[] curDropBigRockSpeedY = new float[5];
    private float[] curDropSmallRockSpeedY = new float[5];

    [SerializeField]
    public GameObject[] creditObj = new GameObject[6]; //entity, background, text, skipButton, skipText, fadeObj
    private int textAlphaChange = 0;
    private string[] creditText = {"Developer\n<size=36>SinPoDo</size>" , "test1", "test2"};
    public int endingCreditEnd = 0;
    public int endingTextIdx;

    private Coroutine endCreditCoroutine; 

    public void startEndingCredit(bool mode)//false :bad   true :happy
    {
        creditObj[0].SetActive(true);
        endingTextIdx = 0;
        endingCreditEnd = 1;

        if (!mode)
        {
            creditObj[1].GetComponent<Image>().color = new Color(255,255,255,1);
            creditObj[5].GetComponent<Image>().color = new Color(255, 255, 255, 1);
            creditObj[2].GetComponent<TextMeshProUGUI>().color = new Color(0, 0, 0, 1);
        }
        else
        {
            creditObj[1].GetComponent<Image>().color = new Color(0, 0, 0, 1);
            creditObj[5].GetComponent<Image>().color = new Color(0, 0, 0, 1);
            creditObj[2].GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, 1);
        }
        
        creditObj[4].GetComponent<TextMeshProUGUI>().text = TalkManager.Instance.getDesc(281);
        creditObj[5].GetComponent<Image>().color = new Color
                    (creditObj[5].GetComponent<Image>().color.r,
                    creditObj[5].GetComponent<Image>().color.g,
                    creditObj[5].GetComponent<Image>().color.b,
                    1f);
        textAlphaChange = 0;

        endCreditCoroutine = StartCoroutine(creditCoroutine());
    }
    public void finishEndingCredit()
    {
        if (endCreditCoroutine != null)
        {
            StopCoroutine(endCreditCoroutine);
        }
        endingTextIdx = 0;
        endingCreditEnd = 0;

        setCreditAlphaMode(0);
        creditObj[0].SetActive(false);
    }
    public int getEndingCreditEnd()
    {
        return endingCreditEnd;
    }
    public void setCreditAlphaMode(int opt)
    {   // 0 : 비활성화 1: 하향 2 : 하향 종료 3: 상승 4 : 상승 종료
        textAlphaChange = opt;

        if (opt == 1 || opt == 4) // 하향 또는 상승 종료
        {
            creditObj[5].GetComponent<Image>().color = new Color
           (creditObj[5].GetComponent<Image>().color.r,
           creditObj[5].GetComponent<Image>().color.g,
           creditObj[5].GetComponent<Image>().color.b,
           1f);
        }
        if(opt == 0 || opt == 3 || opt == 2) // 비활성화 또는 상승 또는 하향 종료
        {
            creditObj[5].GetComponent<Image>().color = new Color
            (creditObj[5].GetComponent<Image>().color.r,
            creditObj[5].GetComponent<Image>().color.g,
            creditObj[5].GetComponent<Image>().color.b,
            0f);
        }
    }
    public void creditTextChange(int idx)
    {
        creditObj[2].GetComponent<TextMeshProUGUI>().text = creditText[idx]; 
    }
    public void hoverInSkipButton()
    {
        Debug.Log("hoverIn");
    }
    public void hoverOutSkipButton()
    {
        Debug.Log("hoverOut");
    }
    public void clickSkipButton()
    {
        Debug.Log("clickSkip");
        endingCreditEnd = 0;
    }

    // 0 : 파괴 X
    // 1: 파괴는 하되, 움직임 X
    // 2 : 파괴 + 움직임
    public void setRockDropState(int opt)
    {

        if (opt <= 2)
        {   //3, 4는 위치 초기화 용도로만 사용
            //0->1 or 1->0 일때만
            if ((opt == 0 && rockDropState == 1) || (opt == 1 && rockDropState == 0)) FadeUIScript2.fadeIn();
            rockDropState = opt;
        }

        if(opt == 0) //모든 drop없애고 위치 초기화
        {
            brokenFieldObj[0].transform.position = 
            new Vector3(2116f, brokenFieldObj[0].transform.position.y, brokenFieldObj[0].transform.position.z);
            brokenBackObj[0].transform.position =
            new Vector3(2116f, brokenBackObj[0].transform.position.y, brokenBackObj[0].transform.position.z);
            brokenFieldObj[0].GetComponent<SpriteRenderer>().sprite = brokenFieldSprite[0];
            brokenBackObj[0].GetComponent<SpriteRenderer>().sprite = brokenBackSprite[0];
            brokenFieldObj[1].GetComponent<SpriteRenderer>().sprite = brokenFieldSprite[0];
            brokenBackObj[1].GetComponent<SpriteRenderer>().sprite = brokenBackSprite[0];
            
            for (int i = 0; i < dropObj2.Length; i++) {
                dropObj2[i].transform.position = new Vector3(2500, 120, 0);
            }
            for (int i = 0; i < dropObj.Length; i++)
            {
                dropObj[i].transform.position = new Vector3(2500, 120, 0);
            }
        }
        if(opt == 1)
        {
            brokenFieldObj[0].transform.position =
            new Vector3(2116f, brokenFieldObj[0].transform.position.y, brokenFieldObj[0].transform.position.z);
            brokenBackObj[0].transform.position =
            new Vector3(2116f, brokenBackObj[0].transform.position.y, brokenBackObj[0].transform.position.z);
            brokenFieldObj[0].GetComponent<SpriteRenderer>().sprite = brokenFieldSprite[1];
            brokenBackObj[0].GetComponent<SpriteRenderer>().sprite = brokenBackSprite[1];
            brokenFieldObj[1].GetComponent<SpriteRenderer>().sprite = brokenFieldSprite[1];
            brokenBackObj[1].GetComponent<SpriteRenderer>().sprite = brokenBackSprite[1];

            for (int i = 0; i < dropObj2.Length; i++)
            {
                dropObj2[i].GetComponent<Animator>().Play("Rock" + Random.Range(0, 4).ToString());
                dropObj2[i].transform.position = 
                    new Vector3(Random.Range(dropRockPointX[0], dropRockPointX[1]), 
                    Random.Range(dropRockPointY[0], dropRockPointY[1]), 0);
                curDropSmallRockSpeedY[i] = Random.Range(dropRockSpeedY[0], dropRockSpeedY[1]);
            }
            for (int i = 0; i < dropObj.Length; i++)
            {
                dropObj[i].GetComponent<Animator>().Play("Rock" + Random.Range(0, 4).ToString());
                dropObj[i].transform.position =
                    new Vector3(Random.Range(dropRockPointX[4], dropRockPointX[5]),
                    Random.Range(dropRockPointY[4], dropRockPointY[5] + 500f), 0);
                curDropBigRockSpeedY[i] = Random.Range(dropRockSpeedY[2], dropRockSpeedY[3]);
            }
        }
        if (opt == 2) //harfy 등장 후 초기화
        {
            //위치 초기화
            brokenFieldObj[0].transform.position =
            new Vector3(2116f, brokenFieldObj[0].transform.position.y, brokenFieldObj[0].transform.position.z);
            brokenBackObj[0].transform.position =
            new Vector3(2116f, brokenBackObj[0].transform.position.y, brokenBackObj[0].transform.position.z);
            //필드, 배경 랜덤 이미지
            brokenFieldObj[1].GetComponent<SpriteRenderer>().sprite = brokenFieldSprite[Random.Range(1, 4)];
            brokenBackObj[1].GetComponent<SpriteRenderer>().sprite = brokenBackSprite[Random.Range(1, 4)];
            
            brokenFieldObj[0].GetComponent<SpriteRenderer>().sprite = brokenFieldSprite[Random.Range(1,4)];
            brokenBackObj[0].GetComponent<SpriteRenderer>().sprite = brokenBackSprite[Random.Range(1,4)];
            
            //종유석 위치 초기화
            for (int i = 0; i < dropObj2.Length; i++)
            {
                dropObj2[i].GetComponent<Animator>().Play("Rock" + Random.Range(0, 4).ToString());
                dropObj2[i].transform.position =
                    new Vector3(Random.Range(dropRockPointX[2], dropRockPointX[3]),
                    Random.Range(dropRockPointY[2], dropRockPointY[3]), 0);
                curDropSmallRockSpeedY[i] = Random.Range(dropRockSpeedY[0], dropRockSpeedY[1]);
            }
            for (int i = 0; i < dropObj.Length; i++)
            {
                dropObj[i].GetComponent<Animator>().Play("Rock" + Random.Range(0, 4).ToString());
                dropObj[i].transform.position =
                    new Vector3(Random.Range(dropRockPointX[6], dropRockPointX[7]),
                    Random.Range(dropRockPointY[6], dropRockPointY[7] +1000), 0);
                curDropBigRockSpeedY[i] = Random.Range(dropRockSpeedY[2], dropRockSpeedY[3]);
            }
        }
        if(opt == 3) //field X 넘어갈 경우 초기화
        {
            brokenFieldObj[0].transform.position =
            new Vector3(2116f, brokenFieldObj[0].transform.position.y, brokenFieldObj[0].transform.position.z);
            brokenFieldObj[1].GetComponent<SpriteRenderer>().sprite = brokenFieldObj[0].GetComponent<SpriteRenderer>().sprite;
            brokenFieldObj[0].GetComponent<SpriteRenderer>().sprite = brokenFieldSprite[Random.Range(1, 4)];
        }
        if(opt == 4) //back 넘어갈 경우 초기화
        {
            brokenBackObj[0].transform.position =
            new Vector3(2116f, brokenBackObj[0].transform.position.y, brokenBackObj[0].transform.position.z);
            brokenBackObj[1].GetComponent<SpriteRenderer>().sprite = brokenBackObj[0].GetComponent<SpriteRenderer>().sprite;
            brokenBackObj[0].GetComponent<SpriteRenderer>().sprite = brokenBackSprite[Random.Range(1, 4)];
        }
    }

    public void endingPhase1(int opt)
    {
        if (opt == -1) //다른 페이즈라 비활성화 필요한 경우
        {
            shadowObj[0].SetActive(false);
            shadowObj[1].SetActive(false);

            ailObj.GetComponent<Animator>().Play("Empty");
            jewelObj.GetComponent<Animator>().Play("Empty");
            witchObj.GetComponent<Animator>().Play("Empty");
            //harfyObj.GetComponent<Animator>().Play("Empty");

            jewelObj.transform.position = ailObj.transform.position;
            //harfyObj.transform.position = new Vector3(2750f, -75f, 0f);

            jewelMove = false;
            harfyMove = false;

            return;
        }
        if (opt == 0) //스타트
        {
            shadowObj[0].SetActive(true);
            shadowObj[1].SetActive(true);
            endingPhase2(-1); //ending 2 잔여물 없애기
            harfyTake = 0;

            ailObj.GetComponent<Animator>().Play("ailIdle");
            jewelObj.GetComponent<Animator>().Play("Empty");
            witchObj.GetComponent<Animator>().Play("OnlyGrin");
            harfyObj.GetComponent<Animator>().Play("Black");
            fieldObj.GetComponent<SpriteRenderer>().sprite = fieldSprite[0];
            backgroundObj.GetComponent<SpriteRenderer>().sprite = backSprite[0];
            //harfy랑 drop관련도 초기화도 필요. 씬 전환 후 다시 엔딩 볼때도 보여야 한다는거 유의해서 할것.

            jewelObj.transform.position = ailObj.transform.position;
            harfyObj.transform.position = new Vector3(2750f, -75f, 0f);

            jewelMove = false;
            harfyMove = false;
            whiteMove = false;
            whiteSpeed = 0f;
            whiteFullObj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
        }
        if (opt == 1)// grin이 ail 육체 소환해줌.
        {
            ailObj.GetComponent<Animator>().Play("ailIdle");
            jewelObj.GetComponent<Animator>().Play("Empty");
            witchObj.GetComponent<Animator>().Play("summonAil");
            harfyObj.GetComponent<Animator>().Play("Black");
            fieldObj.GetComponent<SpriteRenderer>().sprite = fieldSprite[0];
            backgroundObj.GetComponent<SpriteRenderer>().sprite = backSprite[0];
            //harfy랑 drop관련도 초기화도 필요. 씬 전환 후 다시 엔딩 볼때도 보여야 한다는거 유의해서 할것.

            jewelObj.transform.position = ailObj.transform.position;
            harfyObj.transform.position = new Vector3(2750f, -75f, 0f);

            jewelMove = false;
            harfyMove = false;
            whiteMove = false;
            whiteSpeed = 0f;
            whiteFullObj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
        }
        if (opt == 2) // ail이 인형이 되고, 보석 빼냄.
        {
            ailObj.GetComponent<Animator>().Play("summonAil");
            jewelObj.GetComponent<Animator>().Play("Create");

            harfyObj.GetComponent<Animator>().Play("Black");
            fieldObj.GetComponent<SpriteRenderer>().sprite = fieldSprite[0];
            backgroundObj.GetComponent<SpriteRenderer>().sprite = backSprite[0];
            //harfy랑 drop관련도 초기화도 필요. 씬 전환 후 다시 엔딩 볼때도 보여야 한다는거 유의해서 할것.

            jewelObj.transform.position = ailObj.transform.position;
            harfyObj.transform.position = new Vector3(2750f, -75f, 0f);

            jewelMove = true;
            harfyMove = false;
            whiteMove = false;
            whiteSpeed = 0f;
            whiteFullObj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);

            //보석 이동 추가 필요

        }
        if (opt == 3)
        { // harfy 원래 위치로
            SoundManager_Sfx.Instance.playSound(72);
            harfyObj.GetComponent<Animator>().Play("Black");
            fieldObj.GetComponent<SpriteRenderer>().sprite = fieldSprite[0];
            backgroundObj.GetComponent<SpriteRenderer>().sprite = backSprite[0];
            //harfy랑 drop관련도 초기화도 필요. 씬 전환 후 다시 엔딩 볼때도 보여야 한다는거 유의해서 할것.

            harfyObj.transform.position = new Vector3(2750f, -75f, 0f);

            harfyMove = true;
            whiteMove = false;
        }
    }
    public void endingPhase2(int opt)
    {
        if(opt == -1) //ending 2 필요하던 것들 없애기
        {

        }
        if (opt == 0)
        {
            endingPhase1(-1);
            whiteMove = false;
            whiteSpeed = whiteChangeVal;
            harfyObj.transform.position = new Vector3(2500f, -75f, 0f);
            harfyObj.GetComponent<Animator>().Play("Idle0");
            harfyPoint = new Vector3(2500f, -75f, 0f); 
            setHarfyTake(1);
        }
        if (opt == 1) //싸움
        {
            harfyObj.GetComponent<Animator>().Play("Fight");
        }
        if(opt == 2) //중앙으로 이동
        {
            setHarfyTake(2);
            harfyObj.GetComponent<Animator>().Play("Idle0");
        }
        if(opt == 3) //grin실망
        {
            harfyObj.GetComponent<Animator>().Play("Idle1");
        }
        if(opt == 4) //harfy 살짝 뒤로
        {
            setHarfyTake(3);
        }
        if(opt == 5) // harfy 앞으로
        {
            whiteMove = true;
            whiteSpeed = whiteChangeVal;
            setHarfyTake(4); 
        }
        if(opt == 6)
        {
            setHarfyTake(5);
        }
    }

    public void setHarfyTake(int opt)
    {
        harfyMoveVal = 0f;
        harfyMoveMoveVal = 0.01f;
        harfyTake = opt;
        if(opt == 1)
        {
            harfyObj.transform.position = Vector3.Lerp(harfyPoint, harfyObj.transform.position, 1f - (harfyMoveVal * harfyMoveVal));
        }
    }

    private IEnumerator creditCoroutine()
    {
        for (int i=0;i<creditText.Length;i++) {
            
            creditTextChange(i);

            setCreditAlphaMode(1); // 하향
            yield return new WaitUntil(() => textAlphaChange == 2);

            yield return new WaitForSeconds(3f);

            setCreditAlphaMode(3); // 상승
            yield return new WaitUntil(() => textAlphaChange == 4);
        }
        endingCreditEnd = 0;
    }

    private void Start()
    {
        onOffEndingSelect(false);
        finishEndingCredit();
    }

    private void FixedUpdate()
    {

        if (textAlphaChange == 1) // 하향
        {
            if (creditObj[5].GetComponent<Image>().color.a > 0f) {
                creditObj[5].GetComponent<Image>().color = new Color
                    (creditObj[5].GetComponent<Image>().color.r,
                    creditObj[5].GetComponent<Image>().color.g,
                    creditObj[5].GetComponent<Image>().color.b,
                    creditObj[5].GetComponent<Image>().color.a - 0.02f);
                if (creditObj[5].GetComponent<Image>().color.a <= 0f)
                {
                    setCreditAlphaMode(2);
                }
            }
        }
        else if (textAlphaChange == 3) // 상승
        {
            if (creditObj[5].GetComponent<Image>().color.a < 1.0f)
            {
                creditObj[5].GetComponent<Image>().color = new Color
                    (creditObj[5].GetComponent<Image>().color.r,
                    creditObj[5].GetComponent<Image>().color.g,
                    creditObj[5].GetComponent<Image>().color.b,
                    creditObj[5].GetComponent<Image>().color.a + 0.02f);
                if (creditObj[5].GetComponent<Image>().color.a >= 1f)
                {
                    setCreditAlphaMode(4);
                }
            }
        }


        if (jewelMove)
        {
            jewelObj.transform.position += new Vector3(1.0f, 0f, 0f);
            if (jewelObj.transform.position.x >= witchObj.transform.position.x - 15f)
            {
                jewelMove = false;
                jewelObj.GetComponent<Animator>().Play("Destroy");
                witchObj.GetComponent<Animator>().Play("AilGrinFix");
            }
        }
        if (harfyMove)
        {
            if (harfyObj.transform.position.x > 2450f)
            {
                harfyObj.transform.position -= new Vector3(20f, 0f, 0f);
            }
            if (harfyObj.transform.position.x < 2550f && !whiteMove)
            {
                whiteMove = true;
                whiteSpeed = whiteChangeVal * 5f;
                whiteFullObj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
                
                harfyMove = false;
                
                harfyObj.GetComponent<Animator>().Play("Black");
                endingManager.Instance.setEndingTiming(true);
            }
        }

        if (harfyTake == 1) // harfy 마구잡이로 이동.
        {
            harfyObj.transform.position = Vector3.Lerp(harfyPoint, harfyObj.transform.position, 1f - (harfyMoveVal * harfyMoveVal));
            harfyMoveVal += harfyMoveMoveVal;
            
            if(harfyMoveVal >= 0.8f) //harfy 위치 변경
            {
                harfyPoint = new Vector3( Random.Range(2470f,2530f), Random.Range(-100f,-50f), harfyObj.transform.position.z);
                harfyMoveVal = 0f;
                harfyMoveMoveVal = Random.Range(0.01f, 0.03f);
            }
        }
        if (harfyTake == 2) //하피 위치 중앙으로
        {
            if (harfyMoveVal < 1.0f)
            {
                harfyPoint = new Vector3(2500f, -75f, 0f);
                harfyObj.transform.position = Vector3.Lerp(harfyPoint, harfyObj.transform.position, 1f - (harfyMoveVal * harfyMoveVal));
                harfyMoveVal += 0.04f;
            }
        }
        if(harfyTake == 3) //하피 잠깐 뒤로
        {
            if (harfyMoveVal < 1.0f)
            {
                harfyPoint = new Vector3(2550f, -75f, 0f);
                harfyObj.transform.position = Vector3.Lerp(harfyPoint, harfyObj.transform.position, 1f - (harfyMoveVal * harfyMoveVal));
                harfyMoveVal += 0.02f;
            }
        }
        if (harfyTake == 4) //하피 빠르게 앞으로
        {
            if (harfyMoveVal < 1.0f)
            {
                harfyPoint = new Vector3(2150f, -75f, 0f);
                harfyObj.transform.position = Vector3.Lerp(harfyPoint, harfyObj.transform.position, 1f - (harfyMoveVal * harfyMoveVal));
                harfyMoveVal += 0.05f;
                if (harfyObj.transform.position.x < 2250f) //벽닿을시 화면 하얗게 
                {
                    whiteMove = true;
                    whiteSpeed = whiteChangeVal * 2f;
                    harfyMove = false;
                    endingPhase1(-1);
                    endingManager.Instance.setEndingTiming(true);
                }
            }
        }
        if (harfyTake == 5) //엔딩 끝나고 하얀 화면 초기화
        {
            whiteMove = false;
            whiteSpeed = 0f;
            whiteFullObj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
        }

        if (!whiteMove && whiteFullObj.GetComponent<SpriteRenderer>().color.a > 0f)
        {
            whiteFullObj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, whiteFullObj.GetComponent<SpriteRenderer>().color.a - whiteSpeed);
        }
        if (whiteMove && whiteFullObj.GetComponent<SpriteRenderer>().color.a < 1f)
        {
            whiteFullObj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, whiteFullObj.GetComponent<SpriteRenderer>().color.a + whiteSpeed);
        }

        if (rockDropState  == 1)
        {
            for(int i = 0; i < dropObj.Length; i++)
            {
                if (curDropBigRockSpeedY[i] > 0f)
                {
                    dropObj[i].transform.position -= new Vector3(0f, curDropBigRockSpeedY[i], 0f);
                    if (dropObj[i].transform.position.y < -250f)
                    {
                        if (whiteFullObj.GetComponent<SpriteRenderer>().color.a == 0f) CameraManager.Instance.attackShakeStart(Random.Range(1f, 3f));
                        curDropBigRockSpeedY[i] = Random.Range(-120f, -300f);
                        dropObj[i].transform.position =
                            new Vector3(dropObj[i].transform.position.x, -110f, dropObj[i].transform.position.z);
                        dropObj[i].GetComponent<Animator>().Play("Boom" + Random.Range(0, 3).ToString());
                    }
                }
                else //폭발시
                {
                    curDropBigRockSpeedY[i] += 1.0f;
                    if (curDropBigRockSpeedY[i] >= 0f) //쿨타임 끝나면 바로 소롼
                    {
                        dropObj[i].transform.position = new Vector3(
                            Random.Range(dropRockPointX[4], dropRockPointX[5]), Random.Range(dropRockPointY[4], dropRockPointY[5]), dropObj[i].transform.position.z);
                        curDropBigRockSpeedY[i] = Random.Range(dropRockSpeedY[2], dropRockSpeedY[3]);
                        dropObj[i].GetComponent<Animator>().Play("Rock" + Random.Range(0, 4).ToString());
                    }
                }
            }

            for (int i = 0; i < dropObj2.Length; i++)
            {
                if (curDropSmallRockSpeedY[i] > 0f) // 낙하 중
                {
                    dropObj2[i].transform.position -= new Vector3(0f, curDropSmallRockSpeedY[i], 0f);
                    if (dropObj2[i].transform.position.y < -52f)
                    {
                        curDropSmallRockSpeedY[i] = Random.Range(-120f, -300f);
                        dropObj2[i].transform.position =
                            new Vector3(dropObj2[i].transform.position.x, -28f, dropObj2[i].transform.position.z);
                        dropObj2[i].GetComponent<Animator>().Play("Boom" + Random.Range(0, 3).ToString());
                    }
                }
                else //폭발중
                {
                    curDropSmallRockSpeedY[i] += 1.0f;
                    if (curDropSmallRockSpeedY[i] >= 0f) //쿨타임 끝날시 새로 소환
                    {
                        
                        dropObj2[i].transform.position = new Vector3(
                            Random.Range(dropRockPointX[0], dropRockPointX[1]), Random.Range(dropRockPointY[0], dropRockPointY[1]), dropObj[i].transform.position.z);
                        curDropSmallRockSpeedY[i] = Random.Range(dropRockSpeedY[0], dropRockSpeedY[1]);
                        dropObj2[i].GetComponent<Animator>().Play("Rock" + Random.Range(0, 4).ToString());
                    }
                }
            }
        }
        if(rockDropState == 2)
        {
            for (int i = 0; i < dropObj.Length; i++)
            {
                if (dropObj[i].transform.position.x < 3500f) dropObj[i].transform.position += new Vector3(dropRockSpeedX[1], 0f, 0f);
                if (curDropBigRockSpeedY[i] > 0f)
                {
                    dropObj[i].transform.position -= new Vector3(0f, curDropBigRockSpeedY[i], 0f);
                    if (dropObj[i].transform.position.y < -250f)
                    {
                        if (whiteFullObj.GetComponent<SpriteRenderer>().color.a <= 0f && harfyTake < 2) CameraManager.Instance.attackShakeStart(Random.Range(1f, 3f));
                        curDropBigRockSpeedY[i] = Random.Range(-120f, -300f);
                        dropObj[i].transform.position =
                            new Vector3(dropObj[i].transform.position.x, -110f, dropObj[i].transform.position.z);
                        dropObj[i].GetComponent<Animator>().Play("Boom" + Random.Range(0, 3).ToString());
                    }
                }
                else //폭발시
                {
                    curDropBigRockSpeedY[i] += 1.0f;
                    if (curDropBigRockSpeedY[i] >= 0f) //쿨타임 끝나면 바로 소롼
                    {
                        
                        dropObj[i].transform.position = new Vector3(
                            Random.Range(dropRockPointX[6], dropRockPointX[7]), Random.Range(dropRockPointY[6], dropRockPointY[7]), dropObj[i].transform.position.z);
                        curDropBigRockSpeedY[i] = Random.Range(dropRockSpeedY[2], dropRockSpeedY[3]);
                        dropObj[i].GetComponent<Animator>().Play("Rock" + Random.Range(0, 4).ToString());
                    }
                }
            }

            for (int i = 0; i < dropObj2.Length; i++)
            {
                if(dropObj2[i].transform.position.x < 3500f) dropObj2[i].transform.position += new Vector3(dropRockSpeedX[0], 0f, 0f);

                if (curDropSmallRockSpeedY[i] > 0f) // 낙하 중
                {
                    dropObj2[i].transform.position -= new Vector3(0f, curDropSmallRockSpeedY[i], 0f);
                    if (dropObj2[i].transform.position.y < -52f)
                    {
                        curDropSmallRockSpeedY[i] = Random.Range(-120f, -300f);
                        dropObj2[i].transform.position =
                            new Vector3(dropObj2[i].transform.position.x, -28f, dropObj2[i].transform.position.z);
                        dropObj2[i].GetComponent<Animator>().Play("Boom_" + Random.Range(0, 3).ToString());
                    }
                }
                else //폭발중
                {
                    curDropSmallRockSpeedY[i] += 1.0f;
                    if (curDropSmallRockSpeedY[i] >= 0f) //쿨타임 끝날시 새로 소환
                    {
                        dropObj2[i].transform.position = new Vector3(
                            Random.Range(dropRockPointX[2], dropRockPointX[3]), Random.Range(dropRockPointY[2], dropRockPointY[3]), dropObj2[i].transform.position.z);
                        curDropSmallRockSpeedY[i] = Random.Range(dropRockSpeedY[0], dropRockSpeedY[1]);
                        dropObj2[i].GetComponent<Animator>().Play("Rock" + Random.Range(0, 4).ToString());
                    }
                }
            }
            brokenFieldObj[0].transform.position += new Vector3(fieldSpeedX,0f,0f);
            if (brokenFieldObj[0].transform.position.x > 2500f) setRockDropState(3);
            brokenBackObj[0].transform.position += new Vector3(backSpeedX, 0f, 0f);
            if (brokenBackObj[0].transform.position.x > 2500f) setRockDropState(4);
        }
    }
}