using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeyserArea : MonoBehaviour
{
    [SerializeField]
    private Vector2 force;

    private List<GameObject> pushedObjects;

    void Start()
    {
        pushedObjects = new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        pushedObjects.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        pushedObjects.Remove(collision.gameObject);
    }

    void FixedUpdate()
    {
        for(int i = 0; i < pushedObjects.Count; ++i)
        {
            Rigidbody2D body = pushedObjects[i].GetComponent<Rigidbody2D>();
            if(body == null)
            {
                body = pushedObjects[i].GetComponentInParent<Rigidbody2D>();
            }
            if(body != null)
            {
                body.AddForce(force);
            }
        }
    }
}
