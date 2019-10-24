using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleBehaviour : MonoBehaviour
{
    [SerializeField]
    private float _gravitationalPullConstant = 0.5f;

    private float _pullFactor = 0.0f;

    private Rigidbody _playerRigidBody;

    public void FixedUpdate()
    {
        if (_playerRigidBody)
        {
            //Server server = FindObjectOfType<Server>();
            //if (!server)
            //{
                float distance = Vector3.Distance(transform.position, _playerRigidBody.transform.position);
            
                PlayerControllerBase _playerController = _playerRigidBody.GetComponent<PlayerControllerBase>();
                if (distance <= 0.2f)
                {
                    Debug.Log("Dead");
                    _playerRigidBody = null;
                    _playerController.OnPlayerDeath();
                    return;
                }
                Vector3 direction = transform.position - _playerRigidBody.transform.position;
                float gravitationalPull = Mathf.Clamp(_pullFactor - distance, 0.0f, _pullFactor);

                _playerController.blackholeIncrement += direction * gravitationalPull * Time.fixedDeltaTime * _gravitationalPullConstant;
                _playerRigidBody.MovePosition(_playerRigidBody.position + direction * gravitationalPull * Time.fixedDeltaTime * _gravitationalPullConstant);
            //}
        }
    }

    private void Update()
    {
        //Server server = FindObjectOfType<Server>();
        //if (_playerRigidBody && server)
        //{
        //    float distance = Vector3.Distance(transform.position, _playerRigidBody.transform.position);

        //    PlayerControllerBase _playerController = _playerRigidBody.GetComponent<PlayerControllerBase>();
        //    if (distance <= 0.08f)
        //    {
        //        Debug.Log("Dead");
        //        _playerRigidBody = null;
        //        _playerController.OnPlayerDeath();
        //        return;
        //    }
        //    Vector3 direction = transform.position - _playerRigidBody.transform.position;
        //    float gravitationalPull = Mathf.Clamp(_pullFactor - distance, 0.0f, _pullFactor);
        //}
    }

    private void OnTriggerEnter(Collider collider)
    {
        PlayerControllerBase playerControllerBase = collider.GetComponent<PlayerControllerBase>();
        if (playerControllerBase)
        {
            _playerRigidBody = playerControllerBase.GetComponent<Rigidbody>();
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        PlayerControllerBase playerControllerBase = collider.GetComponent<PlayerControllerBase>();
        if (playerControllerBase)
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
