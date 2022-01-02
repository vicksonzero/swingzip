using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Tracks quest progression. Is persisted between save/load
/// </summary>
public abstract class BQuest2Quest : MonoBehaviour
{
    // AVAILABLE -> PREPARE or COUNTDOWN
    // PREPARE -> IN_PROGRESS
    // COUNTDOWN -> IN_PROGRESS
    // IN_PROGRESS -> RESULT
    // any -> RESULT
    // RESULT -> FINISHED
    public enum States
    {
        AVAILABLE, // Available to be picked up
        COUNTDOWN, // (optional) if an objective has a timer or objective title is blocking, the player cannot move during a countdown
        PREPARE, // (optional) if a quest has a timer, and the player has picked up an order, but not yet started running. Timer will not tick.
        IN_PROGRESS, // running; on fulfilling an order, goes back to AVAILABLE
        RESULT, // show ending screen with a confirm button. the player cannot move until pressing OK button
        FINISHED, // objective is finished. waiting for Director to change it to AVAILABLE (if needed)
    };
    public string title;
    public string description;
    public string reward;
    public Sprite icon;
    public Color iconBg;

    [Header("States")]
    public States state = States.AVAILABLE; // default IDLE

    public IQuestGiver questGiver;


    public delegate void StateChanged(States before, string payload);
    public StateChanged stateChanged;

    private bool stateIsChanging = false;

    /// <summary>
    /// Make this quest available to the player, for example, activate NPC, adding this quest to the NPC
    /// </summary>
    public abstract void InitQuest();

    /// <summary>
    /// Whether rider can accept this quest now
    /// "Quest available or not" is determined by Quest2Director,
    /// but "Quest acceptable or not" is determined by individual quests
    /// </summary>
    public abstract bool CanAcceptQuest(BQuest2Rider rider);

    /// <summary>
    /// Set up the stage for the quest, for example, barriers or extra dangers
    /// </summary>
    public abstract void SetupQuest(BQuest2Rider rider);

    /// <summary>
    /// Tear down the stage after the quest
    /// </summary>
    public abstract void TearDownQuest();

    /// <summary>
    /// Change state, signaling its state change as well
    /// </summary>
    protected void ChangeState(States newState, string payload = "")
    {
        if (stateIsChanging)
        {
            throw new System.Exception($"BQuest2Quest cannot ChangeState() while stateIsChanging (title={title})");
        }
        stateIsChanging = true;
        var oldState = state;
        state = newState;
        stateChanged?.Invoke(oldState, payload);
        stateIsChanging = false;
    }
}
