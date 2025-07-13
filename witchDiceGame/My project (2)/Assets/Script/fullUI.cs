using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fullUI : MonoBehaviour
{
    // Start is called before the first frame update
    static CanvasGroup canvasGroup;
    static float alphaVal;
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
        alphaVal = 0.0f;
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
    }
}
