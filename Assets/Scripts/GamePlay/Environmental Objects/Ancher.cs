using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ancher : MonoBehaviour
{
    [SerializeField]
    private float gravityMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody2D>().gravityScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7 || collision.gameObject.layer == 8)
        {
            GetComponent<Rigidbody2D>().gravityScale = gravityMultiplier;
        }
    }
}
