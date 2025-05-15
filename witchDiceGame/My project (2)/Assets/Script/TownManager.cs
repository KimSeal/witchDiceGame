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


    GameObject mainCamera;
    GameObject clickAndImageChange;

    public void clickTownUI(int i)
    {
        Debug.Log("click!");
        //0 : 타워 1 : 집 2: 도서관 3: 마을
        if (i == 0)
        {
            AdventureManager.Instance.startAdventure();
           // clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_tower_on");
        }
        if (i == 1) clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_home_on");
        if (i == 2)
        {
            mainCamera.GetComponent<CameraShake>().updateInitPosition(new Vector3(-1500f, 0f, mainCamera.transform.position.z));
            LibraryManager.Instance.enterLibrary();
        }
        if (i == 3) clickAndImageChange.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/townUI/spr_town_town_on");
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
        mainCamera.GetComponent<CameraShake>().updateInitPosition(new Vector3(-500f, -500f, mainCamera.transform.position.z));
    }
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GameObject.Find("Main Camera");
        clickAndImageChange = GameObject.Find("spr_town_home_click");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
