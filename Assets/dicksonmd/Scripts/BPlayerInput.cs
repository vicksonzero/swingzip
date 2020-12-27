using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent (typeof (BPlayer))]
public class BPlayerInput : MonoBehaviour {

	BPlayer player;

    void Start () {
		player = GetComponent<BPlayer> ();
	}

	void Update () {
        // uses update instead of FixedUpdate, input can be lost between FixedUpdate calls

        // keyboard-specific controls
        //Vector2 directionalInput = new Vector2 (Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw ("Vertical"));
		//player.OnDirectionalInput (directionalInput);

		if (Input.GetKeyDown (KeyCode.Space)) {
			player.OnJumpInputDown ();
		}
		if (Input.GetKeyUp (KeyCode.Space)) {
			player.OnJumpInputUp ();
		}
	}

    public void OnVirtualPointerDown(BaseEventData evt)
    {
        
    }

    public void OnVirtualPointerUp(BaseEventData evt)
    {

    }
}
