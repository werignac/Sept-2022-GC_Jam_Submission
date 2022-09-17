using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalArea : MonoBehaviour
{
	public LevelsOrder levelsOrder;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.GetComponentInParent<Player>() != null)
		{
			// Win the level.
			// Move to the next level.
			levelsOrder.LoadNextLevel();
		}
	}
}
