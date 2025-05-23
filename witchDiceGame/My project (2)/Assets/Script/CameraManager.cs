using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

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
    float ZoomTime = -1f;
    float ShakeTime;
    Vector3 initialPosition;

    // Start is called before the first frame update
    public void VibrateForeTime(float time) {
        Debug.Log("camera Shake");
        ShakeTime = time;
    }
    public int pixelWidth = 384;  // 낮은 해상도 너비
    public int pixelHeight = 216;  // 낮은 해상도 높이

    void Start()
    {
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        tempSize = gameObject.GetComponent<UnityEngine.U2D.PixelPerfectCamera>().assetsPPU * timeDelay;
        Debug.Log("hey! " + tempSize);
        initialPosition = transform.position;
        ZoomTime = -1f;

    }

    int tempSize;
    int direction = 0;
    int timeDelay = 3;

    // Update is called once per frame
    void Update()
    {
        if (ZoomTime > 0)
        {
            
            if (direction == 0)
            {
                gameObject.GetComponent<UnityEngine.U2D.PixelPerfectCamera>().assetsPPU = (++tempSize) / 3;
                if (tempSize >= 130 * timeDelay)
                {
                    tempSize = 130 * timeDelay;
                    direction = 1;
                }
            }
            
            if (direction == 1)
            {
                gameObject.GetComponent<UnityEngine.U2D.PixelPerfectCamera>().assetsPPU = (--tempSize) / 3;
                if (tempSize <= 100 * timeDelay)
                {
                    tempSize = 100 * timeDelay;
                    direction = 0;
                    ZoomTime = -1;

                }
            }
           

        }
        else
        {
            if (ShakeTime > 0)
            {
                Vector3 temp = Random.insideUnitSphere * ShakeAmount + initialPosition;
                transform.position = new Vector3(temp.x, temp.y, transform.position.z);

                ShakeTime -= Time.deltaTime;
            }
            else
            {

                ShakeTime = 0.0f;
                transform.position = initialPosition;
            }
        }




    }
    public void zoomEvent()
    {
        ZoomTime = 1;
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
        ShakeTime = 0.0f;
    }
}
