using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CollectableStarfish : MonoBehaviour
{
	[SerializeField]
	private CollectableStarfishData.StarfishColor _color;

	private Animator animator;

	private HashSet<GameObject> playerPartsInContact = new HashSet<GameObject>();

	private Animator GetOrFindAnimator()
	{
		if (animator == null)
			animator = GetComponent<Animator>();
		return animator;
	}

	private void Update()
	{
		GetOrFindAnimator().SetBool("In Range", playerPartsInContact.Count > 0);
	}

	public void FarColliderInBound(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
			playerPartsInContact.Add(other.gameObject);
	}

	public void FarColliderOutBound(Collider2D other)
	{
		if (other.gameObject.CompareTag("Player"))
			playerPartsInContact.Remove(other.gameObject);
	}

	public void NearColliderInBound(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
			Destroy(gameObject);
	}

	public void SetColor(CollectableStarfishData.StarfishColor color)
	{
		animator = GetOrFindAnimator();
		animator.runtimeAnimatorController = CollectableStarfishData.GetAnimatorForColor(color);
	}

	public void SetColor()
	{
		SetColor(_color);
	}
}

#if UNITY_EDITOR
[CustomEditor(typeof(CollectableStarfish))]
public class CollectableStarfishSettingsIMGUIRegister : Editor
{
	SerializedProperty colorProp;

	void OnEnable()
	{
		colorProp = serializedObject.FindProperty("_color");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.PropertyField(colorProp);
		if (serializedObject.ApplyModifiedProperties())
			((CollectableStarfish)target).SetColor();
	}
}
#endif
