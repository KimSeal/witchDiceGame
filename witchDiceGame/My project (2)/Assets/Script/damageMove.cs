using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class damageMove : MonoBehaviour
{
    private Vector3 makePlace;
    private float ySpeed;
    private float xSpeed;
    private string textSave = "";
    TextMeshPro textObj;
    // Start is called before the first frame update
    void Start()
    {
        ySpeed = Random.Range(1.0f,1.5f);
        xSpeed = Random.Range(-0.5f, 0.5f);
        makePlace = transform.position;
        textObj = GetComponent<TextMeshPro>();

        //this.GetComponent<TextMeshPro>().text = "";
        StartCoroutine(textSizeMove());


    }

    public void textChange(int damage)
    {
        textSave = damage.ToString();
    }
    // Update is called once per frame
    void Update()
    {


    }

    private IEnumerator textSizeMove()
    {
        
        //this.transform.position += new Vector3(xSpeed, ySpeed, 0);
        //ySpeed -= 0.01f;

        for (int fontSizeIdx = 0; fontSizeIdx < 10; fontSizeIdx++)
        {
            this.transform.position += new Vector3(0, 2f, 0);
            textObj.text = "<size=" + (150 - (fontSizeIdx-5) * (fontSizeIdx - 5)/6f * 2).ToString() + ">" + textSave//상단부에 적용될 text값 적기
            + "</size>";
            yield return new WaitForSeconds(0.02f);
        }
        for (int i = 15; i >0; i--)
        {
            this.transform.position += new Vector3(0, 2f, 0);

            float f = i / 15.0f;
            textObj.color = new Color(textObj.color.r, textObj.color.g, textObj.color.b, f);
            yield return new WaitForSeconds(0.02f);
        }
        Destroy(gameObject);
    }
}
