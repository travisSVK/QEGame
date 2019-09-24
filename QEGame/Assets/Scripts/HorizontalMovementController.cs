using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalMovementController : MonoBehaviour
{
    public float movementSpeed = 5.0f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }
    
    void Update()
    {
        moveDirection = new Vector3(Input.GetAxis("Horizontal") * movementSpeed, 0.0f, 0.0f);
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
