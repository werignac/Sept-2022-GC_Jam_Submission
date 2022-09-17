using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{
	[Tooltip("The amount of torque, multiplied by the current mass of the starfish, to be used for rolling.")]
	public float rollTorquePerKilogram = 1f;
	public float armPushDistance = 1f;
	public Rigidbody2D body;
	/// <summary> Indexed references to each of this Player's Tentacles, or to null if an index currently has no tentacle. </summary>
	[SerializeField]
	[HideInInspector]
	private Tentacle[] myTentacles;

	private void Start()
	{
		// 2022-09-17-10:38: This way of initializing myTentacles must be changed if a player starts with <5 tentacles, possibly by creating "tentacle base" objects.
		myTentacles = GetComponentsInChildren<Tentacle>();
	}

	public void PunchWithArm(int armIndex)
	{
		GetTentacle(armIndex)?.ExtendPush(armPushDistance);
	}

	public void ExtendArm(int armIndex, Vector2 targetPosition)
	{
		GetTentacle(armIndex)?.ExtendGrapple(targetPosition);
	}

	public void DetachArm(int armIndex)
	{
		GetTentacle(armIndex)?.StopGrapple();
	}

	/// <summary>Call this function from FixedUpdate() or similar physics events.</summary>
	/// <param name="torqueDirection"> From -1..1, where -1 and 1 apply maximum roll torque in opposite directions. </param>
	public void ApplyRollTorque(float torqueDirection)
	{
		body?.AddTorque(torqueDirection * rollTorquePerKilogram * (body.mass + myTentacles.Sum(t => t==null ?0f :t.GetMass())), ForceMode2D.Force);
	}

	public Tentacle GetTentacle(int index) => myTentacles[index];
	public int MaxTentacleCount => myTentacles.Length;
}
