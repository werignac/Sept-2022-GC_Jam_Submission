using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalArea : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.GetComponentInParent<Player>() != null)
		{
			// Win the level.
			// Move to the next level.
			int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
			if(currentSceneIndex < SceneManager.sceneCountInBuildSettings-1)
				SceneManager.LoadScene(currentSceneIndex + 1);
		}
	}
}
