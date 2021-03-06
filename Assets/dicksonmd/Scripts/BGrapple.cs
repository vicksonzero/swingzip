﻿using DG.Tweening;
using UnityEngine;

public class BGrapple : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public Transform buttonSprite;
    public Transform buttonSprite2;
    public Transform buttonSpriteDisabled;
    void Start()
    {
    }

    public void StartGrappleSprites(bool isSolidGrapple)
    {
        if (isSolidGrapple)
        {
            buttonSprite.transform.localScale = Vector3.one * 0.2f;
            buttonSprite2.transform.localScale = Vector3.one * 0.4f;
            buttonSpriteDisabled.transform.localScale = Vector3.zero;
        }
        else
        {
            buttonSprite.transform.localScale = Vector3.one * 0;
            buttonSprite2.transform.localScale = Vector3.one * 0;
            buttonSpriteDisabled.transform.localScale = Vector3.one;
        }

    }

    public void PlayAnchorTweens()
    {
        buttonSprite.transform.localScale = Vector3.one * 0.3f;
        buttonSprite.transform.rotation = Quaternion.identity;
        buttonSprite.transform.DORotate(new Vector3(0, 0, 90f), 0.4f);

        buttonSprite2.transform.localScale = Vector3.one * 0.1f;
        buttonSprite2.transform.DOScale(Vector3.one * 0.6f, 0.2f);
    }

    public void UpdateLineRenderer(Vector3 swingPos, float grapplePercent)
    {
        Vector2 lineVector = swingPos - transform.position;
        lineRenderer.SetPosition(0, lineVector);
        lineRenderer.SetPosition(1, lineVector * (1 - grapplePercent));
    }
    void OnDestroy()
    {
        DOTween.Kill(buttonSprite);
        DOTween.Kill(buttonSprite2);
    }

}
