using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BPlayerBattery : MonoBehaviour
{
    [Header("State")]
    public int grappleShotCount = 3;

    [Header("Config")]
    public int grappleShotCountMax = 3;
    public bool rechargeInParallel = false;

    [Header("Config (OnGround)")]
    public bool isRechargeOnGround = true;
    public bool isRechargeOnGroundAtOnce = false;
    public float rechargeOnGroundTime = 1f;
    Coroutine rechargeOnGroundTimer;

    [Header("Config (Anytime)")]
    public bool isRechargeAnytime = true;
    public bool isRechargeAnytimeAtOnce = false;
    public float rechargeAnytimeTime = 5f;
    Coroutine rechargeAnytimeTimer;

    [Header("Linkage")]
    public BLivesMeter grappleShotCounter;

    void Start()
    {

        if (grappleShotCounter == null)
        {
            throw new Exception("grappleShotCounter not found");
        }
    }

    public bool HasBattery(int requirement)
    {
        return grappleShotCount >= requirement;
    }

    public bool BatteryIsFull()
    {
        return grappleShotCount >= grappleShotCountMax;
    }
    public void TryAddGrappleShots(int shots)
    {
        grappleShotCount += shots;
        if (grappleShotCount > grappleShotCountMax)
        {
            grappleShotCount = grappleShotCountMax;
        }
        grappleShotCounter.SetLives(grappleShotCount);
    }

    public void RemoveGrappleShots(int shots)
    {
        grappleShotCount -= shots;
        grappleShotCounter.SetLives(grappleShotCount);
        OnGrappleUsed();
    }

    public void OnLeaveGround()
    {
        Debug.Log("OnLeaveGround");
        if (rechargeOnGroundTimer != null)
        {
            StopCoroutine(rechargeOnGroundTimer);
            rechargeOnGroundTimer = null;
        }
    }
    public void OnTouchGround()
    {
        Debug.Log("OnTouchGround");
        CheckAndWaitAndRechargeOnGround();
    }
    public void OnGrappleUsed()
    {
        CheckAndWaitAndRechargeAnytime();
    }

    public void CheckAndWaitAndRechargeOnGround()
    {
        var playerController = GetComponent<BPlayerController>();
        if (isRechargeOnGround && rechargeOnGroundTimer == null && !BatteryIsFull() && playerController.collisions.below)
        {
            rechargeOnGroundTimer = StartCoroutine(
                WaitAndRefillBattery(
                    rechargeOnGroundTime,
                    (isRechargeOnGroundAtOnce ? grappleShotCountMax : 1),
                    false
                )
            );
        }
    }
    public void CheckAndWaitAndRechargeAnytime()
    {
        if (isRechargeAnytime && rechargeAnytimeTimer == null && !BatteryIsFull())
        {
            rechargeAnytimeTimer = StartCoroutine(
                WaitAndRefillBattery(
                    rechargeAnytimeTime,
                    (isRechargeAnytimeAtOnce ? grappleShotCountMax : 1),
                    true
                )
            );
        }
    }

    IEnumerator WaitAndRefillBattery(float time, int amount, bool isAnytimeTimer)
    {
        Debug.Log("WaitAndRefillBattery " + time + " " + isAnytimeTimer);
        yield return new WaitForSeconds(time);
        TryAddGrappleShots(amount);

        if (isAnytimeTimer)
        {
            Debug.Log("Recharged by Anytime. Now " + grappleShotCount);
            rechargeAnytimeTimer = null;
            if (!rechargeInParallel && rechargeOnGroundTimer != null)
            {
                StopCoroutine(rechargeOnGroundTimer);
                rechargeOnGroundTimer = null;
            }
        }
        else
        {
            Debug.Log("Recharged by OnGround. Now " + grappleShotCount);
            rechargeOnGroundTimer = null;
            if (!rechargeInParallel && rechargeAnytimeTimer != null)
            {
                StopCoroutine(rechargeAnytimeTimer);
                rechargeAnytimeTimer = null;
            }
        }
        CheckAndWaitAndRechargeOnGround();
        CheckAndWaitAndRechargeAnytime();
    }
}
