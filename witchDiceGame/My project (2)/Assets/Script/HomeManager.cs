using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HomeManager : MonoBehaviour
{
    private static HomeManager instance = null;

    [SerializeField]
    public GameObject[] jewel0 = new GameObject[3];

    [SerializeField]
    public GameObject[] jewel1 = new GameObject[3];

    [SerializeField]
    public GameObject[] jewel2 = new GameObject[3];

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


    private TextMeshPro textBoxText;
    private List<DescReader> homeNPCText = new List<DescReader>();

    private int[] chapterIdx = { 6, 1, 2 };
    private int[,] chapterTalkBefore = { { 0,0,0}, { 23, 26, 29 }, { 0, 0, 0 } };
    private int[,] chapterTalk = { { 0,0,0},{ 24, 27, 30 }, { 0, 0, 0 } };
    private int[,] chapterTalkAfter = { { 0,0,0},{ 25, 28, 31 }, { 0,0,0} };
    private int[] chapterClear = { 0,19,0 };

    private int homeSoundIdx = 19;
    private float textBoxTimer = 0f;
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
        homeNPCText = CSVReader.Read<DescReader>("HomeDesc");
        for (int i=0;i<homeNPCText.Count;i++)
        {
            homeNPCText[i].KR = TalkManager.Instance.SpecialTextChange(homeNPCText[i].KR);
            homeNPCText[i].JP = TalkManager.Instance.SpecialTextChange(homeNPCText[i].JP);
            homeNPCText[i].EN = TalkManager.Instance.SpecialTextChange(homeNPCText[i].EN);
        }

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
    }


    public string getDesc(int idx)
    {
        if (jsonDataManager.Instance.getLanguage() == 0) return homeNPCText[idx].KR;
        if (jsonDataManager.Instance.getLanguage() == 0) return homeNPCText[idx].EN;
        if (jsonDataManager.Instance.getLanguage() == 0) return homeNPCText[idx].JP;
        return homeNPCText[idx].EN;
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
        if (num >= 0 && num < 3) { // 데모에선 1챕터만 봐야하니 다른거 막아두기
            int chapterNum = 1;//chapterIdx[num / 3];
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
        else
        {
            //SoundManager_Main.Instance.stopSound(homeSoundIdx);
            //TalkManager.Instance.startTalk(13);
            fullUI.showFull(5);
        }
        updateJewelImage();
    }
    

    public void enterHome()
    {
        textBoxTimer = 0.0f;
        
        chapterIdx[0] = 6;
        chapterIdx[1] = 1;
        chapterIdx[2] = 2;
        //CameraManager.Instance.zoomEvent();
        CameraManager.Instance.updateInitPosition(new Vector3(-1000f, -1000f, CameraManager.Instance.cameraPointZ()));
        //SoundManager_Main.Instance.playSound(homeSoundIdx);
        //SoundManager_Main.Instance.stopSound(0);
        //SoundManager_Main.Instance.stopSound(7);
        updateJewelImage();
    }
    
    private void updateJewelImage()
    {
        for (int i=0;i<3;i++)
        {
            if (jsonDataManager.Instance.getChapterRead(1, i) >= 1) { 
                jewel1[i].GetComponent<SpriteRenderer>().sprite = jewelSprite[2*i]; 
            }
            else { jewel1[i].GetComponent<SpriteRenderer>().sprite = jewelSprite[2 * i + 1]; };

            if (jsonDataManager.Instance.getChapterRead(1, i) == 1) { 
                newMark[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_newMark");
            }
            else
            {
                newMark[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/empty_0");
            }
        }
    }

    public void exitHome() {
        TownManager.Instance.backToTownUI();
        SoundManager_Main.Instance.stopSound(homeSoundIdx);
    }
}
