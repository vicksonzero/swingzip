using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BDeliveryOrder : MonoBehaviour
{
    public bool isOrderEnabled = true;
    public string itemName = "Item";
    [TextArea]
    public string itemDescription = "Item Description";
    public Sprite itemIcon;

    [Tooltip("Order pick up UI appears when player enters the offer collider. Prefer smaller size than departCollider")]
    public BColliderHandlers2D offerCollider;

    [Tooltip("Delivery order starts when player leaves the depart collider. Prefer larger size than offerCollider")]
    public BColliderHandlers2D departCollider;

    [Tooltip("Delivery order ends when player enters the destination collider")]
    public BColliderHandlers2D destCollider;
    public BInteractionButton interactionIcon;

    public float rewardPerDistance = 0;
    public float reward = 0;

    public float penaltyPerMiss = 0;

    public float expectedTimeS = 10;
    public float quickReward = 50;
    public delegate void OrderDepartEvent(BDeliveryOrder order);
    public OrderDepartEvent orderDepart;
    public delegate void OrderArriveEvent(BDeliveryOrder order);
    public OrderArriveEvent orderArrive;


    // Start is called before the first frame update
    void Start()
    {
        if (offerCollider.GetComponent<BDoor>())
        {
            interactionIcon.transform.position = offerCollider.GetComponent<BDoor>().iconRoot.position;
        }
        offerCollider.triggerEnter += OnOfferTriggerEnter;
        offerCollider.triggerExit += OnOfferTriggerExit;

        departCollider.triggerExit += OnOrderDepart;
        destCollider.triggerEnter += OnOrderArrive;
        reward = Mathf.Floor(rewardPerDistance * (departCollider.transform.position - destCollider.transform.position).magnitude);
    }

    public void OnOfferTriggerEnter(Collider2D other)
    {
        Debug.Log("triggerEnter");
        var missionHandler = other.GetComponent<BRushHourMissionHandler>();
        if (missionHandler)
        {
            missionHandler.OnOfferTriggerEnter(this);
        }
    }
    public void OnOfferTriggerExit(Collider2D other)
    {
        var missionHandler = other.GetComponent<BRushHourMissionHandler>();
        if (missionHandler)
        {
            missionHandler.OnOfferTriggerExit(this);
        }
    }
    public void OnOrderDepart(Collider2D other)
    {
        if (orderDepart != null) orderDepart(this);
    }

    public void OnOrderArrive(Collider2D other)
    {
        if (orderArrive != null) orderArrive(this);
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void ToggleIcon(bool val)
    {
        interactionIcon.ToggleIcon(val && isOrderEnabled);
    }

    public void ToggleIconDim(bool val)
    {
        interactionIcon.ToggleIconDim(val && isOrderEnabled);
    }

    public void DisableOrder()
    {
        this.isOrderEnabled = false;

        ToggleIcon(false);
        offerCollider.triggerEnter -= OnOfferTriggerEnter;
        offerCollider.triggerExit -= OnOfferTriggerExit;
    }

    void OnDrawGizmos()
    {
        if (!isOrderEnabled) return;
        if (departCollider == null) return;
        if (destCollider == null) return;

        var globalWaypoints = new Vector3[]{
            departCollider.transform.position,
            destCollider.transform.position,
        };

        float size = 3f;

        for (int i = 0; i < globalWaypoints.Length; i++)
        {
            Vector3 globalWaypointPos = globalWaypoints[i];
            if (i > 0)
            {
                Vector3 globalWaypointPosPrev = globalWaypoints[i - 1];
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(globalWaypointPosPrev, globalWaypointPos);
                Gizmos.color = Color.white;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }
}
