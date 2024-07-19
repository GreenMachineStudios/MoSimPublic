using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed = 0f;

    private Vector2 translateValue;
    private Vector3 startingDirection;
    private Vector3 startingRotation;

    void Start()
    {
        startingDirection = transform.forward;
        startingRotation = transform.right;
    }

    void FixedUpdate()
    {
        Vector3 moveDirection = startingDirection * translateValue.y + startingRotation * translateValue.x;

        rb.AddForce(moveDirection * moveSpeed);
    }

    public void OnMove(InputAction.CallbackContext ctx) 
    {
        translateValue = ctx.ReadValue<Vector2>();
        if (Math.Abs(translateValue.magnitude) > 0f) 
        {
            CameraPan.parentMoving = true;
        }
        else 
        {
            CameraPan.parentMoving = false;
        }
    }
}
