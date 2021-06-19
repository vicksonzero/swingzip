using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BOrderList : MonoBehaviour
{
    public int orderCount = 0;
    public int orderFinishedCount = 0;

    public float updateInterval = 2f;
    public float nextUpdate = 2f;


    // Start is called before the first frame update
    void Start()
    {
        nextUpdate = Time.time;
        UpdateOrderCount();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= nextUpdate)
        {
            UpdateOrderCount();
            nextUpdate += updateInterval;
        }
    }

    public void UpdateOrderCount()
    {
        var orders = FindObjectsOfType<BDeliveryOrder>();
        orderCount = orders.Length;

        var finishedOrders = orders.ToList().Where(orders => !orders.isOrderEnabled).ToArray();
        orderFinishedCount = finishedOrders.Length;
    }
}
