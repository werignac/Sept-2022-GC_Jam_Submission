using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        print("Hot Dog Activation");
        Cuttable cut = other.gameObject.GetComponentInParent<Cuttable>();
        cut.Cut(other.gameObject);
    }
}
