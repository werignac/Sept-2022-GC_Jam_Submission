using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelsOrder : ScriptableObject
{
	public string mainGameScenePath;
	[SerializeField]
	private GameObject[] levels;
	[SerializeField]
	[HideInInspector]
	private GameObject currentLevel = null;
	[SerializeField]
	[HideInInspector]
	private GameObject levelToInstantiate = null;
	[SerializeField]
	[HideInInspector]
	private int currentLevelIndex;

	public int NumLevels
	{
		get { return (levels.Length); }
	}

	public void LoadLevel(int levelIndex)
	{
		GameObject newLevel = GetLevel(levelIndex);
		if(newLevel != null)
		{
			if(currentLevel != null)
				Destroy(currentLevel);
			currentLevel = null;
			currentLevelIndex = levelIndex;

			if(SceneManager.GetActiveScene().path != mainGameScenePath)
			{
				levelToInstantiate = newLevel;
				SceneManager.sceneLoaded += InstantiateLevelCallback;
				SceneManager.LoadScene(mainGameScenePath);
			} else
				InstantiateLevel(newLevel);
		}
	}
	public void LoadNextLevel()
	{
	  if (currentLevel == null)
			LoadLevel(0);
	  else
			LoadLevel(currentLevelIndex + 1);
	}

	/// <summary> An event handler to instantiate the next level when the main game scene finished loading. </summary>
	private void InstantiateLevelCallback(Scene scene, LoadSceneMode sceneLoadMode)
	{
		SceneManager.sceneLoaded -= InstantiateLevelCallback;
		InstantiateLevel(levelToInstantiate);
	}
	/// <summary> Instantiates the given level prefab and sets the currentLevel to point to it. </summary>
	private void InstantiateLevel(GameObject level)
	{
		currentLevel = Instantiate(level);
	}

	private GameObject GetLevel(int index)
	{
		if(index < 0 || index >= levels.Length)
		{
			return (null);
		}
		return (levels[index]);
	}


  #if UNITY_EDITOR
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
  #endif

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