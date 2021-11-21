using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Brain of a quest receiver, handler, as a Rider in the Company
/// Owns all CargoSpaces, get a CargoSpace and assign cargo
/// tracks BQuest2Objective progress that are owned by BQuest2ObjectiveList
/// </summary>
public class BQuest2Rider : MonoBehaviour, IOrderHandler
{
    public BQuest2Quest[] currentQuests;
    
    BQuest2UI ui;
    BCargoSpace[] cargoSpaces;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void IOrderHandler.OnOfferTriggerEnter(BDeliveryOrder order)
    {
        Debug.Log("BQuest2Rider.OnOfferTriggerEnter");
    }

    void IOrderHandler.OnOfferTriggerExit(BDeliveryOrder order)
    {
        Debug.Log("BQuest2Rider.OnOfferTriggerEnter");
    }
}
