using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BPlayer))]
public class BPlayerInput : MonoBehaviour
{

    BPlayer player;
    public BZipTarget zipTargetPrefab;
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

        if (Input.GetKeyDown(KeyCode.W))
        {
            player.OnJumpInputDown();
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            player.OnJumpInputUp();
        }
    }

    public void OnVirtualPointerDown(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;
        if (player.zipButton != null && player.zipButton.gameObject.activeSelf)
        {
            var zipTarget = Instantiate(zipTargetPrefab, player.zipButton.transform.position, Quaternion.identity);
            player.StartZipToPoint(zipTarget);
        }
        else if (!player.IsZippingToPoint())
        {
            Vector3 pos = pointerData.position;
            pos.z = 10.0f;
            Vector2 pos2 = Camera.main.ScreenToWorldPoint(pos);
            player.PutGrapple(pos2);
        }
        touchingPointerId = pointerData.pointerId;
    }

    public void OnVirtualPointerUp(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;
        if (pointerData.pointerId == touchingPointerId)
        {
            player.RemoveGrapple();
            touchingPointerId = -1;
        }
    }
}
