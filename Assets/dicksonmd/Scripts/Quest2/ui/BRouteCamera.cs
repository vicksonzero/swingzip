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

    public void SetTarget(Transform newTarget)
    {
        RemoveTargets();
        routePreviewTargetGroup.AddMember(newTarget, 1, 2);
        lr.positionCount = 1;
        lr.SetPositions(new Vector3[]{
            newTarget.position
        });
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
    }

    public void RemoveTargets()
    {
        while (routePreviewTargetGroup.m_Targets.Length > 0)
        {
            routePreviewTargetGroup.RemoveMember(routePreviewTargetGroup.m_Targets[0].target);
        }
    }
}
