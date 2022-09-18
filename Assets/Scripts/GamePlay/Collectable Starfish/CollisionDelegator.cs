using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CollisionDelegator : MonoBehaviour
{
	public UnityEvent<Collision2D> onCollisionEnter = new UnityEvent<Collision2D>();
	public UnityEvent<Collision2D> onCollisionExit = new UnityEvent<Collision2D>();
	public UnityEvent<Collider2D> onTriggerEnter = new UnityEvent<Collider2D>();
	public UnityEvent<Collider2D> onTriggerExit = new UnityEvent<Collider2D>();

	private void OnCollisionEnter2D(Collision2D collision)
	{
		onCollisionEnter.Invoke(collision);
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		onCollisionExit.Invoke(collision);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		onTriggerEnter.Invoke(collision);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		onTriggerEnter.Invoke(collision);
	}
}
