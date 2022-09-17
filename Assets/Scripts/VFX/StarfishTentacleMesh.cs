using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class StarfishTentacleMesh : MonoBehaviour
{
	public Transform rootBone;
	public Transform leafBone;
	public int myIndex;
	public SkinnedMeshRenderer skinnedMeshRenderer;
	public Material wholeMaterial;
	public Material detachedMaterial;
	private Player myPlayer;
	private Vector3 tentacleLocalPos;
	private Quaternion tentacleLocalRot;
	private Tentacle MyTentacle => myPlayer.GetTentacle(myIndex);
	private Vector3 leafBoneStartPos;
	private Quaternion leafBoneStartRot;

	private void Start()
	{
		leafBoneStartPos = leafBone.localPosition;
		leafBoneStartRot = leafBone.localRotation;
		myPlayer = GetComponentInParent<Player>();
		tentacleLocalPos = MyTentacle.transform.worldToLocalMatrix.MultiplyPoint(leafBone.position);
		tentacleLocalRot = Quaternion.Inverse(MyTentacle.transform.rotation) * leafBone.rotation;
	}

	private void LateUpdate()
	{
		if(MyTentacle.State == Tentacle.TentacleState.DETACHED)
		{
			skinnedMeshRenderer.material = detachedMaterial;
			leafBone.transform.localPosition = leafBoneStartPos;
			leafBone.transform.localRotation = leafBoneStartRot;
		} else
		{
			skinnedMeshRenderer.material = wholeMaterial;
			leafBone.transform.position = MyTentacle.transform.localToWorldMatrix.MultiplyPoint(tentacleLocalPos);
			leafBone.transform.rotation = MyTentacle.transform.rotation * tentacleLocalRot;
		}
	}
}
