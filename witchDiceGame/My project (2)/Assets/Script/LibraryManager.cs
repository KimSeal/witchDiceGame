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
    [SerializeField]
    public GameObject[] curCharacter = new GameObject[2];

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

    public void hoverInCharacterSelectButton(int idx) {
        Debug.Log("?");
        characterSelectOutline[idx].GetComponent<SpriteRenderer>().sprite =
            Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
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
    }

    public void clickCharacterSelectButton(int idx)
    {
        if (idx == -1) curCharacterIdx = -1;
        else curCharacterIdx = idx + curCharacterBigIdx * 9;
        hoverOutCharacterSelectButton();
    }

    public void hoverInCurCharacter(int idx)
    { 
        curCharacter[idx].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
    }
    public void hoverOutCurCharacter()
    {
        curCharacter[0].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
        curCharacter[1].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
    }
    public void clickCurCharacter(int idx) {
        if (curCharacterIdx != -1) {
            if (idx == 0 && curCharacterIdx == 0) //만약 두번째 캐릭터에게 주인공운명을 입힐 경우, 불가능.
            {
                curCharacter[idx].GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/libraryDoll/animCon_libraryDoll");
                return;
            }
            Destiny destinyTemp = CharacterManager.Instance.getDestiny(curCharacterIdx);
            curCharacter[idx].GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("sprite/TestSprite/CharacterImg/" + destinyTemp.getName() + "/animator_" + destinyTemp.getName());
            curCharacterSelectIdx[idx] = idx;
            if (idx == 1 && curCharacterIdx == 0) {
                curCharacter[idx].GetComponent<Animator>().Play("library");
            }
        }
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
        SoundManager_Main.Instance.playSound(1);
        SoundManager_Main.Instance.stopSound(0);
        SoundManager_Main.Instance.stopSound(7);
        jsonDataManager.Instance.addMoney(0);
        curCharacterBigIdx = 0;
        curCharacterIdx = -1;
        for (int i=0;i<9;i++)
        {
            Destiny destinyTemp = CharacterManager.Instance.getDestiny(curCharacterBigIdx * 9 + i);
            characterSelectButton[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + destinyTemp.getName() + "_face");
        }
        hoverOutCharacterSelectButton();
        hoverOutCurCharacter();
        curCharacterIdx = curCharacterSelectIdx[0];
        clickCurCharacter(0);
        curCharacterIdx = curCharacterSelectIdx[1];
        clickCurCharacter(1);
        curCharacterIdx = -1;
    }
    public void exitLibrary() {
        //둘다 선택이 되었을 경우에만 나갈 수 있도록

            TownManager.Instance.backToTownUI();
            SoundManager_Main.Instance.stopSound(1);
            //buyUI[0].SetActive(false);

    }
}
