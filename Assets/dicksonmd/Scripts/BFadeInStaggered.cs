using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFadeInStaggered : MonoBehaviour
{
    public Transform[] items;
    public float delay;
    public float interval;
    public float tweenDuration;
    public float tweenAmt;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in items)
        {
            item.gameObject.SetActive(false);
        }
        StartCoroutine(FadeInOneByOne());
    }

    IEnumerator FadeInOneByOne()
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < items.Length; i++)
        {
            var item = items[i];
            item.gameObject.SetActive(true);
            item.transform.localPosition = Vector3.left * tweenAmt;
            item.DOLocalMoveX(0, tweenDuration);
            yield return new WaitForSeconds(interval);
        }
    }

}
