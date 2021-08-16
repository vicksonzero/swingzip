using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BCountdownPanel : MonoBehaviour
{
    // Start is called before the first frame update
    public Text counter;

    public CountdownFinishedEvent onCountdownFinished { get; set; }

    public delegate void CountdownFinishedEvent();

    float countUntil = -1;

    void Update()
    {
        if (countUntil < 0) return;
        if (Time.time >= countUntil)
        {
            onCountdownFinished.Invoke();
            StopCounter();
            return;
        }
        counter.text = "" + Mathf.Ceil(countUntil - Time.time);
    }

    public void StartCounter(int count)
    {
        countUntil = Time.time + count;
        counter.text = "" + Mathf.Ceil(countUntil - Time.time);
        counter.gameObject.SetActive(true);
    }
    public void StopCounter()
    {
        countUntil = -1;
        counter.gameObject.SetActive(false);
    }
}
