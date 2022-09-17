using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Level Sequence", menuName = "LevelSequence", order = 1)]
public class LevelSequence : ScriptableObject
{
	[Tooltip("An array of levels, specified by their file path relative to the Assets folder.\nExample: Scenes/Example.unity")]
	public string[] levelPaths;

	public void LoadNextLevel()
	{
		int levelIndex = System.Array.IndexOf(levelPaths, SceneManager.GetActiveScene().path);
		if(levelIndex != -1)
		{
			if(levelIndex < levelPaths.Length-1)
				SceneManager.LoadScene(levelPaths[levelIndex + 1]);
			else
			{
				// TODO: That's all the levels. Take me to a victory screen!
			}
		}
	}

	#if UNITY_EDITOR
	private void OnValidate()
	{
		foreach(string levelPath in levelPaths)
		{
			bool hasScene = false;
			for(int i=0; i<SceneManager.sceneCountInBuildSettings; i++)
			{
				if(SceneManager.GetSceneByBuildIndex(i).path == levelPath)
				{
					hasScene = true;
					break;
				}
			}
			if(! hasScene)
				Debug.LogError("Scene: "+levelPath+" was not found in the Build Settings.");
		}
	}
	#endif
}
