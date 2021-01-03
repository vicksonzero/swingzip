﻿using DG.Tweening;
using UnityEngine;

public class BGrapple : MonoBehaviour
{
    public int grappleTime = 500; // in ms, TODO: Change to frames
    public bool wasComplete = false;
    public bool isActive = false;
    public float grappleLength = 0;
    public LineRenderer lineRenderer;
    public Transform buttonSprite;

    public float grappleCompleteTime = 0;

    void Start()
    {
    }

    public void StartGrapple()
    {
        buttonSprite.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        buttonSprite.transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.1f);
        grappleCompleteTime = Time.time + grappleTime / 1000f;
        isActive = true;
    }

    public void EndGrapple()
    {
        isActive = false;
        wasComplete = false;
        grappleCompleteTime = 0;
    }

    public bool IsComplete()
    {
        return Time.time >= grappleCompleteTime;
    }

    public float GetGrapplePercent()
    {
        if (IsComplete())
        {
            return 1;
        }

        return Mathf.Clamp01(1 - (grappleCompleteTime - Time.time) / (grappleTime / 1000f));
    }

    public float DistanceTo(Transform t)
    {
        Vector2 displacement = t.position - transform.position;
        return displacement.magnitude;
    }

    public void UpdateLineRenderer(Vector3 swingPos)
    {
        Vector2 lineVector = swingPos - transform.position;
        lineRenderer.SetPosition(0, lineVector);
        lineRenderer.SetPosition(1, lineVector * (1 - GetGrapplePercent()));
    }
}
