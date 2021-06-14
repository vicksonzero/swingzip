using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BMissionEnd : MonoBehaviour
{
    public delegate void TriggerEnterEvent(Collider2D collider);
    public TriggerEnterEvent triggerEnter;
    public delegate void TriggerExitEvent(Collider2D collider);
    public TriggerExitEvent triggerExit;

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (triggerEnter != null)
        {
            triggerEnter(collider);
        }
    }
    // Start is called before the first frame update
    public void OnTriggerExit2D(Collider2D collider)
    {
        if (triggerExit != null)
        {
            triggerExit(collider);
        }
    }
}
