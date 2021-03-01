using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BForceFPS : MonoBehaviour
{
    public int targetFrameRate = 60;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
