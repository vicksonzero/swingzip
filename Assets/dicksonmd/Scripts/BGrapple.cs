using UnityEngine;

public class BGrapple : MonoBehaviour
{
    public int grappleTime = 500; // in ms, TODO: Change to frames
    public bool wasComplete = false;
    public bool isActive = false;
    public float grappleLength = 0;

    private float grappleCompleteTime = 0;
    

    public void StartGrapple()
    {
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

        return (grappleCompleteTime - Time.time) / (grappleTime / 1000f);
    }

    public float DistanceTo(Transform t)
    {
        Vector2 displacement = t.position - transform.position;
        return displacement.magnitude;
    }
}
