using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BItemsManager : MonoBehaviour
{

    public void waitAndRespawn(BItem obj, float time)
    {
        StartCoroutine(_waitAndRespawn(obj, time));
    }
    IEnumerator _waitAndRespawn(BItem obj, float time)
    {
        yield return new WaitForSeconds(time);
        respawn(obj);
    }

    public void respawn(BItem obj)
    {
        obj.enableSelf();
    }
}
