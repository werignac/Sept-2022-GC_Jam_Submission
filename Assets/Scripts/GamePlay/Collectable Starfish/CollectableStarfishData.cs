using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

#if UNITY_EDITOR
public class CollectableStarfishData : ScriptableObject
{
	public const string settingsPath = "Assets/WorldData/Collectable_Starfish_Settings.asset";
	
	public enum StarfishColor { BLUE = 0, ORANGE = 1, RED = 2, GREEN = 3, RANDOM = 4, GOAL = 5 }

	[Serializable]
	public struct StarfishEntry
	{
		public StarfishColor color;
		public RuntimeAnimatorController animator;
	}

	[SerializeField]
	private StarfishEntry[] starfish;

	internal static CollectableStarfishData GetOrCreateSettings()
	{
		var settings = AssetDatabase.LoadAssetAtPath<CollectableStarfishData>(settingsPath);
		if (settings == null)
		{
			settings = ScriptableObject.CreateInstance<CollectableStarfishData>();
			settings.starfish = new StarfishEntry[0];
			AssetDatabase.CreateAsset(settings, settingsPath);
			AssetDatabase.SaveAssets();
		}
		return settings;
	}

	public static RuntimeAnimatorController GetAnimatorForColor(StarfishColor color)
	{
		if (color == StarfishColor.RANDOM)
			color = (StarfishColor) UnityEngine.Random.Range(0, (int) StarfishColor.RANDOM);

		CollectableStarfishData data = GetOrCreateSettings();

		foreach (StarfishEntry entry in data.starfish)
		{
			if (entry.color == color)
				return entry.animator;
		}

		return null;
	}

}
#endif
