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
        AdventureManager.Instance.startAdventure();
    }
    public void enterAdventureReady()
    {
        drawSelectPower(0, BattleManager.Instance.getWitchPower(1));
        drawSelectPower(1, BattleManager.Instance.getWitchPower(2));
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
        LibraryManager.Instance.enterLibrary(1);
    }
    private string[] powerType = { "reroll", "turn", "add", "sub" };
    private string[] targetType = { "my", "enemy", "any" };

    private void drawSelectPower(int idx, int power) //
    {
        if (power == 0) witchPowerObj[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPowerUI/spr_witchUI_nothing");
        else witchPowerObj[idx].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/witchPower/witchPowerUI/spr_witchUI_" + powerType[(power - 1) / 3] + "_" + targetType[(power - 1) % 3]);
    }
}
