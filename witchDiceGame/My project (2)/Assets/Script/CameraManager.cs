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
        ShakeTime = time;
        ShakeAmount = 0.1f;
    }
    public void VibrateForeTime(float time, float power)
    {
        ShakeTime = time;
        ShakeAmount = power * 2;
    }
    public int pixelWidth = 384;  // 낮은 해상도 너비
    public int pixelHeight = 216;  // 낮은 해상도 높이

    void Start()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        tempSize = gameObject.GetComponent<UnityEngine.U2D.PixelPerfectCamera>().assetsPPU * timeDelay;
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

    public void changeScreenSize(int idx)
    {
        if(idx >=0 && idx<5)jsonDataManager.Instance.setScreenSize(idx);
        if (jsonDataManager.Instance.getScreenSize() == 0) Screen.SetResolution(640, 360, FullScreenMode.Windowed);
        if (jsonDataManager.Instance.getScreenSize() == 1) Screen.SetResolution(960, 540, FullScreenMode.Windowed);
        if (jsonDataManager.Instance.getScreenSize() == 2) Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
        if (jsonDataManager.Instance.getScreenSize() == 3) Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        if (jsonDataManager.Instance.getScreenSize() == 4) Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }

    // Update is called once per frame
    void Update()
    {
        if (!windowChk)
        {
            windowChk = true;
            changeScreenSize(jsonDataManager.Instance.getScreenSize());
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
    void FixedUpdate()
    {
        
    }
    private void LateUpdate()
    {
        if (dirShakeSize > 0)
        {
            this.transform.position = initialPosition + new Vector3(dirShakeSize * Mathf.Sin(dirShakeVal) * Mathf.Cos(dirShake), dirShakeSize * Mathf.Sin(dirShakeVal) * Mathf.Sin(dirShake), 0f);
            dirShakeSize -= (subShakeVal * Time.deltaTime);
            dirShakeVal += 20f * Time.deltaTime;
            if (dirShakeSize <= 0) this.transform.position = initialPosition;
        }
    }

    [SerializeField]
    public float dirShakeSize = 0; //흔들리는 크기
    public float dirShake = 0; //실제 방향
    public float dirShakeVal = 0; //흔들릴때 시간 축이 될 변수
    public float subShakeVal = 0.02f;

    public void attackShakeStart(float power)
    {
        dirShakeVal = 0.0f;
        
        if (power < 0) {
            dirShakeVal = Mathf.PI;
            power *= -1.0f;
        }
        dirShakeSize = power;
        subShakeVal = power;
        dirShake = Random.Range(Mathf.PI * -0.25f, Mathf.PI * 0.25f);

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
        Debug.Log("hello?" + caseVal.ToString());
        int updateMoney = AdventureManager.Instance.getAdventureGold();
        money.text = "$" + updateMoney.ToString(); //돈 관련 텍스트 업데이트;
        if (caseVal == 0) {
            money.text += " / 2 = ";
            AdventureManager.Instance.addMoney(0, (AdventureManager.Instance.getAdventureGold() / 2) * -1);
            money.text += AdventureManager.Instance.getAdventureGold().ToString();
        }
        
        /*
        if (caseVal == 2)
        {
            
        }
        */

        if(caseVal == 0) loseUI.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/design_ui_lose");
        if(caseVal == 1) loseUI.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/design_ui_giveUp");
        if (caseVal == 2) loseUI.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/design_ui_stageClear");
        for (int i = 0; i < 4; i++) {
            int destinyIdx = AdventureManager.Instance.getLastCharacter(i); //마지막으로 전투했던 캐릭터들 정보 얻기.
            if (destinyIdx != -99999) {
                
                if (caseVal == 0)
                {
                    jsonDataManager.Instance.addCharacterToken(destinyIdx, 1); //캐릭터들의 토큰을 얻는다.
                    partGet.text = " Adventure Last Member ";
                }
                if (caseVal == 1) {
                    
                    jsonDataManager.Instance.addCharacterToken(destinyIdx, 1); //캐릭터들의 토큰을 얻는다.
                    partGet.text = " Adventure Last Member ";
                }
                if (caseVal == 2) //데모 클리어시 5배로 준다.
                {
                    jsonDataManager.Instance.addCharacterToken(destinyIdx, 1); //캐릭터들의 토큰을 얻는다.
                    partGet.text = " Adventure Last Member ";
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
        jsonDataManager.Instance.setMoney(AdventureManager.Instance.getAdventureGold());
        /*
        if (caseVal == 2)
        {
            for (int i = 0; i < updateMoney; i++)
            {
                BattleManager.Instance.makeCoin(0, loseUI.transform.position - new Vector3(90f, 0f, 0f));
            }
            jsonDataManager.Instance.setMoney(updateMoney * 2);
        }
        else
        {
            jsonDataManager.Instance.setMoney(updateMoney);
        }
        */

    }


    public void loseScreenUnActive()
    {
        upDownManager.Instance.clickItem(-1);
        upDownManager.Instance.clickItem(-1);
        upDownManager.Instance.clickUpperItemTypeInit(false);
        loseChk = false;
        loseUI.transform.position = new Vector3(0,300, loseUI.transform.position.z);
        AdventureManager.Instance.exitBattleCanvas(false);
    }
    public bool getLoseScreenActive()
    {
        return loseChk;
    }
}
