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

    [SerializeField]
    public GameObject ailObj;
    public GameObject jewelObj;
    public GameObject witchObj;
    public GameObject harfyObj;
    public GameObject fieldObj;
    public GameObject backgroundObj;
    public GameObject[] dropObj = new GameObject[5];
    public GameObject[] dropObj2 = new GameObject[5];

    public GameObject whiteFullObj;

    public Sprite[] fieldSprite = new Sprite[2];
    public Sprite[] backSprite = new Sprite[2];

    private bool jewelMove;
    private bool harfyMove;
    public void test(int opt)
    {
        if (opt == 0) //스타트
        {
            ailObj.GetComponent<Animator>().Play("ailIdle");
            witchObj.GetComponent<Animator>().Play("OnlyGrin");
            jewelObj.GetComponent<Animator>().Play("Empty");
            fieldObj.GetComponent<SpriteRenderer>().sprite = fieldSprite[0];
            backgroundObj.GetComponent<SpriteRenderer>().sprite = backSprite[0];
            //harfy랑 drop관련도 초기화도 필요. 씬 전환 후 다시 엔딩 볼때도 보여야 한다는거 유의해서 할것.
            jewelObj.transform.position = ailObj.transform.position;
            jewelMove = false;
            harfyMove = false;
            whiteFullObj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
            harfyObj.transform.position = new Vector3(2750f, -75f, 0f);
        }
        if (opt == 1)// grin이 ail 육체 소환해줌.
        {
            witchObj.GetComponent<Animator>().Play("summonAil");
        }
        if (opt == 2) // ail이 인형이 되고, 보석 빼냄.
        {
            ailObj.GetComponent<Animator>().Play("summonAil");
            jewelObj.GetComponent<Animator>().Play("Create");
            jewelObj.transform.position = ailObj.transform.position;
            //보석 이동 추가 필요
            jewelMove = true;
        }
        if (opt == 3)
        { // harfy 원래 위치로
            harfyObj.transform.position = new Vector3(2750f, -75f, 0f);
            harfyMove = false;
            whiteFullObj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);
        }
        if(opt == 4)
        {
            harfyMove = true;
        }
    }

    private void FixedUpdate()
    {
        if (jewelMove)
        {
            jewelObj.transform.position += new Vector3(0.1f, 0f, 0f);
            if(jewelObj.transform.position.x > witchObj.transform.position.x)
            {
                jewelMove = false;
                jewelObj.GetComponent<Animator>().Play("Empty");
                witchObj.GetComponent<Animator>().Play("AilGrinFix");
                jewelObj.transform.position = ailObj.transform.position;
            }
        }
        if (harfyMove)
        {
            if (harfyObj.transform.position.x > 2550f)
            {
                harfyObj.transform.position -= new Vector3(1f, 0f, 0f);
                if(harfyObj.transform.position.x < 2650f)
                {
                    whiteFullObj.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, whiteFullObj.GetComponent<SpriteRenderer>().color.a + 5);
                }
                //하얗게 배경 + 카메라 전환해서 하피 시점으로 이동.
            }
            else
            {
                harfyMove = false;
            }
        }

    }

}
