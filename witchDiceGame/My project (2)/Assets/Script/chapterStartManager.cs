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

    public string[] chapterTitle = new string[7];
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
    }

    private void FixedUpdate()
    {
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
            startChater(2);
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
            yield return new WaitForSeconds(2f);
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
            yield return new WaitForSeconds(2f);
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
            yield return new WaitForSeconds(2f);
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
