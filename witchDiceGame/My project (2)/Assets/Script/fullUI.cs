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
    static private string[] UIstringKR = { "더이상 자리가 없습니다!\n아이템 창에서\n빈 자리를 만들어주세요!" , "주사위를 굴릴 캐릭터를 선택해주세요!" , "아이템을 모두 먹고 와주세요!",
    "아이템 위치 변경은\n강화 창에서만 가능합니다.", "아직 되찾지 못한 이야기입니다!", "본편에서 개방될 이야기입니다.", "데모에선 막힌 구간입니다!\n본편을 기대해주세요!",
    "능력을 2개 선택해주세요!"};
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
        alphaVal = 0.0f;
        textTemp = textObj.GetComponent <TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canvasGroup.alpha > 0.0f)
        {
            alphaVal -= 1.0f * Time.deltaTime;
            canvasGroup.alpha = alphaVal;
        }
    }
    
    static public void showFull(int strIdx)
    {
        alphaVal = 2.5f;
        canvasGroup.alpha = 1.00f;
        textTemp.text = TalkManager.Instance.getDesc(strIdx);//UIstringKR[strIdx];
    }

}
