using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class damageMove : MonoBehaviour
{
    private Vector3 makePlace;
    private float ySpeed;
    private float xSpeed;
    // Start is called before the first frame update
    void Start()
    {
        ySpeed = Random.Range(1.0f,1.5f);
        xSpeed = Random.Range(-0.5f, 0.5f);
        makePlace = transform.position;
        TextMeshPro tmp = GetComponent<TextMeshPro>();
        Material mat = tmp.fontMaterial;
        mat.renderQueue = 2000;
        GetComponent<MeshRenderer>().sortingOrder = 100;
        //this.GetComponent<TextMeshPro>().text = "";
    }

    public void textChange(int damage)
    {
        this.GetComponent<TextMeshPro>().text = damage.ToString();
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.position += new Vector3(xSpeed, ySpeed, 0);
        ySpeed -= 0.01f;
        if (makePlace.y - this.transform.position.y > 300) Destroy(gameObject);
    }
}
