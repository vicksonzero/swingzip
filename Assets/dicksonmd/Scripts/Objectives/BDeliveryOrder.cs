using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BDeliveryOrder : MonoBehaviour
{
    public bool isOrderEnabled = true;
    public bool isDrawGizmo = true;
    public bool isAutoGenerate = true;
    public string key = "";
    public string itemName = "Item";
    [TextArea]
    public string itemDescription = "Item Description";
    public Sprite itemIcon;

    [Tooltip("Order pick up UI appears when player enters the offer collider. Prefer smaller size than departCollider")]
    public BNpcTrigger2D offerCollider;

    [Tooltip("Delivery order starts when player leaves the depart collider. Prefer larger size than offerCollider")]
    public BNpcTrigger2D departCollider;

    [Tooltip("Delivery order ends when player enters the destination collider")]
    public BNpcTrigger2D destCollider;
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

    [HideInInspector]
    public bool isValidOrder = true;


    // Start is called before the first frame update
    void Start()
    {
        var interactionIconIsPrefab = (PrefabUtility.GetPrefabAssetType(interactionIcon) != PrefabAssetType.NotAPrefab);
        if (offerCollider.GetComponent<BDoor>())
        {
            if (interactionIconIsPrefab)
            {
                interactionIcon = Instantiate(interactionIcon, Vector3.zero, Quaternion.identity, transform);
            }

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
        var missionHandler = other.gameObject.GetComponent<IOrderHandler>();
        if (missionHandler != null)
        {
            missionHandler.OnOfferTriggerEnter(this);
        }
    }
    public void OnOfferTriggerExit(Collider2D other)
    {
        var missionHandler = other.gameObject.GetComponent<IOrderHandler>();
        if (missionHandler != null)
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

#if UNITY_EDITOR
    void OnValidate()
    {
        var canDoDistance = (departCollider != null && destCollider != null);
        if (reward == 0 && rewardPerDistance > 0 && canDoDistance)
        {
            var globalWaypoints = new Vector3[]{
                departCollider.transform.position,
                destCollider.transform.position,
            };
            var distance = (globalWaypoints[1] - globalWaypoints[0]).magnitude;
            reward = Mathf.Floor(distance * rewardPerDistance);
        }
        if (expectedTimeS == 0 && canDoDistance)
        {
            var globalWaypoints = new Vector3[]{
                departCollider.transform.position,
                destCollider.transform.position,
            };
            var distance = (globalWaypoints[1] - globalWaypoints[0]).magnitude;
            expectedTimeS = Mathf.Ceil(distance * 0.12f);
        }

        var colliders = FindObjectsOfType<BNpcTrigger2D>();
        if (isAutoGenerate && departCollider == null)
        {
            var index = (int)(Random.value * colliders.Length);
            Debug.Log("Random src " + index);
            departCollider = colliders[index];
            offerCollider = departCollider;
        }

        if (isAutoGenerate && destCollider == null)
        {
            destCollider = departCollider;
            for (int i = 0; destCollider == departCollider && i < 20; i++)
            {
                var index = (int)(Random.value * colliders.Length);
                Debug.Log("Random dest " + index);
                destCollider = colliders[index];
            }
        }
    }
#endif

    void OnDrawGizmosSelected()
    {
        if (!isDrawGizmo) return;
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
                if (itemIcon != null) DrawSpriteGizmo(itemIcon, globalWaypointPosPrev);
            }
        }
    }

    void DrawSpriteGizmo(Sprite sprite, Vector3 position)
    {

        Rect dstRect = new Rect(position.x - sprite.bounds.max.x,
                                 position.y + sprite.bounds.max.y,
                                 sprite.bounds.size.x,
                                 -sprite.bounds.size.y);
        // Debug.Log(sprite.bounds.size.x + " " + -sprite.bounds.size.y);

        Rect srcRect = new Rect(sprite.rect.x / sprite.texture.width,
                                 sprite.rect.y / sprite.texture.height,
                                 sprite.rect.width / sprite.texture.width,
                                 sprite.rect.height / sprite.texture.height);

        Graphics.DrawTexture(dstRect,
                             sprite.texture,
                             srcRect,
                             0, 0, 0, 0);

    }

}
