using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class hoverKeyMapping : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
{
    [SerializeField]
    public int keyMappingIdx;
    
    void Start()
    {
        
    }

    // Update is called once per frame
   
    public void OnPointerEnter(PointerEventData eventData)
    {
        optionManager.Instance.hoverInKeyMapping(keyMappingIdx);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        optionManager.Instance.hoverOutKeyMapping(keyMappingIdx);
    }

    public void activeBtn() { 

    }
}
