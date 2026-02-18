using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class dragObj_item_character : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    Vector3 defaultPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SoundManager_Sfx.Instance.playSound(0);
        defaultPoint = this.transform.position;
        int temp = -1;
        for (int i=0;i<=3;i++)
        {
            if (eventData.pointerDrag.name == "obj_itemUI_characterBtn_" + i.ToString()) {
                temp = i;
                break;
            }
        }
        /*
        itemManager.Instance.setDragCharacterStartNum(temp);
        Debug.Log(itemManager.Instance.getDragCharacterStartNum());
        itemManager.Instance.turnOffCharacterCollider_item();
        */
        //GetComponent<BoxCollider2D>().enabled = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 vectorTemp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vectorTemp.y = defaultPoint.y;
        this.transform.position = vectorTemp + new Vector3(0,0f,6f);//eventData.position;//currentPos;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        SoundManager_Sfx.Instance.playSound(4);
        this.transform.position = defaultPoint;
           // itemManager.Instance.turnOnCharacterCollider_item();

        // 레이캐스트 타겟도 원래대로 돌려준다
        //GetComponent<Image>().raycastTarget = true;
    }

   

    // Update is called once per frame
    void Update()
    {
        
    }
}
