using DG.Tweening;
using UnityEngine;

public class BZipTarget : MonoBehaviour
{
    public Transform buttonSprite;
    public Transform buttonSprite2;
    public LineRenderer lineRenderer;

    void Start()
    {
        buttonSprite.transform.localScale = Vector3.one * 0.3f;

        buttonSprite2.transform.rotation = Quaternion.identity;
        buttonSprite2.transform.localScale = Vector3.one * 1.2f;
        buttonSprite2.transform.DORotate(new Vector3(0, 0, -45), 0.2f);
        buttonSprite2.transform.DOScale(Vector3.one * 0.5f, 0.2f);
    }
    public void UpdateLineRenderer(Vector3 swingPos)
    {
        lineRenderer.SetPosition(0, Vector3.zero);
        lineRenderer.SetPosition(1, swingPos - transform.position);
    }
    void OnDestroy()
    {
        DOTween.Kill(buttonSprite2);
    }
}
