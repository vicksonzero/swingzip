using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Directory of all Quest2 compatible UI elements for other Quest2 code to call
/// </summary>
public class BQuest2UI : MonoBehaviour
{
    public BDistanceUI distanceUI;

    public void EnableUI(Component behaviour){
        behaviour.gameObject.SetActive(true);
    }
    public void DisableUI(Component behaviour){
        behaviour.gameObject.SetActive(false);
    }
}
