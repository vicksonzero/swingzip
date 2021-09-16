using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeLimitModifier", menuName = "Quest System/Modifiers/Time Limit Modifier", order = 51)]
[System.Serializable]
public class TimeLimitModifier : IModifier
{
    public string label = "Timer";
    [Tooltip("In seconds")]
    public float time = 0;



}
