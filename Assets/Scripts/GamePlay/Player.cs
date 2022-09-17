using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public void PunchWithArm(int armIndex)
	{
		Debug.Log("Punch "+armIndex);
		// TODO
	}

	public void ExtendArm(int armIndex, Vector3 targetPosition)
	{
		Debug.Log("Extend " + armIndex);
		// TODO
	}

	public void DetachArm(int armIndex)
	{
		Debug.Log("Detach " + armIndex);
		// TODO
	}
}
