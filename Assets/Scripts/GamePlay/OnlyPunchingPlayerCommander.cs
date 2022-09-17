using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlyPunchingPlayerCommander : PlayerCommander
{
	public KeyCode[] keysForEachArm = { KeyCode.LeftShift, KeyCode.A, KeyCode.W, KeyCode.D, KeyCode.Space};

	public override bool ShouldPunchWithArm(int armIndex)
	{
		return Input.GetKeyDown(keysForEachArm[armIndex]);
	}

	public override float GetTorqueDirection()
	{
		return 0; // This commander cannot roll.
	}
}
