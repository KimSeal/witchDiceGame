using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemManager : MonoBehaviour
{

    private static itemManager instance = null;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static itemManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }


    private Item[,] ItemArr = new Item[5,12];
    private bool[,] ItemExistArr = new bool[5, 12];
    private int curSelectItemType = 0;  // 현재 선택한 아이템 종류 선택
    private int curSelectItemIndex = -1; // 현재 선택한 아이템의 인덱스

    public void click_item_bagButton(int idx) //하단부 아이템 박스에서 아이템 클릭하는 경우
    {
        if(ItemExistArr[curSelectItemType, idx]) //아이템이 있는 경우 
        {
            
            if (curSelectItemIndex == idx) {// 이미 자신이 선택한 아이템인 경우
                curSelectItemIndex = -1;
            }
            else
            {
                //아닌 경우
            }
            {
                curSelectItemIndex = idx;
            }
        }
        else //아이템이 없는 경우 해제
        {
            curSelectItemIndex = -1;
        } 
        
    }


    public void click_itemType_selectButton(int idx) // 중단부 아이템 종류 선택 버튼 클릭하는 경우
    {
        if(idx != curSelectItemType) //같으면 이동 안함. 다르니까 이동하는 경우에 대한 처리
        {
            curSelectItemIndex = -1; //택한 아이템 초기화
            curSelectItemType = idx; //타입 변경   
        }
    }

    public void click_dice_changeNum(int idx) //
    {
        if (curSelectItemType == 2)
        {
            
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
