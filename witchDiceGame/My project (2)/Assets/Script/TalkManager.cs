using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TalkManager : MonoBehaviour
{

    //ΩÃ≈¨≈Ê
    private static TalkManager instance = null;
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
    public static TalkManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }
    private GameObject entity;
    private GameObject[] characterImage = new GameObject[2];
    private GameObject characterName;
    private GameObject characterTalk;

    private List<TalkReader> talkList = new List<TalkReader>();
    private Material[] material = new Material[2];
    private int curIdx = 0;
    private int curTalk = 0;
    private bool talkingChk = false;
    // Start is called before the first frame update
    void Start()
    {
        talkList = CSVReader.Read<TalkReader>("Talk");

        entity = GameObject.Find("ui_communicate");
        characterImage[0] = GameObject.Find("ui_communicate_character_left");
        characterImage[1] = GameObject.Find("ui_communicate_character_right");
        characterName = GameObject.Find("ui_communicate_name");
        characterTalk = GameObject.Find("ui_communicate_talk");

        material[0] = characterImage[0].GetComponent<Image>().material;
        material[1] = characterImage[1].GetComponent<Image>().material;

        entity.SetActive(false);
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }
    public void startTalk(int a)
    {
        if (!talkingChk)
        {
            entity.SetActive(true);
            curIdx = a;
            curTalk = talkList[a].talkIdx;
            talkingChk = true;
            printTalk(curIdx);
        }
    }
    public void goToNextTalk()
    {
        if (talkingChk)
        {
            if (talkList[curIdx].talkIdx != talkList[curIdx + 1].talkIdx)
            {
                stopTalk();
            }
            else
            {
                curIdx++;
                printTalk(curIdx);
            }
        }
    }
    public void printTalk(int a)
    {
        characterImage[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterTalkStand/spr_stand_" + talkList[a].characterLeft + "_" + talkList[a].characterLeftFace);
        characterImage[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterTalkStand/spr_stand_" + talkList[a].characterRight + "_" + talkList[a].characterRightFace);

        if (talkList[a].brightCharacter == 0 || talkList[a].brightCharacter == 2) material[0].SetFloat("_Transparency", 0.7f);
        else material[0].SetFloat("_Transparency", 0.0f);

        if (talkList[a].brightCharacter == 0 || talkList[a].brightCharacter == 1) material[1].SetFloat("_Transparency", 0.7f);
        else material[1].SetFloat("_Transparency", 0.0f);
        Debug.Log(talkList[a].Name);
        characterName.GetComponent<TextMeshProUGUI>().text = talkList[a].Name;
        characterTalk.GetComponent<TextMeshProUGUI>().text = talkList[a].Text;
    }
    private void stopTalk()
    {
        if (talkingChk)
        {
            characterImage[0].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/spr_characterEmpty");
            characterImage[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/spr_characterEmpty");

            material[0].SetFloat("_Transparency", 0.7f);
            material[1].SetFloat("_Transparency", 0.7f);

            characterName.GetComponent<TextMeshProUGUI>().text = "";
            characterTalk.GetComponent<TextMeshProUGUI>().text = "";
            entity.SetActive(false);
            talkingChk = false;
        }
    }
}
