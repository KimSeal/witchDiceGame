using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class fullUI : MonoBehaviour
{
    // Start is called before the first frame update
    static CanvasGroup canvasGroup;

    [SerializeField]
    public GameObject textObj;
    static float alphaVal;
    static TextMeshProUGUI textTemp;
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
        alphaVal = 0.0f;
        textTemp = textObj.GetComponent <TextMeshProUGUI>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canvasGroup.alpha > 0.0f)
        {
            alphaVal -= 0.01f;
            canvasGroup.alpha = alphaVal;
        }
    }
    static public void showFull()
    {
        alphaVal = 1.2f;
        canvasGroup.alpha = 1.00f;
        textTemp.text =
            "더이상 자리가 없습니다!\n아이템 창에서\n빈 자리를 만들어주세요!";
    }

    static public void showFull(string str)
    {
        alphaVal = 1.2f;
        canvasGroup.alpha = 1.00f;
        textTemp.text = str;
    }
}
