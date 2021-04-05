using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BPlayerSwing : MonoBehaviour
{
    [Header("State")]
    public bool wasComplete = false;
    public bool isActive = false;
    public bool isSolidGrapple = false;
    public float grappleLength = 0;
    public float grappleCompleteTime = 0;
    public bool pointerWasUp = false;
    public bool pointerWasDownAgain = false;

    public float swingPull = 0.1f;
    public float swingBoost = 0.4f;

    [Header("Linkages")]
    BPlayer player;

    [Header("Config")]
    public SOMovementLimit limits;
    [Tooltip("Common click time is 220ms")]
    public float grappleShootTime = 220; // in ms, TODO: Change to frames

    // when player is falling within grapple constraint, speed that is normal to travelling direction will be discarded.
    // Choose how much percent of speed is kept by rotating speed vector component into travelling direction
    [Tooltip("x percent speed will be added to swing speed per tick")]
    [Range(0, 1)]
    public float tangentSpeedKept = 0.7f;

    // (>0 means swing end-point is always higher, <0 means energy is lost while swinging)
    [Tooltip("x percent speed will be added to swing speed per tick")]
    [Range(-1, 1)]
    public float swingSpeedBonusPerTick = 0f;

    public bool canInterrupt = true;
    public bool interruptByPointerUp = true;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<BPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.grapple != null && isActive)
        {
            RenderGrappleLine();
        }
    }

    public bool IsShooting()
    {
        return player.grapple != null && isActive && !IsComplete();
    }

    public bool IsSwinging()
    {
        return player.grapple != null && isActive && isSolidGrapple && IsComplete();
    }

    public void UpdateSwing()
    {
        if (!wasComplete)
        {
            InitGrapple();
        }
        DoDirectionalBoost(player.directionalInput);
        player.DoSwingGravity();

        DoGrappleConstraint();
        DoSwingPull();

        player.directionalInput.y = -1; // HACK: force through platforms when swinging
        player.DisplaceSelf(false);
        wasComplete = true;
    }

    public bool IsComplete()
    {
        return Time.time >= grappleCompleteTime;
    }

    private void RenderGrappleLine()
    {
        player.grapple.UpdateLineRenderer(transform.position, GetGrapplePercent());
    }

    private void DoSwingPull()
    {
        Vector2 grapplePos = player.grapple.transform.position;
        Vector2 displacementFromGrapple = (Vector2)transform.position - grapplePos;
        var dist = displacementFromGrapple.magnitude;
        if (dist > grappleLength)
        {
            player.velocity += (Vector3)(-displacementFromGrapple.normalized * swingPull * Time.deltaTime);
        }
    }

    private void DoDirectionalBoost(Vector2 directionalInput)
    {
        Vector2 grapplePos = player.grapple.transform.position;
        Vector2 displacementFromGrapple = (Vector2)transform.position - grapplePos;
        Vector2 veloToGrapple = player.velocity - Vector3.Project((Vector2)player.velocity, displacementFromGrapple);
        float signedAngle = Vector3.SignedAngle(displacementFromGrapple, (Vector2)player.velocity, Vector3.forward);
        // var dir = Quaternion.AngleAxis(90, Vector3.up) * displacementFromGrapple;
        var sign = directionalInput.x < 0 ? -1 : (directionalInput.x > 0 ? 1 : 0);
        player.velocity += (Vector3)veloToGrapple.normalized * Mathf.Sign(signedAngle) * sign * swingBoost * Time.deltaTime;
    }
    private void DoGrappleConstraint()
    {
        if (player.grapple == null)
        {
            return;
        }

        Vector2 grapplePos = player.grapple.transform.position;
        Vector2 displacementFromGrapple = (Vector2)transform.position - grapplePos;
        var dist = displacementFromGrapple.magnitude;


        if (dist > grappleLength)
        {
            Vector2 veloToGrapple = Vector3.Project((Vector2)player.velocity, displacementFromGrapple);
            player.velocity = (Vector2)player.velocity - veloToGrapple;
        }
    }
    private void InitGrapple()
    {
        Debug.Log("InitGrapple");
        // init grapple length
        grappleLength = DistanceTo(player.grapple.transform);

        player.grapple.PlayAnchorTweens();
        RenderGrappleLine();
    }

    public void PutGrapple(Vector2 pos, Collider2D[] hitColliders)
    {
        if (player.grapple == null)
        {
            player.grapple = Instantiate(player.grapplePrefab);
        }

        pointerWasUp = false;
        pointerWasDownAgain = false;
        player.grapple.gameObject.SetActive(true);
        player.grapple.transform.position = pos;

        var hasBackWall = false;
        foreach (var collider in hitColliders)
        {
            if (collider.GetComponent<BBackWall>() != null)
            {
                hasBackWall = true;
                break;
            }
        }
        StartGrapple(hasBackWall);
        player.StopDash();
    }

    public void RemoveGrapple()
    {
        if (player.grapple == null || !player.grapple.gameObject.activeSelf)
        {
            return;
        }

        EndGrapple();
        player.grapple.gameObject.SetActive(false);
    }

    public void StartGrapple(bool isOnBackWall)
    {
        isSolidGrapple = isOnBackWall;

        player.grapple.StartGrappleSprites(isSolidGrapple);

        grappleCompleteTime = Time.time + grappleShootTime / 1000f;
        isActive = true;
    }

    public void EndGrapple()
    {
        isActive = false;
        wasComplete = false;
        grappleCompleteTime = 0;
        pointerWasUp = false;
        pointerWasDownAgain = false;
    }

    public float DistanceTo(Transform t)
    {
        Vector2 displacement = t.position - transform.position;
        return displacement.magnitude;
    }

    public float GetGrapplePercent()
    {
        if (IsComplete())
        {
            return 1;
        }

        return Mathf.Clamp01(1 - (grappleCompleteTime - Time.time) / (grappleShootTime / 1000f));
    }
}
