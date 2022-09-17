using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class Tentacle : MonoBehaviour
{
	/// <summary>
	/// Todo: Remove
	/// </summary>
	[SerializeField]
	private bool canGrapple = false;

	public enum TentacleState
	{
		IDLE, EXTENDED_PUSH, EXTENDED_GRAPPLE, GRAPPLED, DETACHED
	}

	/// <summary>
	/// Todo: Remove
	/// </summary>
	[SerializeField]
	private KeyCode extentionKey = KeyCode.Q;
	/// <summary>
	/// The physics spring used to control the
	/// arm.
	/// </summary>
	private RelativeJoint2D joint;
	/// <summary>
	/// The local distance (-y) the arm is from the center of the
	/// starfish. Gotten from the joint at start.
	/// </summary>
	private Vector2 baseExtention;
	/// <summary>
	/// The local rotation in degrees the arm is from the
	/// center of the starfish. Gotten from the joint at start.
	/// </summary>
	private float baseAngularOffset;
	/// <summary>
	/// The current tentacle state (e.g. what the tentacle is
	/// currently doing).
	/// </summary>
	public TentacleState State { get; private set; }

	/// <summary>
	/// The maximum distance the starfish can extend to
	/// grapple.
	/// </summary>
	private float maxExtendDistance = 5f;

    // Start is called before the first frame update
    void Start()
    {
		joint = GetComponent<RelativeJoint2D>();
		if (joint)
		{
			baseExtention = joint.linearOffset;
			baseAngularOffset = joint.angularOffset;
			Debug.LogFormat("Base Angular Offset for {0}: {1}", name, baseAngularOffset);
		}
		State = TentacleState.IDLE;
    }

	/// <summary>
	/// Returns a uniform vector in the direction that
	/// the arm is pointing in (most of the time).
	/// </summary>
	public Vector2 GetExtentionDirection()
	{
		return Quaternion.AngleAxis(baseAngularOffset, Vector3.forward) * -baseExtention.normalized;
	}

	/// <summary>
	/// Determines whether a tentacle can extend straight (QWOP)
	/// given its current state.
	/// </summary>
	public bool CanExtendPush()
	{
		switch(State)
		{
			case TentacleState.IDLE:
			case TentacleState.EXTENDED_PUSH:
			case TentacleState.GRAPPLED:
				return true;
			default:
				return false;
		}
	}

	/// <summary>
	/// Determines whether a tentacle can extend towards a point
	/// for grappling.
	/// </summary>
	public bool CanExtendGrapple()
	{
		switch(State)
		{
			case TentacleState.IDLE:
			case TentacleState.EXTENDED_PUSH:
			case TentacleState.EXTENDED_GRAPPLE:
				return true;
			default:
				return false;
		}
	}

	/// <summary>
	/// Determines whether a tentacle an stop extending.
	/// </summary>
	public bool CanStopExtention()
	{
		switch(State)
		{
			case TentacleState.IDLE:
			case TentacleState.EXTENDED_PUSH:
			case TentacleState.EXTENDED_GRAPPLE:
				return true;
			default:
				return false;
		}
	}

	/// <summary>
	/// Extends this arm straight by the given amount.
	/// Does not need to be continuously called to preserve
	/// extention.
	/// </summary>
	/// <param name="extentionAmount">How much farther the arm should stretch in world units.</param>
	/// <returns>Whether the arm could extend (e.g. not detached or grappling).</returns>
	public bool ExtendPush(float extentionAmount)
	{
		if (CanExtendPush())
		{
			joint.linearOffset = baseExtention + baseExtention.normalized * extentionAmount;
			State = TentacleState.EXTENDED_PUSH;
			return true;
		}
		else
			return false;
	}

	/// <summary>
	/// Extends the arm towards a target.
	/// </summary>
	/// <param name="targetPoint">Where in world space the arm should strech to.</param>
	/// <returns>Whether the arm could extend (e.g. not detached or grappling).</returns>
	public bool ExtendGrapple(Vector2 targetPoint)
	{
		if (CanExtendGrapple() && canGrapple)
		{
			WorldToJointOffets(targetPoint);
			State = TentacleState.EXTENDED_GRAPPLE;

			return true;
		}
		else
			return false;
	}

	/// <summary>
	/// Turns a target world point into tentacle spring settings
	/// such that the tentacle reaches the target.
	/// </summary>
	/// <param name="worldPoint">The world point to reach.</param>
	private void WorldToJointOffets(Vector3 worldPoint)
	{
		Vector2 relativePos = joint.connectedBody.transform.InverseTransformPoint(worldPoint);
		relativePos = -relativePos;
		relativePos = Quaternion.AngleAxis(baseAngularOffset, Vector3.forward) * relativePos;

		Debug.DrawLine(Vector3.zero, relativePos.normalized, Color.red);
		Debug.DrawLine(Vector3.zero, baseExtention.normalized, Color.blue);

		float angularDifference = Vector2.SignedAngle(relativePos.normalized, baseExtention.normalized);


		joint.angularOffset = baseAngularOffset + angularDifference;
		joint.linearOffset = new Vector2(0, -relativePos.magnitude);//Quaternion.AngleAxis(angle, Vector3.forward) * relativePos;
	}

	/// <summary>
	/// Stops the tentacle's current extention if it can.
	/// </summary>
	/// <returns>Whether the current extention could be stopped.</returns>
	public bool StopExtending()
	{
		if (CanStopExtention())
		{
			joint.linearOffset = baseExtention;
			joint.angularOffset = baseAngularOffset;
			State = TentacleState.IDLE;
			return true;
		}
		else
			return false;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public bool StopGrapple()
	{
		return false;
	}

	private struct TentacleInputs
	{
		public bool extendPush;
		public bool extendGrapple;
		public Vector2 extendPosition;
		public bool anyInput;
		public TentacleInputs(KeyCode grappleKey)
		{
			extendPush = Input.GetKey(grappleKey);
			// TODO: UI overrides click on screen.
			extendGrapple = Input.GetMouseButton(0);
			extendPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			anyInput = extendPush || extendGrapple;
		}
	}

	// Update is called once per frame
	void FixedUpdate()
    {
		TentacleInputs inputs = new TentacleInputs(extentionKey);

		if (inputs.anyInput)
		{
			if (inputs.extendGrapple)
			{
				ExtendGrapple(inputs.extendPosition);
			}
			else if (inputs.extendPush)
			{
				ExtendPush(1);
			}
		}
		else if (State != TentacleState.IDLE)
		{
			StopExtending();
		}
    }

	private void OnDrawGizmos()
	{
		if (joint)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere(joint.target, 0.25f);
		}
	}
}
