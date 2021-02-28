using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BFuelCell : BItem
{
    public Transform cellGraphics;
    public ParticleSystem enabledPS;
    public ParticleSystem disabledPS;
    public ParticleSystem gotPS;

    Collider2D hitbox;
    // Start is called before the first frame update
    internal new void Start()
    {
        hitbox = GetComponent<Collider2D>();
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal override void disableSelf()
    {
        cellGraphics.gameObject.SetActive(false);
        enabledPS.Stop();
        disabledPS.Play();
        hitbox.enabled = false;
    }
    internal override void enableSelf()
    {
        cellGraphics.gameObject.SetActive(true);
        enabledPS.Play();
        disabledPS.Stop();
        hitbox.enabled = true;
    }
    internal override void playGotAnimSelf()
    {
        gotPS.Play();
    }
}
