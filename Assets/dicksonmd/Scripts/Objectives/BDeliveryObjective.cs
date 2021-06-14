using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BDeliveryObjective : MonoBehaviour
{
    public enum States { IDLE, START, IN_PROGRESS, RESULT };
    [Header("States")]
    public States state = States.IDLE;
    [Header("Linkage")]

    public Transform missionStart;
    public Transform missionEnd;
    public RectTransform missionPanel;
    public Button startButton;
    public Text recordsLabel;

    public RectTransform resultPanel;
    public Button retryButton;
    public Text resultsLabel;
    public Text resultsRecordsLabel;
    public Text timerLabel;

    public float startTime;
    public float gameTime = 0;

    public float score = 0;
    public int landing = 0;


    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(() =>
        {
            if (state == States.IDLE)
            {
                state = States.START;
                missionPanel.gameObject.SetActive(false);
            }
        });

        missionEnd.GetComponent<BMissionEnd>().triggerEnter += (Collider2D Collider) =>
        {
            if (state == States.IN_PROGRESS)
            {
                gameTime = Time.time - startTime;

                var record = PlayerPrefs.GetFloat("Record.gameTime", Mathf.Infinity);
                record = Mathf.Min(record, gameTime);
                PlayerPrefs.SetFloat("Record.gameTime", record);


                var playCount = PlayerPrefs.GetInt("Record.playCount", 0);
                playCount++;
                PlayerPrefs.SetInt("Record.playCount", playCount);

                int recordScorePerMinute = PlayerPrefs.GetInt("Record.scorePerMinute", 0);
                int scorePerMinute = (int)(score * 1000 / (gameTime / 60));
                recordScorePerMinute = Mathf.Max(recordScorePerMinute, scorePerMinute);
                PlayerPrefs.SetInt("Record.scorePerMinute", recordScorePerMinute);

                int recordLanding = PlayerPrefs.GetInt("Record.landing", 10000);
                recordLanding = Mathf.Min(recordLanding, landing);
                PlayerPrefs.SetInt("Record.landing", recordLanding);

                TimeSpan timeSpan = TimeSpan.FromSeconds(Time.time - startTime);
                resultsLabel.text = ("" +
                    String.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds) + "\n" +
                    (scorePerMinute / 1000.0f) + "\n" +
                    landing
                );
                timeSpan = TimeSpan.FromSeconds(record);
                resultsRecordsLabel.text = ("" +
                    playCount + "\n" +
                    String.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds) + "\n" +
                    (recordScorePerMinute / 1000.0f) + "\n" +
                    recordLanding
                );

                timerLabel.gameObject.SetActive(false);
                resultPanel.gameObject.SetActive(true);
                retryButton.gameObject.SetActive(false);
                state = States.RESULT;
            }
        };

        retryButton.onClick.AddListener(() =>
        {
            missionEnd.gameObject.SetActive(false);
            timerLabel.gameObject.SetActive(false);
            retryButton.gameObject.SetActive(false);

            FindObjectOfType<BPlayer>().transform.position = missionStart.transform.position + Vector3.up * 1.6f;

            state = States.IDLE;
        });
        // missionEnd.GetComponent<BMissionEnd>().triggerExit += (Collider2D Collider) =>
        // {
        //     if (state == States.RESULT)
        //     {
        //         missionEnd.gameObject.SetActive(false);
        //         resultPanel.gameObject.SetActive(false);
        //         state = States.IDLE;
        //     }
        // };
        missionEnd.gameObject.SetActive(false);
        missionPanel.gameObject.SetActive(false);
        resultPanel.gameObject.SetActive(false);
        timerLabel.gameObject.SetActive(false);
        retryButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == States.IN_PROGRESS)
        {
            var seconds = Time.time - startTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(Time.time - startTime);
            var a = (
                String.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds) + "\n" +
                score
            );
            // Debug.Log(a);
            timerLabel.text = a;
        }
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<BPlayer>())
        {
            if (state == States.IDLE)
            {
                var record = PlayerPrefs.GetFloat("Record.gameTime", 3600);
                TimeSpan timeSpan = TimeSpan.FromSeconds(record);
                var playCount = PlayerPrefs.GetInt("Record.playCount", 0);
                int recordScorePerMinute = PlayerPrefs.GetInt("Record.scorePerMinute", 0);
                int recordLanding = PlayerPrefs.GetInt("Record.landing", 10000);
                recordsLabel.text = ("" +
                    playCount + "\n" +
                    String.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds) + "\n" +
                    (recordScorePerMinute / 1000f) + "\n" +
                    recordLanding
                );

                missionPanel.gameObject.SetActive(true);
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.GetComponent<BPlayer>())
        {
            missionPanel.gameObject.SetActive(false);

            if (state == States.START)
            {
                state = States.IN_PROGRESS;
                startTime = Time.time;
                score = 0;
                landing = 0;
                missionEnd.gameObject.SetActive(true);
                timerLabel.gameObject.SetActive(true);
                retryButton.gameObject.SetActive(true);
            }
        }
    }

    public void AddScore(float amt)
    {
        score += amt;
    }

    public void AddLanding(int amt)
    {
        landing += amt;
    }
}
