using UnityEngine;

[RequireComponent(typeof(BPlayerController))]
public class BPlayer : MonoBehaviour
{
    BPlayerController controller;
    Vector2 directionalInput;

    public enum EPlayerStates { FREE, SWINGING, DASHING, ZIPPING_TO_POINT, RUN, WALL_RUN };

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
    Vector3 velocity;
    float velocityXSmoothing;

    #endregion

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

    #region Grapple Fields

    [Header("Grapple")]
    public BGrapple grapplePrefab;

    BGrapple grapple;

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

    #region Zip-to-point Fields

    [Header("Zip-to-point")]
    public BZipButton zipButtonPrefab;
    public BZipButton zipButton;
    public BZipTarget zipTarget;
    public float zipSpeed = 0; // per second
    public float zipStartSpeedCap = 3; // per second
    public float zipAcceleration = 0; // per second squared
    public float zipSpeedProgress = 0; // per second
    public float zipUntil = 0;

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

        dashSpeed = 2 * dashDistance / dashTime - dashEndSpeed;// Mathf.Sqrt(2 * dashDistance * dashDeceleratation);//dashDistance / dashTime + 0.5f * dashDeceleratation * dashTime;
        dashDeceleration = (dashSpeed * dashSpeed - dashEndSpeed * dashEndSpeed) / 2 / dashDistance;
    }

    void Update()
    {
        if (IsZippingToPoint())
        {
            currentState = EPlayerStates.ZIPPING_TO_POINT;
            UpdateZipToPoint();
        }
        else if (IsDashing())
        {
            currentState = EPlayerStates.DASHING;
            UpdateDash();
        }
        else if (IsSwinging())
        {
            currentState = EPlayerStates.SWINGING;
            UpdateSwing();
        }
        else
        {
            currentState = EPlayerStates.FREE;
            UpdateMovement();
        }


        if (grapple != null && grapple.isActive)
        {
            RenderGrappleLine();
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

    public bool IsSwinging()
    {
        return grapple != null && grapple.isActive && grapple.IsComplete();
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

    void UpdateSwing()
    {
        if (!grapple.wasComplete)
        {
            InitGrapple();
        }
        DoGravity();

        DoGrappleConstraint();

        DisplaceSelf();
        grapple.wasComplete = true;
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

    private void UpdateZipToPoint()
    {
        Vector2 remainingDisplacement = zipTarget.transform.position - transform.position;

        zipSpeedProgress += zipAcceleration * Time.deltaTime;

        velocity = remainingDisplacement.normalized * zipSpeedProgress;

        if (Time.time > zipUntil)
        {
            Destroy(zipTarget.gameObject);
            zipTarget = null;

            return;
        }

        if (remainingDisplacement.magnitude <= zipSpeedProgress * Time.deltaTime)
        {
            // snap to end point
            controller.Move(remainingDisplacement, directionalInput);
            Destroy(zipTarget.gameObject);
            zipTarget = null;
        }
        else
        {
            // normal zip to point
            controller.Move(velocity * Time.deltaTime, directionalInput);
            zipTarget.UpdateLineRenderer(transform.position);
        }
    }

    #endregion

    #region Constraints Methods
    private void RenderGrappleLine()
    {
        grapple.UpdateLineRenderer(transform.position);
    }

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

    private void DoGravity()
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

    private void DisplaceSelf()
    {
        controller.Move(velocity * Time.deltaTime, directionalInput);
    }

    private void DoGrappleConstraint()
    {
        if (grapple == null)
        {
            return;
        }

        var nextPosition = transform.position + velocity * Time.deltaTime;

        Vector2 displacementFromGrapple = nextPosition - grapple.transform.position;
        var dist = displacementFromGrapple.magnitude;
        if (dist > grapple.grappleLength)
        {
            Vector3 targetDisplacementFromGrapple = displacementFromGrapple.normalized * grapple.grappleLength;
            Vector3 targetPosition = grapple.transform.position + targetDisplacementFromGrapple;
            Vector3 targetMovement = targetPosition - transform.position;
            controller.Move(targetMovement, Vector3.zero);
            velocity = targetMovement / Time.deltaTime;
        }
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
    private void InitGrapple()
    {
        Debug.Log("InitGrapple");
        // init grapple length
        grapple.grappleLength = grapple.DistanceTo(transform);

        RenderGrappleLine();
    }

    public void PutGrapple(Vector2 pos)
    {
        if (grapple == null)
        {
            grapple = Instantiate(grapplePrefab);
        }

        grapple.gameObject.SetActive(true);
        grapple.transform.position = pos;
        grapple.StartGrapple();
        dashUntil = Time.time;
    }

    public void RemoveGrapple()
    {
        if (grapple == null || !grapple.gameObject.activeSelf)
        {
            return;
        }

        if (!grapple.IsComplete())
        {
            StartDash();
            zipButton.gameObject.SetActive(true);
            zipButton.InitButton(grapple.transform.position);
        }

        grapple.EndGrapple();
        grapple.gameObject.SetActive(false);
    }

    public void StartDash()
    {
        Debug.Log("StartDash");
        Vector2 displacementToGrapple = grapple.transform.position - transform.position;
        dashTarget = transform.position + ((Vector3)displacementToGrapple.normalized * dashDistance);
        dashUntil = Time.time + dashTime;
        dashSpeedProgress = dashSpeed;
    }

    public void StartZipToPoint(BZipTarget zipTarget)
    {
        Debug.Log("StartZipToPoint!");
        this.zipTarget = zipTarget;

        if (zipButton != null)
        {
            zipButton.StopButton();
        }

        // make t the subject of s=ut+0.5at^2, where u !=0
        // (-u + sqrt(2 a s + u^2))/a
        Vector2 displacementToZipTarget = zipTarget.transform.position - transform.position;
        velocity = Vector3.Project(velocity, displacementToZipTarget);
        zipSpeedProgress = Mathf.Clamp(velocity.magnitude, 0, zipStartSpeedCap);
        velocity = velocity.normalized * zipSpeedProgress;

        var s = displacementToZipTarget.magnitude;
        var a = zipAcceleration;
        var u = zipSpeedProgress;

        zipUntil = Time.time + (-u + Mathf.Sqrt(2 * a * s + u * u)) / a;
        Debug.Log(Time.time + " " + zipUntil);
    }

    #endregion
}
