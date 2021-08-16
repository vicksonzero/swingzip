using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRushHourMissionHandler : MonoBehaviour
{
    public string objectiveStr = "Rush Hour Delivery %countTargets%";
    [Tooltip("In Seconds")]
    public float missionLengthSec = 300;
    // IDLE -> AVAILABLE
    // AVAILABLE -> PREPARE
    // PREPARE -> IN_PROGRESS
    // IN_PROGRESS -> AVAILABLE
    // any -> RESULT
    public enum States
    {
        IDLE, // before rush hour actually starts, show some UI
        COUNTDOWN, // before rush hour actually starts, show countdown
        AVAILABLE, // during rush hour, finding orders to pick up
        PREPARE, // picked up an order, not yet started running
        IN_PROGRESS, // running; on fulfilling an order, goes back to AVAILABLE
        RESULT, // time's up during any state. show ending screen with a confirm button
    };
    [Header("States")]
    public States state = States.IDLE; // default IDLE

    public BDeliveryOrder currentOrder = null;

    public BMissionUI missionUI;

    public BOrderList orderList;
    public BOrderRadar orderRadar;

    public float score = 0;

    public float endTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (!missionUI) missionUI = FindObjectOfType<BMissionUI>();
        if (!orderList) orderList = FindObjectOfType<BOrderList>();
        if (!orderRadar) orderRadar = FindObjectOfType<BOrderRadar>();

        DisableUI(missionUI.timerLabel);

        missionUI.titlePanel.startButton.onClick.AddListener(OnStartButtonClicked);
        missionUI.countdownPanel.onCountdownFinished += OnCountdownFinished;

        EnableUI(missionUI.titlePanel);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == States.AVAILABLE || state == States.PREPARE || state == States.IN_PROGRESS)
        {
            var seconds = endTime - Time.time;
            if (seconds > 0)
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
                var timeStr = String.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
                var countTargetsStr = String.Format("({0:D}/{1:D})", orderList.orderCount - orderList.orderFinishedCount, orderList.orderCount);
                var objectiveStrFiltered = objectiveStr.Replace("%countTargets%", countTargetsStr);
                var a = (
                    objectiveStrFiltered + "\n" +
                    timeStr + "\n" +
                    "$" + score
                );
                // Debug.Log(a);
                missionUI.timerLabel.text = a;
            }
            else
            {
                OnTimesUp();
            }
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
        DisableUI(missionUI.distanceUI);
    }

    public void ShowOffer(BDeliveryOrder order)
    {
        Debug.Log("ShowOffer");
        missionUI.missionTitleLabel.text = order.itemName;
        missionUI.missionDescriptionLabel.text = order.itemDescription;
        missionUI.missionIconImage.sprite = order.itemIcon;

        var pos = missionUI.destCamera.transform.position;
        pos.x = order.destCollider.transform.position.x;
        pos.y = order.destCollider.transform.position.y;
        missionUI.destCamera.transform.position = pos;

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

        missionUI.distanceUI.target = order.destCollider.transform;
        EnableUI(missionUI.distanceUI);
        EnableUI(missionUI.missionPanel);
    }

    public void HideOffer()
    {
        DisableUI(missionUI.missionPanel);
    }

    public void AddTriggersToOrder(BDeliveryOrder order)
    {
        order.orderDepart += OnOrderDepart;
        order.orderArrive += OnOrderArrive;
    }
    public void RemoveTriggersFromOrder(BDeliveryOrder order)
    {
        order.orderDepart -= OnOrderDepart;
        order.orderArrive -= OnOrderArrive;
    }



    public void OnStartButtonClicked()
    {
        if (state != States.IDLE) return;
        DisableUI(missionUI.titlePanel);
        EnableUI(missionUI.countdownPanel);
        missionUI.countdownPanel.StartCounter(3);

        state = States.COUNTDOWN;
    }

    public void OnCountdownFinished()
    {
        Debug.Log("OnCountdownFinished");
        if (state != States.COUNTDOWN) return;

        DisableUI(missionUI.countdownPanel);

        endTime = Time.time + missionLengthSec;
        EnableUI(missionUI.timerLabel);
        state = States.AVAILABLE;
    }
    public void OnOrderPickedUp()
    {
        if (state != States.AVAILABLE) return;

        state = States.PREPARE;
        AddTriggersToOrder(currentOrder);
        ToggleOrdersAvailable(false);
        currentOrder.ToggleIconDim(true);

        if (currentOrder.destCollider.GetComponent<BDoor>())
        {
            currentOrder.interactionIcon.transform.position =
                currentOrder.destCollider.GetComponent<BDoor>().iconRoot.position;
        }

        orderRadar.isRadarEnabled = false;
        HideOffer();
    }
    public void OnOrderDepart(BDeliveryOrder order)
    {
        if (state != States.PREPARE) return;
        if (order != currentOrder) return;

        state = States.IN_PROGRESS;
    }

    public void OnOrderArrive(BDeliveryOrder order)
    {
        if (state != States.IN_PROGRESS) return;
        if (order != currentOrder) return;

        UpdateScore(currentOrder);
        currentOrder.DisableOrder();
        orderList.UpdateOrderCount();
        currentOrder = null;

        DisableUI(missionUI.distanceUI);

        state = States.AVAILABLE;
        ToggleOrdersAvailable(true);
        orderRadar.isRadarEnabled = true;
    }

    public void OnTimesUp()
    {
        if (state == States.AVAILABLE || state == States.PREPARE || state == States.IN_PROGRESS)
        {
            currentOrder = null;
            HideOffer();
            DisableUI(missionUI.distanceUI);
            // TODO: Perhaps even more clean up if the stage is reused for even more stuff, but otherwise just changing the state is already doing lots of work


            EnableUI(missionUI.resultPanel);

            state = States.RESULT;
        }
    }

    public void UpdateScore(BDeliveryOrder order)
    {
        if (state != States.IN_PROGRESS) return;
        if (order != currentOrder) return;

        float reward = order.reward;

        score += reward;

        missionUI.scoreToastSet.AddToast("+" + reward);
    }

    void ToggleOrdersAvailable(bool val)
    {
        var orders = FindObjectsOfType<BDeliveryOrder>();
        foreach (var order in orders)
        {
            order.ToggleIconDim(val);
        }
    }

    void EnableUI(Component behaviour)
    {
        behaviour.gameObject.SetActive(true);
    }
    void DisableUI(Component behaviour)
    {
        behaviour.gameObject.SetActive(false);
    }
}
