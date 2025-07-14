using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using TMPro;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance = null;
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

    public static CameraManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }


    public float ShakeAmount;
    float ZoomTime = -1f;
    float ShakeTime;
    Vector3 initialPosition;
    GameObject loseUI;
    GameObject[] characterPart = new GameObject[4];
    TextMeshPro money;
    TextMeshPro partGet;

    // Start is called before the first frame update
    public void VibrateForeTime(float time) {
        Debug.Log("camera Shake");
        ShakeTime = time;
        ShakeAmount = 0.1f;
    }
    public void VibrateForeTime(float time, float power)
    {
        Debug.Log("camera Shake");
        ShakeTime = time;
        ShakeAmount = power * 2;
    }
    public int pixelWidth = 384;  // 낮은 해상도 너비
    public int pixelHeight = 216;  // 낮은 해상도 높이

    void Start()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        tempSize = gameObject.GetComponent<UnityEngine.U2D.PixelPerfectCamera>().assetsPPU * timeDelay;
        Debug.Log("hey! " + tempSize);
        initialPosition = transform.position;
        ZoomTime = -1f;
        loseUI = GameObject.Find("obj_ui_lose");
        for (int i=0;i<4;i++)
        {
            characterPart[i] = GameObject.Find("obj_ui_lose_character_" + i.ToString());
        }
        money = GameObject.Find("obj_ui_lose_money").GetComponent<TextMeshPro>();
        partGet = GameObject.Find("obj_ui_lose_characterPart").GetComponent<TextMeshPro>();
    }

    int tempSize;
    int direction = 0;
    int timeDelay = 3;

    bool windowChk = false;
    // Update is called once per frame
    void Update()
    {
        if (!windowChk)
        {
            windowChk = true;
            Screen.SetResolution(960, 540, FullScreenMode.Windowed);
        }
        if (ZoomTime > 0)
        {
            
            if (direction == 0)
            {
                gameObject.GetComponent<UnityEngine.U2D.PixelPerfectCamera>().assetsPPU = (++tempSize) / 3;
                if (tempSize >= 130 * timeDelay)
                {
                    tempSize = 130 * timeDelay;
                    direction = 1;
                }
            }
            
            if (direction == 1)
            {
                gameObject.GetComponent<UnityEngine.U2D.PixelPerfectCamera>().assetsPPU = (--tempSize) / 3;
                if (tempSize <= 100 * timeDelay)
                {
                    tempSize = 100 * timeDelay;
                    direction = 0;
                    ZoomTime = -1;

                }
            }
           

        }
        else
        {
            if (ShakeTime > 0)
            {
                Vector3 temp = Random.insideUnitSphere * ShakeAmount + initialPosition;
                transform.position = new Vector3(temp.x, temp.y, transform.position.z);

                ShakeTime -= Time.deltaTime;
            }
            else
            {

                ShakeTime = 0.0f;
                transform.position = initialPosition;
            }
        }




    }
    public void zoomEvent()
    {
        ZoomTime = 1;
    }
    public float cameraPointX()
    {
        return this.transform.position.x;
    }
    public float cameraPointY()
    {
        return this.transform.position.y;
    }
    public float cameraPointZ()
    {
        return this.transform.position.z;
    }
    public void updateInitPosition(Vector3 vec )
    {
        initialPosition = vec;
        ShakeTime = 0.0f;
        FadeUIScript.fadeIn();
    }
    bool loseChk = false;
    public void resultScreenActive(int caseVal)
    {
        int updateMoney = AdventureManager.Instance.getAdventureMoney();
        money.text = "$" + updateMoney.ToString(); //돈 관련 텍스트 업데이트;
        if (caseVal == 2) updateMoney *= 5;
        if (caseVal == 2) money.text += " X 5";

        jsonDataManager.Instance.addMoney(updateMoney); //남은 돈은 다 수익으로 돌아간다.

        for (int i = 0; i < 4; i++) {
            int destinyIdx = AdventureManager.Instance.getLastCharacter(i); //마지막으로 전투했던 캐릭터들 정보 얻기.
            if (destinyIdx != -99999) {
                
                if (caseVal == 0)
                {
                    loseUI.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/design_ui_lose");
                    jsonDataManager.Instance.addCharacterToken(destinyIdx, 1); //캐릭터들의 토큰을 얻는다.
                    partGet.text = " X 1";
                }
                if (caseVal == 2) //데모 클리어시 5배로 준다.
                {
                    loseUI.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/design_ui_stageClear");
                    jsonDataManager.Instance.addCharacterToken(destinyIdx, 1); //캐릭터들의 토큰을 얻는다.
                    partGet.text = " X 5\n( special stage clear bonus! )";
                }
                characterPart[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_" + CharacterManager.Instance.getDestiny(destinyIdx).getName() + "_face");
            }
            else
            {
                characterPart[i].GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/CharacterImg/faceImage/spr_no_face");
            }
        }


        loseChk = true;
        loseUI.transform.position = new Vector3(initialPosition.x, initialPosition.y, loseUI.transform.position.z);
    }


    public void loseScreenUnActive()
    {
        loseChk = false;
        loseUI.transform.position = new Vector3(0,300, loseUI.transform.position.z);
    }
    public bool getLoseScreenActive()
    {
        return loseChk;
    }
}
