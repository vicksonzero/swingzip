using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BBackButtonLeave : MonoBehaviour
{
    public bool alsoBackspace = true;
    public enum BackTo { ExitGame, nextScene };
    public BackTo onPressBackButton;
    public string sceneName;


    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || (alsoBackspace && Input.GetKeyDown(KeyCode.Backspace)))
        {
            if (onPressBackButton == BackTo.ExitGame)
            {
                Application.Quit();
            }
            else
            {
                SceneManager.LoadScene(sceneName);
            }
        }
    }
}