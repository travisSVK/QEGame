using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalPlayerController : MonoBehaviour
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
        moveDirection = new Vector3(Input.GetAxis("Horizontal") * _movementSpeed, 0.0f, 0.0f);
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
