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
    public bool isRechargeOnGroundOneImmediately = false;
    public bool isRechargeOnGroundAllAtOnce = false;
    public float rechargeOnGroundTime = 1f;
    Coroutine rechargeOnGroundTimer;

    [Header("Config (Anytime)")]
    public bool isRechargeAnytime = true;
    public bool isRechargeAnytimeAtOnce = false;
    public float rechargeAnytimeTime = 5f;
    Coroutine rechargeAnytimeTimer;

    [Header("Linkage")]
    public BLivesMeter grappleShotCounter;
    public ChargesUsedEvent onChargesUsed;
    public delegate void ChargesUsedEvent(int charges);
    public ParticleSystem dischargePS;
    public ParticleSystem dischargeBigPS;
    public ParticleSystem rechargePS;

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
    public void TryAddGrappleBattery(int shots)
    {
        bool batteryWasFull = BatteryIsFull();
        grappleShotCount += shots;
        if (grappleShotCount > grappleShotCountMax)
        {
            grappleShotCount = grappleShotCountMax;
        }
        if (!batteryWasFull && rechargePS != null) rechargePS?.Play();
        Debug.Log("TryAddGrappleShots (+" + shots + "=" + grappleShotCount + ")");
        grappleShotCounter.SetLives(grappleShotCount);
    }

    public void RemoveGrappleBattery(int shots)
    {
        grappleShotCount -= shots;
        onChargesUsed(shots);
        if (grappleShotCount > 0)
        {
            if (dischargePS != null) dischargePS.Play();
        }
        else
        {
            if (dischargeBigPS != null) dischargeBigPS.Play();
        }
        Debug.Log("RemoveGrappleShots (-" + shots + "=" + grappleShotCount + ")");
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
        if (isRechargeOnGroundOneImmediately) TryAddGrappleBattery(1);
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
                    (isRechargeOnGroundAllAtOnce ? grappleShotCountMax : 1),
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
        TryAddGrappleBattery(amount);

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
