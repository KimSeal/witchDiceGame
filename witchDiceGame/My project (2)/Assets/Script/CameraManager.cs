using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance = null;
    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static CameraManager Instance
    {
        get
        {
            if (null == instance) { return null; }
            return instance;
        }
    }


    public float ShakeAmount;
    float ShakeTime;
    Vector3 initialPosition;

    // Start is called before the first frame update
    public void VibrateForeTime(float time) {
        Debug.Log("camera Shake");
        ShakeTime = time;
    }


    void Start()
    {
        initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (ShakeTime > 0)
        {
            Vector3 temp = Random.insideUnitSphere * ShakeAmount + initialPosition;
            transform.position = new Vector3(temp.x, temp.y, transform.position.z);

            ShakeTime -= Time.deltaTime;
        }
        else {
            
            ShakeTime = 0.0f;
            transform.position = initialPosition;
        }


    }

    public float camraPointX()
    {
        return this.transform.position.x;
    }
    public float camraPointY()
    {
        return this.transform.position.y;
    }
    public float camraPointZ()
    {
        return this.transform.position.z;
    }
    public void updateInitPosition(Vector3 vec )
    {
        initialPosition = vec;
    }
}
