using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class mapperSlideBar : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private float initX;
    private Vector3 initWoodPosition;
    private bool chkBegin = false;
    Vector3 defaultPoint;
    // Start is called before the first frame update
    void Start()
    {
        initX = this.transform.position.x;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        chkBegin = true;
    }
    public void OnDrag(PointerEventData eventData)
    {
        //Vector3 currentPos = Camera.main.ScreenToWorldPoint(eventData.position);

        // 이렇게 넣어주지 않으면 오브젝트가 이상한 곳에 위치하길래 조정해주었다
        // 원인을 아시는 분은 댓글로 알려주시면 감사하겠습니다
        //currentPos.z = 90f;
        //currentPos.y -= 160f;
        if (chkBegin)
        { //이동중이면 움직임 X
            float mouseX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
            if(mouseX > -860) mouseX = -860;
            if(mouseX < -1075) mouseX = -1075;
            MapperManager.Instance.setEventIdxText(mouseX);
            this.transform.position = new Vector3( mouseX, this.transform.position.y, this.transform.position.z);//eventData.position;//currentPos;
        }
    }
   
    public void OnEndDrag(PointerEventData eventData)
    {
        if (chkBegin)
        { 
            chkBegin = false;
        }

    }


    public void moveToNextIdx(float movePoint)
    {
        this.transform.position = new Vector3(movePoint, this.transform.position.y, this.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {

    }
}