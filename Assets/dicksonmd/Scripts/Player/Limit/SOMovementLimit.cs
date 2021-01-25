using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SOMovementLimit", menuName = "ScriptableObjects/SOMovementLimit", order = 1)]
public class SOMovementLimit : ScriptableObject
{
    public enum SurfaceTypes { VOID, BACK_WALL, PLATFORM, SOLID };
    public SurfaceTypes[] surfaceTypes;
    public bool useLineOfSight = false;
    public int charges = -1; // -1 means infinite
    public enum EventTypes { LANDING, RUN, JUMP, SWING, DASH, ZIP_TO_POINT };
    public EventTypes[] rechargeByDongStuff;
    public float rechargeByCooldown = -1; // in milliseconds, later in frames, -1 means off
    public bool rechargeAtOnce = true;
    public GameObject[] rechargeByItems = new GameObject[] { };
    /*

    On which surfaces: List<Physics Layer>
    Need line of sight?
    Charges: int
        -1 means unlimited
    Recharging methods
        By touching ground?
        By timed cooldowns: float
        Timed cooldowns can recharge one or all?
        By collecting items: Item

    */
}
