using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
	[Tooltip("The amount of torque, multiplied by the current inertia of the starfish, to be used for rolling.")]
	public float rollTorquePerKgMeter = 10f;
	public float armPushDistance = 1f;
	public Rigidbody2D body;
	public float maxAngular = 200f;
	public float rollForcePerKilogram = 2f;
	/// <summary> Indexed references to each of this Player's Tentacles, or to null if an index currently has no tentacle. </summary>
	[SerializeField]
	[HideInInspector]
	private Tentacle[] myTentacles;
	private Tentacle[] MyTentacles
	{
		get
		{
			if(myTentacles.Length == 0)
				myTentacles = GetComponentsInChildren<Tentacle>();
			return myTentacles;
		}
	}
	public IEnumerable<Tentacle> tentacleEnumerable => MyTentacles;
	[SerializeField]
	[Tooltip("The maximum distance a tentacle can extend for grappling. If it is lower than the distance for pushing, both will be affected.")]
	private float maxTentacleExtentionDistance = 5f;

	public UnityEvent onTentacleViolentDetach;
	public UnityEvent onEat;

	private void Start()
	{
		// 2022-09-17-10:38: This way of initializing myTentacles must be changed if a player starts with <5 tentacles, possibly by creating "tentacle base" objects.
		myTentacles = GetComponentsInChildren<Tentacle>();

		foreach (Tentacle tentacle in MyTentacles)
			tentacle.SetMaxExtentionLength(maxTentacleExtentionDistance);

		//GameObject.FindWithTag("CameraTargetGroup").GetComponent<CameraGroupController>().SetUpTentacles(gameObject);

		FindObjectOfType<SimpleCameraFollower>().AddTarget(body.transform);
	}

	public void PushWithArm(int armIndex)
	{
		GetTentacle(armIndex)?.ExtendPush(armPushDistance);
	}

	public void RetractArm(int armIndex)
	{
		GetTentacle(armIndex)?.StopExtending();
	}

	public bool ExtendArm(int armIndex, Vector2 targetPosition)
	{
		Tentacle tentacle = GetTentacle(armIndex);
		if(tentacle == null)
			return false;
		else
			return tentacle.ExtendGrapple(targetPosition);
	}

	public void StopGrappleExtending(int armIndex)
	{
		Tentacle tentacle = GetTentacle(armIndex);
		if (tentacle != null)
		{
			if(tentacle.State == Tentacle.TentacleState.GRAPPLED)
				tentacle.StopGrapple();
			else if(tentacle.State == Tentacle.TentacleState.EXTENDED_GRAPPLE)
				tentacle.StopExtending();
		}
	}

	/// <summary>Call this function from FixedUpdate() or similar physics events.</summary>
	/// <param name="torqueDirection"> From -1..1, where -1 and 1 apply maximum roll torque in opposite directions. </param>
	public void ApplyRollTorque(float torqueDirection)
	{

		body?.AddTorque(torqueDirection * rollTorquePerKgMeter * (body.inertia + MyTentacles.Sum(t => t==null ?0f :t.GetInertiaAroundPlayer())), ForceMode2D.Force);
		body?.AddForce(new Vector2(-torqueDirection * rollForcePerKilogram * (body.mass + MyTentacles.Sum(t => t == null ? 0f : t.GetMass())), 0), ForceMode2D.Force);
	}

	public Tentacle GetTentacle(int index) => MyTentacles[index];
	public int MaxTentacleCount => MyTentacles.Length;
}
