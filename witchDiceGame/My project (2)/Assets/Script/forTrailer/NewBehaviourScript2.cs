using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript2 : MonoBehaviour
{
    [SerializeField]
    public float ySpd = 0;
    [SerializeField]
    public float yAxcel = 0;
    [SerializeField]
    public float spinAxcel = 0;
    [SerializeField]
    public float spinAmountAxcel = 0;
    [SerializeField]
    public float spinVal = 1.0f;
    [SerializeField]
    public float spinAmountVal = 90.0f;

    [SerializeField]
    public float xAxcel = 0.1f;
    [SerializeField]
    public float xSpd = -20f;

    [SerializeField]
    public Sprite spriteTemp;

    private int phaseVal = 0;
    Vector3 defaultPoint = new Vector3(-1561.9f, -900f, 0f);
    // Start is called before the first frame update
    void Start()
    {
        spinVal = 1.0f;
        spinAmountVal = 90.0f;
        this.transform.rotation = Quaternion.Euler(0, 0, 90f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (phaseVal == 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, spinAmountVal * Mathf.Sin(spinVal * Mathf.PI));

            if (spinAmountVal < 0) spinAmountVal = 0;
            else spinAmountVal -= spinAmountAxcel;

            spinVal += spinAxcel;
            if (this.transform.position.y < -520f)
            {
                if (ySpd > 0) ySpd -= yAxcel;
                if (ySpd < 0) ySpd = 0;
                if (ySpd > 1) ySpd -= yAxcel;
                if (ySpd < 1) ySpd = 1;
            }
        }
        if (phaseVal == 1) {
            xSpd += xAxcel;
        }
    }
    public void OnMouseDown()
    {
        StartCoroutine(trailerAilMove());
    }
    private IEnumerator trailerAilMove() {

        Debug.Log("clicked!");
        this.transform.position = defaultPoint;
        this.transform.rotation = Quaternion.Euler(0, 0, 90f);
        spinVal = 1.0f;
        spinAmountVal = 90.0f;
        while (this.transform.position.y < -520f)
        {

            this.transform.position += new Vector3(0, ySpd, 0);
            yield return new WaitForSeconds(0.02f);
        }

        this.GetComponent<SpriteRenderer>().sprite = spriteTemp;
        phaseVal = 1;

        while(this.transform.position.x < 0f){
            this.transform.position += new Vector3(xSpd,0, 0);
            yield return new WaitForSeconds(0.02f);
        }
        this.transform.position = defaultPoint;
        phaseVal = 0;
    }


}
