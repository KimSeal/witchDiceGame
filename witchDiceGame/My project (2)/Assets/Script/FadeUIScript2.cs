using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeUIScript2 : MonoBehaviour
{
    static CanvasGroup canvasGroup;
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (canvasGroup.alpha > 0.0f)
        {
            canvasGroup.alpha -= 2f * Time.deltaTime;
        }
    }
    static public void fadeIn()
    {
        canvasGroup.alpha = 1.0f;
    } 
}
