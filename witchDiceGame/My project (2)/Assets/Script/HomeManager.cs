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

    [SerializeField]
    public Sprite [] jewelSprite = new Sprite[6];

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

    private int[] chapterIdx = {6, 1, 2};
    private int[] chapter1Talk = { 24, 27, 30 };
    private int homeSoundIdx = 1;
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
        homeSoundIdx = 1;
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
        SoundManager_Main.Instance.stopSound(homeSoundIdx);
        TalkManager.Instance.startTalk(talkIdx);
        yield return new WaitUntil(() => !TalkManager.Instance.getTalkChk());
        FadeUIScript.fadeIn();
        SoundManager_Main.Instance.playSound(homeSoundIdx);
    }
    public void clickJewel(int num)
    {
        if (num >= 3 && num < 6) { // 데모에선 1챕터만 봐야하니 다른거 막아두기
            int chapterNum = chapterIdx[num / 3];
            int detailIdx = num % 3;
            if (jsonDataManager.Instance.getChapterRead(chapterNum, detailIdx) == 2) {//스토리 진행된 부분이라면 틀어주기.
                StartCoroutine(jewelTalk(chapter1Talk[detailIdx]));
            }
            else
            {
                
                fullUI.showFull(4);
            }
        }
        else
        {
            TalkManager.Instance.startTalk(31);
            fullUI.showFull(5);
        }
    }
    

    public void enterHome()
    {
        textBoxTimer = 0.0f;
        
        chapterIdx[0] = 6;
        chapterIdx[1] = 1;
        chapterIdx[2] = 2;
        //CameraManager.Instance.zoomEvent();
        CameraManager.Instance.updateInitPosition(new Vector3(-1000f, -1000f, CameraManager.Instance.cameraPointZ()));
        SoundManager_Main.Instance.playSound(homeSoundIdx);
        SoundManager_Main.Instance.stopSound(0);
        SoundManager_Main.Instance.stopSound(7);
        updateJewelImage();
    }
    
    private void updateJewelImage()
    {
        for (int i=0;i<3;i++)
        {
            if (jsonDataManager.Instance.getChapterRead(1, i) == 2) { jewel1[i].GetComponent<SpriteRenderer>().sprite = jewelSprite[2*i]; }
            else { jewel1[i].GetComponent<SpriteRenderer>().sprite = jewelSprite[2 * i + 1]; };
        }
    }

    public void exitHome() {
        TownManager.Instance.backToTownUI();
        SoundManager_Main.Instance.stopSound(homeSoundIdx);
    }
}
