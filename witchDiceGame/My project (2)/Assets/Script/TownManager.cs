using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownManager : MonoBehaviour
{
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

    public void clickTownUI(int i)
    {
        Debug.Log("click!");
        //0 : 타워 1 : 집 2: 도서관 3: 마을
        if (i == 0)
        {

            AdventureReadyManager.Instance.enterAdventureReady();
            SoundManager_Main.Instance.stopSound(0);
            // clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_tower_on");
        }
        if (i == 1)
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
            clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_home_on");
        }
        if (i == 2)
        {
            TalkManager.Instance.startTalk(1);
            Debug.Log("libraryEnter");
            CameraManager.Instance.updateInitPosition(new Vector3(-1500f, 0f, CameraManager.Instance.cameraPointZ()));
            LibraryManager.Instance.enterLibrary(0);
            SoundManager_Main.Instance.stopSound(0);
        }
        if (i == 3)
        {
            Screen.SetResolution(960, 540, FullScreenMode.Windowed);
            clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_town_on");
        }
    }
    public void hoverInUIBtn(int i)
    {
        //0 : 타워 1 : 집 2: 도서관 3: 마을
        if(i == 0) clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_tower_on");
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
        CameraManager.Instance.updateInitPosition(new Vector3(-500f, -500f, CameraManager.Instance.cameraPointZ()));
        SoundManager_Main.Instance.playSound(0);
    }
    // Start is called before the first frame update
    void Start()
    {
        clickAndImageChange = transform.Find("spr_town_home_click").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
