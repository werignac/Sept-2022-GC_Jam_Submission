using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary> This script runs earlier in the Script Execution Order (see Project Settings). </summary>
[RequireComponent(typeof(Player))]
public abstract class PlayerCommander : MonoBehaviour
{
	[SerializeField]
	[HideInInspector]
	private Player myPlayer;
	/// <summary> Maps a mouse button index to the index of the arm the button is currently controlling, or to -1 if the button isn't controlling an arm. </summary>
	private int[] mouseButtonToControlledArmIndex = { -1, -1 };
	private float torqueDirection = 0;

	private void Start()
	{
		myPlayer = GetComponent<Player>();
	}

	private void Update()
	{
		// Update the arm controlled by each mouse button.
		for(int mouseButton=0; mouseButton<mouseButtonToControlledArmIndex.Length; mouseButton++)
		{
			int controlledArmIndex = mouseButtonToControlledArmIndex[mouseButton];
			if (controlledArmIndex == -1) // If the button isn't controlling an arm...
			{
				// Can extend and start controlling an arm:
				if(Input.GetMouseButtonDown(mouseButton))
				{
					Vector3 worldMousePosition = WorldMousePosition();
					int newControlledArmIndex = FindBestArmToTarget(worldMousePosition);
					myPlayer.ExtendArm(newControlledArmIndex, worldMousePosition);
					mouseButtonToControlledArmIndex[mouseButton] = newControlledArmIndex;
				}
			} else // If the button IS controlling an arm...
			{
				// Can [retract or detach] and stop controlling the current arm:
				if( ! Input.GetMouseButton(mouseButton))
				{
					myPlayer.StopGrappleExtending(controlledArmIndex);
					mouseButtonToControlledArmIndex[mouseButton] = -1;
				}
			}
		}

		// Try to punch with any arms that should punch.
		for(int armIndex=0; armIndex<myPlayer.MaxTentacleCount; armIndex++)
		{
			if(ShouldPushWithArm(armIndex))
				myPlayer.PushWithArm(armIndex);
			else if(myPlayer.GetTentacle(armIndex).State == Tentacle.TentacleState.EXTENDED_PUSH)
				myPlayer.RetractArm(armIndex);
		}

		// Determine the torque direction.
		torqueDirection = GetTorqueDirection();
	}

	private void FixedUpdate()
	{
		// Apply any torque.
		myPlayer.ApplyRollTorque(torqueDirection);
	}

	/// <returns> The index of the arm best suited to shooting toward the target world-space position. Returns 0 if the player has no tentacles.</returns>
	public int FindBestArmToTarget(Vector3 targetPosition)
	{
		// This function operates in the Player's local space.
		Vector3 targetDirection = myPlayer.body.transform.worldToLocalMatrix.MultiplyPoint(targetPosition).normalized;
		int bestArmIndex = 0;
		float bestArmScore = float.NegativeInfinity;
		for(int armIndex=0; armIndex<myPlayer.MaxTentacleCount; armIndex++)
		{
			Tentacle tentacle = myPlayer.GetTentacle(armIndex);
			if(tentacle!=null && (tentacle.State==Tentacle.TentacleState.IDLE || tentacle.State==Tentacle.TentacleState.EXTENDED_PUSH))
			{
				float newScore = Vector3.Dot(tentacle.GetExtentionDirection(), targetDirection);
				if(newScore > bestArmScore)
				{
					bestArmScore = newScore;
					bestArmIndex = armIndex;
				}
			}
		}
		return bestArmIndex;
	}

	public Vector3 WorldMousePosition()
	{
		Vector3 result = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		result.z = 0;
		return result;
	}

	public abstract bool ShouldPushWithArm(int armIndex);
	public abstract float GetTorqueDirection();
}
