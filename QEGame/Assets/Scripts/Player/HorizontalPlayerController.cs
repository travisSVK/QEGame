using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalPlayerController : MonoBehaviour
{
    [SerializeField]
    private float _movementSpeed = 5.0f;
    private CharacterController _characterController;
    private Vector3 _moveDirection = Vector3.zero;

    [SerializeField] private TutorialLevelController tlc; // TEMPORARY

    /**
     * @brief Speed of the game object horizontal movement.
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
        _moveDirection = new Vector3(Input.GetAxis("Horizontal") * _movementSpeed, 0.0f, 0.0f);

        // TEMPORARY
        if (!(_moveDirection.x == 0f && _moveDirection.y == 0f && _moveDirection.z == 0f))
        {
            tlc.NotifyMovement(isHorizontal: true);
        }

        _characterController.Move(_moveDirection * Time.deltaTime);
    }
}
