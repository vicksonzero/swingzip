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
            if (!isSolidGrapple && IsComplete())
            {
                RemoveGrapple();
            }
        }
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
        player.DoGravity();

        DoGrappleConstraint();

        player.DisplaceSelf();
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
    private void DoGrappleConstraint()
    {
        if (player.grapple == null)
        {
            return;
        }

        var nextPosition = transform.position + player.velocity * Time.deltaTime;

        Vector2 displacementFromGrapple = nextPosition - player.grapple.transform.position;
        var dist = displacementFromGrapple.magnitude;
        if (dist > grappleLength)
        {
            Vector3 targetDisplacementFromGrapple = displacementFromGrapple.normalized * grappleLength;
            Vector3 targetPosition = player.grapple.transform.position + targetDisplacementFromGrapple;
            Vector3 targetMovement = targetPosition - transform.position;
            player.controller.Move(targetMovement, Vector3.zero);
            player.velocity = targetMovement / Time.deltaTime;
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

    public void PutGrapple(Vector2 pos)
    {
        if (player.grapple == null)
        {
            player.grapple = Instantiate(player.grapplePrefab);
        }

        player.grapple.gameObject.SetActive(true);
        player.grapple.transform.position = pos;
        var hitColliders = Physics2D.OverlapCircleAll(pos, 0.5f);

        Debug.Log(hitColliders.Length);
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

        if (!IsComplete())
        {
            player.StartDash();
            player.zipButton.gameObject.SetActive(true);
            player.zipButton.InitButton(player.grapple.transform.position);
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
