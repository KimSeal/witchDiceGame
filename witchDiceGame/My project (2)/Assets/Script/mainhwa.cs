using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mainhwa : MonoBehaviour
{
    [SerializeField]
    public GameObject[] frontObj  = new GameObject[3]; //dice, ail, sheep

    private float[] animSpeed = { 0f, 0f, 0f };
    private float[] animTransVal = { 0f, 0f, 0f };
    private float witchTransVal = 0.3f;
    private bool animTrans;

    // Start is called before the first frame update
    void Start()
    {
        for (int i=0;i<3;i++)
        {
            animSpeed[i] = frontObj[i].GetComponent<Animator>().speed;
        }
        animTrans = false;
        witchTransVal = 0.15f;
        this.GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", witchTransVal);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!animTrans && this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            for (int i = 0; i < frontObj.Length; i++)
            {
                if (animTransVal[i] > 0f)
                {
                    animTransVal[i] -= 0.02f;
                    frontObj[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", animTransVal[i]);
                }
            }
            if (witchTransVal < 0.15f)
            {
                witchTransVal += 0.01f;
                this.GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", witchTransVal);
            }
        }

    }

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.Z))
        {
            witchError(true);
            TalkManager.Instance.setDescString("Add WishList & Play Demo Now!");
            TalkManager.Instance.setDescClickLock(true);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            witchError(false);
        }
        */
    }

    public void witchError(bool onOff)
    {
        if (onOff)
        {
            if (this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                animTrans = true;
                this.GetComponent<Animator>().Play("Noise");

                witchTransVal = 0f;
                this.GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0f);
                
                for (int i = 0; i < frontObj.Length; i++)
                {
                    frontObj[i].GetComponent<Animator>().speed = 0;
                    frontObj[i].GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.7f);
                    animTransVal[i] = 0.7f;
                }
            }
            
            
        }
        else
        {
            if (this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("NoiseEnd") || this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Noise"))
            {
                this.GetComponent<Animator>().Play("Smile");
                animTrans = false;
                for (int i = 0; i < frontObj.Length; i++)
                {
                    frontObj[i].GetComponent<Animator>().speed = animSpeed[i];
                }
            }
            
        }
    }
}
