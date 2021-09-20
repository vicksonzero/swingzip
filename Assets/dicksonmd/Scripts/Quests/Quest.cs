using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestScript", menuName = "Quest System/Quest", order = 51)]
public class Quest : ScriptableObject
{
    [Tooltip("Name of an npc in the format Zone.NpcName")]
    public string questGiverName = "";

    [Tooltip("Placeholder for a condition object")]
    public string condition = ""; // TODO: point to a condition object
    
    
    [HideInInspector]
    public IStep[] script;
}
