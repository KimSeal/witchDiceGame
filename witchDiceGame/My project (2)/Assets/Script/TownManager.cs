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

    [SerializeField]GameObject clickAndImageChange;

    public int curTownIdx = 0;
    public int[] townSound = new int[8];
    public bool townActive = false;

    public GameObject[] townNewMark = new GameObject[7];

    public bool getTownNewMark(int idx)
    {
        if (idx == 0 && !jsonDataManager.Instance.getTowerMeet()) { //tower
            return true;
        }
        if (idx == 1 &&
            (jsonDataManager.Instance.getChapterRead(0,0) == 1 || jsonDataManager.Instance.getChapterRead(0, 1) == 1 || jsonDataManager.Instance.getChapterRead(0, 2) == 1 ||
            jsonDataManager.Instance.getChapterRead(1, 0) == 1 || jsonDataManager.Instance.getChapterRead(1, 1) == 1 || jsonDataManager.Instance.getChapterRead(1, 2) == 1 ||
            jsonDataManager.Instance.getChapterRead(2, 0) == 1 || jsonDataManager.Instance.getChapterRead(2, 1) == 1 || jsonDataManager.Instance.getChapterRead(2, 2) == 1 ||
            jsonDataManager.Instance.getChapterRead(3, 0) == 1 || jsonDataManager.Instance.getChapterRead(3, 1) == 1 || jsonDataManager.Instance.getChapterRead(3, 2) == 1 ||
            jsonDataManager.Instance.getChapterRead(4, 0) == 1 || jsonDataManager.Instance.getChapterRead(4, 1) == 1 || jsonDataManager.Instance.getChapterRead(4, 2) == 1 ||
            jsonDataManager.Instance.getChapterRead(5, 0) == 1 || jsonDataManager.Instance.getChapterRead(5, 1) == 1 || jsonDataManager.Instance.getChapterRead(5, 2) == 1 ||
            !jsonDataManager.Instance.getHomeMeet()) ) {//home
            return true;
        }
        if (idx == 2 &&
            (jsonDataManager.Instance.getChapterRead(0,2) == 2 && !jsonDataManager.Instance.getLibraryMeet())) {//library
            return true;
        }
        if (idx == 3 &&
            (jsonDataManager.Instance.getChapterRead(0, 2) == 2 && !jsonDataManager.Instance.getRecordArchiveMeet())){
            return true;
        }
        if(idx == 4 &&
             (jsonDataManager.Instance.getChapterRead(1, 2) == 2 && !jsonDataManager.Instance.getFoodStreetMeet())){
            return true;
        }

        if (idx == 7)//
        {
            if(getTownNewMark(0) || getTownNewMark(1) || getTownNewMark(2) || getTownNewMark(3) || getTownNewMark(4) || getTownNewMark(5) || getTownNewMark(6))
                return true;
        }

        return false;
    }
    private void shakeTownText(int idx)
    {
        townText[idx].GetComponent<hoverRotate>().shakeStart();
    }


    public void clickTownUI(int i)
    {
        int curTownTemp2 = curTownIdx;
        AdventureReadyManager.Instance.exitAdventureReady();
        upDownManager.Instance.clickCharacterButton(-1);
        MapperManager.Instance.exitMapper();
        FoodStreetManager.Instance.exitFoodStreet();
        //0 : 타워 1 : 집 2: 도서관 3: 아카이브 4:음식거리   7: 로비
        if (i == 0)
        {
            if (!jsonDataManager.Instance.getTowerMeet())
            {
                jsonDataManager.Instance.towerMeet();
                TalkManager.Instance.startTalk(17);
            }
            SoundManager_Sfx.Instance.playSound(0);
            for (int cloudIdx = 0; cloudIdx < cloudObj.Length; cloudIdx++) cloudObj[cloudIdx].GetComponent<cloudMove>().cloudStop();
            AdventureReadyManager.Instance.enterAdventureReady();
            curTownIdx = 0;
            // clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_tower_on");
        }
        if (i == 1)
        {
            SoundManager_Sfx.Instance.playSound(69);
            HomeManager.Instance.enterHome();
            curTownIdx = 1;
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
            SoundManager_Sfx.Instance.playSound(70);
            CameraManager.Instance.updateInitPosition(new Vector3(-1500f, 0f, CameraManager.Instance.cameraPointZ()));
            LibraryManager.Instance.enterLibrary(0);
            curTownIdx = 2;
        }
        if (i == 3)
        {
            if (!jsonDataManager.Instance.getRecordArchiveMeet())
            {
                jsonDataManager.Instance.recordArchiveMeet();
                TalkManager.Instance.startTalk(53);
            }
            MapperManager.Instance.enterMapper();
            curTownIdx = 3;
            SoundManager_Sfx.Instance.playSound(70);
            CameraManager.Instance.updateInitPosition(new Vector3(-1000f, 0f, CameraManager.Instance.cameraPointZ()));
        }
        if (i == 4)
        {
            if (!jsonDataManager.Instance.getFoodStreetMeet())
            {
                jsonDataManager.Instance.foodStreetMeet();
                TalkManager.Instance.startTalk(71);
            }
            curTownIdx = 4;
            FoodStreetManager.Instance.enterFoodStreet();
            SoundManager_Sfx.Instance.playSound(70);
            CameraManager.Instance.updateInitPosition(new Vector3(-500f, -1000f, CameraManager.Instance.cameraPointZ()));
            /*
            fullUI.showFull(6);
            SoundManager_Sfx.Instance.playSound(0);
            clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_town_on");
            */
        }
        
        if (i == 7) {
            for (int newMarkIdx=0;newMarkIdx<7;newMarkIdx++)
            {
                if (getTownNewMark(newMarkIdx)) townNewMark[newMarkIdx].GetComponent<Animator>().Play("NewEvent");
                else townNewMark[newMarkIdx].GetComponent<Animator>().Play("Empty");
            }
            SoundManager_Sfx.Instance.playSound(0);
            CameraManager.Instance.updateInitPosition(new Vector3(-500f, -500f, CameraManager.Instance.cameraPointZ()));
            curTownIdx = 7;
            for (int cloudIdx = 0; cloudIdx < cloudObj.Length; cloudIdx++) cloudObj[cloudIdx].GetComponent<cloudMove>().cloudActive();
        }

        if (townSound[curTownIdx] != townSound[curTownTemp2]) {
            SoundManager_Main.Instance.stopSound(townSound[curTownTemp2]);
            SoundManager_Main.Instance.playSound(townSound[curTownIdx]);
        }
    }
    public void hoverInUIBtn(int i)
    {
        //0 : 타워 1 : 집 2: 도서관 3: 마을
        //shakeTownText(i);

        if (i == 0) clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_tower_on");
        if (i == 1) clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_homeNew_on");
        if (i == 2) clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_library_on");
        if(i == 3) clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_Record Archieve_on");
        if (i == 4) clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_town_on");
    }
    public void hoverOutUIBtn()
    {
        clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
    }
    public void backToTownUI()
    {
        upDownManager.Instance.setInit(jsonDataManager.Instance.getMoney(), 0);

        for (int i = 0; i < cloudObj.Length; i++) cloudObj[i].GetComponent<cloudMove>().cloudActive();
        SoundManager_Main.Instance.playSound(7);
        setTownActive(true);
        clickTownUI(7);

        
    }
    public void backToMain()
    {
        clickTownUI(7);
        SoundManager_Sfx.Instance.playSound(0);
        CameraManager.Instance.updateInitPosition(new Vector3(-1500f, -500f, CameraManager.Instance.cameraPointZ()));
        SoundManager_Main.Instance.stopSound(7);
        SoundManager_Main.Instance.playSound(0);
        upDownManager.Instance.changeOption(3, true);
        setTownActive(false);
    }
    public void setTownActive(bool idx)
    {
        townActive = idx;
        upDownManager.Instance.activeTownUI(townActive);
    }
    public bool getTownActive()
    {
        return townActive;
    }
    // Start is called before the first frame update
    void Start()
    {
        townActive = false;
        curTownIdx = 7;
        townSound[0] = 7; townSound[1] = 19; townSound[2] = 20; townSound[3] = 20;
        townSound[4] = 7; townSound[5] = 7; townSound[6] = 7; townSound[7] = 7;
        townSound[7] = 7;
        setTownActive(false);
        //Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
        //Screen.SetResolution(960, 540, FullScreenMode.Windowed);
        //Screen.SetResolution(960, 540, FullScreenMode.Windowed);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
