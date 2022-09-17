using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MovingObject : MonoBehaviour
{
    [SerializeField]
    private Vector2 moveDirection;

    [SerializeField]
    private float moveTime;

    [SerializeField]
    private bool platform;

    private Vector2 startPos;
    private Vector2 endPos;
    private bool returning;
    private Vector2 currentVelocity;
    private float targetSpeed;

    private const float acceleration = 100f;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        endPos = startPos + moveDirection;
        returning = false;
        targetSpeed = (endPos - startPos).magnitude / moveTime;
    }


    // Update is called once per frame
    void Update()
    {

        Debug.DrawLine(transform.position, transform.position + new Vector3(moveDirection.x, moveDirection.y, 0));

        if (Application.isPlaying)
        {
            Vector2 v2Pos = new Vector2(transform.position.x, transform.position.y);
            Vector2 targetPos = endPos;
            if (returning)
            {
                targetPos = startPos;
            }
            currentVelocity += acceleration * (targetPos - v2Pos).normalized;
            if(currentVelocity.magnitude > targetSpeed)
            {
                currentVelocity = targetSpeed * currentVelocity.normalized;
            }
            transform.Translate(currentVelocity * Time.deltaTime);
            if (((targetPos.x - v2Pos.x) * (targetPos.x - transform.position.x) <= 0 && (targetPos.y - v2Pos.y) * (targetPos.y - transform.position.y) <= 0))
            {
                returning = !returning;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (platform)
        {
            collision.transform.Translate(currentVelocity * Time.deltaTime);
        }
    }
}
