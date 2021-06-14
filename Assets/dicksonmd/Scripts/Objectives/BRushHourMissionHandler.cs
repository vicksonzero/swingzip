using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRushHourMissionHandler : MonoBehaviour
{
    public string objectiveStr = "Rush Hour Delivery";
    // IDLE -> AVAILABLE
    // AVAILABLE -> PREPARE
    // PREPARE -> IN_PROGRESS
    // IN_PROGRESS -> AVAILABLE
    // any -> RESULT
    public enum States
    {
        IDLE, // before rush hour actually starts, show some UI
        AVAILABLE, // during rush hour, finding orders to pick up
        PREPARE, // picked up an order, not yet started running
        IN_PROGRESS, // running; on fulfilling an order, goes back to AVAILABLE
        RESULT, // time's up during any state. show ending screen with a confirm button
    };
    [Header("States")]
    public States state = States.AVAILABLE; // default IDLE

    public BDeliveryOrder currentOrder = null;

    public BMissionUI missionUI;

    public float score = 0;

    public float endTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (!missionUI) missionUI = FindObjectOfType<BMissionUI>();

        missionUI.timerLabel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == States.IN_PROGRESS)
        {
            var seconds = Time.time - endTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(Time.time - endTime);
            var timeStr = String.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
            var a = (
                objectiveStr + "\n" +
                timeStr + "\n" +
                "$" + score
            );
            // Debug.Log(a);
            missionUI.timerLabel.text = a;
        }
    }

    public void OnOfferTriggerEnter(BDeliveryOrder order)
    {
        Debug.Log("OnOfferTriggerEnter");
        if (state != States.AVAILABLE) return;
        if (order == currentOrder) return;

        currentOrder = order;
        ShowOffer(order);
    }

    public void OnOfferTriggerExit(BDeliveryOrder order)
    {
        if (state != States.AVAILABLE) return;
        if (order != currentOrder)
        {
            RemoveTriggersFromOrder(order);
            return;
        };
        currentOrder = null;
        HideOffer();
    }

    public void ShowOffer(BDeliveryOrder order)
    {
        Debug.Log("ShowOffer");
        missionUI.missionTitleLabel.text = order.itemName;
        missionUI.missionDescriptionLabel.text = order.itemDescription;
        missionUI.missionIconImage.sprite = order.itemIcon;

        var record = PlayerPrefs.GetFloat("Record.gameTime", 3600);
        TimeSpan timeSpan = TimeSpan.FromSeconds(record);
        var playCount = PlayerPrefs.GetInt("Record.playCount", 0);
        int recordScorePerMinute = PlayerPrefs.GetInt("Record.scorePerMinute", 0);
        int recordLanding = PlayerPrefs.GetInt("Record.landing", 10000);
        missionUI.recordsLabel.text = ("" +
            playCount + "\n" +
            String.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds) + "\n" +
            (recordScorePerMinute / 1000f) + "\n" +
            recordLanding
        );

        missionUI.startButton.onClick.AddListener(OnOrderPickedUp);

        missionUI.missionPanel.gameObject.SetActive(true);
    }

    public void HideOffer()
    {
        missionUI.missionPanel.gameObject.SetActive(false);
    }

    public void AddTriggersToOrder(BDeliveryOrder order)
    {
        order.departCollider.triggerExit += OnOrderDepart;
        order.destCollider.triggerEnter += OnOrderArrive;
    }
    public void RemoveTriggersFromOrder(BDeliveryOrder order)
    {
        order.departCollider.triggerExit -= OnOrderDepart;
        order.destCollider.triggerEnter -= OnOrderArrive;
    }



    public void OnOrderPickedUp()
    {
        if (state != States.AVAILABLE) return;

        state = States.PREPARE;
        AddTriggersToOrder(currentOrder);
        HideOffer();
    }
    public void OnOrderDepart(Collider2D other)
    {
        if (state != States.PREPARE) return;

        state = States.IN_PROGRESS;
    }

    public void OnOrderArrive(Collider2D other)
    {
        if (state != States.IN_PROGRESS) return;

        currentOrder = null;
        state = States.AVAILABLE;
    }
}