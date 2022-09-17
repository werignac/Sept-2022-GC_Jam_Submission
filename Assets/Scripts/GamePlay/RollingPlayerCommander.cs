using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RollingPlayerCommander : PlayerCommander
{
	public KeyCode jumpKey = KeyCode.Space;
	public KeyCode[] leftKeys = { KeyCode.A, KeyCode.LeftArrow };
	public KeyCode[] rightKeys = { KeyCode.D, KeyCode.RightArrow };

	public override bool ShouldPushWithArm(int armIndex)
	{
		return Input.GetKey(jumpKey);
	}

	public override float GetTorqueDirection()
	{
		return
			(leftKeys.Any(kc => Input.GetKey(kc)) ?1f :0f)
			+ (rightKeys.Any(kc => Input.GetKey(kc)) ?-1f : 0f);
	}
}
