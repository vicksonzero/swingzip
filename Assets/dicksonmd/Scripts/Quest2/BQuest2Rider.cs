using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Brain of a quest receiver, handler, as a Rider in the Company
/// Owns all CargoSpaces, get a CargoSpace and assign cargo
/// tracks BQuest2Objective progress that are owned by BQuest2ObjectiveList
/// </summary>
public class BQuest2Rider : MonoBehaviour
{
    public BQuest2Quest[] currentQuests;
    public BCargoSpace[] cargoSpaces;

    BQuest2UI ui;
    // Start is called before the first frame update
    void Start()
    {
        cargoSpaces = GetComponents<BCargoSpace>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool TryAcceptQuest(BQuest2Quest quest)
    {
        var canAcceptQuest = quest.CanAcceptQuest(this);
        if (!canAcceptQuest) return false;

        currentQuests = currentQuests.Concat(new[] { quest }).ToArray();
        quest.SetupQuest(this);
        return true; // return success
    }

    public bool HasFreeCargoSpace(int amount = 1)
    {
        return cargoSpaces.Where(c => !c.hasCargo).Count() >= amount;
    }
}
