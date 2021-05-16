using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BParallaxBG : MonoBehaviour
{
    public float width;
    public float leftBound = 0;
    public float rightBound = 100;

    private float maxWidth = 0;
    public float height;
    public float downBound = 0;

    public float upBound = 100;
    private float maxHeight = 0;



    public Camera cam;


    public void Start()
    {
        cam = cam != null ? cam : Camera.main;
        UpdateBounds();
    }

    private void UpdateBounds()
    {
        maxWidth = rightBound - leftBound;
        maxHeight = upBound - downBound;
    }

    void LateUpdate()
    {
        float camHeight = cam.orthographicSize * 2;
        float camWidth = camHeight * cam.aspect;


        float dynamicWidth = Mathf.Max(0, maxWidth - camWidth);
        float dynamicHeight = Mathf.Max(0, maxHeight - camHeight);


        var newPos = cam.transform.position;
        newPos.z = transform.position.z;

        var scrollProgressX = Mathf.Clamp(
            ((cam.transform.position.x - camWidth / 2) / dynamicWidth - 0.5f) * 2,
            -1, 1
        );
        var scrollProgressY = Mathf.Clamp(
            ((cam.transform.position.y - camHeight / 2) / dynamicHeight - 0.5f) * 2,
            -1, 1
        );

        newPos.x -= scrollProgressX * (width - camWidth) / 2;
        newPos.y -= scrollProgressY * (height - camHeight) / 2;

        transform.position = newPos;
    }
}
