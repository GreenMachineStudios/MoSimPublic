using System;
using Unity.Mathematics;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    [SerializeField] private Alliance alliance;

    public Transform[] targets;
    public float smoothSpeed = 5f;
    public Vector2 rotationConstraints = new Vector2(-90, 90);
    public float closeDistanceThreshold = 2f;
    public float forwardMoveDistance = 5f;

    public static bool parentMoving = false;
    private bool move = true;

    private Vector3 targetPosition;
    private Transform target;

    [SerializeField] private Transform follow;

    void Start()
    {
        target = GetEnabledTarget();
        targetPosition = follow.position;

        if (alliance == Alliance.Red) { forwardMoveDistance = -forwardMoveDistance; }
    }

    void LateUpdate()
    {
        if (target == null) { return; }

        float distance = Vector3.Distance(transform.position, target.position);

        if (parentMoving)
        {
            move = true;
            targetPosition = follow.position;
        }
        else
        {
            if (distance < closeDistanceThreshold && move)
            {
                move = false;
                targetPosition = new Vector3(transform.position.x + forwardMoveDistance, transform.position.y, transform.position.z);
            }
            else if (distance >= closeDistanceThreshold && !move)
            {
                move = true;
            }
        }

        if (move) 
        {
            targetPosition = follow.position;
        }
        
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        transform.LookAt(target);
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
