using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class BRouteCamera : MonoBehaviour
{
    public CinemachineTargetGroup routePreviewTargetGroup;
    public LineRenderer lr;
    public Canvas canvas;
    public Text lengthLabel;

    public float textScale = 0.001f;

#nullable enable
    void UpdateDistanceMarker(Transform? from, Transform? to)
    {
        if (from == null || to == null)
        {
            lengthLabel.text = "";
            return;

        }
        Vector2 displacement = to.position - from.position;
        lengthLabel.text = $"{displacement.magnitude.ToString("0.0")}m";

        var angle = Vector2.SignedAngle(Vector2.up, displacement);
        if (angle > 0) angle -= 180;

        canvas.transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);

        var maxDist = Mathf.Max(Mathf.Abs(displacement.x), Mathf.Abs(displacement.y));
        canvas.transform.localScale = Vector3.one * 0.3f * (1 + Mathf.Pow(maxDist, 1.7f) * textScale);
    }
#nullable disable

    public void SetTarget(Transform newTarget)
    {
        RemoveTargets();
        routePreviewTargetGroup.AddMember(newTarget, 1, 2);
        lr.positionCount = 1;
        lr.SetPositions(new Vector3[]{
            newTarget.position
        });
        UpdateDistanceMarker(null, null);
    }

    public void SetTargets(Transform playerTarget, Transform newTarget)
    {
        RemoveTargets();
        routePreviewTargetGroup.AddMember(playerTarget, 1, 2);
        routePreviewTargetGroup.AddMember(newTarget, 1, 2);
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[]{
            playerTarget.position,
            newTarget.position
        });
        UpdateDistanceMarker(playerTarget, newTarget);
    }

    public void SetTargets(Transform[] targets)
    {
        RemoveTargets();
        foreach (var target in targets)
        {
            routePreviewTargetGroup.AddMember(target, 1, 2);
        }
        lr.positionCount = targets.Length;
        lr.SetPositions(targets.Select(t => t.position).ToArray());

        UpdateDistanceMarker(targets[0], targets[targets.Length - 1]);
    }

    public void RemoveTargets()
    {
        while (routePreviewTargetGroup.m_Targets.Length > 0)
        {
            routePreviewTargetGroup.RemoveMember(routePreviewTargetGroup.m_Targets[0].target);
        }
    }
}
