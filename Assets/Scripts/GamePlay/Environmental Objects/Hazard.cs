using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
		Cuttable cut = other.gameObject.GetComponentInParent<Cuttable>();
		if (cut != null)
		{
			cut.Cut();
		}
    }

	private void OnCollisionStay2D(Collision2D other)
	{
		OnCollisionEnter2D(other);
	}
}
