using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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
	private KeyCode restartButton = KeyCode.R;
	public bool Reloaded { get; private set; }
	/// <summary>
	/// How much better the score of a new grappling arm must be over
	/// the old grappling arm to switch.
	/// </summary>
	private const float armTransferThreshold = 0.5f;

	private void Start()
	{
		myPlayer = GetComponent<Player>();
		Reloaded = false;
	}

	private int DecideBestTentacle(Tuple<float, int>[] sortedTentacles, int forMouseButton)
	{
		// If the old index is close enough, use it.
		int oldIndex = mouseButtonToControlledArmIndex[forMouseButton];
		bool relevantPredecessor = (oldIndex >= 0) ? myPlayer.GetTentacle(oldIndex).CanExtendGrapple() : false;

		float scoreToBeat = float.NegativeInfinity;
		if (relevantPredecessor)
		{
			for (int i = 0; i < sortedTentacles.Length; i++)
				if (sortedTentacles[i].Item2 == oldIndex)
					scoreToBeat = sortedTentacles[i].Item1;
		}

		int newControlledArmIndex = (relevantPredecessor) ? oldIndex : -1;
		for (int i = 0; i < sortedTentacles.Length; i++)
		{
			Tuple<float, int> entry = sortedTentacles[i];
			int tentacleIndex = entry.Item2;
			Tentacle tentacle = myPlayer.GetTentacle(tentacleIndex);

			int indexInMouseMapping = Array.IndexOf(mouseButtonToControlledArmIndex, tentacleIndex);

			bool canExtend = tentacle.CanExtendGrapple();
			bool isNotBeingUsedByOtherButton = (indexInMouseMapping == -1 || indexInMouseMapping == forMouseButton);
			bool beatsScore = entry.Item1 - scoreToBeat > armTransferThreshold;

			if (canExtend && isNotBeingUsedByOtherButton && beatsScore)
			{
				newControlledArmIndex = tentacleIndex;
				break;
			}
		}

		return newControlledArmIndex;
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
					Tuple<float,int>[] bestArms = FindBestArmsToTarget(worldMousePosition);
					int newControlledArmIndex = DecideBestTentacle(bestArms, mouseButton);
					if (newControlledArmIndex != -1)
					{
						myPlayer.ExtendArm(newControlledArmIndex, worldMousePosition);
						mouseButtonToControlledArmIndex[mouseButton] = newControlledArmIndex;
					}
				}
			} else // If the button IS controlling an arm...
			{
				// Update the position of the arm if the mouse is still being held
				if (Input.GetMouseButton(mouseButton))
				{
					int oldControlledArmIndex = mouseButtonToControlledArmIndex[mouseButton];
					Tentacle oldTentacle = myPlayer.GetTentacle(oldControlledArmIndex);
					// If the arm for this mouse button isn't grappled onto anything...
					// (If the arm is grappled onto something, we shouldn't look for a new arm
					// regardless of score, otherwise we would immediately detach our grappled
					// arm)
					if (! (oldTentacle.State == Tentacle.TentacleState.GRAPPLED || 
						oldTentacle.State == Tentacle.TentacleState.GRAPPLED_AND_PUSH || 
						oldTentacle.State == Tentacle.TentacleState.DETACHED))
					{
						//Check to see if there's a better tentacle to grab with
						Vector3 worldMousePosition = WorldMousePosition();
						Tuple<float, int>[] bestArms = FindBestArmsToTarget(worldMousePosition);
						int newControlledArmIndex = DecideBestTentacle(bestArms, mouseButton);
						if (newControlledArmIndex != oldControlledArmIndex)
							myPlayer.StopGrappleExtending(oldControlledArmIndex);
						if (newControlledArmIndex != -1)
						{
							myPlayer.ExtendArm(newControlledArmIndex, worldMousePosition);
							mouseButtonToControlledArmIndex[mouseButton] = newControlledArmIndex;
						}
					}
				}
				// Can [retract or detach] and stop controlling the current arm:
				else
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
			else if(myPlayer.GetTentacle(armIndex).State == Tentacle.TentacleState.EXTENDED_PUSH || myPlayer.GetTentacle(armIndex).State == Tentacle.TentacleState.GRAPPLED_AND_PUSH)
				myPlayer.RetractArm(armIndex);
		}

		// Determine the torque direction.
		torqueDirection = GetTorqueDirection();

		if(Input.GetKeyUp(restartButton) && ! Reloaded)
        {
			WerignacUtils.ResetLevel();
			Reloaded = true;
		}
	}

	private void OnLevelWon()
	{
		Reloaded = true;
	}

	private void OnReset()
	{
		Reloaded = true;
	}

	private void FixedUpdate()
	{
		// Apply any torque.
		myPlayer.ApplyRollTorque(torqueDirection);
	}

	/// <summary>
	/// Orders the arm indices by those that are the best suited
	/// to reach for a point in world space.
	/// 
	/// Does not check arm state.
	/// </summary>
	/// <param name="targetPosition">The point to reach.</param>
	/// <returns>An ordered list of the indicies with the first being the best arm to reach the point.</returns>
	public Tuple<float,int>[] FindBestArmsToTarget(Vector3 targetPosition)
	{
		// Keys: Score, Values: ArmIndex (Sorts by score)
		List<Tuple<float, int>> scoredBestArms = new List<Tuple<float, int>>();
		Vector3 targetDirection = myPlayer.body.transform.worldToLocalMatrix.MultiplyPoint(targetPosition).normalized;

		for (int armIndex = 0; armIndex < myPlayer.MaxTentacleCount; armIndex++)
		{
			Tentacle tentacle = myPlayer.GetTentacle(armIndex);
			if (tentacle != null)
			{
				float newScore = Vector3.Dot(tentacle.GetExtentionDirection(), targetDirection);
				Tuple<float, int> entry = new Tuple<float, int>(newScore, armIndex);
				// I am too lazy to do a binary search.
				int i = 0;
				while (i < scoredBestArms.Count && scoredBestArms[i].Item1 > entry.Item1)
					i++;

				scoredBestArms.Insert(i, entry);
			}
		}

		return scoredBestArms.ToArray();
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
