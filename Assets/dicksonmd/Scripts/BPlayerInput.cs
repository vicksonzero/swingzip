using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BPlayer))]
public class BPlayerInput : MonoBehaviour
{

    BPlayer player;
    public BZipTarget zipTargetPrefab;

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
        if (player.zipButton != null && player.zipButton.gameObject.activeSelf)
        {
            var zipTarget = Instantiate(zipTargetPrefab, player.zipButton.transform.position, Quaternion.identity);
            player.StartZipToPoint(zipTarget);
        }
        else if(!player.IsZippingToPoint())
        {
            var pos = Input.mousePosition;
            pos.z = 10.0f;
            Vector2 pos2 = Camera.main.ScreenToWorldPoint(pos);
            player.PutGrapple(pos2);
        }
    }

    public void OnVirtualPointerUp(BaseEventData evt)
    {
        player.RemoveGrapple();
    }
}
