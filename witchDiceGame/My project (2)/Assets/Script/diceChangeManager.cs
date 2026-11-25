using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class diceChangeManager : MonoBehaviour
{
    private static diceChangeManager instance = null;


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

    public static diceChangeManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }

    [SerializeField]
    public GameObject[] diceChangeButton = new GameObject[6];
    public GameObject[] diceChangeUpDownEntity = new GameObject[6];
    public GameObject[] diceChangeUpButton = new GameObject[6];
    public GameObject[] diceChangeDownButton = new GameObject[6];
    public Sprite[] diceChangeSprite = new Sprite[6];

    public GameObject characterChangeButton;
    public Sprite[] characterChangeSprite = new Sprite[2];

    public GameObject remainNum;
    public Sprite[] remainNumSprite = new Sprite[10];


    private int curCharacter = 0;
    private int fullCost = 0;
    private int[,] useCost = {{ 0, 0, 0, 0, 0, 0 },
                                { 0, 0, 0, 0, 0, 0 }};

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void hoverInUpDownButton(int idx)
    {

        hoverInDiceButton(idx % 10);
        //ex 05 : down 6th dice  ,  13 : up 4th dice
        if (idx / 10 == 1) // up
        {
            diceChangeUpButton[idx % 10].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
        }
        else if(idx / 10 == 0) // down
        {
            diceChangeDownButton[idx % 10].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
        }
    }
    public void hoverOutUpDownButton(int idx)
    {
        diceChangeUpButton[idx].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
        diceChangeDownButton[idx].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
    }
    public void clickUpDownButton(int idx)
    {
        
        // ex 03 : down 4th dice    14 : up 5th dice
        if (idx / 10 == 0) // down
        {
            if (useCost[curCharacter, idx % 10] > 0) //now add num
            {
                useCost[curCharacter, idx % 10] -= 1;
            }
            else if (fullCost - getRemainCost() > 0)// now sub or no use num
            {
                if ((getDiceInitNum(idx % 10) - 1 + useCost[curCharacter, idx % 10] + 6) % 6 == 0)
                {
                    //더이상 아래로 못가는 경우
                    return;
                }
                else
                {
                    useCost[curCharacter, idx % 10] -= 1;
                }
            }
        }
        else if (idx / 10 == 1) // up
        {
            if (useCost[curCharacter, idx % 10] < 0) //now sub num
            {
                useCost[curCharacter, idx % 10] += 1;
            }
            else if (fullCost - getRemainCost() > 0)// now add or no use num
            {
                if ((getDiceInitNum(idx % 10) - 1 + useCost[curCharacter, idx % 10] + 6) % 6 == 5)
                {
                    //더이상 아래로 위로 못가는 경우
                    return;
                }
                else
                {
                    useCost[curCharacter, idx % 10] += 1;
                }
            }
        }

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                jsonDataManager.Instance.setDiceChangeCost(i, j, useCost[i, j]);
                Debug.Log("diceChangeSave");
            }
        }
        jsonDataManager.Instance.SavePlayerDataToJson();

        updateAll();
    }

    public void hoverInDiceButton(int idx)
    {
        for (int i = 0; i < 6; i++) {
            diceChangeButton[i].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0); 
        }
        diceChangeButton[idx].GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
    }
    public void hoverOutDiceButton()
    {
       
    }
    public void clickDiceButton(int idx)
    {

    }

    public void hoverInCharacterChangeButton()
    {
        characterChangeButton.GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
    }
    public void hoverOutCharacterChangeButton()
    {
        characterChangeButton.GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
    }
    public void clickCharacterChangeButton()
    {
        if (curCharacter == 1 && jsonDataManager.Instance.getCharacterSelect(0) == 0)
        {
            return; //사용불가인 경우
        }

        if (curCharacter == 0) curCharacter = 1;
        else curCharacter = 0;
        updateAll();
    }

    public int getRemainCost()
    {
        int tempRemainCost = 0;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (useCost[i, j] < 0) tempRemainCost -= useCost[i, j];
                else tempRemainCost += useCost[i, j];
            }
        }

        return tempRemainCost;
    }

    public void updateAll()
    {
        for (int i = 0; i < 6; i++) {
            diceChangeButton[i].GetComponent<SpriteRenderer>().sprite = diceChangeSprite[((getDiceInitNum(i) - 1 + useCost[curCharacter, i] + 6) % 6) ];
        }
        Debug.Log(
            ((getDiceInitNum(0) - 1 + useCost[curCharacter, 0] + 6) % 6).ToString() + "_" +
            ((getDiceInitNum(1) - 1 + useCost[curCharacter, 1] + 6) % 6).ToString() + "_" +
            ((getDiceInitNum(2) - 1 + useCost[curCharacter, 2] + 6) % 6).ToString() + "_" +
            ((getDiceInitNum(3) - 1 + useCost[curCharacter, 3] + 6) % 6).ToString() + "_" +
            ((getDiceInitNum(4) - 1 + useCost[curCharacter, 4] + 6) % 6).ToString() + "_" +
            ((getDiceInitNum(5) - 1 + useCost[curCharacter, 5] + 6) % 6).ToString() + "_" 
            );
        characterChangeButton.GetComponent<SpriteRenderer>().sprite = characterChangeSprite[curCharacter];
        
        remainNum.GetComponent<SpriteRenderer>().sprite = remainNumSprite[fullCost - getRemainCost()];
    }

    public int getDiceInitNum(int idx)
    {
        return CharacterManager.Instance.getDestiny( jsonDataManager.Instance.getCharacterSelect(curCharacter) ).getBaseDiceNum(idx) ;
    }
    public int getDiceInitNum(int characterIdx, int idx)
    {
        return CharacterManager.Instance.getDestiny(jsonDataManager.Instance.getCharacterSelect(characterIdx)).getBaseDiceNum(idx);
    }

    public void updateDiceChange(int idx, ref Character characterTemp)
    {
        for (int i = 0; i < 6; i++)
        {
            if (((getDiceInitNum(idx, i) + useCost[idx, i] + 6) % 6) == 0)
            {
                characterTemp.changeDiceNum(i, 6);
            }
            else
            {
                characterTemp.changeDiceNum(i, ((getDiceInitNum(idx, i) + useCost[idx, i] + 6) % 6));
            }
        }
    }

    public void enterDiceChange()
    {
        curCharacter = 1;

        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 6; j++) {
                useCost[i, j] = jsonDataManager.Instance.getDiceChangeCost(i, j);
            } 
        }
        fullCost = 6;

        hoverOutCharacterChangeButton();
        hoverInDiceButton(0);
        updateAll();
    }
    public void exitDiceChange()
    {
        //if(jsonDataManager.Instance.getCharacterSelect(0) == 0) // 동료 없을 시, cost를 0으로
        //{
         //   for (int i = 0; i < 6; i++) useCost[0,i] = 0;
        //}

        
    }
}
