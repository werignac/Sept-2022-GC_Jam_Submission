using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DecorFade : MonoBehaviour
{
	[SerializeField]
	[Min(0)]
	private float fadeRate = 1;
	[SerializeField]
	[Range(0,1)]
	private float fadeAmount = 0.5f;

	private float progress = 1;

	private HashSet<GameObject> playerPartsInContact = new HashSet<GameObject>();
	private SpriteRenderer[] sprites;

	private void Start()
	{
		sprites = GetComponentsInChildren<SpriteRenderer>();
	}

	private void Update()
	{
		if (playerPartsInContact.Count == 0)
			progress += fadeRate * Time.deltaTime;
		else
			progress -= fadeRate * Time.deltaTime;

		progress = Mathf.Clamp(progress, fadeAmount, 1);

		foreach(SpriteRenderer sprite in sprites)
		{
			Color color = sprite.color;
			color.a = progress;
			sprite.color = color;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
			playerPartsInContact.Add(collision.gameObject);
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
			playerPartsInContact.Remove(collision.gameObject);
	}

}
