using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class BButtonLink : MonoBehaviour
{
    public Button button;
    public string url;


    // Use this for initialization
    void Start()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
        button.onClick.AddListener(() =>
        {
            Application.OpenURL(url);
        });
    }

}