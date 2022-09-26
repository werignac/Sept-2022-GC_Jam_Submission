using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBounds : MonoBehaviour
{
	private float startDelay = 1f;
	private bool reloaded = false;

	private void FixedUpdate()
	{
		if (startDelay >= 0)
			startDelay -= Time.fixedDeltaTime;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		PlayerCommander player = collision.gameObject.GetComponentInParent<PlayerCommander>();
		Tentacle tentacle = collision.gameObject.GetComponentInParent<Tentacle>();
		if (tentacle)
			player = null;
			

		if (player && startDelay < 0 && !reloaded)
		{
			WerignacUtils.ResetLevel();
			reloaded = true;
		}
	}

	private void OnLevelWon()
	{
		startDelay = float.PositiveInfinity;
	}

	private void OnReset()
	{
		reloaded = true;
	}
}
