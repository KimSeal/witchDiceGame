using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class mapperSlideBar2 : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField]
    GameObject woodBar;
    private float initY;
    private Vector3 initWoodPosition;
    private bool chkBegin = false;
    Vector3 defaultPoint;
    // Start is called before the first frame update
    void Start()
    {
        initWoodPosition = woodBar.transform.position;
        initY = this.transform.position.y;
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
            float mouseY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;
            if(mouseY > 60) mouseY = 60;
            if(mouseY < -64) mouseY = -64;
            this.transform.position = new Vector3(this.transform.position.x, mouseY, this.transform.position.z);//eventData.position;//currentPos;
            woodBar.transform.position = initWoodPosition + new Vector3(0f,  2 * (initY -this.transform.position.y ),0f);
        }
    }
   
    public void OnEndDrag(PointerEventData eventData)
    {
        if (chkBegin)
        { 
            chkBegin = false;
        }

    }



    // Update is called once per frame
    void Update()
    {

    }
}