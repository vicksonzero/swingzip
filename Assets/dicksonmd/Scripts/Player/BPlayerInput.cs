using UnityEngine;
using UnityEngine.EventSystems;


public class BPlayerInput : MonoBehaviour
{

    BPlayer player;
    public BMobileJoystick mobileJoystick;
    public int touchingPointerId = -1;

    public InputObject latestInput = new InputObject();

    void Start()
    {
        player = GetComponent<BPlayer>();
        Debug.Log("mouse.wasDown: " + latestInput.mouse.wasDown);
    }

    void Update()
    {
        // uses update instead of FixedUpdate, input can be lost between FixedUpdate calls

        // keyboard-specific controls
        Vector2 directionalInput = (mobileJoystick.isDown ? mobileJoystick.joystickInput
            : new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"))
        );

        if (Mathf.Abs(directionalInput.x) <= 0.2f) directionalInput.x = 0;
        directionalInput.x = Mathf.Clamp(directionalInput.x, -1, 1);
        directionalInput.y = Mathf.Clamp(directionalInput.y, -1, 1);
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
        if (touchingPointerId == -1 && pointerData.button == PointerEventData.InputButton.Left)
        {
            player.OnVirtualPointerDown(pointerData.position);
            touchingPointerId = pointerData.pointerId;
        }
    }

    public void OnVirtualPointerUp(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;
        if (pointerData.pointerId == touchingPointerId && pointerData.button == PointerEventData.InputButton.Left)
        {
            player.OnVirtualPointerUp(pointerData.position);
            touchingPointerId = -1;
        }
    }

    public struct InputObject
    {
        public InputObjectAxis mouse;
        public InputObjectAxis stickL;
        public bool isReplayEnd;
    }

    public struct InputObjectAxis
    {
        // mouse
        public int xInt;
        public float getX()
        {
            return xInt / 1000f;
        }

        public int yInt;
        public float getY()
        {
            return yInt / 1000f;
        }

        public bool wasDown;
        public bool wasUp;
    }

}
