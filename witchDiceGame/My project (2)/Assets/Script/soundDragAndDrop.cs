using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class soundDragAndDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField]
    public int soundOption;

    [SerializeField]
    public float maxVal;
    [SerializeField]
    public float minVal;

    // Start is called before the first frame update
    void Start()
    {
        minVal = 125f * 1;//widthArr[opt] / 1920f; 
        maxVal = 750f * 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeSoundMinMaxVal()
    {
        int opt = jsonDataManager.Instance.getScreenSize();
        float[] widthArr = { 640f, 960f, 1280f, 1920f, 1920f };

        minVal = 125f * 1;//widthArr[opt] / 1920f; 
        maxVal = 750f * 1;//widthArr[opt] / 1920f;
    }

    public void setUIButton()
    {
        this.GetComponent<hoverRotateUI>().shakeStart();
        if (soundOption == 0) this.transform.position = new Vector3((SoundManager_Main.Instance.backgroundVolume * (maxVal - minVal) /2.5f) + minVal,this.transform.position.y, this.transform.position.z);
        if (soundOption == 1) this.transform.position = new Vector3((SoundManager_Sfx.Instance.SFXVolume * (maxVal - minVal) / 2.5f) + minVal, this.transform.position.y, this.transform.position.z);
    }

    public void OnBeginDrag(PointerEventData eventData) {
    }

    public void OnDrag(PointerEventData eventData)
    {
        float nextX = 0.0f;
        nextX = Input.mousePosition.x;
        if (nextX < minVal) nextX = minVal;
        if (nextX > maxVal) nextX = maxVal;
        transform.position = new Vector3(nextX, this.transform.position.y, this.transform.position.z);
        
        if(soundOption == 0) SoundManager_Main.Instance.setBackgroundVolume((nextX - minVal) * 2.5f/ (maxVal - minVal));
        if (soundOption == 1) SoundManager_Sfx.Instance.setSFXVolume((nextX - minVal) * 2.5f / (maxVal - minVal));

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<hoverRotateUI>().shakeStart();
        float nextX = 0.0f;
        nextX = Input.mousePosition.x;
        if (nextX < minVal) nextX = minVal;
        if (nextX > maxVal) nextX = maxVal;
        transform.position = new Vector3(nextX, this.transform.position.y, this.transform.position.z);

        if (soundOption == 0)
        {
            jsonDataManager.Instance.setBackgroundVol((nextX - minVal) * 2.5f / (maxVal - minVal));
        }
        if (soundOption == 1) {
            jsonDataManager.Instance.setSFXVol((nextX - minVal) * 2.5f / (maxVal - minVal));
            SoundManager_Sfx.Instance.playSound(1);
        }
        Debug.Log((nextX - minVal) * 2.5f / (maxVal - minVal));

    }
}
