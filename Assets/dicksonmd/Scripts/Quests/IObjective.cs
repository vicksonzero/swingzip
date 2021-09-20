using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IObjective : IStep
{
    [Header("Objective")]
    public IModifier[] modifiers;
}
