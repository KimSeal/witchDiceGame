using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Camera))]
public class PixelPerfectCamera : MonoBehaviour
{
    public int referenceResolutionWidth = 320;
    public int referenceResolutionHeight = 180;
    public int pixelsPerUnit = 16;

    private Camera cam;
    private float zoom;

    void Awake()
    {
        cam = GetComponent<Camera>();
        UpdateCamera();
    }

    void Update()
    {
        UpdateCamera();
    }

    void UpdateCamera()
    {
        cam.orthographic = true;
        float screenRatioWidth = (float)Screen.width / referenceResolutionWidth;
        float screenRatioHeight = (float)Screen.height / referenceResolutionHeight;

        zoom = Mathf.Floor(Mathf.Min(screenRatioWidth, screenRatioHeight));
        zoom = Mathf.Max(1, zoom);

        // orthoSize 설정 (height 기준)
        float orthoSize = referenceResolutionHeight / (pixelsPerUnit * 2f) / zoom;
        cam.orthographicSize = orthoSize;

        // 카메라 위치 픽셀 스냅
        float unitsPerPixel = 1f / (pixelsPerUnit * zoom);
        Vector3 camPos = cam.transform.position;
        camPos.x = Mathf.Round(camPos.x / unitsPerPixel) * unitsPerPixel;
        camPos.y = Mathf.Round(camPos.y / unitsPerPixel) * unitsPerPixel;
        cam.transform.position = camPos;

        // 레터박스 Viewport 계산
        float targetRatio = (float)referenceResolutionWidth / referenceResolutionHeight;
        float windowRatio = (float)Screen.width / Screen.height;

        if (windowRatio >= targetRatio)
        {
            // 화면이 너무 넓음 → 좌우에 빈 공간
            float viewportWidth = targetRatio / windowRatio;
            float viewportX = (1f - viewportWidth) / 2f;
            cam.rect = new Rect(viewportX, 0, viewportWidth, 1);
        }
        else
        {
            // 화면이 너무 좁음 → 위아래에 빈 공간
            float viewportHeight = windowRatio / targetRatio;
            float viewportY = (1f - viewportHeight) / 2f;
            cam.rect = new Rect(0, viewportY, 1, viewportHeight);
        }
    }
}