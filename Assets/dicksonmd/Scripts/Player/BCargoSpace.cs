using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BCargoSpace : MonoBehaviour
{
    public bool hasCargo = false; // TODO: add more drones

    public BCargoFollower drone;

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
        drone.FollowTarget(pickupPoint);
    }

    public void OnLeavePickupPoint()
    {
        drone.FollowTargetAfterTime(cargoAnchor, Time.time + 0.7f);
    }

    public void OnOrderPickedUp(BDeliveryOrder order)
    {
        drone.AttachItem(order.itemIcon);
        drone.FollowTarget(cargoAnchor);
    }

    public void OnOrderArrive(Transform dropOffPoint)
    {
        drone.FollowTarget(dropOffPoint);
        drone.RemoveItemsAfterTime(Time.time + 0.5f);
        drone.FollowTargetAfterTime(cargoAnchor, Time.time + 1.5f);
    }
}
