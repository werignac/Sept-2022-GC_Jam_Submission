using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelBuilder))]
public class LevelBuilderInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if(GUILayout.Button("Save Level"))
        {
            ((LevelBuilder)target).SaveLevel();
        }
    }
}
