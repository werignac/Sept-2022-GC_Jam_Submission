using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalArea : MonoBehaviour
{
	public LevelsOrder levelsOrder;
	public LevelProgress levelProgress;
	public ParticleSystem goalParticles;

	private bool levelWon = false;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if( !levelWon && other.GetComponentInParent<Player>() != null)
		{
			// Win the level.
			goalParticles.Play();
			levelWon = true;
			levelProgress.CompletedLevel(levelsOrder.CurrentLevelIndex);
			// Move to the next level.
			Invoke("NextLevel", 2f);
		}
	}

	private void NextLevel()
	{
		levelsOrder.LoadNextLevel();
	}
}
