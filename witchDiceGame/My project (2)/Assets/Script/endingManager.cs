using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endingManager : MonoBehaviour
{
    //싱클톤
    private static endingManager instance = null;
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
    public static endingManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }

    private Vector3 harfyPoint;
    private float harfyMoveVal;
    private float harfyMoveMoveVal;

    [SerializeField]
    public GameObject ailObj;
    public GameObject jewelObj;
    public GameObject witchObj;
    public GameObject harfyObj;
    public GameObject fieldObj;
    public GameObject backgroundObj;
    public GameObject[] dropObj = new GameObject[5];
    public GameObject[] dropObj2 = new GameObject[5];
    public GameObject[] shadowObj = new GameObject[2];

    public GameObject whiteFullObj;

    public Sprite[] fieldSprite = new Sprite[2];
    public Sprite[] backSprite = new Sprite[2];

    private bool jewelMove = false;
    private bool harfyMove = false;
    private bool whiteMove = false;
    private float whiteSpeed = 0f;
    [SerializeField]
    private float whiteChangeVal = 0.05f;

    private int harfyTake = 0;

    public void endingPhase1(int opt)
    {
        if (opt == -1) //다른 페이즈라 비활성화 필요한 경우
        {
            shadowObj[0].SetActive(false);
            shadowObj[1].SetActive(false);

            ailObj.GetComponent<Animator>().Play("Empty");
            jewelObj.GetComponent<Animator>().Play("Empty");
            witchObj.GetComponent<Animator>().Play("Empty");
            harfyObj.GetComponent<Animator>().Play("Empty");

            jewelObj.transform.position = ailObj.transform.position;
            //harfyObj.transform.position = new Vector3(2750f, -75f, 0f);

            jewelMove = false;
            harfyMove = false;

            return;
        }
        if (opt == 0) //스타트
        {
            shadowObj[0].SetActive(true);
            shadowObj[1].SetActive(true);
            endingPhase2(-1); //ending 2 잔여물 없애기
            harfyTake = 0;

            ailObj.GetComponent<Animator>().Play("ailIdle");
            jewelObj.GetComponent<Animator>().Play("Empty");
            witchObj.GetComponent<Animator>().Play("OnlyGrin");
            harfyObj.GetComponent<Animator>().Play("Black");
            fieldObj.GetComponent<SpriteRenderer>().sprite = fieldSprite[0];
            backgroundObj.GetComponent<SpriteRenderer>().sprite = backSprite[0];
            //harfy랑 drop관련도 초기화도 필요. 씬 전환 후 다시 엔딩 볼때도 보여야 한다는거 유의해서 할것.

            jewelObj.transform.position = ailObj.transform.position;
            harfyObj.transform.position = new Vector3(2750f, -75f, 0f);

            jewelMove = false;
            harfyMove = false;
            whiteMove = false;
            whiteSpeed = 0f;
            whiteFullObj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
        }
        if (opt == 1)// grin이 ail 육체 소환해줌.
        {
            ailObj.GetComponent<Animator>().Play("ailIdle");
            jewelObj.GetComponent<Animator>().Play("Empty");
            witchObj.GetComponent<Animator>().Play("summonAil");
            harfyObj.GetComponent<Animator>().Play("Black");
            fieldObj.GetComponent<SpriteRenderer>().sprite = fieldSprite[0];
            backgroundObj.GetComponent<SpriteRenderer>().sprite = backSprite[0];
            //harfy랑 drop관련도 초기화도 필요. 씬 전환 후 다시 엔딩 볼때도 보여야 한다는거 유의해서 할것.

            jewelObj.transform.position = ailObj.transform.position;
            harfyObj.transform.position = new Vector3(2750f, -75f, 0f);

            jewelMove = false;
            harfyMove = false;
            whiteMove = false;
            whiteSpeed = 0f;
            whiteFullObj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
        }
        if (opt == 2) // ail이 인형이 되고, 보석 빼냄.
        {
            ailObj.GetComponent<Animator>().Play("summonAil");
            jewelObj.GetComponent<Animator>().Play("Create");

            harfyObj.GetComponent<Animator>().Play("Black");
            fieldObj.GetComponent<SpriteRenderer>().sprite = fieldSprite[0];
            backgroundObj.GetComponent<SpriteRenderer>().sprite = backSprite[0];
            //harfy랑 drop관련도 초기화도 필요. 씬 전환 후 다시 엔딩 볼때도 보여야 한다는거 유의해서 할것.

            jewelObj.transform.position = ailObj.transform.position;
            harfyObj.transform.position = new Vector3(2750f, -75f, 0f);

            jewelMove = true;
            harfyMove = false;
            whiteMove = false;
            whiteSpeed = 0f;
            whiteFullObj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);

            //보석 이동 추가 필요

        }
        if (opt == 3)
        { // harfy 원래 위치로
            SoundManager_Sfx.Instance.playSound(72);
            harfyObj.GetComponent<Animator>().Play("Black");
            fieldObj.GetComponent<SpriteRenderer>().sprite = fieldSprite[0];
            backgroundObj.GetComponent<SpriteRenderer>().sprite = backSprite[0];
            //harfy랑 drop관련도 초기화도 필요. 씬 전환 후 다시 엔딩 볼때도 보여야 한다는거 유의해서 할것.

            harfyObj.transform.position = new Vector3(2750f, -75f, 0f);

            harfyMove = true;
            whiteMove = false;
        }
    }
    public void endingPhase2(int opt)
    {
        if(opt == -1) //ending 2 필요하던 것들 없애기
        {

        }
        if (opt == 0)
        {
            endingPhase1(-1);
            whiteMove = false;
            whiteSpeed = whiteChangeVal;
            harfyObj.transform.position = new Vector3(2500f, -75f, 0f);
            harfyObj.GetComponent<Animator>().Play("Idle0");

            setHarfyTake(1);
        }
        if (opt == 1) //싸움
        {
            harfyObj.GetComponent<Animator>().Play("Fight");
        }
        if(opt == 2) //중앙으로 이동
        {
            setHarfyTake(2);
            harfyObj.GetComponent<Animator>().Play("Idle0");
        }
        if(opt == 3) //grin실망
        {
            harfyObj.GetComponent<Animator>().Play("Idle1");
        }
        if(opt == 4) //harfy 살짝 뒤로
        {
            setHarfyTake(3);
        }
        if(opt == 5) // harfy 앞으로
        {
            whiteMove = true;
            whiteSpeed = whiteChangeVal;
            setHarfyTake(4); 
        }
        if(opt == 6)
        {
            setHarfyTake(5);
        }
    }

    public void setHarfyTake(int opt)
    {
        harfyMoveVal = 0f;
        harfyMoveMoveVal = 0.01f;
        harfyTake = opt;
    }

    private void FixedUpdate()
    {
        if (jewelMove)
        {
            jewelObj.transform.position += new Vector3(0.5f, 0f, 0f);
            if (jewelObj.transform.position.x >= witchObj.transform.position.x)
            {
                jewelMove = false;
                jewelObj.GetComponent<Animator>().Play("Destroy");
                witchObj.GetComponent<Animator>().Play("AilGrinFix");
            }
        }
        if (harfyMove)
        {
            if (harfyObj.transform.position.x > 2450f)
            {
                harfyObj.transform.position -= new Vector3(10f, 0f, 0f);
            }
            if (harfyObj.transform.position.x < 2550f && !whiteMove)
            {
                whiteMove = true;
                whiteSpeed = whiteChangeVal;
                whiteFullObj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
                
                harfyMove = false;
                endingPhase1(-1);
                BattleManager.Instance.setEndingTiming(true);
            }
        }

        if (harfyTake == 1) // harfy 마구잡이로 이동.
        {
            harfyObj.transform.position = Vector3.Lerp(harfyPoint, harfyObj.transform.position, 1f - (harfyMoveVal * harfyMoveVal));
            harfyMoveVal += harfyMoveMoveVal;
            
            if(harfyMoveVal >= 0.8f) //harfy 위치 변경
            {
                harfyPoint = new Vector3( Random.Range(2470f,2530f), Random.Range(-100f,-50f), harfyObj.transform.position.z);
                harfyMoveVal = 0f;
                harfyMoveMoveVal = Random.Range(0.01f, 0.03f);
            }
        }
        if (harfyTake == 2) //하피 위치 중앙으로
        {
            if (harfyMoveVal < 1.0f)
            {
                harfyPoint = new Vector3(2500f, -75f, 0f);
                harfyObj.transform.position = Vector3.Lerp(harfyPoint, harfyObj.transform.position, 1f - (harfyMoveVal * harfyMoveVal));
                harfyMoveVal += 0.04f;
            }
        }
        if(harfyTake == 3) //하피 잠깐 뒤로
        {
            if (harfyMoveVal < 1.0f)
            {
                harfyPoint = new Vector3(2550f, -75f, 0f);
                harfyObj.transform.position = Vector3.Lerp(harfyPoint, harfyObj.transform.position, 1f - (harfyMoveVal * harfyMoveVal));
                harfyMoveVal += 0.02f;
            }
        }
        if (harfyTake == 4) //하피 빠르게 앞으로
        {
            if (harfyMoveVal < 1.0f)
            {
                harfyPoint = new Vector3(2150f, -75f, 0f);
                harfyObj.transform.position = Vector3.Lerp(harfyPoint, harfyObj.transform.position, 1f - (harfyMoveVal * harfyMoveVal));
                harfyMoveVal += 0.05f;
                if (harfyObj.transform.position.x < 2250f) //벽닿을시 화면 하얗게 
                {
                    whiteMove = true;
                    whiteSpeed = whiteChangeVal * 2f;
                    harfyMove = false;
                    endingPhase1(-1);
                    BattleManager.Instance.setEndingTiming(true);
                }
            }
        }
        if (harfyTake == 5) //엔딩 끝나고 하얀 화면 초기화
        {
            whiteMove = false;
            whiteSpeed = 0f;
            whiteFullObj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
        }

        if (!whiteMove && whiteFullObj.GetComponent<SpriteRenderer>().color.a > 0f)
        {
            whiteFullObj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, whiteFullObj.GetComponent<SpriteRenderer>().color.a - whiteSpeed);
        }
        if (whiteMove && whiteFullObj.GetComponent<SpriteRenderer>().color.a < 1f)
        {
            whiteFullObj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, whiteFullObj.GetComponent<SpriteRenderer>().color.a + whiteSpeed);
        }
    }
}