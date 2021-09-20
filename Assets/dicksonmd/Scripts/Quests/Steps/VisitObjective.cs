

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VisitObjective", menuName = "Quest System/Objectives/Visit Objective", order = 51)]
public class VisitObjective : IObjective
{
    [Header("VisitObjective")]
    public string destinationId;
}
