using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[DefaultExecutionOrder(1)]
public class DetachedTentacleMesh : MonoBehaviour
{
	public Transform rootBone;
	public SkinnedMeshRenderer skinnedMeshRenderer;
	public float snapBackSpeed = 10f;
	private Player myPlayer;
	private StarfishTentacleMesh myStarfishTentacleMesh;
	private Tentacle myTentacle;
	private Vector3 tentacleLocalPos;
	private Quaternion tentacleLocalRot;
	private Vector3 rootBoneStartPos;
	private Quaternion rootBoneStartRot;

	private void Start()
	{
		rootBoneStartPos = rootBone.localPosition;
		rootBoneStartRot = rootBone.localRotation;
		myPlayer = GetComponentInParent<Player>();
		myTentacle = GetComponentInParent<Tentacle>();
		myStarfishTentacleMesh = myPlayer.GetComponentsInChildren<StarfishTentacleMesh>().Where(stm => myPlayer.GetTentacle(stm.myIndex) == myTentacle).First();
		tentacleLocalPos = myTentacle.transform.worldToLocalMatrix.MultiplyPoint(rootBone.position);
		tentacleLocalRot = Quaternion.Inverse(myTentacle.transform.rotation) * rootBone.rotation;
	}

	private void LateUpdate()
	{
		if (myTentacle.State == Tentacle.TentacleState.DETACHED)
		{
			skinnedMeshRenderer.enabled = true;
			rootBone.transform.position = Vector3.Lerp(rootBone.transform.position, rootBone.parent.localToWorldMatrix.MultiplyPoint(rootBoneStartPos), Time.deltaTime * snapBackSpeed);
			rootBone.transform.localRotation = Quaternion.Euler(Vector3.Lerp(rootBone.localRotation.eulerAngles, rootBoneStartRot.eulerAngles, Time.deltaTime * snapBackSpeed * 30f));
		} else
		{
			skinnedMeshRenderer.enabled = false;
			rootBone.transform.position = myStarfishTentacleMesh.rootBone.transform.position;
			rootBone.transform.rotation = myStarfishTentacleMesh.rootBone.transform.rotation;
		}
	}
}
