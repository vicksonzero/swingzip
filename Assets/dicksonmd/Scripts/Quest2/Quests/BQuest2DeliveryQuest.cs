using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BQuest2DeliveryQuest : BQuest2Quest
{
    public BQuest2Order order;
    public override void InitQuest()
    {
        questGiver.RegisterQuest(this);
    }

    public override bool CanAcceptQuest(BQuest2Rider rider)
    {
        return rider.HasFreeCargoSpace();
    }

    public override void SetupQuest(BQuest2Rider rider)
    {
        Debug.Log($"BQuest2DeliveryQuest.SetupQuest {title}");
        var ui = FindObjectOfType<BQuest2UI>();

        var cargoSpace = rider.cargoSpaces.First(c => !c.hasCargo);
        cargoSpace.hasCargo = true;
        cargoSpace.drone.AttachItem(order.itemIcon);

        order.toNpc.trigger.triggerEnter += (Collider2D collider) =>
        {
            OnOrderArrive();
        };

        ui.distanceUI.target = order.toNpc.transform; // TODO: let rider control this by itself?
        ui.EnableUI(ui.distanceUI);

        var hasTimer = false;
        var nextState = hasTimer ? States.PREPARE : States.IN_PROGRESS;
        ChangeState(nextState, "SetupQuest"); // skipping prepare coz no timer
    }

    public override void TearDownQuest()
    {
        // nothing
    }

    private void OnOrderArrive()
    {
        // show result
        ChangeState(States.RESULT, "OnOrderArrive");
    }
}
