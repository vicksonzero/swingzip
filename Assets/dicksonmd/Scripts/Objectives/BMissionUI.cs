using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BMissionUI : MonoBehaviour
{
    public RectTransform missionPanel;
    public Button startButton;
    public Text recordsLabel;
    public Image missionIconImage;
    public Text missionTitleLabel;
    public Text missionDescriptionLabel;

    public RectTransform resultPanel;
    public Button retryButton;
    public Text resultsLabel;
    public Text resultsRecordsLabel;
    public Text timerLabel;

    public Camera destCamera;

    public BRouteCamera routeCamera;
    public BDistanceUI distanceUI;

    public BScoreToastSet scoreToastSet;
    
    public BTitlePanel titlePanel;
    public BCountdownPanel countdownPanel;

    public BRushHourResultsPanel rushHourResultsPanel;
    public BQuest2DeliveryQuestUi deliveryQuestUi;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
