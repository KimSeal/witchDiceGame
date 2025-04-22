using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class textDepth : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TextMeshPro tmp = GetComponent<TextMeshPro>();
        Material mat = tmp.fontMaterial;
        mat.renderQueue = 2000;
        GetComponent<MeshRenderer>().sortingOrder = 40;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
