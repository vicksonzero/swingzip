using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BBreathingImage : MonoBehaviour
{
    [Tooltip("can be null")]
    public Image image;
    public float speed = 1;
    public float min = 0.6f;
    public float max = 1;

    private float alphaProgress = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (image == null) image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        alphaProgress += speed * Time.deltaTime;
        var color = image.color;
        color.a = min + (Mathf.Sin(alphaProgress) + 1) / 2 * (max - min);
        image.color = color;
    }
}
