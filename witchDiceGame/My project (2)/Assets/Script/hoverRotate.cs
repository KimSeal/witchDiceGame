using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hoverRotate : MonoBehaviour
{
    [SerializeField]
    public bool shakeAbleBySelf = false;
    [SerializeField]
    public bool expandAbleBySelf = false;
    [SerializeField]
    public bool shakeAbleByClick = false;

    [SerializeField]
    public float shakeVal = 0.5f;
    [SerializeField]
    public float shakeAmountChangeVal = 1.0f;
    [SerializeField]
    private float expandVal = 0.1f;

    private float shakeAmount = 0;
    private float rotateVal = 0;
    private float sizeVal = 1.0f;
    private float sizeLerpVal = 0.0f;
    private bool sizeOnOff = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.eulerAngles = new Vector3(0, 0, shakeAmount * Mathf.Sin(rotateVal));
        rotateVal += shakeVal;
        if (shakeAmount > 0)
        {
            shakeAmount -= shakeAmountChangeVal;
            if (shakeAmount < 0) shakeAmount = 0.0f;
        }
        if (rotateVal > 100 * Mathf.PI) rotateVal -= 100.0f * Mathf.PI;

        if (sizeOnOff) {
            if (sizeVal < 0.25f) sizeVal += expandVal;
            else sizeVal = 0.25f;
            if (sizeLerpVal >= Mathf.PI * 0.75f) { sizeLerpVal = Mathf.PI * 0.75f; }
            else { sizeLerpVal += 0.1f; }
        }
        else
        {
            if (sizeLerpVal > 0.0f) { sizeLerpVal -= 0.25f; }
            else sizeLerpVal = 0.0f;
            if (sizeVal > 0.0f) sizeVal -= 0.025f;
            else sizeVal = 0.0f;
        }
        transform.localScale = new Vector3((sizeVal * Mathf.Abs(Mathf.Sin(sizeLerpVal))) + 1f, (sizeVal * Mathf.Abs(Mathf.Sin(sizeLerpVal))) + 1f, 0.0f);
        //transform.transform.position -= new Vector3(0, 0.1f, 0);
    }

   
    private void OnMouseEnter() {
        //shakeAmount = 30;
        //rotateVal = 0;
        if (shakeAbleBySelf) shakeStart();
        if (expandAbleBySelf) expandStart();
    }

    private void OnMouseUp() {
        if(shakeAbleByClick) shakeStart();
    }
    private void OnMouseExit()
    {
        if (expandAbleBySelf) expandEnd();
    }

    public void shakeStart()
    {
        shakeAmount = 30;
        shakeAmountChangeVal = 1.0f;
        rotateVal = 0;
    }

    public void shakeStart(float amount)
    {
        shakeAmount = amount;
        shakeAmountChangeVal = shakeAmount/30.0f;
        rotateVal = 0;
    }

    public void expandStart()
    {
        sizeLerpVal = 0f;
        sizeOnOff = true;
    }
    public void expandEnd()
    {
        sizeLerpVal = Mathf.PI * 0.25f;
        sizeOnOff = false;
    }
    public void shakeAble(bool onOff)
    {
        this.shakeAbleBySelf = onOff;
    }
    public void expandAble(bool onOff) {
        this.expandAbleBySelf = onOff;
        if(!onOff) expandEnd(); 
    }
    public void clickShakeAble(bool onOff) { 
        this.shakeAbleByClick = onOff;
    }
}
