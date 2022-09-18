using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Cinemachine;

[ExecuteInEditMode]
public class LevelBuilder : MonoBehaviour
{
    public string levelName;
    public Vector2 levelDimentions;
    public GameObject playerSpawnPoint;

    private void Start()
    {
        if (Application.isPlaying)
        {
            var collider = gameObject.AddComponent<PolygonCollider2D>();
            collider.points = new Vector2[] { new Vector2(0, 0), new Vector2(levelDimentions.x, 0), levelDimentions, new Vector2(0, levelDimentions.y) };
            collider.isTrigger = true;
            //GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = collider;
        }
    }

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
