using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BRecordLabel : MonoBehaviour
{

    public Text label;
    // Start is called before the first frame update
    void Start()
    {
        if (label == null)
        {
            label = GetComponent<Text>();
        }

        var record = PlayerPrefs.GetFloat("Record.old.gameTime", 3600);
        TimeSpan timeSpan = TimeSpan.FromSeconds(record);
        var playCount = PlayerPrefs.GetInt("Record.old.playCount", 0);
        int recordScorePerMinute = PlayerPrefs.GetInt("Record.old.scorePerMinute", 0);
        int recordLanding = PlayerPrefs.GetInt("Record.old.landing", 10000);
        label.text = ("" +
            playCount + "\n" +
            String.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds) + "\n" +
            (recordScorePerMinute / 1000f) + "\n" +
            recordLanding
        );
    }

    // Update is called once per frame
    void Update()
    {

    }
}
