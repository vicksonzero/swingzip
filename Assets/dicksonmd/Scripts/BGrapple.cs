using DG.Tweening;
using UnityEngine;

public class BGrapple : MonoBehaviour
{
    public int grappleTime = 500; // in ms, TODO: Change to frames
    public bool wasComplete = false;
    public bool isActive = false;
    public bool isSolidGrapple = false;
    public float grappleLength = 0;
    public LineRenderer lineRenderer;
    public Transform buttonSprite;
    public Transform buttonSprite2;
    public float grappleCompleteTime = 0;

    void Start()
    {
    }

    public void StartGrapple(bool isOnBackWall)
    {
        isSolidGrapple = isOnBackWall;

        if (isSolidGrapple)
        {
            buttonSprite.transform.localScale = Vector3.one * 0.2f;
            buttonSprite2.transform.localScale = Vector3.one * 0.4f;
        }
        else
        {
            buttonSprite.transform.localScale = Vector3.one * 0.1f;
            buttonSprite2.transform.localScale = Vector3.one * 0.1f;
        }

        grappleCompleteTime = Time.time + grappleTime / 1000f;
        isActive = true;
    }

    public void EndGrapple()
    {
        isActive = false;
        wasComplete = false;
        grappleCompleteTime = 0;
    }

    public void PlayAnchorTweens()
    {
        buttonSprite.transform.localScale = Vector3.one * 0.3f;
        buttonSprite.transform.rotation = Quaternion.identity;
        buttonSprite.transform.DORotate(new Vector3(0, 0, 90f), 0.4f);

        buttonSprite2.transform.localScale = Vector3.one * 0.1f;
        buttonSprite2.transform.DOScale(Vector3.one * 0.6f, 0.2f);
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
