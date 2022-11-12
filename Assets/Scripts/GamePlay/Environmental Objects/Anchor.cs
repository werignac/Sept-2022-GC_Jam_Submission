using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anchor : MonoBehaviour
{
	[SerializeField]
	private float respawnTimer = 3f;
	private Vector3 respawnPosition;
	private Quaternion respawnRotation;
	private Rigidbody2D rigid;

	private Coroutine respawnRoutine;

	private int grappleCount = 0;

    // Start is called before the first frame update
    void Start()
    {
		rigid = GetComponent<Rigidbody2D>();
		rigid.constraints = RigidbodyConstraints2D.FreezeAll;

		respawnPosition = transform.position;
		respawnRotation = transform.rotation;

		grappleCount = 0;

	}

	private void Respawn()
	{
		Instantiate(gameObject, respawnPosition, respawnRotation, transform.parent);
		Destroy(gameObject);
	}

	private IEnumerator RespawnTimer()
	{
		yield return new WaitForSeconds(respawnTimer);
		Respawn();
	}

	private void OnGrapple(ContactPoint2D contact)
	{
		if (respawnRoutine != null)
		{
			StopCoroutine(respawnRoutine);
			respawnRoutine = null;
		}
		rigid.constraints = 0;
		grappleCount++;
	}

	private void OnGrappleDetach()
	{
		grappleCount--;
		if (grappleCount == 0)
			respawnRoutine = StartCoroutine(RespawnTimer());
	}
}
