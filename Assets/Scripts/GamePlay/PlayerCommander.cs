using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
				// Can detach and stop controlling the current arm:
				if( ! Input.GetMouseButton(mouseButton))
				{
					myPlayer.DetachArm(controlledArmIndex);
					mouseButtonToControlledArmIndex[mouseButton] = -1;
				}
			}
		}

		// Try to punch with any arms that should punch.
		for(int armIndex=0; armIndex<myPlayer.MaxTentacleCount; armIndex++)
		{
			if(ShouldPushWithArm(armIndex))
				myPlayer.PushWithArm(armIndex);
			else
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

	/// <returns> The index of the arm best suited to shooting toward the target world-space position. </returns>
	public int FindBestArmToTarget(Vector3 targetPosition)
	{
		return 0; // TODO
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
