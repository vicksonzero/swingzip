using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class BRouteCamera : MonoBehaviour
{
    public CinemachineTargetGroup routePreviewTargetGroup;
    public LineRenderer lr;

    public void SetTarget(Transform playerTarget, Transform newTarget)
    {
        RemoveTargets();
        routePreviewTargetGroup.AddMember(newTarget, 1, 2);
        lr.SetPositions(new Vector3[]{
            playerTarget.position,
            newTarget.position
        });
    }

    public void RemoveTargets()
    {
        while (routePreviewTargetGroup.m_Targets.Length > 1)
        {
            routePreviewTargetGroup.RemoveMember(routePreviewTargetGroup.m_Targets[1].target);
        }
    }
}
