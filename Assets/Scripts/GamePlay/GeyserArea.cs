using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeyserArea : MonoBehaviour
{
	[SerializeField]
	private float forceMagnitude = 20f;
	[SerializeField]
	private float timeToFullForce = 1f;
	[SerializeField]
	private float sinusoidBottom = 25;
	[SerializeField]
	private float sinusoidPeriod = 1f;


	private Dictionary<GameObject, float> pushedObjects = new Dictionary<GameObject, float>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        pushedObjects.Add(collision.gameObject, 0);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        pushedObjects.Remove(collision.gameObject);
    }

    void FixedUpdate()
    {
		GameObject[] keyCopy = new GameObject[pushedObjects.Count];
		pushedObjects.Keys.CopyTo(keyCopy, 0);

		float relativeSinusoidCap = GetRelativeHeight(sinusoidBottom);

		foreach (GameObject entry in keyCopy)
        {
            Rigidbody2D body = entry.GetComponent<Rigidbody2D>();
			
            if(body == null)
            {
                body = entry.GetComponentInParent<Rigidbody2D>();
            }
            if(body != null)
            {
				float relativeHeight = GetRelativeHeight(body.transform.position);

				if (relativeHeight > relativeSinusoidCap)
				{
					pushedObjects[entry] = (pushedObjects[entry] + Time.fixedDeltaTime) % sinusoidPeriod;

					float sinusoidMultiplier = Mathf.Cos(pushedObjects[entry] * 2 * Mathf.PI / sinusoidPeriod + Mathf.PI) / 2 + 0.5f;

					body.AddForce(transform.up * (forceMagnitude * sinusoidMultiplier));
				}
				else
				{
					pushedObjects[entry] += 0f;
					body.AddForce(transform.up * forceMagnitude);
				}
            }
        }
    }

	private float GetRelativeHeight(Vector3 worldPoint)
	{
		Vector3 localPoint = transform.InverseTransformPoint(worldPoint);
		return GetRelativeHeight(localPoint.y);
	}

	private float GetRelativeHeight(float localHeight)
	{
		BoxCollider2D b_collider = GetComponent<BoxCollider2D>();

		float minHeight = b_collider.offset.y - b_collider.size.y / 2;
		float maxHeight = b_collider.offset.y + b_collider.size.y / 2;


		float alpha = Mathf.Clamp01((localHeight - minHeight) / (maxHeight - minHeight));
		return alpha;
	}
}
