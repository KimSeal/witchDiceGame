using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript3 : MonoBehaviour
{
    [SerializeField]
    public float yAmount = 10f;
    [SerializeField]
    public float yAxcel = 0.1f;
    [SerializeField]
    public float spinAxcel = 0;
    [SerializeField]
    public float spinVal = 0;

    private float yVal = 0;

    Vector3 defaultPoint = new Vector3(-1561.9f, -659f, 0f);
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(this.transform.position.x, -538.8f + yAmount * Mathf.Sin(yVal * Mathf.PI), 0);
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Sin(spinVal * Mathf.PI));

        yVal += yAxcel;
        spinVal += spinAxcel;
    }



}
