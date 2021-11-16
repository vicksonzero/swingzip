using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BCargoSpace))]
public class BQuest2 : MonoBehaviour, IOrderHandler
{
    private BCargoSpace cargoSpace;

    void IOrderHandler.OnOfferTriggerEnter(BDeliveryOrder order)
    {
        throw new System.NotImplementedException();
    }

    void IOrderHandler.OnOfferTriggerExit(BDeliveryOrder order)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        cargoSpace = GetComponent<BCargoSpace>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
