using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BFPSLabel : MonoBehaviour
{
    public Text label;
    public string template = "";
    // Start is called before the first frame update
    void Start()
    {
        if (label == null)
        {
            label = GetComponent<Text>();
        }
        if (template == "")
        {
            template = label.text;
        }
    }

    void Update()
    {
        label.text = (template
            .Replace("%fps%", "" + (int)(1f / Time.smoothDeltaTime))
        );
    }
}
