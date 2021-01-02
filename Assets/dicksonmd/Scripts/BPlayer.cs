using System;
using UnityEngine;

[RequireComponent(typeof(BPlayerController))]
public class BPlayer : MonoBehaviour
{

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    float accelerationTimeAirborne = .2f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed = 6;

    public float maxFallingSpeed = 6;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;
    float timeToWallUnstick;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    BPlayerController controller;

    Vector2 directionalInput;
    bool wallSliding;
    int wallDirX;


    public BGrapple grapplePrefab;
    BGrapple grapple;


    public float xDir = 0;

    public int jumpDownFrames = 0;
    public int jumpUpFrames = 0;

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
    }

    void Update()
    {
        if (CanDoSwing())
        {
            UpdateSwing();
        }
        else
        {
            UpdateMovement();
        }
        if (grapple != null && grapple.isActive)
        {
            UpdateGrappleLine();
        }
    }

    private bool CanDoSwing()
    {
        return grapple != null && grapple.isActive && grapple.IsComplete();
    }

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

    private void UpdateGrappleLine()
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

    private void InitGrapple()
    {
        // init grapple length
        grapple.grappleLength = grapple.DistanceTo(transform);

        UpdateGrappleLine();
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
        if (grapple == null)
        {
            grapple = Instantiate(grapplePrefab);
        }

        grapple.gameObject.SetActive(true);
        grapple.transform.position = pos;
        grapple.StartGrapple();
    }

    public void RemoveGrapple()
    {
        if (grapple == null)
        {
            return;
        }

        grapple.EndGrapple();
        grapple.gameObject.SetActive(false);
    }


}
