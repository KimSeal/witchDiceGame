using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HomeManager : MonoBehaviour
{
    private static HomeManager instance = null;

    [SerializeField]
    public GameObject[] jewel1 = new GameObject[3];

    [SerializeField] public GameObject[] newMark = new GameObject[3];

    [SerializeField]
    public Sprite[] jewelSprite = new Sprite[6];

    [SerializeField]
    public GameObject textBox;

    [SerializeField]
    public GameObject textBoxTrue;

    [SerializeField]
    public GameObject homeNPC;

    [SerializeField]
    public Sprite[] homeNPCSprite = new Sprite[6];

    [SerializeField]
    public GameObject[] chapterDiceObject = new GameObject[6];
    public GameObject[] chapterDiceNewMark = new GameObject[6];
    public Sprite[] diceSpriteOn = new Sprite[6];
    public Sprite[] diceSpriteOff = new Sprite[6];

    public GameObject boardObject;
    private float boardYVal;

    public GameObject titleObj;

    private TextMeshPro textBoxText;
    private List<DescReader> homeNPCText = new List<DescReader>();

    private int[] chapterIdx = { 6, 1, 2 };
    private int[,] chapterTalkBefore = { { 23, 26, 29 }, { 0, 0, 0 } };
    private int[,] chapterTalk = { { 24, 27, 30 }, { 0, 0, 0 } };
    private int[,] chapterTalkAfter = {{ 25, 28, 31 }, { 0,0,0} };
    private int[] chapterClear = { 19,0 };

    private int homeSoundIdx = 19;
    private float textBoxTimer = 0f;

    private int curChapterIdx;

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

    public static HomeManager Instance
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
        textBoxText = textBoxTrue.GetComponent<TextMeshPro>();
        textBox.SetActive(false);
        homeSoundIdx = 19;
        chapterIdx[0] = 6;
        chapterIdx[1] = 1;
        chapterIdx[2] = 2;
        boardYVal = 1.0f;
        homeNPCText = CSVReader.Read<DescReader>("HomeDesc");
        for (int i=0;i<homeNPCText.Count;i++)
        {
            homeNPCText[i].KR = TalkManager.Instance.SpecialTextChange(homeNPCText[i].KR);
            homeNPCText[i].JP = TalkManager.Instance.SpecialTextChange(homeNPCText[i].JP);
            homeNPCText[i].EN = TalkManager.Instance.SpecialTextChange(homeNPCText[i].EN);
        }
        curChapterIdx = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (textBox.activeSelf && textBoxTimer > 0.0f) {
            textBoxTimer -= 0.1f;
        }
        else
        {
            textBoxTimer = 0.0f;
            textBox.SetActive(false);
            homeNPC.GetComponent<SpriteRenderer>().sprite = homeNPCSprite[0];
            textBoxText.text = "";
        }

        if (boardYVal < 1.0f) {

            titleObj.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, boardYVal * boardYVal);
            
            for (int i=0;i<6;i++)
            {
                chapterDiceObject[i].transform.position = 
                    Vector3.Lerp(new Vector3(chapterDiceObject[i].transform.position.x, -950f, chapterDiceObject[i].transform.position.z),
                new Vector3(chapterDiceObject[i].transform.position.x, -979f, chapterDiceObject[i].transform.position.z),
                boardYVal * boardYVal);
            }
            boardObject.transform.position = Vector3.Lerp(new Vector3(boardObject.transform.position.x, -1100, boardObject.transform.position.z),
                new Vector3(boardObject.transform.position.x, -1067, boardObject.transform.position.z),
                boardYVal * boardYVal);
            boardYVal += addVal;
            if (boardYVal >= 1.0f)
            {
                SoundManager_Sfx.Instance.playSound(3);

                for (int i = 0; i < 6; i++) {
                    chapterDiceObject[i].GetComponent<hoverRotate>().shakeVal = ((float)Random.Range(0, 1) - 0.5f) ;
                    chapterDiceObject[i].GetComponent<hoverRotate>().shakeStart();
                }
                for (int i = 0; i < 3; i++) jewel1[i].GetComponent<hoverRotate>().shakeStart();
                titleObj.GetComponent<hoverRotate>().shakeStart();

                titleObj.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 1f);
                for (int i = 0; i < 6; i++)
                {
                    chapterDiceObject[i].transform.position = new Vector3(chapterDiceObject[i].transform.position.x, -979f, chapterDiceObject[i].transform.position.z);
                }
                boardObject.transform.position = new Vector3(boardObject.transform.position.x, -1067, boardObject.transform.position.z);
            }
        }

    }


    public string getDesc(int idx)
    {
        if (jsonDataManager.Instance.getLanguage() == 0) return homeNPCText[idx].KR;
        if (jsonDataManager.Instance.getLanguage() == 0) return homeNPCText[idx].EN;
        if (jsonDataManager.Instance.getLanguage() == 0) return homeNPCText[idx].JP;
        return homeNPCText[idx].EN;
    }

    public void hoverInDice(int idx)
    {
        chapterDiceObject[idx].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
    }
    public void hoverOutDice()
    {
        for (int i=0;i<6;i++)
        {
            if (curChapterIdx == i) {
                chapterDiceObject[i].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
            }
            else
            {
                chapterDiceObject[i].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
            }
        }
    }
    public void clickDice(int idx)
    {
        curChapterIdx = idx;
        for (int i = 0; i < 3; i++) jewel1[i].GetComponent<hoverRotate>().shakeStart();
        chapterDiceObject[idx].GetComponent<hoverRotate>().shakeStart();
        hoverOutDice();
        updateJewelImage();
    }

    public void clickNpc()
    {
        textBox.SetActive(true);
        int randomVal = Random.Range(1, 6);
        homeNPC.GetComponent<SpriteRenderer>().sprite = homeNPCSprite[randomVal];
        textBoxText.text = getDesc(randomVal);
        textBoxTimer = 12f;
    }
    IEnumerator jewelTalk(int talkIdx)
    {
        Debug.Log("Item is false 0 ");
        SoundManager_Main.Instance.stopSound(homeSoundIdx);
        TalkManager.Instance.startTalk(talkIdx);
        yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
        Debug.Log("Item is false 2");
        FadeUIScript.fadeIn();
        SoundManager_Main.Instance.playSound(homeSoundIdx);
    }

    IEnumerator jewelTalk(int chapterNum, int detailIdx)
    {
        Debug.Log("Item is true 0 ");
        SoundManager_Main.Instance.stopSound(homeSoundIdx);

        TalkManager.Instance.startTalk(chapterTalkBefore[chapterNum, detailIdx]);
        yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
        Debug.Log("Item is true 1 ");
        TalkManager.Instance.startTalk(chapterTalk[chapterNum, detailIdx]);
        yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
        Debug.Log("Item is true 2 ");
        TalkManager.Instance.startTalk(chapterTalkAfter[chapterNum, detailIdx]);
        yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
        Debug.Log("Item is true 3 ");
        if (detailIdx == 2) {
            TalkManager.Instance.startTalk(chapterClear[chapterNum]);
            yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
        }

        FadeUIScript.fadeIn();
        SoundManager_Main.Instance.playSound(homeSoundIdx);
    }



    public void clickJewel(int num)
    {
        if (curChapterIdx != 0)// 데모에선 1챕터만 봐야하니 다른거 막아두기
        {
            fullUI.showFull(4);
            return;
        }
        if (num >= 0 && num < 3) {
            int chapterNum = curChapterIdx;
            int detailIdx = num % 3;
            Debug.Log("jewel test");
            Debug.Log(jsonDataManager.Instance.getChapterRead(chapterNum, detailIdx));
            if (jsonDataManager.Instance.getChapterRead(chapterNum, detailIdx) == 2) {//스토리 진행된 부분이라면 틀어주기.
                StartCoroutine(jewelTalk(chapterTalk[chapterNum,detailIdx]));
            }
            else if (jsonDataManager.Instance.getChapterRead(chapterNum, detailIdx) == 1)
            {//스토리 진행된 부분이라면 틀어주기.
                if (detailIdx > 0 && jsonDataManager.Instance.getChapterRead(chapterNum, detailIdx - 1) < 2) //이전 스토리를 읽지 않았다면 읽을 수 없도록.
                {
                    fullUI.showFull(50);
                }
                else
                {
                    StartCoroutine(jewelTalk(chapterNum, detailIdx));
                    jsonDataManager.Instance.setChapterRead(chapterNum, detailIdx);
                }
            }
            else
            {
                
                fullUI.showFull(4);
            }
        }
        updateJewelImage();
        upDownManager.Instance.hoverOutUnderTownButton();
    }
    

    public void enterHome()
    {
        if (!jsonDataManager.Instance.getHomeMeet())
        {
            TalkManager.Instance.startTalk(51);
            jsonDataManager.Instance.HomeMeet();
        }
        textBoxTimer = 0.0f;
        
        chapterIdx[0] = 6;
        chapterIdx[1] = 1;
        chapterIdx[2] = 2;
        boardYVal = 0f;
        
        //CameraManager.Instance.zoomEvent();
        CameraManager.Instance.updateInitPosition(new Vector3(-1000f, -1000f, CameraManager.Instance.cameraPointZ()));
        //SoundManager_Main.Instance.playSound(homeSoundIdx);
        //SoundManager_Main.Instance.stopSound(0);
        //SoundManager_Main.Instance.stopSound(7);
        updateJewelImage();
        hoverOutDice();
        
    }
    [SerializeField]
    public float addVal = 0.01f;

    private void updateJewelImage()
    {
        for (int i=0;i<3;i++)
        {
            if (jsonDataManager.Instance.getChapterRead(curChapterIdx, i) >= 1) { 
                jewel1[i].GetComponent<SpriteRenderer>().sprite = jewelSprite[2*i]; 
            }
            else { jewel1[i].GetComponent<SpriteRenderer>().sprite = jewelSprite[2 * i + 1]; };

            if (jsonDataManager.Instance.getChapterRead(curChapterIdx, i) == 1) { 
                newMark[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_newMark");
            }
            else
            {
                newMark[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            }
        }
        for (int i=0;i<6;i++)
        {
            if (jsonDataManager.Instance.getChapterRead(i, 0) == 1 ||
                jsonDataManager.Instance.getChapterRead(i, 1) == 1 ||
                jsonDataManager.Instance.getChapterRead(i, 2) == 1 )
            {
                chapterDiceNewMark[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_newMark");
            }
            else
            {
                chapterDiceNewMark[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            }

            if (jsonDataManager.Instance.getChapterRead(i, 0) == 0 &&
                jsonDataManager.Instance.getChapterRead(i, 1) == 0 &&
                jsonDataManager.Instance.getChapterRead(i, 2) == 0)
            {
                chapterDiceObject[i].GetComponent<SpriteRenderer>().sprite = diceSpriteOff[i];
            }
            else
            {
                chapterDiceObject[i].GetComponent<SpriteRenderer>().sprite = diceSpriteOn[i];
            }
        }
    }

    public void exitHome() {
        boardYVal = 1.1f;
        TownManager.Instance.backToTownUI();
        SoundManager_Main.Instance.stopSound(homeSoundIdx);
    }
}
