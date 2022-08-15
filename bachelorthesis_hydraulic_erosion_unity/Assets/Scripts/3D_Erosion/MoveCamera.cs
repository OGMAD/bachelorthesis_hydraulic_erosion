using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] public float MovementSpeed = 0.1f;
    [SerializeField] private float TurnSpeed = 45.0f;
    [SerializeField] private float HorizontalInput;
    [SerializeField] private float ForwardInput;
    [SerializeField] private float RotationalInput;
    void LateUpdate()
    {
        HorizontalInput = Input.GetAxis("Horizontal");
        ForwardInput = Input.GetAxis("Vertical");
        RotationalInput = Input.GetAxis("Rotational");

        transform.position = transform.position + Vector3.forward * ForwardInput * MovementSpeed;
        transform.position = transform.position + Vector3.right * HorizontalInput * MovementSpeed;
        transform.Rotate(Vector3.up, RotationalInput * TurnSpeed * Time.deltaTime);
    }
}
