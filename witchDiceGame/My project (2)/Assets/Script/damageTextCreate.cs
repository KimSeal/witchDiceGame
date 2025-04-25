using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class damageTextCreate : MonoBehaviour
{
    [SerializeField]
    private GameObject damageText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Instantiate(damageText, this.transform.position, new Quaternion(0,0,0,0));
    }
}
