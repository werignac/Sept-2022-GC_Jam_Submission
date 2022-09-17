using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[RequireComponent(typeof(Rigidbody2D))]
public class Tentacle : MonoBehaviour
{
	public enum TentacleState
	{
		IDLE, EXTENDED_PUSH, EXTENDED_GRAPPLE, GRAPPLED, DETACHED
	}

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
	/// <summary>
	/// The point in world space the arm wants to
	/// extend to whilst in EXTENDED_GRAPPLE mode.
	/// </summary>
	private Vector2 extentionPullWorldPoint;

	#region Events
	[Header("Events")]
	public UnityEvent onPush = new UnityEvent();
	public UnityEvent<Collision2D> onPushContact = new UnityEvent<Collision2D>();
	public UnityEvent onStretch = new UnityEvent();
	public UnityEvent onStretchCancel = new UnityEvent();
	public UnityEvent<Collision2D, ContactPoint2D> onGrapple = new UnityEvent<Collision2D, ContactPoint2D>();
	public UnityEvent onDetach = new UnityEvent();
	#endregion

	#region Initialization

	// Start is called before the first frame update
	void Start()
    {
		joint = GetComponent<RelativeJoint2D>();
		baseExtention = joint.linearOffset;
		baseAngularOffset = joint.angularOffset;
		State = TentacleState.IDLE;
	}

	public float GetMass() => 1f; // TODO

	/// <summary>
	/// Sets the maximum distance any arm can extend.
	/// </summary>
	/// <param name="newMax">The new max distance the arm can extend.</param>
	public void SetMaxExtentionLength(float newMax)
	{
		if (newMax < 0)
			throw new System.ArgumentException(string.Format("Got a nonsensical new max extention length {0}.", newMax));

		maxExtendDistance = newMax;
	}

	#endregion

	#region Utils

	/// <summary>
	/// Returns a uniform vector in the direction that
	/// the arm is pointing in (most of the time) in the body's local space.
	/// </summary>
	public Vector2 GetExtentionDirection()
	{
		return Quaternion.AngleAxis(-baseAngularOffset, Vector3.forward) * -baseExtention.normalized;
	}

	#endregion

	#region Check Possible Actions

	/// <summary>
	/// Determines whether a tentacle can extend straight
	/// given its current state.
	/// </summary>
	public bool CanExtendPush()
	{
		switch(State)
		{
			case TentacleState.IDLE:
			case TentacleState.EXTENDED_PUSH:
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

	#endregion

	#region Tentacle Actions

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
			extentionAmount = Mathf.Clamp(extentionAmount, 0, maxExtendDistance);
			joint.linearOffset = baseExtention + baseExtention.normalized * extentionAmount;
			State = TentacleState.EXTENDED_PUSH;

			onPush.Invoke();

			return true;
		}
		else
			return false;
	}

	/// <summary>
	/// Extends the arm towards a target. After calling this method,
	/// the tentacle will continue stretching to the world point.
	/// </summary>
	/// <param name="targetPoint">Where in world space the arm should strech to.</param>
	/// <returns>Whether the arm could extend (e.g. not detached or grappling).</returns>
	public bool ExtendGrapple(Vector2 targetPoint)
	{
		if (CanExtendGrapple())
		{
			// See FixedUpdate to see how the arm
			// adjusts itself to reach to the targetPoint.
			extentionPullWorldPoint = targetPoint;
			State = TentacleState.EXTENDED_GRAPPLE;

			onStretch.Invoke();
			
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

		float angularDifference = Vector2.SignedAngle(relativePos.normalized, baseExtention.normalized);

		joint.angularOffset = baseAngularOffset + angularDifference;
		joint.linearOffset = new Vector2(0, -Mathf.Clamp(relativePos.magnitude, 0, maxExtendDistance + baseExtention.magnitude));
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
			TentacleState previousState = State;
			State = TentacleState.IDLE;

			if (previousState == TentacleState.EXTENDED_GRAPPLE)
				onStretchCancel.Invoke();

			return true;
		}
		else
			return false;
	}

	/// <summary>
	/// If the arm is grappling, detaches the arm.
	/// </summary>
	/// <returns>Whether the grappling was able to be stopped.</returns>
	public bool StopGrapple()
	{
		if (State == TentacleState.GRAPPLED)
		{
			Detach();
			return true;
		}
		return false;
	}

	/// <summary>
	/// Detaches the arm from the starfish.
	/// </summary>
	/// <returns>Whether the arm was able to be detached.</returns>
	private bool Detach()
	{
		if (State != TentacleState.DETACHED)
		{
			Destroy(joint);
			// Do some visual effects to remove the arm.
			State = TentacleState.DETACHED;

			onDetach.Invoke();
			
			return true;
		}
		else
			return false;
	}

	#region Collision / Grappling

	/// <summary>
	/// Create a joint to attach to non-rigidbody objects
	/// (e.g. just attach to a point in space)
	/// </summary>
	/// <param name="worldPos">The point to attach this arm to.</param>
	private void CreateHingeJointFixedWorld(Vector2 worldPos)
	{
		HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D>();

		Vector3 relativeJointAnchor = transform.InverseTransformPoint(worldPos);
		
		joint.enableCollision = true;
		joint.connectedBody = null;
		joint.autoConfigureConnectedAnchor = true;
		joint.anchor = relativeJointAnchor;
		joint.connectedAnchor = worldPos;
		joint.useMotor = false;
		joint.useLimits = false;
		joint.breakForce = float.PositiveInfinity;
		joint.breakTorque = float.PositiveInfinity;
	}

	/// <summary>
	/// Create a joint to attach to rigidbody objects
	/// (e.g. a box -dynamic- or shark -kinematic-)
	/// </summary>
	/// <param name="worldPos">The position to put the archor of the joint.</param>
	/// <param name="connectedBody">The rigidbody to attach to.</param>
	private void CreateHingeJointRigidBody(Vector2 worldPos, Rigidbody2D connectedBody)
	{
		FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D>();

		Vector3 relativeJointAnchor = transform.InverseTransformPoint(worldPos);
		Vector3 relativeJointConnectedAnchor = connectedBody.transform.InverseTransformPoint(worldPos);

		joint.enableCollision = true;
		joint.connectedBody = connectedBody;
		joint.autoConfigureConnectedAnchor = true;
		joint.anchor = relativeJointAnchor;
		joint.connectedAnchor = relativeJointConnectedAnchor;
		joint.dampingRatio = 0;
		joint.frequency = 0;
		joint.breakForce = float.PositiveInfinity;
		joint.breakTorque = float.PositiveInfinity;
	}

	/// <summary>
	/// Handles the grappling collision logic.
	/// </summary>
	/// <param name="collision">The collision encountered.</param>
	private void GrappleCollisionHandling(Collision2D collision)
	{
		bool lookingForGrapple = State == TentacleState.EXTENDED_GRAPPLE;
		bool grappleable = !collision.gameObject.CompareTag("Non-Grappleable");

		if (lookingForGrapple && grappleable)
		{
			Vector3 forward = transform.up;

			foreach (ContactPoint2D contact in collision.contacts)
			{
				// Check that the collision is towards the tip of
				// the tentacle instead of at the back. (e.g. filter
				// out weird hits from behind).
				if (Vector2.Dot(contact.normal, forward) < 0)
				{
					if (TryGetComponentInParent(collision.gameObject, out Rigidbody2D otherRigid))
						CreateHingeJointRigidBody(contact.point, otherRigid);
					else
						CreateHingeJointFixedWorld(contact.point);

					// Start retracting the arm.
					joint.linearOffset = baseExtention;
					joint.angularOffset = baseAngularOffset;
					State = TentacleState.GRAPPLED;

					onGrapple.Invoke(collision, contact);

					break;
				}
			}
		}
	}

	private static bool TryGetComponentInParent<T>(GameObject gameObject, out T component) where T : Component
	{
		component = gameObject.GetComponentInParent<T>();
		return component != null;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (State == TentacleState.EXTENDED_PUSH)
			onPushContact.Invoke(collision);

		GrappleCollisionHandling(collision);
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		GrappleCollisionHandling(collision);
	}

	#endregion

	// Update is called once per frame
	void FixedUpdate()
    {
		// Keep pointing a grappling arm out if
		// we are still extending.
		if (State == TentacleState.EXTENDED_GRAPPLE)
		{
			WorldToJointOffets(extentionPullWorldPoint);
		}
    }

	#endregion

	#region Debugging Info	
#if UNITY_EDITOR
	private void OnGUI()
	{
		GUI.Label(new Rect(new Vector2(0, Screen.height)+Vector2.Scale(Camera.main.WorldToScreenPoint(transform.position), new Vector2(1, -1)), new Vector2(100, 20)), State.ToString());
	}
#endif
	#endregion
}