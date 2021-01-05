using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BZipButton : MonoBehaviour
{
    public Transform buttonSprite;
    public Transform buttonSprite2;

    public float zipToPointInputWindow = 0.25f;

    Coroutine sleepTimer;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitButton(Vector3 position)
    {
        if (sleepTimer != null)
        {
            StopCoroutine(sleepTimer);
        }
        transform.position = position;

        buttonSprite.transform.rotation = Quaternion.identity;
        buttonSprite.transform.DORotate(new Vector3(0, 0, -45), 0.2f);

        buttonSprite2.transform.rotation = Quaternion.identity;
        buttonSprite2.transform.localScale = Vector3.one * 0.4f;
        buttonSprite2.transform.DORotate(new Vector3(0, 0, 45), 0.2f);
        buttonSprite2.transform.DOScale(Vector3.one * 0.7f, 0.2f);

        sleepTimer = StartCoroutine(SleepAfter(zipToPointInputWindow));
    }

    public void StopButton()
    {
        if (sleepTimer != null)
        {
            StopCoroutine(sleepTimer);
        }
        gameObject.SetActive(false);
    }

    IEnumerator SleepAfter(float amount)
    {
        yield return new WaitForSeconds(amount);
        gameObject.SetActive(false);
    }
}
