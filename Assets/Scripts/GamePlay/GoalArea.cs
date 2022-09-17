using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalArea : MonoBehaviour
{
	[Tooltip("The level sequence to use when deciding which level to load next.")]
	public LevelSequence levelSequence;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if(other.GetComponentInParent<Player>() != null)
		{
			// Win the level.
			levelSequence.LoadNextLevel();
		}
	}
}
