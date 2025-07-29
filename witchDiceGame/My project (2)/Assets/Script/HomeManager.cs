using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HomeManager : MonoBehaviour
{
    private static HomeManager instance = null;


    private int[] chapterIdx = {6, 1, 2};
    private int[] chapter1Talk = { 24, 27, 30 };
    private int homeSoundIdx = 1;
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
        homeSoundIdx = 1;
        chapterIdx[0] = 6;
        chapterIdx[1] = 1;
        chapterIdx[2] = 2;
    }

    // Update is called once per frame
    void Update()
    {
        
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
                fullUI.showFull("아직 되찾지 못한 이야기입니다!");
            }
        }
        else
        {
            fullUI.showFull("본편에서 개방될 이야기입니다.");
        }
    }
    

    public void enterHome()
    {
        chapterIdx[0] = 6;
        chapterIdx[1] = 1;
        chapterIdx[2] = 2;
        //CameraManager.Instance.zoomEvent();
        CameraManager.Instance.updateInitPosition(new Vector3(-1000f, -1000f, CameraManager.Instance.cameraPointZ()));
        SoundManager_Main.Instance.playSound(homeSoundIdx);
        SoundManager_Main.Instance.stopSound(0);
        SoundManager_Main.Instance.stopSound(7);

    }
    
    public void exitHome() {
        TownManager.Instance.backToTownUI();
        SoundManager_Main.Instance.stopSound(homeSoundIdx);
    }
}
