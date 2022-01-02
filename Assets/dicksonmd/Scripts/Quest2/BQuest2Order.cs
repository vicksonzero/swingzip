using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Static data for an item to be delivered
/// Can generate in editor or in runtime
/// </summary>
public class BQuest2Order : MonoBehaviour
{
    public bool isDrawGizmo = true;
    public bool isAutoGenerate = true;
    public string key = "";
    public string itemName = "Item";
    [TextArea]
    public string itemDescription = "Item Description";
    public Sprite itemIcon;

    public BQuest2Npc fromNpc;

    public BQuest2Npc toNpc;

    public BInteractionButton interactionIcon; // TODO: move button to be owned by the NPC

    public float rewardPerDistance = 0;
    public float reward = 0;

    public float penaltyPerMiss = 0;

    public float expectedTimeS = 10;
    public float quickReward = 50;

    [HideInInspector]
    public bool isValidOrder = true;

    [HideInInspector]
    public bool isGeneratedRuntime = false;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

#if UNITY_EDITOR
    void OnValidate()
    {
        var canDoDistance = (fromNpc != null && toNpc != null);
        if (reward == 0 && rewardPerDistance > 0 && canDoDistance)
        {
            var globalWaypoints = new Vector3[]{
                fromNpc.transform.position,
                toNpc.transform.position,
            };
            var distance = (globalWaypoints[1] - globalWaypoints[0]).magnitude;
            reward = Mathf.Floor(distance * rewardPerDistance);
        }
        if (expectedTimeS == 0 && canDoDistance)
        {
            var globalWaypoints = new Vector3[]{
                fromNpc.transform.position,
                toNpc.transform.position,
            };
            var distance = (globalWaypoints[1] - globalWaypoints[0]).magnitude;
            expectedTimeS = Mathf.Ceil(distance * 0.12f);
        }

        var colliders = FindObjectsOfType<BQuest2Npc>();
        if (isAutoGenerate && fromNpc == null)
        {
            var index = (int)(Random.value * colliders.Length);
            Debug.Log("Random src " + index);
            fromNpc = colliders[index];
        }

        if (isAutoGenerate && toNpc == null)
        {
            toNpc = fromNpc;
            for (int i = 0; toNpc == fromNpc && i < 20; i++)
            {
                var index = (int)(Random.value * colliders.Length);
                Debug.Log("Random dest " + index);
                toNpc = colliders[index];
            }
        }

        if (isAutoGenerate && key == "")
        {
            var orders = FindObjectsOfType<BDeliveryOrder>();
            var ordersWithKey = orders.Where(o => o.key != "").ToList();
            key = $"Generated.Order-{ordersWithKey.Count}";
            Debug.Log("Random key " + key);
        }
    }
#endif

    void OnDrawGizmosSelected()
    {
        if (!isDrawGizmo) return;
        if (fromNpc == null) return;
        if (toNpc == null) return;

        var globalWaypoints = new Vector3[]{
            fromNpc.transform.position,
            toNpc.transform.position,
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
