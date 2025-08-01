using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class dragObj : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private bool chkBegin = false;
    Vector3 defaultPoint;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (AdventureManager.Instance.curCanvasItemCanvas && !itemManager.Instance.getItemBoxMove())
        { //이동중이면 움직임 X
            SoundManager_Sfx.Instance.playSound(0);
            defaultPoint = this.transform.position;
            int temp = -1;
            for (int i = 0; i <= 10; i++)
            {
                if (eventData.pointerDrag.name == "obj_inventory_" + i.ToString())
                {
                    temp = i;
                    break;
                }
            }
            chkBegin = true;
            Debug.Log("begin Number : " + temp.ToString());
            itemManager.Instance.setDragObjStartNum(temp);
            itemManager.Instance.turnOffItemCollider_item();
            //GetComponent<BoxCollider2D>().enabled = false;
        }
        else if(!itemManager.Instance.getItemBoxMove()) {
            fullUI.showFull("아이템 위치 변경은\n강화 창에서만 가능합니다.");
        }
    }
    public void OnDrag(PointerEventData eventData)
    {
        //Vector3 currentPos = Camera.main.ScreenToWorldPoint(eventData.position);

        // 이렇게 넣어주지 않으면 오브젝트가 이상한 곳에 위치하길래 조정해주었다
        // 원인을 아시는 분은 댓글로 알려주시면 감사하겠습니다
        //currentPos.z = 90f;
        //currentPos.y -= 160f;
        if (AdventureManager.Instance.curCanvasItemCanvas && chkBegin && !itemManager.Instance.getItemBoxMove() )
        { //이동중이면 움직임 X
            this.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 6f);//eventData.position;//currentPos;
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (AdventureManager.Instance.curCanvasItemCanvas && chkBegin && !itemManager.Instance.getItemBoxMove())
        { //이동중이면 움직임 X
            SoundManager_Sfx.Instance.playSound(4);
            this.transform.position = defaultPoint;
            //GetComponent<BoxCollider2D>().enabled = true;
            itemManager.Instance.turnOnItemCollider_item();
            //itemManager.Instance.setDragObjStartNum(-1);
            chkBegin = false;
        }
        // 레이캐스트 타겟도 원래대로 돌려준다
        //GetComponent<Image>().raycastTarget = true;
    }

   

    // Update is called once per frame
    void Update()
    {
        
    }
}
