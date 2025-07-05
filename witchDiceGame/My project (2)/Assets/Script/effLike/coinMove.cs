using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinMove : MonoBehaviour
{
    int phase = 0;
    float spd = 0;
    float dir = 0;
    float xspd = 0;
    float yspd = 0;
    float lastTime = 0;
    Vector3 dest;
    int destBag = 0;
    // Start is called before the first frame update
    void Start()
    {
        spd = Random.Range(2.5f, 4.0f);
        dir = Random.Range(0f, 2* Mathf.PI);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (phase == 0)
        {
            xspd = spd * Mathf.Cos(dir);
            yspd = spd * Mathf.Sin(dir);
            this.transform.position += new Vector3(xspd, yspd, 0f);
            spd -= 0.1f;
            if (spd < 0.1f)
            {
                lastTime = 0.0f;
                phase = 1;
                changeDestTrue(destBag);
            }
        }
        else if (phase == 1)
        {
            transform.position = Vector2.Lerp(transform.position, dest, lastTime);
            lastTime += 0.02f;
            if (lastTime >= 1.0f) phase = 2;
        }
        else if (phase == 2) Destroy(gameObject);
    }

    public void changeDest(int i)
    {
        destBag = i;
    }
    public void changeDestTrue(int i)
    {
        GameObject bagPoint;
        if (i == 0) bagPoint = GameObject.Find("obj_battle_btn_itemInfo");  //배틀일때
        else bagPoint = GameObject.Find("obj_btn_bag"); //adventure일때
        dest = bagPoint.transform.position;
    }
}
