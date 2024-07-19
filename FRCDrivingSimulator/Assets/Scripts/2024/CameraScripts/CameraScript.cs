using UnityEngine;
using Cinemachine;

public class CameraScript : MonoBehaviour
{
    public Transform[] targets;

    public bool robotCentric;
    private Transform target;
    private CinemachineVirtualCamera vcam;

    void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>();
        target = GetEnabledTarget();

        if (robotCentric) 
        {
            transform.SetParent(target);
        }
        else
        {
            vcam.Follow = target;
            vcam.LookAt = target;
        }
    }

    Transform GetEnabledTarget()
    {
        foreach (Transform target in targets)
        {
            if (target.gameObject.activeSelf)
            {
                return target;
            }
        }
        Debug.LogError("No enabled targets found!");
        return null;
    }
}
