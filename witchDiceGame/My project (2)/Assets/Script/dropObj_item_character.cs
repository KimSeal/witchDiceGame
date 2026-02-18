using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class dropObj_item_character : MonoBehaviour, IDropHandler
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OnDrop(PointerEventData eventData)
    {
        /*
        Debug.Log("swap character test :");
        Debug.Log(itemManager.Instance.getDragCharacterStartNum());
        if (itemManager.Instance.getDragCharacterStartNum() != -1)
        {
            int temp = -1;
            for (int i = 0; i <= 3; i++)
            {
                if (gameObject.name == "obj_itemUI_characterBtn_box_" + i.ToString())
                {
                    temp = i;
                    break;
                }
            }
            itemManager.Instance.setDragCharacterEndNum(temp);
            Debug.Log("swap Character! : " + itemManager.Instance.getDragCharacterStartNum().ToString() + " / " + itemManager.Instance.getDragCharacterEndNum().ToString());
            itemManager.Instance.swapCharacter();
        }
        */
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
