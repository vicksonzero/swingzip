using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class BOrderRadar : MonoBehaviour
{

    HashSet<BDeliveryOrder> orderSet = new HashSet<BDeliveryOrder>();
    List<Transform> pool = new List<Transform>();

    public bool isRadarEnabled = true;
    public Transform dotPrefab;
    public float minDistance = 1;
    public float minRadius = 3;
    public float maxRadius = 5;

    CircleCollider2D circleCollider;

    // Start is called before the first frame update
    void Start()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        if (isRadarEnabled)
        {
            foreach (var order in orderSet)
            {
                var dot = GetDot(i);
                dot.gameObject.SetActive(true);

                var displacement = order.offerCollider.transform.position - transform.position;
                displacement.z = 0;
                if (displacement.magnitude < minDistance) continue;
                var adjustedDistance = minRadius + (maxRadius - minRadius) * displacement.magnitude / circleCollider.radius;

                dot.localPosition = displacement.normalized * adjustedDistance;
                i++;
            }
        }

        // Debug.Log("pool.Count: " + i + " " + pool.Count);
        for (; i < pool.Count; i++)
        {
            var dot = GetDot(i);
            dot.gameObject.SetActive(false);
        }
    }

    Transform GetDot(int i)
    {
        while (i >= pool.Count)
        {
            pool.Add(Instantiate(dotPrefab, transform));
        }
        return pool[i];
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        var order = collider.GetComponentInParent<BDeliveryOrder>();
        if (order == null) return;
        if (!order.isOrderEnabled) return;
        if (order.offerCollider.GetComponent<Collider2D>() != collider) return;

        orderSet.Add(order);
    }
    // Start is called before the first frame update
    public void OnTriggerExit2D(Collider2D collider)
    {
        var order = collider.GetComponentInParent<BDeliveryOrder>();
        if (order == null) return;
        if (order.offerCollider.GetComponent<Collider2D>() != collider) return;
        orderSet.Remove(order);
    }
}
