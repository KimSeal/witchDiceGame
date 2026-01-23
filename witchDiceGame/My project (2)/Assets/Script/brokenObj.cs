using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class brokenObj : MonoBehaviour
{
    [SerializeField]
    public float gravityVal;
    public float ySpeed;
    public float xSpeed;
    public float rotationVal;
    public float rotationCurVal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rotationCurVal += 1.0f;
        this.transform.rotation = Quaternion.Euler(0f, 0f, rotationCurVal * rotationVal);
        
        this.transform.position += new Vector3(xSpeed, ySpeed, 0f);
        ySpeed -= gravityVal;
        if (this.transform.position.y < -120f) { 
            Destroy(gameObject);
        }

    }
    public void setBrokenDice()
    {
        float scaleVal = Random.Range(0.5f, 1.0f);
        this.transform.localScale = new Vector3(scaleVal, scaleVal, 0f);

        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("sprite/TestSprite/diceImage/ChainBreak/spr_diceBroken_" + Random.Range(0, 4).ToString());
        xSpeed = Random.Range(-2.5f, 2.5f);
        ySpeed = Random.Range(3.0f, 4.5f);
        gravityVal = 0.2f;
        rotationVal = Random.Range(-20.0f, 20.0f);
        rotationCurVal = Random.Range(0f, 70f);
    }
}
