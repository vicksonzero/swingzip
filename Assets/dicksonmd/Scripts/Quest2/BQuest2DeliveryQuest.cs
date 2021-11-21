using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BQuest2DeliveryQuest : BQuest2Quest
{
    public BQuest2Order order;
    public override void InitQuest()
    {
        questGiver.RegisterQuest(this);
    }

    public override void SetupQuest()
    {
        // nothing
    }

    public override void TearDownQuest()
    {
        // nothing
    }
}
