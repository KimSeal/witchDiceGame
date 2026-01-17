using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class hoverRotateUI : MonoBehaviour
    , IPointerClickHandler
    , IPointerEnterHandler
    , IPointerExitHandler
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
    [SerializeField]
    private bool textExist = true;

    private float shakeAmount = 0;
    private float rotateVal = 0;
    private float sizeVal = 1.0f;
    private float sizeLerpVal = 0.0f;
    private bool sizeOnOff = false;

    private bool lanOnOff = false;
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
        if (textExist)
        {
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSize = 12f * (sizeVal * Mathf.Abs(Mathf.Sin(sizeLerpVal))) + 24f;
            if (!lanOnOff)
            {
                if (transform.GetChild(0).transform.localPosition.x > 0f)
                {
                    transform.GetChild(0).transform.localPosition -= new Vector3(15, 0, 0);
                }
            }
            else
            {
                if (transform.GetChild(0).transform.localPosition.x < 75f && 75f - transform.GetChild(0).transform.localPosition.x > 0.1f)
                {
                    transform.GetChild(0).transform.localPosition += new Vector3((75f - transform.GetChild(0).transform.localPosition.x) / 4.0f, 0, 0);
                }
            }
        }
        
            //new Vector3((sizeVal * Mathf.Abs(Mathf.Sin(sizeLerpVal))) + 1f, (sizeVal * Mathf.Abs(Mathf.Sin(sizeLerpVal))) + 1f, 0.0f);
        //transform.transform.position -= new Vector3(0, 0.1f, 0);
    }

   
    public void OnPointerEnter(PointerEventData eventData)
    {
        //shakeAmount = 30;
        //rotateVal = 0;
        if (shakeAbleBySelf) shakeStart();
        if (expandAbleBySelf) expandStart();
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (shakeAbleByClick) shakeStart();
    }
    public void setLanguageActive(bool input)
    {
        lanOnOff = input;
        if (input) transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "* "+ 
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.Substring(2, transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.Length - 2);
        else transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "- " +
                transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.Substring(2, transform.GetChild(0).GetComponent<TextMeshProUGUI>().text.Length - 2);
    }
    public void OnPointerExit(PointerEventData eventData)
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

    public void activeBtn() { 

    }
}
