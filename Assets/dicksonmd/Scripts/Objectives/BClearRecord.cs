using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BClearRecord : MonoBehaviour
{
    public Button clearButton;
    // Start is called before the first frame update
    void Start()
    {
        if (clearButton == null)
        {
            clearButton = GetComponent<Button>();
        }
        clearButton.onClick.AddListener(() =>
        {
            PlayerPrefs.DeleteKey("Record.old.playCount");
            PlayerPrefs.DeleteKey("Record.old.gameTime");
            PlayerPrefs.DeleteKey("Record.old.scorePerMinute");
            PlayerPrefs.DeleteKey("Record.old.landing");

            PlayerPrefs.DeleteKey("Record.RushHour.score");
            PlayerPrefs.DeleteKey("Record.RushHour.orders");
            PlayerPrefs.DeleteKey("Record.RushHour.airTimeMs");
            PlayerPrefs.DeleteKey("Record.RushHour.topSpeed");
            PlayerPrefs.DeleteKey("Record.RushHour.chargesUsed");
        });
    }

}
