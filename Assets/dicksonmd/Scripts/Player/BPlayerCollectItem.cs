using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPlayerCollectItem : MonoBehaviour
{
    // Start is called before the first frame update
    public void OnTriggerEnter2D(Collider2D other)
    {
        BFuelCell item = other.GetComponent<BFuelCell>();
        if (item != null)
        {
            GetComponent<BPlayerBattery>().TryAddGrappleShots(1);
            item.playGotAnim();
            item.disableItem();
        }
    }
}
