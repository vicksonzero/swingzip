

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TitleEvent", menuName = "Quest System/Events/Title Event", order = 51)]
public class TitleEvent : IEvent
{
    [Header("TitleEvent")]
    [Tooltip("Will be shown if not empty")]
    public string title = "";

    [Tooltip("Will be shown if not empty")]
    public string subtitle = "";

    [Tooltip("In seconds")]
    public float countDown = 0;

    public TitleStyle style;

    [Tooltip("Will the title, subtitle, countDown appear one after the other?")]
    public bool staggered;

    [Tooltip("Will be shown if not empty. Must click to continue")]
    public string buttonLabel = "";

    public enum TitleStyle
    {
        FADE_IN,
        FADE_IN_OUT,
        SLIDE_IN,
        SLIDE_IN_OUT,
    }
}
