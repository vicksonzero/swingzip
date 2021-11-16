using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BRushHourResultsPanel;

public class BRushHourMissionHandler : MonoBehaviour, IOrderHandler
{
    public string objectiveStr = "Rush Hour Delivery %countTargets%";
    [Tooltip("In Seconds， Default 300s = 5min")]
    public float missionLengthSec = 300;
    public int countdownAmount = 1;
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
        RESULT_LOCKED, // time's up during any state. show ending screen with a confirm button
        RESULT, // time's up during any state. show ending screen with a confirm button
    };
    [Header("States")]
    public States state = States.IDLE; // default IDLE

    public BDeliveryOrder currentOrder = null;

    public BMissionUI missionUI;

    public BOrderList orderList;
    public BOrderRadar orderRadar;

    public float endTime = 0;
    public float lockResultScreenForSec = 2;
    public float lockResultScreenUntil = -1;
    // Start is called before the first frame update

    RushHourDeliveryResults metrics = new RushHourDeliveryResults();

    BPlayer player;

    void Start()
    {
        if (!missionUI) missionUI = FindObjectOfType<BMissionUI>();
        if (!orderList) orderList = FindObjectOfType<BOrderList>();
        if (!orderRadar) orderRadar = FindObjectOfType<BOrderRadar>();
        if (!player) player = FindObjectOfType<BPlayer>();

        player.onChargesUsed += OnPlayerChargesUsed;

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
                    "$" + (metrics.score / 1000)
                );
                // Debug.Log(a);
                missionUI.timerLabel.text = a;
            }
            else
            {
                OnTimesUp();
            }
        }

        if (state == States.RESULT_LOCKED)
        {
            if (Time.time >= lockResultScreenUntil)
            {
                UnlockResultNextButton();
            }
        }
    }

    void IOrderHandler.OnOfferTriggerEnter(BDeliveryOrder order)
    {
        Debug.Log("OnOfferTriggerEnter");
        if (state != States.AVAILABLE) return;
        if (order == currentOrder) return;

        currentOrder = order;
        GetComponent<BCargoSpace>().OnApproachPickupPoint(currentOrder.offerCollider.transform);
        ShowOffer(order);
    }

    void IOrderHandler.OnOfferTriggerExit(BDeliveryOrder order)
    {
        if (state != States.AVAILABLE) return;
        if (order != currentOrder)
        {
            RemoveTriggersFromOrder(order);
            return;
        };
        GetComponent<BCargoSpace>().OnLeavePickupPoint();
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

        var record = PlayerPrefs.GetFloat("Record.old.gameTime", 3600);
        TimeSpan timeSpan = TimeSpan.FromSeconds(record);
        var playCount = PlayerPrefs.GetInt("Record.old.playCount", 0);
        int recordScorePerMinute = PlayerPrefs.GetInt("Record.old.scorePerMinute", 0);
        int recordLanding = PlayerPrefs.GetInt("Record.old.landing", 10000);
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
        missionUI.countdownPanel.StartCounter(countdownAmount);

        state = States.COUNTDOWN;
    }

    public void OnCountdownFinished()
    {
        Debug.Log("OnCountdownFinished");
        if (state != States.COUNTDOWN) return;

        DisableUI(missionUI.countdownPanel);

        metrics.timeMs = (int)(missionLengthSec * 1000);
        metrics.score = 0;
        metrics.orders = 0;
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


        GetComponent<BCargoSpace>().OnOrderPickedUp(currentOrder);
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

        GetComponent<BCargoSpace>().OnOrderArrive(order.destCollider.transform);
    }

    public void OnTimesUp()
    {
        if (state == States.AVAILABLE || state == States.PREPARE || state == States.IN_PROGRESS)
        {
            currentOrder = null;
            HideOffer();
            DisableUI(missionUI.distanceUI);
            DisableUI(missionUI.timerLabel);
            // TODO: Perhaps even more clean up if the stage is reused for even more stuff, but otherwise just changing the state is already doing lots of work

            var record = UpdateRecords(metrics);
            missionUI.rushHourResultsPanel.SetResultLabels(metrics);
            missionUI.rushHourResultsPanel.SetRecordLabels(record);

            DisableUI(missionUI.rushHourResultsPanel.nextButton);
            EnableUI(missionUI.rushHourResultsPanel);

            lockResultScreenUntil = Time.time + lockResultScreenForSec;
            state = States.RESULT_LOCKED;
        }
    }

    public void UnlockResultNextButton()
    {
        EnableUI(missionUI.rushHourResultsPanel.nextButton);
        state = States.RESULT;
    }

    public void UpdateScore(BDeliveryOrder order)
    {
        if (state != States.IN_PROGRESS) return;
        if (order != currentOrder) return;

        float reward = order.reward;

        metrics.score += (int)(reward * 1000);
        metrics.orders += 1;

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

    void OnPlayerChargesUsed(int charges)
    {
        metrics.chargesUsed += charges;
    }

    RushHourDeliveryResults UpdateRecords(RushHourDeliveryResults metrics)
    {
        var record = new RushHourDeliveryResults()
        {
            score = PlayerPrefs.GetInt("Record.RushHour.score", 0),
            orders = PlayerPrefs.GetInt("Record.RushHour.orders", 0),
            airTimeMs = PlayerPrefs.GetInt("Record.RushHour.airTimeMs", 0),
            topSpeed = PlayerPrefs.GetInt("Record.RushHour.topSpeed", 0),
            chargesUsed = PlayerPrefs.GetInt("Record.RushHour.chargesUsed", 999999999),
        };

        record.score = Math.Max(record.score, metrics.score);
        record.orders = Math.Max(record.orders, metrics.orders);
        record.airTimeMs = Math.Max(record.airTimeMs, metrics.airTimeMs);
        record.topSpeed = Math.Max(record.topSpeed, metrics.topSpeed);
        record.chargesUsed = Math.Min(record.chargesUsed, metrics.chargesUsed);

        PlayerPrefs.SetInt("Record.RushHour.score", record.score);
        PlayerPrefs.SetInt("Record.RushHour.orders", record.orders);
        PlayerPrefs.SetInt("Record.RushHour.airTimeMs", record.airTimeMs);
        PlayerPrefs.SetInt("Record.RushHour.topSpeed", record.topSpeed);
        PlayerPrefs.SetInt("Record.RushHour.chargesUsed", record.chargesUsed);

        return record;
    }
}
