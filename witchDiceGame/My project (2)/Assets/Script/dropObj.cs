using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class dropObj : MonoBehaviour, IDropHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("You Drop!");
        Debug.Log(itemManager.Instance.getItemBoxMove());
        Debug.Log(itemManager.Instance.getDragCharacterStartNum());
        if (!itemManager.Instance.getItemBoxMove() && itemManager.Instance.getDragObjStartNum() != -1)
        {
            int temp = -1;
            for (int i = 0; i <= 11; i++)
            {
                if (gameObject.name == "obj_inventory_box_" + i.ToString())
                {
                    temp = i;
                    break;
                }
            }
            itemManager.Instance.setDragObjEndNum(temp);

            //itemManager.Instance.setDragObjEndNum();
            //Debug.Log(eventData.pointerDrag.name);
            Debug.Log(itemManager.Instance.getDragObjStartNum());
            Debug.Log(itemManager.Instance.getDragObjEndNum());
            itemManager.Instance.swapItem();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
