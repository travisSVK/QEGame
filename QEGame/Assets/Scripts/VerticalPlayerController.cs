using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlayerController : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed = 5.0f;
    private CharacterController _characterController;
    private Vector3 _moveDirection = Vector3.zero;

    public float MovementSpeed
    {
        get { return _movementSpeed; }
        set { _movementSpeed = value; }
    }

    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        _moveDirection = new Vector3(0.0f, 0.0f, Input.GetAxis("Vertical") * _movementSpeed);
        _characterController.Move(_moveDirection * Time.deltaTime);
    }
}
