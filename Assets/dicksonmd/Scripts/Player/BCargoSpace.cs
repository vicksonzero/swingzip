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

    private BCargoFollower GetVacantDrone()
    {
        // TODO: get a vacant drone instead of default to #0
        return drones[0];
    }

    private BCargoFollower GetAssignedDrone(Transform t)
    {
        // TODO: get assigned drone instead of default to #0
        return drones[0];
    }


    public void OnApproachPickupPoint(Transform pickupPoint)
    {
        // TODO: set self state to parked state, and coordinate all child drones
        var drone = GetVacantDrone();
        if (drone == null) return;

        // TODO: store this drone in holder

        drone.FollowTarget(pickupPoint);
    }

    public void OnLeavePickupPoint()
    {
        // TODO: set self state to parked state, and coordinate all child drones
        for (int i = 0; i < drones.Length; i++)
        {
            drones[i].FollowTargetAfterTime(cargoAnchor, Time.time + 0.7f);
        }
    }

    public void OnOrderPickedUp(BDeliveryOrder order)
    {
        // TODO: get drone from holder instead of getting a vacant one agin
        var drone = GetVacantDrone();
        if (drone == null) return;

        drone.AttachItem(order.itemIcon);
        drone.FollowTarget(cargoAnchor);
    }

    public void OnOrderArrive(Transform dropOffPoint)
    {
        var drone = GetAssignedDrone(dropOffPoint);
        if (drone == null) return;

        drone.FollowTarget(dropOffPoint);
        drone.RemoveItemsAfterTime(Time.time + 0.5f);
        drone.FollowTargetAfterTime(cargoAnchor, Time.time + 1.5f);
    }
}
