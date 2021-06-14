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
        icon.gameObject.SetActive(val);
    }
}
