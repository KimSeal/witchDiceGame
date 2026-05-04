using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class topniRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public float rotationSpeed = 100f;

    void Update()
    {
        // 매 프레임마다 Z축 기준으로 회전
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
