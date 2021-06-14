using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BDistanceUI : MonoBehaviour
{
    public Text distanceLabel;
    public Transform target;

    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        if (cam == null) cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        var disp = target.position - cam.transform.position;
        disp.z = 0;

        float angle = Mathf.Atan2(disp.y, disp.x) * Mathf.Rad2Deg;
        transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        distanceLabel.text = disp.magnitude.ToString("0.0") + "m";
        distanceLabel.transform.rotation = Quaternion.identity;
    }
}
