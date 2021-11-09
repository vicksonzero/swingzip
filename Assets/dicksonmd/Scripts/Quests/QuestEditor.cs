#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Quest))]
[CanEditMultipleObjects]
public class QuestDataEditor : Editor
{
    private ReorderableList scriptList;

    private SerializedProperty scriptProp;

    private struct AbilityCreationParams
    {
        public string Path;
    }

    public void OnEnable()
    {
        scriptProp = serializedObject.FindProperty("script");

        scriptList = new ReorderableList(
                serializedObject,
                scriptProp,
                draggable: true,
                displayHeader: true,
                displayAddButton: true,
                displayRemoveButton: true);

        scriptList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Script lines");
        };

        scriptList.onRemoveCallback = (ReorderableList l) =>
        {
            var element = l.serializedProperty.GetArrayElementAtIndex(l.index);
            var obj = element.objectReferenceValue;

            AssetDatabase.RemoveObjectFromAsset(obj);

            DestroyImmediate(obj, true);
            l.serializedProperty.DeleteArrayElementAtIndex(l.index);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ReorderableList.defaultBehaviours.DoRemoveButton(l);
        };

        scriptList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = scriptProp.GetArrayElementAtIndex(index);

            rect.y += 2;
            rect.width -= 10;
            rect.height = EditorGUIUtility.singleLineHeight;

            if (element.objectReferenceValue == null)
            {
                return;
            }

            // Convert this element's data to a SerializedObject so we can iterate
            // through each SerializedProperty and render a PropertyField.
            SerializedObject nestedObject = new SerializedObject(element.objectReferenceValue);



            string label = element.objectReferenceValue.name;
            label = nestedObject.FindProperty("label")?.stringValue + " (" + label + ")";
            EditorGUI.LabelField(rect, label, EditorStyles.boldLabel);

            // Loop over all properties and render them
            SerializedProperty prop = nestedObject.GetIterator();
            float y = rect.y;
            rect.y += EditorGUIUtility.singleLineHeight;
            while (prop.NextVisible(true))
            {
                if (prop.name == "m_Script")
                {
                    continue;
                }
                // Debug.Log("drawElementCallback prop.name: "+prop.name);

                rect.height = EditorGUI.GetPropertyHeight(prop);

                var rect2 = rect;

                if (prop.isArray && prop.type != "string")
                {
                    rect2.x += 20;
                    rect2.width -= 20;
                }
                EditorGUI.PropertyField(rect2, prop, true);
                rect.y += EditorGUI.GetPropertyHeight(prop);
            }

            nestedObject.ApplyModifiedProperties();

            // Mark edits for saving
            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        };

        scriptList.elementHeightCallback = (int index) =>
        {
            float baseProp = EditorGUI.GetPropertyHeight(
                scriptList.serializedProperty.GetArrayElementAtIndex(index), true);

            float additionalProps = 0;
            SerializedProperty element = scriptProp.GetArrayElementAtIndex(index);
            if (element.objectReferenceValue != null)
            {
                SerializedObject ability = new SerializedObject(element.objectReferenceValue);
                SerializedProperty prop = ability.GetIterator();
                while (prop.NextVisible(true))
                {
                    // XXX: This logic stays in sync with loop in drawElementCallback.
                    if (prop.name == "m_Script")
                    {
                        continue;
                    }
                    additionalProps += EditorGUI.GetPropertyHeight(prop);
                }
            }

            float spacingBetweenElements = EditorGUIUtility.singleLineHeight / 2;

            return baseProp + spacingBetweenElements + additionalProps;
        };

        scriptList.onAddDropdownCallback = (Rect buttonRect, ReorderableList l) =>
        {
            var menu = new GenericMenu();
            var guids = AssetDatabase.FindAssets("", new[] { "Assets/dicksonmd/Scripts/Quests/Steps" });
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var type = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
                if (type is null)
                {
                    continue;
                }
                if (type.name == "CardAbility")
                {
                    continue;
                }

                menu.AddItem(
                    new GUIContent(Path.GetFileNameWithoutExtension(path)),
                    false,
                    addClickHandler,
                    new AbilityCreationParams() { Path = path });
                Debug.Log("a " + type);
            }
            menu.ShowAsContext();
        };
    }

    private void addClickHandler(object dataObj)
    {
        // Make room in list
        var data = (AbilityCreationParams)dataObj;
        var index = scriptList.serializedProperty.arraySize;
        scriptList.serializedProperty.arraySize++;
        scriptList.index = index;
        var element = scriptList.serializedProperty.GetArrayElementAtIndex(index);

        // Create the new Ability
        var type = AssetDatabase.LoadAssetAtPath(data.Path, typeof(UnityEngine.Object));
        var newAbility = ScriptableObject.CreateInstance(type.name);
        newAbility.name = type.name;

        // Add it to CardData
        var cardData = (Quest)target;
        AssetDatabase.AddObjectToAsset(newAbility, cardData);
        AssetDatabase.SaveAssets();
        element.objectReferenceValue = newAbility;
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        scriptList.DoLayoutList();

        if (GUILayout.Button("Delete All Abilities"))
        {
            var path = AssetDatabase.GetAssetPath(target);
            Object[] assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] is IStep)
                {
                    Object.DestroyImmediate(assets[i], true);
                }
            }
            AssetDatabase.SaveAssets();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif