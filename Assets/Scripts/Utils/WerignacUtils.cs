using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class WerignacUtils
{
	public static void BroadcastToAll(string function, object param = null, SendMessageOptions options = SendMessageOptions.RequireReceiver)
	{
		foreach (GameObject go in SceneManager.GetActiveScene().GetRootGameObjects())
		{
			if (param != null)
				go.BroadcastMessage(function, param, options);
			else
				go.BroadcastMessage(function, options);
		}
	}

	public static void BroadcastToAll(string function, SendMessageOptions options)
	{
		BroadcastToAll(function, null, options);
	}

	public static void ResetLevel()
	{
		BroadcastToAll("OnReset", SendMessageOptions.DontRequireReceiver);
		LevelsOrder l = Resources.Load<LevelsOrder>("LevelOrder");
		l.LoadLevel(l.CurrentLevelIndex);
	}
}
