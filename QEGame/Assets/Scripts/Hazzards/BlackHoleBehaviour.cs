using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleBehaviour : MonoBehaviour
{
    [SerializeField]
    private float _gravitationalPullConstant = 0.5f;
    [SerializeField]
    private float _deadlyDistance = 0.5f;
    private float _pullFactor = 0.0f;
    private Rigidbody _playerRigidBody;

    public void FixedUpdate()
    {
        if (_playerRigidBody)
        {
            float distance = Vector3.Distance(transform.position, _playerRigidBody.transform.position);
            if (distance <= 0.5f)
            {
                PlayerTag playerTag = _playerRigidBody.GetComponent<PlayerTag>();
                playerTag.OnPlayerDeath();
            }
            Vector3 direction = transform.position - _playerRigidBody.transform.position;
            float gravitationalPull = Mathf.Clamp(_pullFactor - distance, 0.0f, _pullFactor);
            _playerRigidBody.MovePosition(_playerRigidBody.position + direction * gravitationalPull * Time.fixedDeltaTime * _gravitationalPullConstant);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        PlayerTag playerTag = collider.GetComponent<PlayerTag>();
        if (playerTag)
        {
            _playerRigidBody = playerTag.GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        PlayerTag playerTag = collider.GetComponent<PlayerTag>();
        if (playerTag)
        {
            _playerRigidBody = null;
        }
    }

    public void Start()
    {
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider)
        {
            _pullFactor = sphereCollider.radius;
        }
        
    }
}
