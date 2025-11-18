using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class upDownManager : MonoBehaviour
{
    [SerializeField]
    public GameObject[] underHoverBar = new GameObject[6]; //entity, picture, dice 0-3
    public TextMeshProUGUI underHoverBarTitle, underHoverBarDesc; // Title, desc

    [SerializeField]
    public GameObject[] upperHoverBar = new GameObject[6]; //entity, picture, dice 0-3
    public TextMeshProUGUI upperHoverBarTitle, upperHoverBarDesc; // Title, desc

    [SerializeField]
    public GameObject[] underSkillButton = new GameObject[8];
    public GameObject[] underSkillOutline = new GameObject[8];

    [SerializeField]
    public GameObject[] upperItemButton = new GameObject[12];
    public GameObject[] upperItemOutline = new GameObject[12];

    [SerializeField]
    public GameObject backBlack;
    public GameObject bigDiceEntity;
    public GameObject[] bigDiceButton = new GameObject[4];
    public GameObject[] bigDiceOutline = new GameObject[4];

    private int lockState = 0;
    private int curSkill = -1;

    private static upDownManager instance = null;


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

    public static upDownManager Instance
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
        backBlack.SetActive(false);
        bigDiceEntity.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        moveBattleUI(moveArrY[0], underHoverBar[0]);
        moveBattleUI(moveArrY[1], upperHoverBar[0]);
    }

    //underBar, upperBar
    private float[] moveArrY = { 9f, 215f,};

    private void onOffUI(int idx, int opt) {
        moveArrY[idx] = moveConstY[opt, idx];
    }
    //off, on
    private float[,] moveConstY = {
        {9f, 215f},
        { 55f, 168f}
    };

    public void hoverInBigDice(int idx)
    {
        bigDiceOutline[idx].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
    }
    public void hoverOutBigDice(int idx)
    {
        bigDiceOutline[idx].GetComponent<Image>().sprite
             = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
    }

    public void hoverInUpperBar(int idx) {
        onOffUI(1, 1);
        upperItemOutline[idx].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
        if (lockState == 0)
        {
            BattleManager.Instance.updateMoveUI(1);
        }
    }
    public void hoverOutUpperBar(int idx)
    {
        onOffUI(1, 0);
        upperItemOutline[idx].GetComponent<Image>().sprite
            = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        if (lockState == 0)
        {
            BattleManager.Instance.updateMoveUI(0);
        }
    }
    public void hoverInUnderBarSkill(int idx)
    {
        underSkillOutline[idx].GetComponent<Image>().sprite
                = Resources.Load<Sprite>("sprite/TestSprite/diceImage/outline1");
        if (lockState != 1)
        {
            onOffUI(0, 1);
            
            if (lockState == 0) { 
                BattleManager.Instance.updateMoveUI(2);
                BattleManager.Instance.makeSkillCommand(idx / 2, idx % 2);
            }
        }
    }
    public void hoverOutUnderBarSkill(int idx)
    {
        if (curSkill != idx)
        {
            underSkillOutline[idx].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        if (lockState != 1) { 
            onOffUI(0, 0);
            
            if (lockState == 0) //고정 상태가 아니라면, 뒤에거도 움직여줄 것.
            {
                BattleManager.Instance.updateMoveUI(0);
            }
        }
    }

    public void clickSkill(int input)
    {
        for (int idx = 0;idx< 2;idx++) {
            underSkillOutline[idx].GetComponent<Image>().sprite
                    = Resources.Load<Sprite>("sprite/TestSprite/diceImage/spr_test_empty");
        }
        curSkill = input;
        if (input != -1)
        {
            backBlack.SetActive(true);
            bigDiceEntity.SetActive(true);
            hoverInUnderBarSkill(input);
            lockState = 1; //클릭시 현재 스킬에 대한 설명으로 고정.
        }
        else {
            backBlack.SetActive(false);
            bigDiceEntity.SetActive(false);
            lockState = 0;
            hoverOutUnderBarSkill(0);
        }
    }

    public void skillDescUpdate(string pictureStr, int needDice0, int needDice1, int needDice2, int needDice3 ,string skillName, string skillCommand) {
        underHoverBar[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/characterSkill/spr_skill_" + pictureStr);

        underHoverBar[2].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_" + needDice0.ToString());
        underHoverBar[3].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_" + needDice1.ToString());
        underHoverBar[4].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_" + needDice2.ToString());
        underHoverBar[5].GetComponent<Image>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/needDice_" + needDice3.ToString());

        underHoverBarTitle.text = skillName;
        underHoverBarDesc.text = skillCommand;
    }

    private void moveBattleUI(float inputY, GameObject gameObjTemp)
    {
        Vector3 destination = new Vector3(gameObjTemp.GetComponent<RectTransform>().anchoredPosition.x, inputY, 0);

        float termY = 0.2f;
        if (gameObjTemp.GetComponent<RectTransform>().anchoredPosition.y < inputY)
        {
            termY *= -1;

            if (gameObjTemp.GetComponent<RectTransform>().anchoredPosition.y < inputY + termY)
            {
                gameObjTemp.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(gameObjTemp.GetComponent<RectTransform>().anchoredPosition, destination, 0.1f);
            }
            else
            {
                gameObjTemp.GetComponent<RectTransform>().anchoredPosition = destination;
            }
        }
        else
        {
            if (gameObjTemp.GetComponent<RectTransform>().anchoredPosition.y > inputY + termY)
            {
                gameObjTemp.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(gameObjTemp.GetComponent<RectTransform>().anchoredPosition, destination, 0.1f);
            }
            else
            {
                gameObjTemp.GetComponent<RectTransform>().anchoredPosition = destination;
            }
        }
    }



}
