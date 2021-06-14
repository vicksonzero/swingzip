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
            PlayerPrefs.DeleteKey("Record.playCount");
            PlayerPrefs.DeleteKey("Record.gameTime");
            PlayerPrefs.DeleteKey("Record.scorePerMinute");
            PlayerPrefs.DeleteKey("Record.landing");
        });
    }

}
