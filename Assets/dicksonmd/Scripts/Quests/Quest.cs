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
    public Line[] script = new[] {
        new Line() { label = "Step1", step = null, nextLine = "" }
    };

    [System.Serializable]
    public struct Line
    {
        public string label;
        public IStep step;
        
        [Tooltip("Name of next line. Keep empty to default to next immediate line")]
        public string nextLine;
    }
}
