using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CameraGroupController : MonoBehaviour
{
    private CinemachineTargetGroup targetGroup;

    // Start is called before the first frame update
    void Start()
    {
        targetGroup = GetComponent<CinemachineTargetGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpTentacles(GameObject playerParent)
    {
        while(targetGroup.m_Targets.Length > 0)
        {
            targetGroup.RemoveMember(targetGroup.m_Targets[0].target);
        }
        var children = playerParent.GetComponentsInChildren<Rigidbody2D>();
        foreach(var child in children)
        {
            Tentacle childTentacle = child.gameObject.GetComponent<Tentacle>();
            if (childTentacle != null)
            {
                childTentacle.onDetach.AddListener(delegate { RemoveTentacle(child.transform); });
            }
            targetGroup.AddMember(child.transform, 1, 0);
        }
    }

    private void RemoveTentacle(Transform tentacleTransform)
    {
        targetGroup.RemoveMember(tentacleTransform);
    }
}
