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
    void Start()
    {
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

        Vector2 pos = transform.position;

        float posXForSlope = pos.x == 0 ? 0.000001f : pos.x;
        float slope = pos.y / posXForSlope; // slope with the center of the screen



        anchor.transform.position = pos;
        Debug.DrawLine(new Vector2(minX, minY), new Vector2(maxX, maxY));
        // if (player != null)
        // {
        //     var disp = transform.position - player.transform.position;
        //     if(disp.x > )
        //     // anchor.transform.position = Vector3.zero;
        // }
        // else
        // {
        //     anchor.transform.localPosition = Vector3.zero;
        // }

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
