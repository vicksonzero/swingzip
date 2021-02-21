﻿using System;
using UnityEngine;


[RequireComponent(typeof(BPlayerController))]
public class BPlayer : MonoBehaviour
{
    [HideInInspector]
    public BPlayerController controller;
    public Vector2 directionalInput;

    public enum EPlayerStates { FREE, SWINGING, DASH_START, DASHING, ZIPPING_TO_POINT, RUN, WALL_RUN };

    public EPlayerStates currentState = EPlayerStates.FREE;

    #region Basic Fields

    [Header("Basic")]
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed = 6;

    public float maxFallingSpeed = 6;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    public Vector3 velocity;
    float velocityXSmoothing;

    #endregion
    [Header("Swing")]
    public BGrapple grapplePrefab;

    [HideInInspector]
    public BGrapple grapple;

    [HideInInspector]
    public BPlayerSwing playerSwing;

    [Header("Zip-to-point")]
    public BZipTarget zipTargetPrefab;
    public BZipButton zipButtonPrefab;

    [HideInInspector]
    public BZipButton zipButton;

    [HideInInspector]
    public BZipTarget zipTarget;

    [HideInInspector]
    public BPlayerZipToPoint playerZipToPoint;


    #region Wall Fields

    [Header("Wall")]
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    float timeToWallUnstick;
    bool wallSliding;
    int wallDirX;

    #endregion

    #region Dash Fields

    [Header("Dash")]


    [HideInInspector]
    public BPlayerDash playerDash;

    #endregion


    #region Auto-move Fields

    [Header("Auto-move")]
    public float xDir = 0;

    [Header("Input Buffers")]
    public int jumpDownFrames = 0;
    public int jumpUpFrames = 0;

    #endregion

    #region Animations
    public Transform spriteRoot;
    Animator playerAnimator;
    #endregion

    void Start()
    {
        controller = GetComponent<BPlayerController>();
        playerAnimator = GetComponent<Animator>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        if (grapple == null)
        {
            grapple = Instantiate(grapplePrefab);
            grapple.gameObject.SetActive(false);
        }
        if (zipButton == null)
        {
            zipButton = Instantiate(zipButtonPrefab);
            zipButton.gameObject.SetActive(false);
        }
        playerSwing = GetComponent<BPlayerSwing>();
        playerZipToPoint = GetComponent<BPlayerZipToPoint>();
        playerDash = GetComponent<BPlayerDash>();

    }

    void Update()
    {
        if (IsZippingToPoint())
        {
            currentState = EPlayerStates.ZIPPING_TO_POINT;
            playerZipToPoint.UpdateZipToPoint();
        }
        else if (IsDashing())
        {
            currentState = EPlayerStates.DASHING;
            playerDash.UpdateDash();
        }
        else if (playerSwing.IsSwinging())
        {
            currentState = EPlayerStates.SWINGING;
            playerSwing.UpdateSwing();
        }
        else
        {
            currentState = EPlayerStates.FREE;
            UpdateMovement();
        }

        var localScale =  spriteRoot.localScale;
        localScale.x = Mathf.Sign(controller.collisions.faceDir);
        spriteRoot.localScale = localScale;

        playerAnimator.SetBool("IsGround", controller.collisions.below);
        playerAnimator.SetBool("IsAir", !(
            controller.collisions.above ||
            controller.collisions.below ||
            controller.collisions.left ||
            controller.collisions.right
        ));
        playerAnimator.SetBool("IsWall", (controller.collisions.left || controller.collisions.right) && !controller.collisions.below);
        playerAnimator.SetFloat("SpeedX", velocity.x);
        playerAnimator.SetBool("IsMovingX", velocity.x < -0.5f || velocity.x > 0.5f);
        playerAnimator.SetFloat("SpeedY", velocity.y);
        playerAnimator.SetBool("IsMovingY", velocity.y < -0.01f || velocity.x > 0.01f);

    }

    #region State Accessors Methods

    public bool IsZippingToPoint()
    {
        return zipTarget != null;
    }

    public bool IsDashing()
    {
        return Time.time < playerDash.dashUntil;
    }

    #endregion

    #region State Handlers Methods

    void UpdateMovement()
    {
        DetermineTargetVelocity();
        DoGravity();
        DoWallSliding();

        DisplaceSelf();

        OverrideVelocity();
    }



    #endregion

    #region Constraints Methods

    private void DoWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }
    }

    public void StartDash()
    {
        playerDash.StartDash();
    }

    public void StopDash()
    {
        playerDash.StopDash();
    }

    private void DetermineTargetVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
    }

    public void DoGravity()
    {
        velocity.y += gravity * Time.deltaTime;

        if (velocity.y < -maxFallingSpeed)
        {
            velocity.y = -maxFallingSpeed;
        }
    }

    private void OverrideVelocity()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }
    }

    public void DisplaceSelf()
    {
        controller.Move(velocity * Time.deltaTime, directionalInput);
    }


    #endregion

    #region Input / State Change / Init Methods

    public void OnVirtualPointerDown(Vector2 inputPosition)
    {

        if (zipButton != null && zipButton.gameObject.activeSelf)
        {
            var zipTarget = Instantiate(zipTargetPrefab, zipButton.transform.position, Quaternion.identity);
            StartZipToPoint(zipTarget);
        }
        else if (wallSliding || controller.collisions.below)
        {
            OnJumpInputDown();
        }
        else if (!IsZippingToPoint())
        {
            Vector3 pos = inputPosition;
            pos.z = 10.0f;
            Vector2 pos2 = Camera.main.ScreenToWorldPoint(pos);
            PutGrapple(pos2);
        }
    }

    public void OnVirtualPointerUp(Vector2 inputPosition)
    {
        OnJumpInputUp();
        RemoveGrapple();
    }

    public void OnDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        if (wallSliding)
        {
            if (wallDirX == directionalInput.x)
            {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y * Mathf.Sqrt(gravity / -50f);
            }
            else if (directionalInput.x == 0)
            {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y * Mathf.Sqrt(gravity / -50f);
            }
            else
            {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y * Mathf.Sqrt(gravity / -50f);
            }
        }
        if (controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                { // not jumping against max slope
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                }
            }
            else
            {
                velocity.y = maxJumpVelocity;
            }
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    public void PutGrapple(Vector2 pos)
    {
        playerSwing.PutGrapple(pos);
    }

    public void RemoveGrapple()
    {
        playerSwing.RemoveGrapple();
    }


    public void StartZipToPoint(BZipTarget zipTarget)
    {
        playerZipToPoint.StartZipToPoint(zipTarget);
    }

    #endregion
}
