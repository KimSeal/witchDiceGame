using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hoverDark : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseOver()
    {
        gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.7f);
    }
    private void OnMouseExit()
    {
        gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_Transparency", 0.0f);

    }
}
