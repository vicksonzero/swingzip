using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSmoothFollower : MonoBehaviour
{
    public Transform target;
    public float maxDistance = 5;
    public float smoothTime = 0.3f;
    Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null) return;

        var _smoothTime = smoothTime;
        Vector3 targetPosition = target.position; // + floatingOffset;
        targetPosition.z = transform.position.z;

        Vector3 displacement = transform.position - targetPosition;
        if (displacement.magnitude > maxDistance)
        {
            _smoothTime = 0.01f;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}
