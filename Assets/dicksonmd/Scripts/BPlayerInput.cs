using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BPlayer))]
public class BPlayerInput : MonoBehaviour
{

    BPlayer player;
    public Transform grapplePrefab;

    Transform grappleHead;

    void Start()
    {
        player = GetComponent<BPlayer>();
    }

    void Update()
    {
        // uses update instead of FixedUpdate, input can be lost between FixedUpdate calls

        // keyboard-specific controls
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
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

    public void OnVirtualPointerDown(BaseEventData evt)
    {
        var pos = Input.mousePosition;
        pos.z = 10.0f;
        Vector2 pos2 = Camera.main.ScreenToWorldPoint(pos);

        if (grappleHead == null)
        {
            grappleHead = Instantiate(grapplePrefab);
        }
        grappleHead.gameObject.SetActive(true);
        grappleHead.transform.position = pos2;
    }

    public void OnVirtualPointerUp(BaseEventData evt)
    {

        if (grappleHead != null)
        {
            grappleHead.gameObject.SetActive(false);
        }
    }
}
