using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BScoreToastSet : MonoBehaviour
{
    public BScoreToast toastPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddToast(string msg)
    {
        var toast = Instantiate(toastPrefab, transform);
        toast.label.text = msg;
    }
}
