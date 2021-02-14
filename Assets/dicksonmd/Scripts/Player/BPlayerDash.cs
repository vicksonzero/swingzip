using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPlayerDash : MonoBehaviour
{
    [Header("State")]
    Vector3 dashTarget;
    public float dashUntil = 0;
    public float dashSpeedProgress = 0; // per second
    public float dashSpeed = 0; // per second
    public float dashDeceleration = 0; // per second squared

    [Header("Linkages")]
    BPlayer player;

    [Header("Config")]
    public SOMovementLimit limits;
    public float dashDistance = 4;
    public float dashTime = 0.4f; // seconds
    public float dashEndSpeed = 6; // per second

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<BPlayer>();
        dashSpeed = 2 * dashDistance / dashTime - dashEndSpeed;// Mathf.Sqrt(2 * dashDistance * dashDeceleratation);//dashDistance / dashTime + 0.5f * dashDeceleratation * dashTime;
        dashDeceleration = (dashSpeed * dashSpeed - dashEndSpeed * dashEndSpeed) / 2 / dashDistance;
    }


    public void StartDash()
    {
        Debug.Log("StartDash");
        Vector2 displacementToGrapple = player.grapple.transform.position - transform.position;
        dashTarget = transform.position + ((Vector3)displacementToGrapple.normalized * dashDistance);
        dashUntil = Time.time + dashTime;
        dashSpeedProgress = dashSpeed;
    }

    public void UpdateDash()
    {
        var remainingDisplacement = dashTarget - transform.position;

        dashSpeedProgress = dashSpeed - dashDeceleration * Mathf.Clamp(dashTime - dashUntil + Time.time, 0, dashTime);
        player.velocity = remainingDisplacement.normalized * dashSpeedProgress;

        if (remainingDisplacement.magnitude < dashSpeedProgress * Time.deltaTime)
        {
            return;
        }
        player.controller.Move(player.velocity * Time.deltaTime, player.directionalInput);
    }

    public void StopDash()
    {
        dashUntil = Time.time;
    }
}
