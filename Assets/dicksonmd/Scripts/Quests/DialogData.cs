using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogData", menuName = "Quest System/Data/Dialog Data", order = 51)]
public class DialogData : ScriptableObject
{
    public Line[] lines = new[] {
        new Line() { line = "Hello" }
    };

    [System.Serializable]
    public struct Line
    {
        public string line;
    }
}
