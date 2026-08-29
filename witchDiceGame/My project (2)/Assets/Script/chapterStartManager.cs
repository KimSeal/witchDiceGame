using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class chapterStartManager : MonoBehaviour
{
    private static chapterStartManager instance = null;

    public GameObject entity;
    public GameObject blackCover;
    public GameObject[] objectArr = new GameObject[8];
    public GameObject[] objectOriginArr = new GameObject[8];

    public TextMeshProUGUI[] chapterStartText = new TextMeshProUGUI[2];

    private bool chapter5move = false;

    public string[] chapterTitle = new string[7];

    [SerializeField]
    public int startChapterIdx = 0;
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
    public static chapterStartManager Instance
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
        chapterStartChk = false;
        entity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -3000f, 0);
        chapterTitle[0] = "Lost People";
        chapterTitle[1] = "Expedient";
        chapterTitle[2] = "Liar";

        chapterTitle[3] = "Crack";
        chapterTitle[4] = "Push Back";
        chapterTitle[5] = "Regret";
        chapterTitle[6] = "Last Time";
    }

    private void FixedUpdate()
    {
        if (chapter5move)
        {
            objectArr[0].GetComponent<RectTransform>().anchoredPosition =
                objectArr[0].GetComponent<RectTransform>().anchoredPosition * 0.98f + 
                new Vector2(-30f, objectArr[0].GetComponent<RectTransform>().anchoredPosition.y) * 0.02f;
            if (objectArr[0].GetComponent<RectTransform>().anchoredPosition.x > -35)
            {
                objectOriginArr[0].GetComponent<Animator>().Play("WitchIdle");
                
                objectArr[0].GetComponent<Image>().sprite = objectOriginArr[0].GetComponent<SpriteRenderer>().sprite;
                chapter5move = false;
            }
        }
        if (getChapterStartEnd())
        {
            for (int i = 0; i < objectArr.Length; i++)
            {
                objectArr[i].GetComponent<Image>().sprite = objectOriginArr[i].GetComponent<SpriteRenderer>().sprite;
                objectArr[i].GetComponent<RectTransform>().sizeDelta = new Vector2(
                objectOriginArr[i].GetComponent<SpriteRenderer>().sprite.bounds.size.x,
               objectOriginArr[i].GetComponent<SpriteRenderer>().sprite.bounds.size.y);
            }
        }
       
    }
    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            startChater(startChapterIdx);
        }
    }
    public void setAnim(int objIdx, string animName, Vector3 vectorTemp)
    {
        objectOriginArr[objIdx].GetComponent<Animator>().runtimeAnimatorController =
            Resources.Load<RuntimeAnimatorController>("sprite/ChapterStart/" + animName + "/animator_" + animName);
        objectArr[objIdx].GetComponent<Image>().sprite = objectOriginArr[objIdx].GetComponent<SpriteRenderer>().sprite;
        objectArr[objIdx].GetComponent<RectTransform>().anchoredPosition = vectorTemp;

        

    }
    public void changeAnim(int objIdx, string animType)
    {
        objectOriginArr[objIdx].GetComponent<Animator>().Play(animType);
        objectArr[objIdx].GetComponent<Image>().sprite = objectOriginArr[objIdx].GetComponent<SpriteRenderer>().sprite;
    }
    public void initPos()
    {
        chapter5move = false;
        chapterStartText[0].text = "";
        chapterStartText[1].text = "";
        for (int i=0;i<objectArr.Length;i++)
        {
            objectArr[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(0,-3000, 0);
        }
    }
    public void startChater(int chapterNum)
    {
        chapterStartChk = true;
        StartCoroutine(chapterStartCoroutine(chapterNum));
    }
    public IEnumerator chapterStartCoroutine(int opt)
    {
        initPos();
        if(opt == 0)
        {
            setAnim(0, "sitAil", new Vector3(-35, -56, 0));
            setAnim(1, "fire", new Vector3(-3, -56, 0));
            setAnim(2, "Portal", new Vector3(39, -36, 0));
            setAnim(3, "witch0", new Vector3(39, -38, 0));
            changeAnim(0, "Idle");
            changeAnim(1, "Idle");
            changeAnim(2, "Idle");
            changeAnim(3, "Idle");
        }
        if (opt == 1)
        {
            setAnim(0, "sitDoll", new Vector3(-1, -56, 0));
            setAnim(1, "witch1", new Vector3(-55, -41, 0));
            setAnim(2, "flutter0", new Vector3(53, -54, 0));
            setAnim(3, "boom0", new Vector3(-3, -52, 0));
            changeAnim(0, "Idle");
            changeAnim(1, "Idle");
            changeAnim(2, "Idle");
            changeAnim(3, "Idle");
        }
        if (opt == 2)
        {
            setAnim(0, "witch1", new Vector3(80, -41, 0));
            setAnim(1, "standAil", new Vector3(55, -55, 0));
            setAnim(2, "flutter0", new Vector3(-96, -53, 0));
            setAnim(3, "Odi", new Vector3(-4, -41, 0));
            setAnim(4, "UnstablePortal", new Vector3(-48, -45, 0));
            changeAnim(0, "LeftWatchIdle");
            changeAnim(1, "Idle");
            changeAnim(2, "TimeStop");
            changeAnim(3, "Idle");
            changeAnim(4, "Idle");
        }
        if (opt == 3)
        {
            setAnim(0, "brokenAil", new Vector3(0, -41, 0));
            setAnim(1, "brokenAil", new Vector3(-55, -41, 0));
            setAnim(2, "brokenAil", new Vector3(55, -41, 0));
            changeAnim(0, "Idle");
            changeAnim(1, "BrokenBody");
            changeAnim(2, "BrokenArm");
        }
        if (opt == 4)
        {
            setAnim(0, "Chapter5", new Vector3(-70, -41, 0));
            setAnim(1, "Chapter5", new Vector3(-30, -53, 0));
            setAnim(2, "Chapter5", new Vector3(30, -41, 0));
            setAnim(3, "Chapter5", new Vector3(93, -41, 0));
            changeAnim(0, "AilIdle");
            changeAnim(1, "FlutterIdle");
            changeAnim(2, "WitchIdle");
            changeAnim(3, "ThingIdle");
        }
        if (opt == 5)
        {
            setAnim(0, "Chapter6", new Vector3(-100, -41, 0));
            setAnim(1, "Chapter6", new Vector3(35, -41, 0));
            setAnim(2, "Chapter6", new Vector3(35, -41, 0));
            changeAnim(0, "WitchBlack");
            changeAnim(1, "Body");
            changeAnim(2, "GreretOpenIdle");
        }
        if(opt == 6)
        {
            setAnim(0, "OdiBed", new Vector3(0f, -53f, 0f));
        }
        blackCover.GetComponent<Image>().color = new Color(blackCover.GetComponent<Image>().color.r,
               blackCover.GetComponent<Image>().color.g,
               blackCover.GetComponent<Image>().color.b,
               1);
        entity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
        while (blackCover.GetComponent<Image>().color.a > 0) {
            blackCover.GetComponent<Image>().color = new Color(blackCover.GetComponent<Image>().color.r,
                blackCover.GetComponent<Image>().color.g,
                blackCover.GetComponent<Image>().color.b,
                blackCover.GetComponent<Image>().color.a - 0.01f);
            chapterStartText[0].text = ("Chapter " + (opt + 1).ToString()).Substring(0, (int)(8 * (1 - blackCover.GetComponent<Image>().color.a)));
            chapterStartText[1].text = "-" + chapterTitle[opt].Substring(0, (int)(chapterTitle[opt].Length * (1 - blackCover.GetComponent<Image>().color.a))) + "-";
            yield return new WaitForSeconds(0.01f);
        }
        chapterStartText[0].text = ("Chapter " + (opt + 1).ToString());
        chapterStartText[1].text = "-" + chapterTitle[opt] + "-";

        if (opt == 0)
        {
            yield return new WaitForSeconds(1.5f);
            changeAnim(0, "WatchRight");
            changeAnim(3, "WatchLeft");
            yield return new WaitForSeconds(1.5f);
            changeAnim(0, "Surprise");
            changeAnim(3, "Smile");
            yield return new WaitForSeconds(1f);
            changeAnim(0, "WatchLeft");
            yield return new WaitForSeconds(1f);
        }
        if (opt == 1)
        {
            yield return new WaitForSeconds(1.5f);
            changeAnim(1, "Spell");
            yield return new WaitForSeconds(0.5f);
            changeAnim(0, "Spell");
            yield return new WaitForSeconds(1.5f);
            changeAnim(1, "SpellFail");
            changeAnim(0, "Burn");
            changeAnim(2, "Surprise");
            changeAnim(3, "Boom");
            yield return new WaitForSeconds(1f);
            changeAnim(2, "Idle2");
            yield return new WaitForSeconds(1f);
        }
        if (opt == 2)
        {
            yield return new WaitForSeconds(1.5f);
            changeAnim(0, "LeftWatchSurprise");
            changeAnim(3, "FocusAngry");
            changeAnim(4, "Make");
            yield return new WaitForSeconds(1.5f);
            changeAnim(0, "LeftWatchNoise");
            changeAnim(1, "Surprise");
            changeAnim(3, "FocusSpin");
            changeAnim(4, "Boom");
            yield return new WaitForSeconds(3f);
        }
        if (opt == 3)
        {
            yield return new WaitForSeconds(1f);
            changeAnim(0, "Wake");
            yield return new WaitForSeconds(1.5f);
            changeAnim(0, "WakeEnd");
            yield return new WaitForSeconds(1.5f);
            changeAnim(0, "Fall");
            yield return new WaitForSeconds(2f);
        }
        if (opt == 4)
        {
            yield return new WaitForSeconds(2f);
            changeAnim(0, "AilIdle2");
            changeAnim(1, "FlutterSad");
            changeAnim(2, "WitchSurprise");
            changeAnim(3, "ThingFall");
            yield return new WaitForSeconds(1.5f);
        }
        if (opt == 5)
        {
            yield return new WaitForSeconds(1f);
            changeAnim(0, "WitchWalkBlack");
            changeAnim(2, "GreretOpen");
            chapter5move = true;
            yield return new WaitForSeconds(3f);
            changeAnim(2, "GreretSmile");
            yield return new WaitForSeconds(1f);
        }
        if (opt == 6)
        {
            yield return new WaitForSeconds(3f);
        }
        while (blackCover.GetComponent<Image>().color.a < 1)
        {
            blackCover.GetComponent<Image>().color = new Color(blackCover.GetComponent<Image>().color.r,
                blackCover.GetComponent<Image>().color.g,
                blackCover.GetComponent<Image>().color.b,
                blackCover.GetComponent<Image>().color.a + 0.01f);
            yield return new WaitForSeconds(0.01f);
        }
        chapterStartChk = false;
        entity.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -3000f, 0);
    }
    public bool chapterStartChk = false;
    public bool getChapterStartEnd()
    {
        return chapterStartChk;
    }
}
