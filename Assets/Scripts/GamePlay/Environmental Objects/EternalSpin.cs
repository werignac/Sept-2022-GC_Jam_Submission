using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EternalSpin : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField]
	[Tooltip("Number of degrees spun per second. Positive is counterclockwise.")]
    float angularVelocity = 90f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		rb.MoveRotation(rb.rotation + angularVelocity * Time.fixedDeltaTime);
    }
}
