using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SimpleCameraFollower : MonoBehaviour
{
	public float lerpSpeed = 10f;
	public float marginWidth = 10f;
	private Camera cam;

	private List<Transform> transformsToFollow = new List<Transform>();

	private void Awake()
	{
		cam = GetComponent<Camera>();
	}

	public void AddTarget(Transform t)
	{
		if(!transformsToFollow.Contains(t))
			transformsToFollow.Add(t);
	}

	public void RemoveTarget(Transform t)
	{
		transformsToFollow.Remove(t);
	}

	private void LateUpdate()
	{
		transformsToFollow.RemoveAll(t => t==null);
		if(transformsToFollow.Count > 0)
		{
			Bounds targetArea = new Bounds(transformsToFollow[0].position, Vector3.zero);
			for(int i=1; i<transformsToFollow.Count; i++)
			{
				targetArea.Encapsulate(transformsToFollow[i].position);
			}
			targetArea.Expand(marginWidth);

			Vector3 newPos = Vector3.Lerp(transform.position, targetArea.center, Time.deltaTime * lerpSpeed);
			newPos.z = transform.position.z;
			transform.position = newPos;

			cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, Mathf.Max(targetArea.extents.y, targetArea.extents.x/cam.aspect), Time.deltaTime*lerpSpeed);
		}
	}
}
