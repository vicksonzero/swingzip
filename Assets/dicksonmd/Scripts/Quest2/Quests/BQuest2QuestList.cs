using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BQuest2QuestList : MonoBehaviour
{
    public List<BQuest2Quest> quests;
    // Start is called before the first frame update
    void Start()
    {
        quests = FindObjectsOfType<BQuest2Quest>().ToList();

        foreach (var order in FindObjectsOfType<BQuest2Order>())
        {
            var go = new GameObject($"Quest.Generated.{order.name}", new System.Type[] { typeof(BQuest2DeliveryQuest) });
            var deliveryQuest = go.GetComponent<BQuest2DeliveryQuest>();
            quests.Add(deliveryQuest);
            go.transform.SetParent(transform);

            deliveryQuest.order = order;
            deliveryQuest.title = order.itemName;
            deliveryQuest.description = order.itemDescription;
            deliveryQuest.reward = $"${order.reward}";
            deliveryQuest.icon = order.itemIcon;
            deliveryQuest.iconBg = new Color32(124, 238, 206, 255);
            deliveryQuest.questGiver = order.fromNpc;

            deliveryQuest.InitQuest();
        }

    }

    // Update is called once per frame
    void Update()
    {
    }


}
