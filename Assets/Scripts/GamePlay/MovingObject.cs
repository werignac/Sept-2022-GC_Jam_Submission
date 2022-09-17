using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MovingObject : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The points the object moves through, relative to starting position")]
    private Vector2[] movePoints;

    [SerializeField]
    [Tooltip("The time it takes to move through all the points one way, or to complete one full loop")]
    private float moveTime;

    [SerializeField]
    [Tooltip("True if it should move objects on top of it as it travels")]
    private bool platform;

    [SerializeField]
    [Tooltip("True if object should go through all points in a loop, false if it should travel through all points one way, and then go back through the points in reverse order")]
    private bool loop;

    private Vector2[] goalPoints;
    private bool returning;
    private Vector2 currentVelocity;
    private float targetSpeed;
    private int currentTarget;
    private Rigidbody2D body;
    private Vector2 pastPosition;

    private const float acceleration = 100f;

    // Start is called before the first frame update
    void Start()
    {
        goalPoints = new Vector2[movePoints.Length + 1];
        goalPoints[0] = transform.position;
        for(int i = 0; i < movePoints.Length; ++i)
        {
            goalPoints[i + 1] = movePoints[i] + goalPoints[0];
        }
        returning = false;
        float distance = 0;
        for(int i = 0; i < movePoints.Length; ++i)
        {
            distance += (goalPoints[i + 1] - goalPoints[i]).magnitude;
        }
        if(loop)
        {
            distance += (goalPoints[0] - goalPoints[goalPoints.Length - 1]).magnitude;
        }
        targetSpeed = distance / moveTime;
        currentTarget = Mathf.Min(1, movePoints.Length);
        body = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void Update()
    {
        if (movePoints.Length > 0)
        {
            Debug.DrawLine(transform.position, transform.position + new Vector3(movePoints[0].x, movePoints[0].y, 0));
        }
        for (int i = 0; i < movePoints.Length - 1; ++i)
        {
            Debug.DrawLine(transform.position + new Vector3(movePoints[i].x, movePoints[i].y, 0), transform.position + new Vector3(movePoints[i + 1].x, movePoints[i + 1].y, 0));
        }
        if (loop && movePoints.Length > 0)
        {
            Debug.DrawLine(transform.position + new Vector3(movePoints[movePoints.Length - 1].x, movePoints[movePoints.Length - 1].y, 0), transform.position);
        }
    }

    private void FixedUpdate()
    {

        if (Application.isPlaying)
        {
            Vector2 v2Pos = new Vector2(transform.position.x, transform.position.y);
            Vector2 targetPos = goalPoints[currentTarget];
            currentVelocity += acceleration * (targetPos - v2Pos).normalized;
            if(currentVelocity.magnitude > targetSpeed)
            {
                currentVelocity = targetSpeed * currentVelocity.normalized;
            }
            if (((targetPos.x - v2Pos.x) * (targetPos.x - pastPosition.x) <= 0 && (targetPos.y - v2Pos.y) * (targetPos.y - pastPosition.y) <= 0))
            {
                if (returning)
                {
                    --currentTarget;
                }
                else
                {
                    ++currentTarget;
                }
                if (currentTarget >= goalPoints.Length || currentTarget < 0)
                {
                    if (loop)
                    {
                        currentTarget = 0;
                    }
                    else
                    {
                        returning = !returning;
                        if (returning)
                        {
                            currentTarget -= 2;
                        }
                        else
                        {
                            currentTarget += 2;
                        }
                    }
                }
            }
            pastPosition = v2Pos;
            body.MovePosition(v2Pos + (currentVelocity * Time.fixedDeltaTime));
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (platform)
        {
            bool onTop = false;
            foreach(var contact in collision.contacts)
            {
                if (contact.normal.y < -0.8)
                {
                    onTop = true;
                    break;
                }
            }
            if (onTop)
            {
                Rigidbody2D otherBody = collision.gameObject.GetComponent<Rigidbody2D>();
                if (otherBody == null)
                {
                    collision.transform.Translate(currentVelocity * Time.deltaTime);
                }
                else
                {
                    otherBody.MovePosition(new Vector2(collision.transform.position.x, collision.transform.position.y) + (currentVelocity * Time.fixedDeltaTime));
                }
            }
        }
    }
}
