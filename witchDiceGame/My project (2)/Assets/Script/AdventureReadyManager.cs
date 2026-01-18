using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureReadyManager : MonoBehaviour
{

    private static AdventureReadyManager instance = null;
    private GameObject[] witchPowerObj = new GameObject[2];
    [SerializeField]
    public GameObject[] enterCharacterObj = new GameObject[2];
    private float[] enterCharacterMove = new float[2];
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
        enterCharacterMove[0] = 200f;
        enterCharacterMove[1] = 200f;
        for(int i=0;i<2;i++) witchPowerObj[i] = GameObject.Find("obj_adventureReady_witchPower_Select_" +i.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate(){
        if (enterCharacterMove[0] < 200f)
        {
            enterCharacterMove[0] += 2.5f;
            if (enterCharacterMove[0] % 50f == 25) SoundManager_Sfx.Instance.playSound(17);
            enterCharacterObj[0].GetComponent<Transform>().position = new Vector3(-1185 + enterCharacterMove[0], -530 + 12 * Mathf.Abs(Mathf.Sin((enterCharacterMove[0]/50) * Mathf.PI)), 0f);
        }

        if (enterCharacterMove[1] < 200)
        {
            enterCharacterMove[1] += 2.5f;
            if (enterCharacterMove[1] % 50f == 25) SoundManager_Sfx.Instance.playSound(17);
            enterCharacterObj[1].GetComponent<Transform>().position = new Vector3(-1260 + enterCharacterMove[1], -530 + 12 * Mathf.Abs(Mathf.Sin((enterCharacterMove[1] / 50) * Mathf.PI)), 0f);
        }
    }


    public void startAdventure()
    {
        enterCharacterMove[0] = 200f;
        enterCharacterMove[1] = 200f;
        SoundManager_Sfx.Instance.stopSound(17);

        SoundManager_Main.Instance.stopSound(7);
        SoundManager_Sfx.Instance.playSound(0);
        TownManager.Instance.setTownActive(false);
        AdventureManager.Instance.startAdventure();
    }
    public void enterAdventureReady()
    {
        
        enterCharacterObj[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/backImage/spr_" + CharacterManager.Instance.getDestiny(jsonDataManager.Instance.getCharacterSelect(1)).getName() + "_back");
        enterCharacterObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/backImage/spr_" + CharacterManager.Instance.getDestiny(jsonDataManager.Instance.getCharacterSelect(0)).getName() + "_back");

        enterCharacterMove[0] = 0;
        enterCharacterMove[1] = -20;

        if (jsonDataManager.Instance.getCharacterSelect(0) == 0)
        {
            enterCharacterMove[1] = 200;
            enterCharacterObj[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        CameraManager.Instance.updateInitPosition(new Vector3(-1000f, -500f, CameraManager.Instance.cameraPointZ()));
    }
    public void exitAdventureReady() {
        SoundManager_Sfx.Instance.stopSound(17);
        enterCharacterMove[0] = 200;
        enterCharacterMove[1] = 200;
        //TownManager.Instance.backToTownUI();
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
