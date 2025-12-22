using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hoverReddit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        this.GetComponent<SpriteRenderer>().material.SetInt("_Radius", 1);
    }
    private void OnMouseExit()
    {
        this.GetComponent<SpriteRenderer>().material.SetInt("_Radius", 0);
    }
}
