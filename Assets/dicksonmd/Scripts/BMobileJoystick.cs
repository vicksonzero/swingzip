using UnityEngine;
using UnityEngine.EventSystems;

public class BMobileJoystick : MonoBehaviour
{
    RectTransform rt;
    public RectTransform parentRT;
    Camera mainCamera;
    Vector2 startingPosition;
    public bool isDown;
    public RectTransform joystickKnob;
    Vector2 knobStartingPosition;

    public float joystickRadius = 150f;
    public int touchingPointerId = -1;

    public Vector2 joystickInput = Vector2.zero;

    public float sensitivity = 1.1f;

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        mainCamera = Camera.main;
        //parentRT = transform.parent.GetComponent<RectTransform>();
        startingPosition = rt.localPosition;
        knobStartingPosition = joystickKnob.localPosition;

        if (!Application.isMobilePlatform)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnJoystickPointerDown(BaseEventData data)
    {
        if (isDown) return;
        PointerEventData pointerData = data as PointerEventData;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRT, pointerData.position, null, out localPoint);

        touchingPointerId = pointerData.pointerId;

        StartJoystick(localPoint);
    }

    public void OnJoystickPointerMove(BaseEventData data)
    {
        if (!isDown)
        {
            return;
        }

        PointerEventData pointerData = data as PointerEventData;
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, pointerData.position, null, out localPoint);

        Vector2 knobPosition = Vector2.ClampMagnitude(localPoint - knobStartingPosition, joystickRadius);
        joystickKnob.localPosition = knobPosition + knobStartingPosition;

        joystickInput = Vector2.ClampMagnitude((localPoint - knobStartingPosition) / joystickRadius, 1) * sensitivity;
    }

    public void OnJoystickPointerUp(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;
        if (pointerData.pointerId == touchingPointerId)
        {

            EndJoystick();
            touchingPointerId = -1;
        }
    }

    public void StartJoystick(Vector2 position)
    {
        rt.localPosition = position;
        isDown = true;

        joystickInput = Vector2.zero;
    }

    public void EndJoystick()
    {
        rt.localPosition = startingPosition;
        joystickKnob.localPosition = knobStartingPosition;
        isDown = false;
    }
}
