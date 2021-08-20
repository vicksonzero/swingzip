using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BCargoSpace : MonoBehaviour
{
    [Tooltip("Only 1 is supported now")]
    public int cargoSpace = 1; // TODO: add more drones

    public BCargoFollower[] drones;

    public Transform cargoAnchor;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnApproachPickupPoint(Transform pickupPoint)
    {
        drones[0].FollowTarget(pickupPoint);
    }
    public void OnLeavePickupPoint()
    {
        drones[0].FollowTargetAfterTime(cargoAnchor, Time.time + 0.7f);
    }
    public void OnOrderPickedUp(BDeliveryOrder order)
    {
        drones[0].AttachItem(order.itemIcon);
        drones[0].FollowTarget(cargoAnchor);
    }

    public void OnOrderArrive(Transform dropOffPoint)
    {
        drones[0].FollowTarget(dropOffPoint);
        drones[0].RemoveItemsAfterTime(Time.time + 0.5f);
        drones[0].FollowTargetAfterTime(cargoAnchor, Time.time + 1.5f);
    }
}
