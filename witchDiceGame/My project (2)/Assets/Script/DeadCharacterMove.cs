using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadCharacterMove : MonoBehaviour
{
    // Start is called before the first frame update

    public int mode = 0;
    [SerializeField]
    public float ySpeed = 0;
    public float xSpeed = 0;
    public float ySpeedDefault = 0;
    public float xSpeedDefault = 0;

    public float gravityVal = 0;
    public float rotateVal = 0;
    public float rotateChangeVal = 0;

    public float shakeVal = 0;
    public float shakeDefault = 0;
    public float shakeDescVal = 0;
    public Vector3 initPoint = new Vector3(0,0,0);
    public float dir = 0;

    public float throwVal = 0;

    public float rotateTest0;
    public float rotateTest1;

    public GameObject wallTouchObj;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(mode == 0)
        {

        }
        else if(mode == 1) // 그냥 돌면서 날아가기
        {
            this.transform.position += new Vector3(xSpeed, ySpeed, 0f);
            ySpeed -= gravityVal;

            this.transform.rotation = Quaternion.Euler(0, 0, rotateVal);
            rotateVal += rotateChangeVal;

            if (this.transform.position.y < -200f || this.transform.position.x > 200f) initMode();
        }
        else if(mode == 2) // 흔들리기 1차
        {
            dir = Random.Range(0, 360);
            this.transform.position = initPoint + (shakeVal + shakeDefault) * new Vector3(Mathf.Sin(dir), Mathf.Cos(dir), 0f);
            shakeVal += shakeDescVal;
            if(shakeVal >= 2)
            {
                dir = rotateTest0;
                mode = 3;
            }
        }
        else if(mode == 3) //흔들리고 직선으로 날아가기 
        {
            this.transform.position = this.transform.position + throwVal * new Vector3(Mathf.Sin(dir), Mathf.Cos(dir), 0f);
            this.transform.rotation = Quaternion.Euler(0, 0, rotateVal);
            rotateVal += rotateChangeVal;
            if (this.transform.position.y > 150f || this.transform.position.x > 200f)
            {
                if (this.transform.position.y > 120f) this.transform.position = new Vector3(this.transform.position.x - 30f, 120f, this.transform.position.z);
                if (this.transform.position.x > 200f) this.transform.position = new Vector3(200, this.transform.position.y - 30f, this.transform.position.z);
                GameObject temp = Instantiate(wallTouchObj, this.transform.position, new Quaternion(0, 0, 0,0));

                Vector2 direction = new Vector3(0, 0, 0) - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                // Z축 기준으로 회전 적용
                temp.transform.rotation = Quaternion.Euler(0, 0, angle + 90f);

                initMode();
            }
        }
        
    }
    public void initMode()
    {
        mode = 0;
        
        ySpeed = 0;
        xSpeed = 0;
        rotateVal = 0;

        shakeVal = 0;
        initPoint = new Vector3(10000f, 0, 0);
        this.transform.position = initPoint;

        shakeVal = shakeDefault;

        dir = 0;
    }
    public void setMode(Vector3 initPoint , int modeVal, int damage)
    {
        initMode();
        this.initPoint = initPoint;
        this.transform.position = initPoint;
        mode = modeVal;

        if(modeVal == 1)
        {
            ySpeed = 5.5f + 0.002f * damage + Random.Range(-0.5f, 0.5f);
            xSpeed = 2.5f + 0.002f * damage + Random.Range(-0.5f, 0.5f);
            rotateChangeVal = -20f + 0.01f * damage + Random.Range(0f, -3f);
        }
        if(modeVal == 2)
        {
            rotateVal = Random.Range(0.1f, 3.0f);
            this.transform.rotation = Quaternion.Euler(0, 0, rotateVal);
            shakeVal = 0;
            shakeDefault = 3f + 0.001f * damage;
            rotateTest0 = Random.Range(0.3f,1.3f);
            rotateChangeVal = Random.Range(-30f,-40f);
            throwVal = Random.Range(40f, 50f);
        }
    }
}
