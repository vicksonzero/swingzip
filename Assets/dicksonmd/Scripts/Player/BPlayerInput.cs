using UnityEngine;
using UnityEngine.EventSystems;


public class BPlayerInput : MonoBehaviour
{

    BPlayer player;
    public BMobileJoystick mobileJoystick;
    public int touchingPointerId = -1;

    void Start()
    {
        player = GetComponent<BPlayer>();
    }

    void Update()
    {
        // uses update instead of FixedUpdate, input can be lost between FixedUpdate calls

        // keyboard-specific controls
        Vector2 directionalInput = (mobileJoystick.isDown ? mobileJoystick.joystickInput
            : new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
        );
        player.OnDirectionalInput(directionalInput);

        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     player.OnJumpInputDown();
        // }
        // if (Input.GetKeyUp(KeyCode.W))
        // {
        //     player.OnJumpInputUp();
        // }
    }

    public void OnVirtualPointerDown(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;
        player.OnVirtualPointerDown(pointerData.position);
        touchingPointerId = pointerData.pointerId;
    }

    public void OnVirtualPointerUp(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;
        if (pointerData.pointerId == touchingPointerId)
        {
            player.OnVirtualPointerUp(pointerData.position);
            touchingPointerId = -1;
        }
    }
}
