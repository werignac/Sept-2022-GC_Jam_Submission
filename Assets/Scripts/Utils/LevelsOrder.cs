using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LevelsOrder : ScriptableObject
{
    [SerializeField]
    private GameObject[] levels;

    internal static LevelsOrder GetLevelsOrder()
    {
        string path = "Assets/LevelOrder.asset";
        LevelsOrder levelOrder = AssetDatabase.LoadAssetAtPath<LevelsOrder>(path);
        if(levelOrder == null)
        {
            levelOrder = ScriptableObject.CreateInstance<LevelsOrder>();
            levelOrder.levels = new GameObject[0];
            AssetDatabase.CreateAsset(levelOrder, path);
            AssetDatabase.SaveAssets();
        }
        return (levelOrder);
    }

    internal static SerializedObject GetSerializedLevelsOrder()
    {
        return (new SerializedObject(GetLevelsOrder()));
    }

}

/*
// Register a SettingsProvider using IMGUI for the drawing framework:
static class MyCustomSettingsIMGUIRegister
{
    [SettingsProvider]
    public static SettingsProvider CreateMyCustomSettingsProvider()
    {
        // First parameter is the path in the Settings window.
        // Second parameter is the scope of this setting: it only appears in the Project Settings window.
        var provider = new SettingsProvider("Project/GameLevels", SettingsScope.Project)
        {
            // By default the last token of the path is used as display name if no label is provided.
            label = "Game Levels",
            // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
            guiHandler = (searchContext) =>
            {
                var levelOrder = LevelsOrder.GetSerializedLevelsOrder();
                EditorGUILayout.PropertyField(levelOrder.FindProperty("levels"));
                levelOrder.ApplyModifiedProperties();
            },
        };

        return provider;
    }
}
*/