using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BPlayerZipToPoint : MonoBehaviour
{

    [Header("State")]
    public float zipSpeedProgress = 0; // per second
    public float zipUntil = 0;
    public BZipTarget zipTargetCandidate;

    [Header("Linkages")]
    BPlayer player;
    public CinemachineTargetGroup targetGroup;
    public CinemachineVirtualCamera groupCamera;

    [Header("Config")]
    public SOMovementLimit limits;

    public float targetShowingTime = 1f;
    public Coroutine targetDestroyTimer = null;

    public float minDistance = 5;
    public float maxDistance = 30;
    public float zipStartSpeedCap = 1; // per second // zipStartSpeedLimit
    public float zipAcceleration = 12; // per second squared
    public float zipSpeed = 20; // per second
    [Tooltip("-1 means no speed limit")]
    public float zipEndSpeedCap = -1; // per second // zipEndSpeedLimit // TODO
    [Tooltip("per second squared")]
    public float zipDeceleration = 0; // TODO

    // add artificial buffer in case colliding on walls
    public float zipTimeBuffer = 0.1f;
    public bool canInterrupt = false;
    public bool interruptByPointerUp = false;

    public float terminatingTime = 0.2f;

    public float terminatingAtTime = 0f;


    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<BPlayer>();
    }
    public void StartZipToPoint(BZipTarget zipTarget)
    {
        Debug.Log("StartZipToPoint!");
        player.zipTarget = zipTarget;
        targetGroup.AddMember(zipTarget.transform, 1, 5);
        groupCamera.Priority = 20;

        // make t the subject of s=ut+0.5at^2, where u !=0
        // (-u + sqrt(2 a s + u^2))/a
        Vector2 displacementToZipTarget = zipTarget.transform.position - transform.position;
        int sign = Vector3.Angle(displacementToZipTarget, (Vector3)(Vector2)player.velocity) > 90 ? -1 : 1;
        player.velocity = Vector3.Project(player.velocity, displacementToZipTarget);
        zipSpeedProgress = sign * Mathf.Clamp(player.velocity.magnitude, 0, zipStartSpeedCap);
        player.velocity = player.velocity.normalized * zipSpeedProgress;

        var a = zipAcceleration;
        var u = zipSpeedProgress; // starting speed
        var v = zipSpeed; // peak speed

        // distance for accel phase
        var s_accel = (v * v - u * u) / 2 / a;
        if (s_accel >= displacementToZipTarget.magnitude)
        {
            // only accel phase is needed
            var s = displacementToZipTarget.magnitude;
            zipUntil = Time.time + (-u + Mathf.Sqrt(2 * a * s + u * u)) / a;
        }
        else
        {
            // accel phase + constant phase
            var s1 = s_accel;
            // s2 = remaining distance, aka at constant speed
            var s2 = displacementToZipTarget.magnitude - s_accel;
            zipUntil = Time.time + ((-u + Mathf.Sqrt(2 * a * s1 + u * u)) / a) + (s2 / v);
        }

        // add artificial buffer in case colliding on walls
        zipUntil *= (1 + zipTimeBuffer);
    }

    public void UpdateZipToPoint()
    {
        Vector2 remainingDisplacement = player.zipTarget.transform.position - transform.position;

        zipSpeedProgress += zipAcceleration * Time.deltaTime;
        zipSpeedProgress = Mathf.Clamp(zipSpeedProgress, 0, zipSpeed);
        player.velocity = remainingDisplacement.normalized * zipSpeedProgress;

        if (Time.time > zipUntil)
        {
            targetGroup.RemoveMember(player.zipTarget.transform);
            groupCamera.Priority = 5;
            Destroy(player.zipTarget.gameObject);
            player.zipTarget = null;

            return;
        }

        if (remainingDisplacement.magnitude <= zipSpeedProgress * Time.deltaTime)
        {
            // snap to end point
            player.controller.Move(remainingDisplacement, player.directionalInput);
            targetGroup.RemoveMember(player.zipTarget.transform);
            groupCamera.Priority = 5;
            Destroy(player.zipTarget.gameObject);
            player.zipTarget = null;
        }
        else
        {
            // normal zip to point
            player.controller.Move(player.velocity * Time.deltaTime, player.directionalInput);
            player.zipTarget.UpdateLineRenderer(transform.position);

            if (!player.controller.collisions.HaveCollision())
            {
                terminatingAtTime = Time.time + terminatingTime;
            }
            if (terminatingAtTime < Time.time)
            {
                Destroy(player.zipTarget.gameObject);
                targetGroup.RemoveMember(player.zipTarget.transform);
                groupCamera.Priority = 5;
                player.zipTarget = null;
            }
        }
    }

    public void PutTarget(Vector2 pos)
    {
        if (zipTargetCandidate != null)
        {
            if (targetDestroyTimer != null) StopCoroutine(targetDestroyTimer);
            Destroy(zipTargetCandidate.gameObject);
            zipTargetCandidate = null;
        }
        var hitList = Physics2D.RaycastAll(transform.position, pos - ((Vector2)transform.position), maxDistance);

        foreach (var hit in hitList)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Obstacle") && hit.distance > minDistance)
            {
                zipTargetCandidate = Instantiate(player.zipTargetPrefab, hit.point, Quaternion.identity);
                targetDestroyTimer = StartCoroutine(DestroyZipTargetAfter(targetShowingTime));
                return;
            }
        }
    }

    public void DestroyZipTargetNow()
    {
        if (zipTargetCandidate != null)
        {
            if (targetDestroyTimer != null) StopCoroutine(targetDestroyTimer);
            Destroy(zipTargetCandidate.gameObject);
            zipTargetCandidate = null;
        }
    }
    IEnumerator DestroyZipTargetAfter(float time)
    {
        yield return new WaitForSeconds(time);
        if (zipTargetCandidate != null)
        {
            Destroy(zipTargetCandidate.gameObject);
            zipTargetCandidate = null;
        }
    }
}
