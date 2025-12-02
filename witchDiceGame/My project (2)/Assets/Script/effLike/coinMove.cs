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
    [SerializeField] GameObject[] arrivePoint = new GameObject[4];
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
            if (lastTime >= 0.7f) phase = 2;
        }
        else if (phase == 2)
        {
            BattleManager.Instance.shakeBag();
            if (destBag == 0 || destBag == 2) upDownManager.Instance.addGold(1);
            else if (destBag == 1 || destBag == 3) upDownManager.Instance.addJewel(1);

            Destroy(gameObject);
        }
    }

    public void changeDest(int i)
    {
        destBag = i; //0 : adventure Gold, 1 : adventure Witch Power 2: battle Gold 3: battle WitchPower 
        if (i == 1 || i == 3) GetComponent<Animator>().Play("jewel");
    }
    public void changeDestTrue(int i)
    {
        if (i == 0) dest = new Vector3(-682, 79, 0);
        if (i == 1) dest = new Vector3(-682, 62, 0);
        if (i == 2) dest = new Vector3(-182, 79, 0);
        if (i == 3) dest = new Vector3(-182, 62, 0);
    }
}
