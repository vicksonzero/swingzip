using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BCargoFollower : MonoBehaviour
{
    public Transform target;
    public float maxDistance = 5;
    public float smoothTime = 0.3f;
    public float floatingFrequency = 2f;
    public float floatingAmplitude = 1;

    private float nextFloat = 0;
    private Vector3 floatingOffset = Vector3.zero;

    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Time.time >= nextFloat)
        {
            floatingOffset = Random.insideUnitCircle * floatingAmplitude;
            nextFloat = Time.time + floatingFrequency;
        }


        var _smoothTime = smoothTime;
        var targetPosition = target.position + floatingOffset;

        var displacement = transform.position - targetPosition;
        if (displacement.magnitude > maxDistance)
        {
            _smoothTime = 0.01f;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
