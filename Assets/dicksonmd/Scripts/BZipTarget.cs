using DG.Tweening;
using UnityEngine;

public class BZipTarget : MonoBehaviour
{
    public Transform anchor;
    public Transform buttonSprite;
    public Transform buttonSprite2;
    public LineRenderer lineRenderer;
    public BPlayer player;
    public float spriteSize;

    public float vertExtent;
    public float horiExtent;
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    new Collider2D collider;
    void Start()
    {
        collider = GetComponent<Collider2D>();
        if (player == null)
        {
            player = FindObjectOfType<BPlayer>();
        }

        buttonSprite.transform.localScale = Vector3.one * 0.3f;

        buttonSprite2.transform.rotation = Quaternion.identity;
        buttonSprite2.transform.localScale = Vector3.one * 1.2f;
        buttonSprite2.transform.DORotate(new Vector3(0, 0, -45), 0.2f);
        buttonSprite2.transform.DOScale(Vector3.one * 0.5f, 0.2f);
    }
    void Update()
    {
        vertExtent = Camera.main.orthographicSize;
        horiExtent = vertExtent * Screen.width / Screen.height;

        Vector2 cameraPos = Camera.main.transform.position;
        // Calculations assume map is position at the origin
        minX = cameraPos.x - horiExtent + spriteSize;
        maxX = cameraPos.x + horiExtent - spriteSize;
        minY = cameraPos.y - vertExtent + spriteSize;
        maxY = cameraPos.y + vertExtent - spriteSize;

        Vector2 playerPos = player.transform.position;
        Vector2 pos = transform.position;
        Vector2 displacement = pos - playerPos;
        float distance = displacement.magnitude;

        float posXForSlope = pos.x == 0 ? 0.000001f : pos.x;
        float slope = pos.y / posXForSlope; // slope with the center of the screen


        Debug.DrawLine(new Vector2(minX, minY), new Vector2(maxX, maxY));
        Debug.DrawLine(new Vector2(maxX, minY), new Vector2(minX, maxY));

        var v0 = new Vector2(maxX, maxY) - playerPos;
        var v1 = new Vector2(minX, maxY) - playerPos;
        var v2 = new Vector2(minX, minY) - playerPos;
        var v3 = new Vector2(maxX, minY) - playerPos;

        var a1 = (Vector2.SignedAngle(v0, v1) + 360) % 360;
        var a2 = (Vector2.SignedAngle(v0, v2) + 360) % 360;
        var a3 = (Vector2.SignedAngle(v0, v3) + 360) % 360;

        var signedAngle = (Vector2.SignedAngle(v0, displacement) + 360) % 360;

        Vector2 proposedDisplacement = displacement;

        if (signedAngle < a1)
        {
            proposedDisplacement.y = Mathf.Min(pos.y, maxY) - playerPos.y;
            proposedDisplacement.x *= proposedDisplacement.y / displacement.y; // scale x by change of y
        }
        else if (signedAngle < a2)
        {
            proposedDisplacement.x = Mathf.Max(pos.x, minX) - playerPos.x;
            proposedDisplacement.y *= proposedDisplacement.x / displacement.x; // scale y by change of x
        }
        else if (signedAngle < a3)
        {
            proposedDisplacement.y = Mathf.Max(pos.y, minY) - playerPos.y;
            proposedDisplacement.x *= proposedDisplacement.y / displacement.y; // scale x by change of y
        }
        else
        {
            proposedDisplacement.x = Mathf.Min(pos.x, maxX) - playerPos.x;
            proposedDisplacement.y *= proposedDisplacement.x / displacement.x; // scale y by change of x
        }

        if (Mathf.Abs(proposedDisplacement.y) - Mathf.Abs(displacement.y) < 0.01f)
        {
            anchor.transform.position = playerPos + proposedDisplacement;
        }
        else
        {
            // anchor.transform.position = pos;
            anchor.transform.position = playerPos + proposedDisplacement;
        }
        collider.offset = playerPos + proposedDisplacement - (Vector2)transform.position;

    }
    public void UpdateLineRenderer(Vector3 swingPos)
    {
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, swingPos - transform.position);
    }
    void OnDestroy()
    {
        DOTween.Kill(buttonSprite2);
        player = null;
    }
}
