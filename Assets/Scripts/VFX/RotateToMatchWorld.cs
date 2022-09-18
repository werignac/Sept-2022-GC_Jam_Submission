using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMatchWorld : MonoBehaviour
{
	public float lerpSpeed = 100f;

	private void LateUpdate()
	{
		transform.localRotation = Quaternion.Euler(0, 0, Mathf.LerpAngle(transform.localRotation.eulerAngles.z, -transform.parent.rotation.eulerAngles.z, Time.deltaTime*lerpSpeed));
	}
}
