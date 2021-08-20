using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPlayerDash : MonoBehaviour
{
    [Header("State")]
    [HideInInspector]
    public Vector3 dashTarget;
    public float dashSpeedProgress = 0; // per second
    public float dashUntil = 0;

    [Header("Linkages")]
    BPlayer player;

    [Header("Config")]
    public SOMovementLimit limits;
    public float dashDistance = 4;
    public float dashTime = 0.5f; // seconds


    public float dashStartSpeedCap = 4; // per second // dashStartSpeedLimit
    public float dashAcceleration = 1000; // per second squared
    public float dashSpeed = 13; // per second
    [Tooltip("-1 means no speed limit")]
    public float dashEndSpeedCap = -1; // per second // dashEndSpeedLimit // TODO
    [Tooltip("per second squared")]
    public float dashDeceleration = 0; // TODO

    // add artificial buffer in case colliding on walls
    public float dashTimeBuffer = 0.1f;

    public float refundTime = 0.1f;
    public float dashStartTime;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<BPlayer>();
    }


    public void StartDash()
    {
        Debug.Log("StartDash");
        dashStartTime = Time.time;
        dashTarget = player.grapple.transform.position - transform.position;
        dashTarget.z = player.transform.position.z;
        dashTarget = transform.position + dashTarget.normalized * dashDistance;

        // make t the subject of s=ut+0.5at^2, where u !=0
        // (-u + sqrt(2 a s + u^2))/a
        Vector2 displacementToDashTarget = dashTarget - transform.position;
        int sign = Vector3.Angle(displacementToDashTarget, (Vector3)(Vector2)player.velocity) > 90 ? -1 : 1;
        player.velocity = Vector3.Project(player.velocity, displacementToDashTarget);
        dashSpeedProgress = sign * Mathf.Clamp(player.velocity.magnitude, 0, dashStartSpeedCap);
        player.velocity = player.velocity.normalized * dashSpeedProgress;
        Debug.Log(displacementToDashTarget.magnitude);

        var a = dashAcceleration;
        var u = dashSpeedProgress; // starting speed
        var v = dashSpeed; // peak speed

        // distance for accel phase
        var s_accel = (v * v - u * u) / 2 / a;
        if (s_accel >= displacementToDashTarget.magnitude)
        {
            // only accel phase is needed
            var s = displacementToDashTarget.magnitude;
            dashUntil = Time.time + (-u + Mathf.Sqrt(2 * a * s + u * u)) / a;
        }
        else
        {
            // accel phase + constant phase
            var s1 = s_accel;
            // s2 = remaining distance, aka at constant speed
            var s2 = displacementToDashTarget.magnitude - s_accel;
            dashUntil = Time.time + ((-u + Mathf.Sqrt(2 * a * s1 + u * u)) / a) + (s2 / v);
        }

        // add artificial buffer in case colliding on walls
        dashUntil *= (1 + dashTimeBuffer);
    }

    public void UpdateDash()
    {
        Debug.Log("UpdateDash");
        Vector2 remainingDisplacement = dashTarget - transform.position;

        dashSpeedProgress += dashAcceleration * Time.deltaTime;
        dashSpeedProgress = Mathf.Clamp(dashSpeedProgress, 0, dashSpeed);
        player.velocity = remainingDisplacement.normalized * dashSpeedProgress;

        if (Time.time > dashUntil)
        {
            Debug.Log("StopDash 1");
            StopDash();
            return;
        }

        if (remainingDisplacement.magnitude <= dashSpeedProgress * Time.deltaTime)
        {
            // snap to end point
            player.controller.Move(remainingDisplacement, player.directionalInput);
            Debug.Log("StopDash 2");
            StopDash();
            return;
        }
        else
        {
            // normal dash
            player.controller.Move(player.velocity * Time.deltaTime, player.directionalInput);

            if (player.controller.collisions.HaveCollision())
            {
                Debug.Log("StopDash 3");
                StopDash();
                return;
            }
        }
    }

    public void StopDash()
    {
        dashUntil = Time.time;
    }
}
