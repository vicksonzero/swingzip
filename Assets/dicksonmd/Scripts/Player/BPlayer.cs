using System;
using UnityEngine;


[RequireComponent(typeof(BPlayerController))]
public class BPlayer : MonoBehaviour
{
    [HideInInspector]
    public BPlayerController controller;
    public Vector2 directionalInput;

    public enum EPlayerStates
    {
        FREE, RUN, WALL_RUN,
        SHOOTING, SWINGING, DASH_START, DASHING, ZIPPING_TO_POINT
    };

    public EPlayerStates currentState = EPlayerStates.FREE;

    #region Basic Fields

    [Header("Basic")]
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    public float accelerationTimeAirborne = 1f;
    public float accelerationTimeGrounded = .1f;
    public float moveSpeed = 6;

    public float maxFallingSpeed = 6;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    public Vector3 velocity;
    float velocityXSmoothing;

    public float shootingFallSpeedCap = 2.5f;

    public bool isFloating = false;

    [HideInInspector]
    public BPlayerBattery playerBattery;

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

    public float wallRunSpeed = 4;
    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    float timeToWallUnstick;
    bool wallSliding;
    bool wallRunning;
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
        playerBattery = GetComponent<BPlayerBattery>();

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
            if (controller.collisions.HaveCollision())
            {
                StopDash();
            }
        }
        else if (playerSwing.IsSwinging())
        {
            if (playerSwing.pointerWasUp)
            {
                if (playerBattery.HasBattery(1))
                {
                    playerBattery.RemoveGrappleBattery(1);
                    StartDash();
                    currentState = EPlayerStates.DASHING;
                }
                RemoveGrapple();
            }
            else
            {
                currentState = EPlayerStates.SWINGING;
                if (!playerSwing.wasComplete)
                {
                    if (playerZipToPoint.zipTargetCandidate != null)
                    {
                        Destroy(playerZipToPoint.zipTargetCandidate.gameObject);
                        playerZipToPoint.zipTargetCandidate = null;
                    }
                }
                if (controller.collisions.HaveCollision())
                {
                    RemoveGrapple();
                }
                playerSwing.UpdateSwing();
            }
        }
        else if (playerSwing.IsShooting())
        {
            currentState = EPlayerStates.SHOOTING;
            UpdateMovement(true);

        }
        else
        {
            currentState = EPlayerStates.FREE;
            UpdateMovement(false);
        }


        if (!playerSwing.isSolidGrapple && playerSwing.IsComplete())
        {
            RemoveGrapple();
            if (playerZipToPoint.zipTargetCandidate != null)
            {
                Destroy(playerZipToPoint.zipTargetCandidate.gameObject);
                playerZipToPoint.zipTargetCandidate = null;
            }
        }

        if (currentState != EPlayerStates.SHOOTING)
        {
            var localScale = spriteRoot.localScale;
            localScale.x = Mathf.Sign(controller.collisions.faceDir);
            spriteRoot.localScale = localScale;
        }

        if (isFloating)
        {
            var collionsAny = (
                controller.collisions.above ||
                controller.collisions.below ||
                controller.collisions.left ||
                controller.collisions.right
            );
            if (collionsAny)
            {
                isFloating = false;
            }
            else
            {
                var inputSign = directionalInput.x < 0 ? -1 : (directionalInput.x > 0 ? 1 : 0);
                var veloSign = velocity.x < 0 ? -1 : (velocity.x > 0 ? 1 : 0);

                var haveXInput = inputSign != 0;
                var sameXInput = inputSign == veloSign;
                // Debug.Log("sameXInput: " + directionalInput.x + " " + inputSign + " vs " + veloSign);
                // Debug.Log("notEnoughSpeed: " + haveXInput + "AND (" + sameXInput + " OR " + (Mathf.Abs(velocity.x) <= moveSpeed) + ")");

                var notEnoughSpeed = (haveXInput && (!sameXInput || Mathf.Abs(velocity.x) <= moveSpeed));
                if (notEnoughSpeed)
                {
                    Debug.Log("Manual air control kick in: vx=" + velocity.x + " (need " + (directionalInput.x * moveSpeed) + ")");
                    isFloating = false;
                }

            }
        }

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
        playerAnimator.SetBool("IsShooting", playerSwing.IsShooting());
        playerAnimator.SetBool("IsSwinging", currentState == EPlayerStates.SWINGING);
        playerAnimator.SetBool("IsDashing", currentState == EPlayerStates.DASHING);
        playerAnimator.SetBool("IsZipping", currentState == EPlayerStates.ZIPPING_TO_POINT);

        if (controller.collisionsOld.below && !controller.collisions.below)
        {
            playerBattery.OnLeaveGround();
        }
        if (!controller.collisionsOld.below && controller.collisions.below)
        {
            playerBattery.OnTouchGround();
            FindObjectOfType<BDeliveryObjective>().AddLanding(1);
        }
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

    void UpdateMovement(bool isShooting)
    {
        DetermineTargetVelocity();
        if (!isShooting) DoGravity();
        DoWallSliding();

        DisplaceSelf(isShooting);

        OverrideVelocity();
    }



    #endregion

    #region Constraints Methods

    private void DoWallSliding()
    {
        // Debug.Log("DoWallSliding " + controller.collisions.ToString());
        wallDirX = (controller.collisions.left) ? -1 : 1;

        wallRunning = false;
        wallSliding = false;

        var inputIsIntoWall = (directionalInput.x == wallDirX && directionalInput.x != 0);
        var isOnWall = (controller.collisions.left || controller.collisions.right) && !controller.collisions.below;
        var isTryingToGoUp = inputIsIntoWall || directionalInput.y > 0;
        if (isOnWall)
        {
            if (isTryingToGoUp)
            {
                velocity.y = Mathf.Max(velocity.y, wallRunSpeed);
                wallRunning = true;
                Debug.Log("DoWallSliding wallRunning");
            }
            if (velocity.y < 0)
            {
                wallSliding = true;
                Debug.Log("DoWallSliding wallSliding");
            }

            if (directionalInput.y >= 0 && velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (!inputIsIntoWall)
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
        if (Time.time - playerDash.dashStartTime < playerDash.refundTime)
        {
            playerBattery.TryAddGrappleBattery(1);
        }
    }

    private void DetermineTargetVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (isFloating ? accelerationTimeAirborne : accelerationTimeGrounded));
    }

    public void DoGravity()
    {
        velocity.y += gravity * Time.deltaTime;

        if (velocity.y < -maxFallingSpeed)
        {
            velocity.y = -maxFallingSpeed;
        }
    }

    public void DoSwingGravity()
    {
        velocity.y += gravity * 2f * Time.deltaTime;

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

    public void DisplaceSelf(bool isShooting)
    {
        Vector2 displacementVelocity = velocity;
        displacementVelocity = (!isShooting ? displacementVelocity : Vector2.ClampMagnitude(displacementVelocity, shootingFallSpeedCap));
        controller.Move(displacementVelocity * Time.deltaTime, directionalInput);
    }


    #endregion

    #region Input / State Change / Init Methods

    public void OnVirtualPointerDown(Vector2 inputPosition)
    {
        Debug.Log("OnVirtualPointerDown " + wallRunning + " " + wallSliding + " " + controller.collisions.below);
        var isOnWall = (controller.collisions.left || controller.collisions.right) && !controller.collisions.below;
        if (playerSwing.pointerWasUp)
        {
            playerSwing.pointerWasDownAgain = true;
        }
        else if (isOnWall || controller.collisions.below)
        {
            OnJumpInputDown();
        }
        else if (!IsZippingToPoint())
        {
            Vector3 pos = inputPosition;
            pos.z = 10.0f;
            Vector2 pos2 = Camera.main.ScreenToWorldPoint(pos);

            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(pos2, 0.5f);
            bool isClickingZipTarget = false;
            foreach (var collider in hitColliders)
            {
                if (collider.GetComponent<BZipTarget>() != null)
                {
                    isClickingZipTarget = true;
                    break;
                }
            }

            if (isClickingZipTarget)
            {
                Debug.Log("isClickingZipTarget");
                if (playerZipToPoint.zipTargetCandidate != null && playerBattery.HasBattery(1))
                {
                    playerBattery.RemoveGrappleBattery(1);
                    zipTarget = playerZipToPoint.zipTargetCandidate;
                    playerZipToPoint.zipTargetCandidate = null;
                    StartZipToPoint(zipTarget);
                    RemoveGrapple();
                    currentState = EPlayerStates.ZIPPING_TO_POINT;
                }
            }
            else
            {
                PutGrapple(pos2, hitColliders);
            }
        }
    }

    public void OnVirtualPointerUp(Vector2 inputPosition)
    {
        OnJumpInputUp();

        if (grapple == null || !grapple.gameObject.activeSelf)
        {
            return;
        }

        if (currentState != EPlayerStates.SWINGING)
        {
            playerSwing.pointerWasUp = true;
        }
        else
        {
            RemoveGrapple();
            isFloating = true;
        }
    }

    public void OnDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }

    public void OnJumpInputDown()
    {
        // Debug.Log("OnJumpInputDown " + wallRunning + " " + wallSliding + " " + controller.collisions.ToString() + " " + wallDirX);
        var isOnWall = (controller.collisions.left || controller.collisions.right) && !controller.collisions.below;
        if (isOnWall)
        {
            if (wallDirX == directionalInput.x)
            {
                Debug.Log("OnJumpInputDown wallJumpClimb");
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y * Mathf.Sqrt(gravity / -50f);
            }
            else if (directionalInput.x == 0)
            {
                Debug.Log("OnJumpInputDown wallJumpOff");
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y * Mathf.Sqrt(gravity / -50f);
            }
            else
            {
                Debug.Log("OnJumpInputDown wallLeap");
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y * Mathf.Sqrt(gravity / -50f);
            }
            controller.collisions.left = controller.collisions.right = false;
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

    public void PutGrapple(Vector2 pos, Collider2D[] hitColliders)
    {
        playerSwing.PutGrapple(pos, hitColliders);
        playerZipToPoint.PutTarget(pos);
    }

    public void RemoveGrapple()
    {
        playerSwing.EndGrapple();
        grapple.gameObject.SetActive(false);
    }


    public void StartZipToPoint(BZipTarget zipTarget)
    {
        playerZipToPoint.StartZipToPoint(zipTarget);
    }

    #endregion
}
