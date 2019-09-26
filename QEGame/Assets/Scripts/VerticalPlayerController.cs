using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlayerController : MonoBehaviour
{
    private float _movementSpeed = 5.0f;
    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;

    public float MovementSpeed
    {
        get { return _movementSpeed; }
        set { _movementSpeed = value; }
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        moveDirection = new Vector3(0.0f, 0.0f, Input.GetAxis("Vertical") * _movementSpeed);
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
