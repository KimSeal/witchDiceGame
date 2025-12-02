using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureReadyManager : MonoBehaviour
{

    private static AdventureReadyManager instance = null;
    private GameObject[] witchPowerObj = new GameObject[2];
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

    public static AdventureReadyManager Instance
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
        for(int i=0;i<2;i++) witchPowerObj[i] = GameObject.Find("obj_adventureReady_witchPower_Select_" +i.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void startAdventure()
    {
        SoundManager_Main.Instance.stopSound(7);
        SoundManager_Sfx.Instance.playSound(0);
        AdventureManager.Instance.startAdventure();
    }
    public void enterAdventureReady()
    {
        SoundManager_Main.Instance.playSound(7);
        CameraManager.Instance.updateInitPosition(new Vector3(-1000f, -500f, CameraManager.Instance.cameraPointZ()));
        //SoundManager_Main.Instance.playSound(1);
    }
    public void exitAdventureReady() {
        TownManager.Instance.backToTownUI();
    }

    public void hoverInCurPower(int i)
    {
        Material material = witchPowerObj[i].GetComponent<SpriteRenderer>().material;
        material.SetFloat("_Transparency", 0.7f);

    }
    public void hoverOutCurPower(int i)
    {
        Material material = witchPowerObj[i].GetComponent<SpriteRenderer>().material;
        material.SetFloat("_Transparency", 0.0f);
    }
    public void clickCurPower()
    {
        if (!jsonDataManager.Instance.getLibraryMeet())
        {
            jsonDataManager.Instance.libraryMeet();
            //TalkManager.Instance.startTalk(1);
        }
        SoundManager_Sfx.Instance.playSound(0);
        LibraryManager.Instance.enterLibrary(1);
    }
    private string[] powerType = { "reroll", "turn", "add", "sub" };
    private string[] targetType = { "my", "enemy", "any" };

}
