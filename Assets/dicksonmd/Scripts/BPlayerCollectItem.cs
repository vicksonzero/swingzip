using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPlayerCollectItem : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter");
        BFuelCell item = other.GetComponent<BFuelCell>();
        if (item!=null)
        {
            item.playGotAnim();
            item.disableItem();
        }
    }
}
