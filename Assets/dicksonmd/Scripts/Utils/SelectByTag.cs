using UnityEditor;
using UnityEngine;

public class SelectByTag : MonoBehaviour
{

    private static string SelectedTag = "Through";

    [MenuItem("Helpers/Select By Tag")]
    public static void SelectObjectsWithTag()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(SelectedTag);
        Selection.objects = objects;
    }
}