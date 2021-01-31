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
    [HideInInspector]
    public BPlayerSwing playerSwing;
    public BGrapple grapplePrefab;
    [HideInInspector]
    public BGrapple grapple;

    [Header("Zip-to-point")]
    [HideInInspector]
    public BPlayerZipToPoint playerZipToPoint;
    public BZipButton zipButtonPrefab;
    [HideInInspector]
    public BZipButton zipButton;
    [HideInInspector]
    public BZipTarget zipTarget;


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
    public float dashDistance = 10;
    public float dashTime = 1; // seconds
    Vector3 dashTarget;
    public float dashUntil = 0;
    public float dashEndSpeed = 0; // per second
    public float dashSpeed = 0; // per second
    public float dashDeceleration = 0; // per second squared
    public float dashSpeedProgress = 0; // per second

    #endregion


    #region Auto-move Fields

    [Header("Auto-move")]
    public float xDir = 0;

    [Header("Input Buffers")]
    public int jumpDownFrames = 0;
    public int jumpUpFrames = 0;

    #endregion

    void Start()
    {
        controller = GetComponent<BPlayerController>();

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

        dashSpeed = 2 * dashDistance / dashTime - dashEndSpeed;// Mathf.Sqrt(2 * dashDistance * dashDeceleratation);//dashDistance / dashTime + 0.5f * dashDeceleratation * dashTime;
        dashDeceleration = (dashSpeed * dashSpeed - dashEndSpeed * dashEndSpeed) / 2 / dashDistance;
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
            UpdateDash();
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
    }

    #region State Accessors Methods

    public bool IsZippingToPoint()
    {
        return zipTarget != null;
    }

    public bool IsDashing()
    {
        return Time.time < dashUntil;
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

    private void UpdateDash()
    {
        var remainingDisplacement = dashTarget - transform.position;

        dashSpeedProgress = dashSpeed - dashDeceleration * Mathf.Clamp(dashTime - dashUntil + Time.time, 0, dashTime);
        velocity = remainingDisplacement.normalized * dashSpeedProgress;

        if (remainingDisplacement.magnitude < dashSpeedProgress * Time.deltaTime)
        {
            return;
        }
        controller.Move(velocity * Time.deltaTime, directionalInput);
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
    
    public void StartDash()
    {
        Debug.Log("StartDash");
        Vector2 displacementToGrapple = grapple.transform.position - transform.position;
        dashTarget = transform.position + ((Vector3)displacementToGrapple.normalized * dashDistance);
        dashUntil = Time.time + dashTime;
        dashSpeedProgress = dashSpeed;
    }

    public void StartZipToPoint(BZipTarget zipTarget){
        playerZipToPoint.StartZipToPoint(zipTarget);
    }

    #endregion
}
