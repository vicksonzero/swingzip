using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BInteractionButton : MonoBehaviour
{
    public bool isJumping = true;
    public SpriteRenderer icon;

    private Tween tween;


    // Start is called before the first frame update
    void Start()
    {
        tween = icon.transform.DOLocalMoveY(0.2f, 0.3f).SetLoops(-1, LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        if (isJumping && !tween.IsPlaying())
        {
            tween.Restart();
        }
        if (!isJumping && tween.IsPlaying())
        {
            tween.SmoothRewind();
        }
    }

    public void ToggleIcon(bool val)
    {
        if (!val && tween.IsPlaying())
        {
            tween.Rewind();
        }
        else
        {
            tween.Restart();
        }
        icon.gameObject.SetActive(val);
    }

    public void ToggleIconDim(bool val)
    {
        if (!val && tween.IsPlaying())
        {
            tween.Rewind();
            isJumping = val;
        }
        else
        {
            tween.Restart();
            isJumping = val;
        }
        var color = icon.color;
        color.r = color.g = color.b = (val ? 1 : 0.5f);
        icon.color = color;
    }
}
