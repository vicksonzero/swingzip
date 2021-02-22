using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BVersionNumber : MonoBehaviour
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
        label.text = (template
            .Replace("%version%", Application.version)
            .Replace("%platform%", Application.platform.ToString())
            .Replace("%platformFlags%", Application.isMobilePlatform ? " (Mobile)" : "")
            .Replace("%datetime%", DateTime.Now.ToString("yyyyMMdd_hhmmss"))
        );
    }
}
