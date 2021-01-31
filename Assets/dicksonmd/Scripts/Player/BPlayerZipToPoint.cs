using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPlayerZipToPoint : MonoBehaviour
{

    [Header("State")]
    public float zipSpeedProgress = 0; // per second
    public float zipUntil = 0;

    [Header("Linkages")]
    BPlayer player;

    [Header("Config")]
    public SOMovementLimit zipToPointLimits;

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

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<BPlayer>();
    }
    public void StartZipToPoint(BZipTarget zipTarget)
    {
        Debug.Log("StartZipToPoint!");
        player.zipTarget = zipTarget;

        if (player.zipButton != null)
        {
            player.zipButton.StopButton();
        }

        // make t the subject of s=ut+0.5at^2, where u !=0
        // (-u + sqrt(2 a s + u^2))/a
        Vector2 displacementToZipTarget = zipTarget.transform.position - transform.position;
        player.velocity = Vector3.Project(player.velocity, displacementToZipTarget);
        zipSpeedProgress = Mathf.Clamp(player.velocity.magnitude, 0, zipStartSpeedCap);
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
            Destroy(player.zipTarget.gameObject);
            player.zipTarget = null;

            return;
        }

        if (remainingDisplacement.magnitude <= zipSpeedProgress * Time.deltaTime)
        {
            // snap to end point
            player.controller.Move(remainingDisplacement, player.directionalInput);
            Destroy(player.zipTarget.gameObject);
            player.zipTarget = null;
        }
        else
        {
            // normal zip to point
            player.controller.Move(player.velocity * Time.deltaTime, player.directionalInput);
            player.zipTarget.UpdateLineRenderer(transform.position);
        }
    }

}
