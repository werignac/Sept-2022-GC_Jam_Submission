using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "LevelProgress", menuName = "LevelProgress", order = 0)]
public class LevelProgress : ScriptableObject
{
    [SerializeField]
    private int levelsCompleted;

    public int LevelsCompleted
    {
        get { return (levelsCompleted); }
    }

    public void CompletedLevel(int num)
    {
        if(num >= levelsCompleted)
        {
            levelsCompleted = num + 1;
        }
    }
}
