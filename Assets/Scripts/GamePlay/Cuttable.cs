using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuttable : MonoBehaviour
{

    public void Cut(GameObject thing)
    {
        print("Oof, ouch, owie my " + thing.name + " has been cut.");
    }
}
