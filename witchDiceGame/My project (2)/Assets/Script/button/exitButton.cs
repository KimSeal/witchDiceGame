using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exitButton : MonoBehaviour
{
    private float clickTime;
    public float minClickTime = 1;
    private bool isClick = false;
    private bool animEndChk = false;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator  = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ButtonDown()
    {
        SoundManager_Sfx.Instance.playSound(5);
        Debug.Log("start");
        isClick = true;
        animator.Play("hold");
    }
    public void ButtonUp()
    {
        Debug.Log("end");
        SoundManager_Sfx.Instance.stopSound(5);
        isClick = false;
        if (animEndChk)
        {
            SoundManager_Sfx.Instance.playSound(6);
            //캐릭터 방출 function
            itemManager.Instance.deleteCharacter();
        }
        else
        {
            SoundManager_Sfx.Instance.playSound(7);
        }
        animator.Play("normal");
        animEndChk = false;
    }
    public void chkAnimEnd() // 애니메이션 끝까지 갔으면 종료 확인
    {
        animEndChk = true;
    }
}
