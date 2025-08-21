using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour
{
    [SerializeField]
    public GameObject[] cloudObj = new GameObject[4];
    
    [SerializeField]
    public GameObject[] townText = new GameObject[4];

    private static TownManager instance = null;
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

    public static TownManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }

    GameObject clickAndImageChange;

    private void shakeTownText(int idx)
    {
        townText[idx].GetComponent<hoverRotate>().shakeStart();
    }
    public void clickTownUI(int i)
    {
        SoundManager_Sfx.Instance.playSound(0);
        //0 : 타워 1 : 집 2: 도서관 3: 마을
        if (i == 0)
        {
            if (!jsonDataManager.Instance.getTowerMeet())
            {
                jsonDataManager.Instance.towerMeet();
                TalkManager.Instance.startTalk(17);
            }
            for (int cloudIdx = 0; cloudIdx < cloudObj.Length; cloudIdx++) cloudObj[cloudIdx].GetComponent<cloudMove>().cloudStop();
            AdventureReadyManager.Instance.enterAdventureReady();
            // clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_tower_on");
        }
        if (i == 1)
        {
            HomeManager.Instance.enterHome();
            //SoundManager_Main.Instance.stopSound(7);
            //fullUI.showFull("데모에선 막힌 구간입니다!\n본편을 기대해주세요!");
            //clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_home_on");
        }
        if (i == 2)
        {
            if (!jsonDataManager.Instance.getLibraryMeet())
            {
                jsonDataManager.Instance.libraryMeet();
                TalkManager.Instance.startTalk(1);
            }
            for (int cloudIdx = 0; cloudIdx < cloudObj.Length; cloudIdx++) cloudObj[cloudIdx].GetComponent<cloudMove>().cloudStop();
            CameraManager.Instance.updateInitPosition(new Vector3(-1500f, 0f, CameraManager.Instance.cameraPointZ()));
            LibraryManager.Instance.enterLibrary(0);
            SoundManager_Main.Instance.stopSound(0);
        }
        if (i == 3)
        {
            //SoundManager_Main.Instance.stopSound(7);
            fullUI.showFull(6);
            clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_town_on");
        }
    }
    public void hoverInUIBtn(int i)
    {
        //0 : 타워 1 : 집 2: 도서관 3: 마을
        shakeTownText(i);

        if (i == 0) clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_tower_on");
        if (i == 1) clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_home_on");
        if (i == 2) clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_library_on");
        if (i == 3) clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_town_on");
    }
    public void hoverOutUIBtn()
    {
        clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
    }
    public void backToTownUI()
    {
        for (int i = 0; i < cloudObj.Length; i++) cloudObj[i].GetComponent<cloudMove>().cloudActive();
        SoundManager_Sfx.Instance.playSound(0);
        SoundManager_Main.Instance.playSound(7);
        CameraManager.Instance.updateInitPosition(new Vector3(-500f, -500f, CameraManager.Instance.cameraPointZ()));
        
    }
    public void backToMain()
    {
        SoundManager_Sfx.Instance.playSound(0);
        CameraManager.Instance.updateInitPosition(new Vector3(-1500f, -500f, CameraManager.Instance.cameraPointZ()));
        SoundManager_Main.Instance.stopSound(7);
        SoundManager_Main.Instance.playSound(0);
    }
    // Start is called before the first frame update
    void Start()
    {
        //Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        //Screen.SetResolution(960, 540, FullScreenMode.Windowed);
        //Screen.SetResolution(960, 540, FullScreenMode.Windowed);
        clickAndImageChange = transform.Find("spr_town_home_click").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
