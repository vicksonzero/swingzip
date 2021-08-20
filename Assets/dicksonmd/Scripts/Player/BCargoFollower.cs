using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BCargoFollower : MonoBehaviour
{
    public Transform target;
    public float maxDistance = 5;
    public float smoothTime = 0.3f;
    public float floatingFrequency = 2f;
    public float floatingAmplitude = 1;

    public Transform itemRoot;

    private float nextFloat = 0;
    private Vector3 floatingOffset = Vector3.zero;

    Vector3 velocity;

    float goBackAtTime = 0;

    public GameObject itemBackground;


    float removeItemsAtTime = -1;
    float returnAtTime = -1;
    public Transform nextTarget;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        if (nextTarget != null)
        {
            if (Time.time >= returnAtTime)
            {
                target = nextTarget;
                nextTarget = null;
            }
        }
        if (removeItemsAtTime > 0)
        {
            if (Time.time >= removeItemsAtTime)
            {
                RemoveItems();
                removeItemsAtTime = -1;
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target == null) return;

        if (Time.time >= nextFloat)
        {
            floatingOffset = Random.insideUnitCircle * floatingAmplitude;
            nextFloat = Time.time + floatingFrequency;
        }

        var _smoothTime = smoothTime;
        Vector3 targetPosition = target.position + floatingOffset;
        targetPosition.z = transform.position.z;

        Vector3 displacement = transform.position - targetPosition;
        if (displacement.magnitude > maxDistance)
        {
            _smoothTime = 0.01f;
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }

    public void AttachItem(Sprite sprite)
    {
        if (HasItems())
        {
            RemoveItems();
        }
        var itemSprite = CreateItemSprite();
        itemSprite.sprite = sprite;
        itemBackground.SetActive(true);
        removeItemsAtTime = -1;
    }
    public void AttachItem(Transform iconSet)
    {
        if (HasItems())
        {
            RemoveItems();
        }
        iconSet.SetParent(itemRoot);
        itemBackground.SetActive(false);
        removeItemsAtTime = -1;
    }
    public bool CanReceiveItems()
    {
        var isScheduledForRemoveItems = (removeItemsAtTime > 0);
        return HasItems() && !isScheduledForRemoveItems;
    }
    public bool HasItems()
    {
        return itemRoot.childCount > 0;
    }
    public void RemoveItems()
    {
        foreach (Transform child in itemRoot)
        {
            GameObject.Destroy(child.gameObject);
        }
        itemBackground.SetActive(false);
    }

    SpriteRenderer CreateItemSprite()
    {
        var go = new GameObject("ItemSprite");
        var itemSprite = go.AddComponent<SpriteRenderer>();
        go.transform.SetParent(itemRoot);
        go.transform.localPosition = new Vector3(0, 0, -1);
        return itemSprite;
    }


    public void FollowTarget(Transform nextTarget)
    {
        this.target = nextTarget;
        this.nextTarget = null;
    }
    public void FollowTargetAfterTime(Transform nextTarget, float atTime)
    {
        this.nextTarget = nextTarget;
        returnAtTime = atTime;
    }
    public void RemoveItemsAfterTime(float atTime)
    {
        removeItemsAtTime = atTime;
    }
}
