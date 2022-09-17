using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CameraGroupController : MonoBehaviour
{
    private CinemachineTargetGroup targetGroup;

    [SerializeField]
    private Transform tentaclesParent;

    // Start is called before the first frame update
    void Start()
    {
        targetGroup = GetComponent<CinemachineTargetGroup>();
        for(int i = 0; i < tentaclesParent.childCount; ++i)
        {
            int current = i;
            tentaclesParent.GetChild(i).GetComponent<Tentacle>().onDetach.AddListener(delegate { RemoveTentacle(current); });
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RemoveTentacle(int index)
    {
        targetGroup.RemoveMember(tentaclesParent.GetChild(index));
    }
}
