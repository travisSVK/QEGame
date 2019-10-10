using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalPlayerController : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed = 5.0f;
    private Rigidbody _rigidBody;
    private Vector3 _input = Vector3.zero;

    /**
     * @brief Speed of the game object vertical movement.
     */
    public float MovementSpeed
    {
        get { return _movementSpeed; }
        set { _movementSpeed = value; }
    }

    private void FixedUpdate()
    {
        _rigidBody.MovePosition(_rigidBody.position + _input * _movementSpeed * Time.fixedDeltaTime);
    }

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        _input = Vector3.zero;
        _input.z = Input.GetAxis("Vertical");
        if (_input != Vector3.zero)
        {
            transform.forward = _input;
        }
    }
}
