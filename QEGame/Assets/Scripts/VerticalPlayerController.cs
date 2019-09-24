using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlayerController : MonoBehaviour
{
    public float movementSpeed = 5.0f;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = new Vector3(0.0f, 0.0f, Input.GetAxis("Vertical") * movementSpeed);
        characterController.Move(moveDirection * Time.deltaTime);
    }
}
