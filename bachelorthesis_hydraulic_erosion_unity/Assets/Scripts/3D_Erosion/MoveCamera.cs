using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] public float MovementSpeed = 1.0f;
    [SerializeField] private float TurnSpeed = 45.0f;
    [SerializeField] private float HorizontalInput;
    [SerializeField] private float ForwardInput;
    [SerializeField] private float RotationalInput;
    [SerializeField] private float FlightInput;
    void LateUpdate()
    {
        HorizontalInput = Input.GetAxis("Horizontal");
        ForwardInput = Input.GetAxis("Vertical");
        RotationalInput = Input.GetAxis("Rotational");
        FlightInput = Input.GetAxis("Flight");

        transform.position = transform.position + Vector3.forward * ForwardInput * MovementSpeed;
        transform.position = transform.position + Vector3.right * HorizontalInput * MovementSpeed;
        transform.position = transform.position + Vector3.up * FlightInput * MovementSpeed;
        transform.Rotate(new Vector3(0,0), RotationalInput * TurnSpeed * Time.deltaTime);
    }
}
