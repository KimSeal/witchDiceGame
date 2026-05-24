using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LibraryManager : MonoBehaviour
{
    private static LibraryManager instance = null;

    [SerializeField]
    public GameObject[] characterSelectButton = new GameObject[9];
    public GameObject[] characterSelectOutline = new GameObject[9];
    public GameObject[] characterSelectNextButton = new GameObject[2];
    public GameObject[] characterSelectSmoke = new GameObject[2];

    [SerializeField]
    public GameObject[] characterSkillButton = new GameObject[2];
    public GameObject[] characterSkillOutline =new GameObject[2];
    public GameObject characterFace;
    public GameObject[] characterInfo = new GameObject[4];


    [SerializeField]
    public GameObject[] curCharacter = new GameObject[2];

    public Character[] curCharacterInfo = new Character[2];

    public int curCharacterBigIdx = 0;
    public int curCharacterIdx = 0;
    public int[] curCharacterSelectIdx = { 0, 0 };
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

    public static LibraryManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }

    public void hoverInCharacterSelectButton(int idx)
    {
        characterSelectOutline[idx].GetComponent<SpriteRenderer>().sprite =
            Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
        if (jsonDataManager.Instance.getPlayerCharacterAble(curCharacterBigIdx * 9 + idx)) 
        {
            updateBlackBoard(curCharacterBigIdx * 9 + idx);
        }
        else updateBlackBoard(-1);
    }
    public void hoverOutCharacterSelectButton()
    {
        for (int i = 0; i < characterSelectButton.Length; i++)
        {
            if (curCharacterIdx == i + curCharacterBigIdx * 9) {
                characterSelectOutline[i].GetComponent<SpriteRenderer>().sprite =
                Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
            }
            else
            {
                characterSelectOutline[i].GetComponent<SpriteRenderer>().sprite
                = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
            }
        }
        updateBlackBoard(curCharacterIdx);
    }

    public void clickCharacterSelectButton(int idx)
    {
        if(!jsonDataManager.Instance.getPlayerCharacterAble(curCharacterBigIdx * 9 + idx))
        {
            fullUI.showFull(66);
        }
        else
        {
            SoundManager_Sfx.Instance.playSound(71);
            curCharacterIdx = idx + curCharacterBigIdx * 9;
            hoverOutCharacterSelectButton();
        }
    }

    public void hoverInCharacterSkill(int idx)
    {
        if (curCharacterIdx == -1) return;

        Destiny destinyTemp = CharacterManager.Instance.getDestiny(curCharacterIdx);
        Skill thisSkill = destinyTemp.getSkill(idx);
        ToolBarManager.Instance.setToolBar(thisSkill);
        /*
        upDownManager.Instance.skillDescUpdate(thisSkill.getSkillName(), thisSkill.getNeedDice(0), thisSkill.getNeedDice(1),
            thisSkill.getNeedDice(2), thisSkill.getNeedDice(3), thisSkill.getSkillName(), thisSkill.getCommand());
        upDownManager.Instance.onOffUI(0, 1);
        */
        characterSkillOutline[idx].GetComponent<SpriteRenderer>().sprite =
            Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
    }

    public void hoverOutCharacterSkill()
    {
        ToolBarManager.Instance.toolBarOnOff(0);
        /*
        upDownManager.Instance.skillDescUpdate("none", 0, 0, 0, 0, "", "");
        upDownManager.Instance.onOffUI(0, 0);
        */
        characterSkillOutline[0].GetComponent<SpriteRenderer>().sprite =
            Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        characterSkillOutline[1].GetComponent<SpriteRenderer>().sprite =
            Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        
    }

    public void updateBlackBoard(int idx)
    {
        if (idx == -1)
        {
            characterFace.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            characterSkillButton[0].GetComponent<SpriteRenderer>().sprite= Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            characterSkillButton[1].GetComponent<SpriteRenderer>().sprite= Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_none");
            for (int i = 0; i < 4; i++) characterInfo[i].GetComponent<TextMeshPro>().text = "---";
        }
        else
        {
            Destiny destinyTemp = CharacterManager.Instance.getDestiny(idx);
            characterFace.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + destinyTemp.getName() + "_face");
            characterSkillButton[0].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + destinyTemp.getSkill(0).getSkillName());
            characterSkillButton[1].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + destinyTemp.getSkill(1).getSkillName());
            characterInfo[0].GetComponent<TextMeshPro>().text = destinyTemp.maxHp.ToString();
            characterInfo[1].GetComponent<TextMeshPro>().text = destinyTemp.phyAtk.ToString();
            characterInfo[2].GetComponent<TextMeshPro>().text = destinyTemp.magAtk.ToString();
            characterInfo[3].GetComponent<TextMeshPro>().text = destinyTemp.speed.ToString();
        }
    }

    public void hoverInCurCharacter(int idx)
    {
        if (idx == 1 && jsonDataManager.Instance.getChapterRead(1, 2) != 2)
        {
            return;
        }
        if (!(idx == 0 && curCharacterSelectIdx[idx] == 0) )
        {
            ToolBarManager.Instance.setToolBar(curCharacterInfo[idx]);
            updateBlackBoard(curCharacterSelectIdx[idx]);
        }
        curCharacter[idx].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
        
    }
    public void hoverOutCurCharacter()
    {
        curCharacter[0].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
        curCharacter[1].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
        updateBlackBoard(-1);
        hoverOutCharacterSelectButton();
        ToolBarManager.Instance.toolBarOnOff(0);
    }
    private bool smokeSound = false;
    public void clickCurCharacter(int idx) {

        if (idx == 1 && jsonDataManager.Instance.getChapterRead(1,2) != 2)
        {
           
            Destiny destinyTemp = CharacterManager.Instance.getDestiny(0);
            curCharacter[idx].GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + destinyTemp.getName() + "/animator_" + destinyTemp.getName());
            curCharacterSelectIdx[idx] = curCharacterIdx;

            jsonDataManager.Instance.setCharacterSelect(idx, 0);
            CharacterManager.Instance.setCharacter_destinyBase(ref curCharacterInfo[idx], jsonDataManager.Instance.getCharacterSelect(idx));
            if (idx == 1) FoodStreetManager.Instance.upgradeInitStat(ref curCharacterInfo[idx]);

            if (curCharacterSelectIdx[idx] == 0 && idx == 1)
            {
                if (jsonDataManager.Instance.getChapterRead(1, 2) == 2)
                {
                    curCharacter[idx].GetComponent<Animator>().Play("library");
                }
                else if (jsonDataManager.Instance.getChapterRead(0, 2) == 2)
                {
                    curCharacter[idx].GetComponent<Animator>().Play("right");
                }

            }
            hoverInCurCharacter(idx);
            return;
            
        }
        if(curCharacterIdx == 0 && idx == 0 && curCharacterSelectIdx[idx] == 0)
        {
            fullUI.showFull(139);
            return;
        }

        if (curCharacterIdx != curCharacterSelectIdx[idx])
        {
            if (smokeSound) SoundManager_Sfx.Instance.playSound(72);
            characterSelectSmoke[idx].GetComponent<Animator>().Play("Smoke");

            if (curCharacterIdx == 0 && idx == 0)
            {
                curCharacterSelectIdx[idx] = curCharacterIdx;
                jsonDataManager.Instance.setCharacterSelect(idx, curCharacterIdx);
                hoverInCurCharacter(idx);
                curCharacter[idx].GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/libraryDoll/animCon_libraryDoll");
                curCharacterInfo[idx] = null;
                ToolBarManager.Instance.toolBarOnOff(0);
                return;
            }
            else {
                //해당 운명으로 변경.
                Destiny destinyTemp = CharacterManager.Instance.getDestiny(curCharacterIdx);
                curCharacter[idx].GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + destinyTemp.getName() + "/animator_" + destinyTemp.getName());
                curCharacterSelectIdx[idx] = curCharacterIdx;

                jsonDataManager.Instance.setCharacterSelect(idx, curCharacterIdx);
                CharacterManager.Instance.setCharacter_destinyBase(ref curCharacterInfo[idx], jsonDataManager.Instance.getCharacterSelect(idx));
                if (idx == 1) FoodStreetManager.Instance.upgradeInitStat(ref curCharacterInfo[idx]);

                if (curCharacterSelectIdx[idx] == 0 && idx == 1)
                {
                    if (jsonDataManager.Instance.getChapterRead(1, 2) == 2) {
                        curCharacter[idx].GetComponent<Animator>().Play("library");
                    }
                    else if (jsonDataManager.Instance.getChapterRead(0, 2) == 2)
                    {
                        curCharacter[idx].GetComponent<Animator>().Play("right");
                    }

                }
                hoverInCurCharacter(idx);
            }
        }
        else //같은 경우, 해제 필요.
        {
            if (curCharacterSelectIdx[idx] == 0) return; //이미 0인 경우, 변경 X
            else
            {
                if (smokeSound) SoundManager_Sfx.Instance.playSound(72);
                characterSelectSmoke[idx].GetComponent<Animator>().Play("Smoke");
                if (idx == 0) //첫번째일 경우,
                {
                    curCharacterSelectIdx[idx] = 0;
                    jsonDataManager.Instance.setCharacterSelect(idx, 0);
                    hoverInCurCharacter(idx);
                    curCharacter[idx].GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/libraryDoll/animCon_libraryDoll");
                    ToolBarManager.Instance.toolBarOnOff(0);
                    curCharacterInfo[idx] = null;
                    return;
                }
                else
                {
                    Destiny destinyTemp = CharacterManager.Instance.getDestiny(0);
                    curCharacter[idx].GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + destinyTemp.getName() + "/animator_" + destinyTemp.getName());
                    curCharacterSelectIdx[idx] = 0;

                    jsonDataManager.Instance.setCharacterSelect(idx, 0);
                    CharacterManager.Instance.setCharacter_destinyBase(ref curCharacterInfo[idx], jsonDataManager.Instance.getCharacterSelect(idx));
                    if (idx == 1) FoodStreetManager.Instance.upgradeInitStat(ref curCharacterInfo[idx]);

                    if (jsonDataManager.Instance.getChapterRead(1, 2) == 2)
                    {
                        curCharacter[idx].GetComponent<Animator>().Play("library");
                    }
                    else if (jsonDataManager.Instance.getChapterRead(0, 2) == 2)
                    {
                        curCharacter[idx].GetComponent<Animator>().Play("right");
                    }

                    hoverInCurCharacter(idx);
                }
            }
        }

    }
    public void updateCharacterSelectImage()
    {
        for (int i = 0; i < 9; i++)
        {
            if (jsonDataManager.Instance.getPlayerCharacterAble(curCharacterBigIdx * 9 + i))
            {
                Destiny destinyTemp = CharacterManager.Instance.getDestiny(curCharacterBigIdx * 9 + i);
                characterSelectButton[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + destinyTemp.getName() + "_face");
                /*
                if (curCharacterBigIdx * 9 + i == 0)
                {
                    characterSelectButton[i].GetComponent<SpriteRenderer>().sprite =
                        Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_dice_Stop");
                }
                */
                characterSelectButton[i].GetComponent<hoverRotate>().shakeStart();
            }
            else
            {
                characterSelectButton[i].GetComponent<SpriteRenderer>().sprite =
                       Resources.Load<Sprite>("sprite/townImage/spr_town_lock");
            }
        }
    }

    public void clickNextButton(int dir)
    {
        if (curCharacterBigIdx == 0) curCharacterBigIdx = 1;
        else curCharacterBigIdx = 0;
        updateCharacterSelectImage();
        hoverOutCurCharacter();
    }
    public void hoverRotateAble(GameObject gameObjectTemp, int eventType, bool onOff)
    {
        if (eventType == 0) gameObjectTemp.GetComponent<hoverRotate>().shakeAble(onOff);
        else if (eventType == 1) gameObjectTemp.GetComponent<hoverRotate>().expandAble(onOff);
        else if (eventType == 2) gameObjectTemp.GetComponent<hoverRotate>().clickShakeAble(onOff); //에러있음.
    }

    public void shakeObject(GameObject gameObjectTemp)
    {
        gameObjectTemp.GetComponent<hoverRotate>().shakeStart();
    }
    /*
    private void makeDarkBtn(int i) {
        Material material = BtnArr[i].GetComponent<SpriteRenderer>().material;
        material.SetFloat("_Transparency", 0.7f);
    }
    private void makeBrightBtn(int i)
    {
        Material material = BtnArr[i].GetComponent<SpriteRenderer>().material;
        material.SetFloat("_Transparency", 0.0f);
    }
    */
    



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void enterLibrary(int idx)
    {
        
        CameraManager.Instance.updateInitPosition(new Vector3(-1500f, 0f, CameraManager.Instance.cameraPointZ()));
        //SoundManager_Main.Instance.playSound(1);
        //SoundManager_Main.Instance.stopSound(0);
        //SoundManager_Main.Instance.stopSound(7);
        jsonDataManager.Instance.addMoney(0);
        curCharacterBigIdx = 0;
        curCharacterIdx = 0;
        updateCharacterSelectImage();
        hoverOutCharacterSelectButton();
        hoverOutCharacterSkill();
        hoverOutCurCharacter();
        smokeSound = false;
        curCharacterIdx = jsonDataManager.Instance.getCharacterSelect(0);
        curCharacterSelectIdx[0] = -1;
        clickCurCharacter(0);
        
        curCharacterIdx = jsonDataManager.Instance.getCharacterSelect(1);
        curCharacterSelectIdx[1] = -1;
        clickCurCharacter(1);
        smokeSound = true;
        curCharacterIdx = 0;
        hoverOutCurCharacter();
    }
    public void exitLibrary() {
        //둘다 선택이 되었을 경우에만 나갈 수 있도록

            TownManager.Instance.backToTownUI();
            SoundManager_Main.Instance.stopSound(1);
            //buyUI[0].SetActive(false);

    }
}
