using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BScoreToast : MonoBehaviour
{
    public Text label;
    // Start is called before the first frame update
    void Start()
    {
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(label.DOFade(1f, 0.5f).From(0));
        mySequence.Join(transform.DOLocalMoveY(0, 0.5f).From(-100));
        mySequence.Join(transform.DOScale(1, 0.5f).From(5));
        mySequence.Append(label.DOFade(0, 1f).SetDelay(2));
        mySequence.onComplete = () => { Destroy(gameObject); };
    }

    // Update is called once per frame
    void Update()
    {

    }
}
