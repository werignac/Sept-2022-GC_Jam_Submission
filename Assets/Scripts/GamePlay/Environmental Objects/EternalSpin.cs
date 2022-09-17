using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EternalSpin : MonoBehaviour
{
    private int direction;
    public Rigidbody2D rb;

    [SerializeField]
    bool reverseDirection = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (reverseDirection)
        {
            direction = -1;
        }
        else
        {
            direction = 1;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.angularVelocity < 90)
        {
            rb.AddTorque(direction * 500.0f);
        }          
    }
}
