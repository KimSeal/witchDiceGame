using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadCharacterMove : MonoBehaviour
{
    // Start is called before the first frame update

    public int mode = 0; //박힌 상태는 999로 칭하기. 
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
    public float wallColliderVal = 0;
    public float rotateTest0;
    public float rotateTest1;

    public bool wallStopChk = false;
    public GameObject dustEff;
    public GameObject wallTouchObj;
    public GameObject circleEffObj;
    public GameObject fieldDustObj;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (mode == 0)
        {

        }
        else if (mode == 1) // 그냥 돌면서 날아가기
        {
            this.transform.position += new Vector3(xSpeed, ySpeed, 0f);
            ySpeed -= gravityVal;

            this.transform.rotation = Quaternion.Euler(0, 0, rotateVal);
            rotateVal += rotateChangeVal;

            if (this.transform.position.y < -150f)
            {
                GameObject temp = Instantiate(wallTouchObj, new Vector3(this.transform.position.x - 30f, -100, this.transform.position.z), new Quaternion(0, 0, 0, 0));
                temp.GetComponent<Animator>().Play("anim_dropDust_" + Random.Range(0, 3).ToString());
                ColorUtility.TryParseHtmlString("#977955", out Color newColor);
                temp.GetComponent<SpriteRenderer>().material.SetColor("_OutlineColor", newColor);
                temp.GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);

                // Z축 기준으로 회전 적용
                temp.transform.rotation = Quaternion.Euler(0, 0, 0);

                int circleNum = Random.Range(3, 7);
                for (int i = 0; i < circleNum; i++)
                {
                    GameObject temp2 = Instantiate(fieldDustObj, temp.transform.position, new Quaternion(0, 0, 0, 0));
                    temp2.GetComponent<fieldDustEff>().setSpeed(Random.Range(-4f, 4f), Random.Range(6f, 8f));
                    //temp2.GetComponent<effMove>().setWallCollistion( (angle - 90f + Random.Range(-30f, 30f))/360f * 2 * Mathf.PI );
                }
                CameraManager.Instance.attackShakeStart(3);
                initMode();
            }
        }
        else if (mode == 2) // 흔들리기 1차
        {
            dir = Random.Range(0, 360);
            this.transform.position = initPoint + (shakeVal + shakeDefault) * new Vector3(Mathf.Sin(dir), Mathf.Cos(dir), 0f);
            shakeVal += shakeDescVal;
            if (shakeVal >= 2)
            {
                dir = rotateTest0;
                mode = 3;
            }
        }
        else if (mode == 3) //흔들리고 직선으로 날아가기 
        {
            this.transform.position = this.transform.position + throwVal * new Vector3(Mathf.Sin(dir), Mathf.Cos(dir), 0f);
            this.transform.rotation = Quaternion.Euler(0, 0, rotateVal);
            rotateVal += rotateChangeVal;
            if (this.transform.position.y > 150f || this.transform.position.x > 200f)
            {
                if (this.transform.position.y > 120f) this.transform.position = new Vector3(this.transform.position.x - 30f, 120f, this.transform.position.z);
                if (this.transform.position.x > 200f) this.transform.position = new Vector3(200, this.transform.position.y - 30f, this.transform.position.z);
                GameObject temp = Instantiate(wallTouchObj, this.transform.position, new Quaternion(0, 0, 0, 0));

                Vector2 direction = new Vector3(0, 0, 0) - transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                // Z축 기준으로 회전 적용
                temp.transform.rotation = Quaternion.Euler(0, 0, angle + 90f);

                int circleNum = Random.Range(3, 7);
                for (int i = 0; i < circleNum; i++)
                {
                    GameObject temp2 = Instantiate(circleEffObj, this.transform.position, new Quaternion(0, 0, 0, 0));
                    temp2.GetComponent<Animator>().Play("circle_S_W");
                    temp2.GetComponent<effMove>().setWallCollistion((angle - 150f) / 360f * Mathf.PI + Random.Range(-0.5f, 0.5f));
                    //temp2.GetComponent<effMove>().setWallCollistion( (angle - 90f + Random.Range(-30f, 30f))/360f * 2 * Mathf.PI );
                }
                circleNum = Random.Range(3, 7);
                for (int i = 0; i < circleNum; i++)
                {
                    GameObject temp2 = Instantiate(fieldDustObj, temp.transform.position, new Quaternion(0, 0, 0, 0));
                    if(this.transform.position.y > 120f) temp2.GetComponent<fieldDustEff>().setSpeed(Random.Range(-5, 0f), Random.Range(-2f, 1f));
                    else temp2.GetComponent<fieldDustEff>().setSpeed(Random.Range(-5, 0f), Random.Range(-1f, 4f));
                    //temp2.GetComponent<effMove>().setWallCollistion( (angle - 90f + Random.Range(-30f, 30f))/360f * 2 * Mathf.PI );
                }
                CameraManager.Instance.attackShakeStart(5);
                initMode();
                if (wallStopChk)
                {
                    this.transform.position = temp.transform.position + new Vector3(Random.Range(0f,-5f),0f,0f);
                    this.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
                    mode = 999;//박힌 상태는 999로 칭한다.'

                    //if (Random.Range(0,2) == 0) { 
                    //    this.transform.rotation = Quaternion.Euler(0, 0, Random.Range(45f, 135f)); 
                    //}
                    //else {
                    //this.transform.rotation = Quaternion.Euler(0, 0, Random.Range(45f +180f, 135f + 180f)); 
                    //}
                }
            }
        }
        else if (mode == 4) //흔들리고 직선으로 날아가기 
        {
            this.transform.position = this.transform.position + throwVal * new Vector3(Mathf.Sin(dir), Mathf.Cos(dir), 0f);
            this.transform.rotation = Quaternion.Euler(0, 0, rotateVal);
            rotateVal += rotateChangeVal;
            if (wallColliderVal >= 3 && (this.transform.position.x > 400f || this.transform.position.x < -400f
                || this.transform.position.y > 150f || this.transform.position.y < -150f))
            {
                initMode();
            }
            if (wallColliderVal < 3 && (this.transform.position.x < -200f || this.transform.position.x > 200f))
            {
                dir *= -1;
                wallColliderVal += 1;
                int circleNum = Random.Range(2, 5);

                for (int i = 0; i < circleNum; i++)
                {
                    GameObject temp2 = Instantiate(fieldDustObj, this.transform.position, new Quaternion(0, 0, 0, 0));
                    if (wallColliderVal % 2 == 1)
                    {
                        this.transform.position = new Vector3(200f, this.transform.position.y, this.transform.position.z);
                        temp2.GetComponent<fieldDustEff>().setSpeed(Random.Range(-5, 0f), Random.Range(-1f, 4f));
                    }
                    else
                    {
                        this.transform.position = new Vector3(-200f, this.transform.position.y, this.transform.position.z);
                        temp2.GetComponent<fieldDustEff>().setSpeed(Random.Range(0, 5f), Random.Range(-1f, 4f));
                    }
                    //temp2.GetComponent<effMove>().setWallCollistion( (angle - 90f + Random.Range(-30f, 30f))/360f * 2 * Mathf.PI );
                }
                CameraManager.Instance.attackShakeStart(2);
            }
            
        }

    }
    public void initMode()
    {
        if(mode > 0)
        {
            GameObject temp = Instantiate(dustEff, this.transform.position, new Quaternion(0, 0, 0, 0));
            temp.GetComponent<Animator>().Play("Smoke_" + Random.Range(0, 3).ToString());
        }
        mode = 0;
        
        ySpeed = 0;
        xSpeed = 0;
        rotateVal = 0;
        wallColliderVal = 0;
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
        wallStopChk = false;
        if (modeVal == 1)
        {
            ySpeed = 5.5f + 0.002f * damage + Random.Range(-0.5f, 0.5f);
            xSpeed = 1.5f + 0.002f * damage + Random.Range(-0.5f, 0.5f);
            rotateChangeVal = -20f + 0.01f * damage + Random.Range(0f, -3f);
        }
        if(modeVal == 2) // super smash
        {
            rotateVal = Random.Range(0.1f, 3.0f);
            this.transform.rotation = Quaternion.Euler(0, 0, rotateVal);
            shakeVal = 0;
            shakeDefault = 3f + 0.0001f * damage;
            rotateTest0 = Random.Range(0.9f,1.5f);
            rotateChangeVal = Random.Range(-30f,-40f);
            throwVal = Random.Range(40f, 50f);
            wallStopChk = true;
        }
        if (modeVal == 4) //튕기면서 날아감.
        {
            rotateVal = Random.Range(0.1f, 3.0f);
            this.transform.rotation = Quaternion.Euler(0, 0, rotateVal);
            dir = Random.Range(1.3f, 1.5f);
            rotateChangeVal = -15 + (0.001f * damage);
            throwVal = 25f + (0.001f * damage);
        }
        if(modeVal == 5)
        {
            rotateVal = Random.Range(0.1f, 3.0f);
            this.transform.rotation = Quaternion.Euler(0, 0, rotateVal);
            shakeVal = 0;
            shakeDefault = 6f;
            rotateTest0 = Random.Range(0.6f, 1.2f);
            rotateChangeVal = Random.Range(-30f, -40f);
            throwVal = Random.Range(40f, 50f);
            wallStopChk = false;
            
            mode = 2;
        }
    }
}
