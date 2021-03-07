using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BLivesMeter : MonoBehaviour
{
    public GameObject[] icons;
    // Start is called before the first frame update

    public void SetLives(int lives)
    {
        for (int i = 0; i < lives && i < icons.Length; i++)
        {
            icons[i].SetActive(true);
        }
        for (int i = lives; i >= 0 && i < icons.Length; i++)
        {
            icons[i].SetActive(false);
        }
    }
}
