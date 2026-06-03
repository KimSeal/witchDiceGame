using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCharacterMove : MonoBehaviour
{
    [SerializeField]
    public int state = 0;
    public int alphaChangeSpeed = 0;

    public float moveVal;
    public float moveSpeed = 0;
    public Vector3 startPoint;
    public Vector3 endPoint;
    // Start is called before the first frame update
    void Start()
    {
        state = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.GetComponent<SpriteRenderer>().color.a < 255f)
        {
            this.GetComponent<SpriteRenderer>().color = 
                new Color(this.GetComponent<SpriteRenderer>().color.r, this.GetComponent<SpriteRenderer>().color.g, this.GetComponent<SpriteRenderer>().color.b, 
                this.GetComponent<SpriteRenderer>().color.a + alphaChangeSpeed * Time.deltaTime);
        }
        if(state == 1)
        {
            if (moveVal > 0.0f)
            {
                moveVal -= moveSpeed * Time.deltaTime;
                if (moveVal <= 0.0f) { moveVal = 0f; state = 0; }
                this.transform.position = Vector3.Lerp(endPoint, startPoint, (moveVal) * (moveVal));
            }
        }
    }

    public void setAnim(int opt, Vector3 vectorInput)
    {
        moveVal = 0.0f;
        state = opt;
        if (opt == 0)
        {
            this.transform.position = new Vector3(0f, 2000f, this.transform.position.z);
        }
        if(opt == 1)
        {
            this.GetComponent<SpriteRenderer>().color =
                new Color(this.GetComponent<SpriteRenderer>().color.r, this.GetComponent<SpriteRenderer>().color.g, this.GetComponent<SpriteRenderer>().color.b, 0f);
            moveVal = 1.0f;
            endPoint = vectorInput;
            startPoint = vectorInput + new Vector3(-40f, 0f, 0f);
        }
        if (opt == 2)
        {
            this.transform.position = vectorInput;
        }
    }
}
