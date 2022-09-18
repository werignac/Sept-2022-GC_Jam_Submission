using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
	[SerializeField]
	private float timeBetweenHits = 1f;
	private float nextTimeToHit = 0f;

    private void OnCollisionEnter2D(Collision2D other)
    {
		if (Time.time > nextTimeToHit)
		{
			Debug.LogFormat("cut at {0}", Time.time);
			Cuttable cut = other.gameObject.GetComponentInParent<Cuttable>();
			if (cut != null)
			{
				cut.Cut();
				nextTimeToHit = Time.time + timeBetweenHits;
			}
		}
    }

	private void OnCollisionStay2D(Collision2D collision)
	{
		OnCollisionEnter2D(collision);
	}
}
