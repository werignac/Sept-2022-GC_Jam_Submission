using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public const int armCount = 5;

	public void PunchWithArm(int armIndex)
	{
		Debug.Log("Punch "+armIndex);
		// TODO
	}

	public void ExtendArm(int armIndex, Vector3 targetPosition)
	{
		Debug.Log("Extend " + armIndex + ", " + targetPosition);
		// TODO
	}

	public void DetachArm(int armIndex)
	{
		Debug.Log("Detach " + armIndex);
		// TODO
	}

	/// <summary></summary>
	/// <param name="torqueDirection"> From -1..1, where -1 and 1 apply maximum roll torque in opposite directions. </param>
	public void ApplyRollTorque(float torqueDirection)
	{
		if(torqueDirection != 0)
			Debug.Log("Roll " + torqueDirection);
		// TODO
	}
}
