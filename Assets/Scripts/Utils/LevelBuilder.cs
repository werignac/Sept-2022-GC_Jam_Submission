using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class LevelBuilder : MonoBehaviour
{
    public string levelName;
    public Vector2 levelDimentions;
    public GameObject playerSpawnPoint;

    public void Update()
    {
        Debug.DrawLine(transform.position, transform.position + (new Vector3(levelDimentions.x, 0, 0)));
        Debug.DrawLine(transform.position + (new Vector3(levelDimentions.x, 0, 0)), transform.position + (new Vector3(levelDimentions.x, levelDimentions.y, 0)));
        Debug.DrawLine(transform.position + (new Vector3(levelDimentions.x, levelDimentions.y, 0)), transform.position + (new Vector3(0, levelDimentions.y, 0)));
        Debug.DrawLine(transform.position + (new Vector3(0, levelDimentions.y, 0)), transform.position);
        gameObject.name = levelName;
    }

#if UNITY_EDITOR
	public void SaveLevel()
    {
        PrefabUtility.SaveAsPrefabAsset(gameObject, "Assets/Resources/Levels/" + levelName + ".prefab");
    }
#endif

}
