using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlayerController : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed = 5.0f;
    private CharacterController _characterController;
    private Vector3 _moveDirection = Vector3.zero;

    /**
     * @brief Speed of the game object vertical movement.
     */
    public float MovementSpeed
    {
        get { return _movementSpeed; }
        set { _movementSpeed = value; }
    }

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        _moveDirection = new Vector3(0.0f, 0.0f, Input.GetAxis("Vertical") * _movementSpeed);
        _characterController.Move(_moveDirection * Time.deltaTime);
    }
}
