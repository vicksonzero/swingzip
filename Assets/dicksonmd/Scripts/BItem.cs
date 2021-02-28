using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BItem : MonoBehaviour
{
    public bool spawnVisible = true;
    public bool canRespawn = false;
    public float respawnTime = 10;
    public void disableItem(bool respawn = true)
    {
        Debug.Log("disable " + respawn);
        if (canRespawn && respawn)
        {
            FindObjectOfType<BItemsManager>().waitAndRespawn(this, respawnTime);
            disableSelf();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void enableItem()
    {
        enableSelf();
    }

    public void playGotAnim()
    {
        playGotAnimSelf();
    }

    // Start is called before the first frame update
    internal void Start()
    {
        if (spawnVisible)
        {
            enableItem();
        }
        else
        {
            disableItem();
        }
    }
    internal virtual void disableSelf()
    {

    }
    internal virtual void enableSelf()
    {

    }
    internal virtual void playGotAnimSelf()
    {

    }
}
