using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BRushHourResultsPanel : MonoBehaviour
{
    public Text summaryValueColumnLabel;
    public Text resultsValueColumnLabel;
    public Text resultsRecordColumnLabel;
    public Button nextButton;

    public struct RushHourDeliveryResults
    {
        public int timeMs;
        public int score;
        public int orders;
        public int airTimeMs;
        public int topSpeed; // mm per seconds
        public int chargesUsed;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetResultLabels(RushHourDeliveryResults r)
    {
        TimeSpan airTimeSpan = TimeSpan.FromMilliseconds(r.airTimeMs);

        var text = "" +
            (r.score / 1000) + "\n" +
            r.orders + "\n" +
            "???" + "\n" + // String.Format("{0:D2}:{1:D2}.{2:D2}", airTimeSpan.Minutes, airTimeSpan.Seconds, airTimeSpan.Milliseconds) + "\n" +
            "???" + "\n" + // (0.001f * r.topSpeed).ToString("0.00") + "\n" +
            r.chargesUsed + "\n" +
            "";

        resultsValueColumnLabel.text = text;

        TimeSpan timeSpan = TimeSpan.FromMilliseconds(r.timeMs);
        summaryValueColumnLabel.text = "" +
            String.Format("{0:D2}:{1:D2}.{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds) + "\n" +
            "";
    }
    public void SetRecordLabels(RushHourDeliveryResults r)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(r.airTimeMs);

        var text = "" +
            (r.score / 1000) + "\n" +
            r.orders + "\n" +
            "???" + "\n" + // String.Format("{0:D2}:{1:D2}.{2:D2}", timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds) + "\n" +
            "???" + "\n" + // (0.001f * r.topSpeed).ToString("0.00") + "\n" +
            r.chargesUsed + "\n" +
            "";

        resultsRecordColumnLabel.text = text;
    }
}
