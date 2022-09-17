using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTester : MonoBehaviour
{
    [SerializeField]
    private GameObject playerObject;

    [SerializeField]
    private LevelBuilder level;

    // Start is called before the first frame update
    void Start()
    {
        playerObject.transform.position = level.playerSpawnPoint.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
