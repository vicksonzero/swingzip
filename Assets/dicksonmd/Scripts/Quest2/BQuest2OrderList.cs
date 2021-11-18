using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BQuest2OrderList : MonoBehaviour
{
    public BDeliveryOrder[] orders;
    private int orderKeyIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        orders = FindObjectsOfType<BDeliveryOrder>();
        orders.ToList().ForEach((order) => CleanUp(order));
        Debug.Log($"BQuest2OrderList: {orders.Where(o => !o.isValidOrder).ToList().Count} Orders are invalid");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public BDeliveryOrder[] GetValidOrders()
    {
        return orders.Where(o => o.isValidOrder).ToArray();
    }

    private void CleanUp(BDeliveryOrder order)
    {
        order.isValidOrder = true;
        if (order.key == "") order.key = $"Generated.Order.{orderKeyIndex++}";
        if (order.offerCollider == null || order.departCollider == null || order.destCollider == null) order.isValidOrder = false;

    }
}
