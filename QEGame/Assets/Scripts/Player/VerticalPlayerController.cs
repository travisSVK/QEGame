using UnityEngine;

public class VerticalPlayerController : PlayerControllerBase
{
    public override void Start() { base.Start(); }

    public virtual void Update()
    {
<<<<<<< HEAD
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
=======
        Vector3 _moveDirection = new Vector3(0.0f, 0.0f, Input.GetAxis("Vertical"));
        if (_moveDirection.z != 0.0)
>>>>>>> Added a tutorial level which works locally
        {
            int dir = System.Math.Sign(_moveDirection.z);
            MoveZ(dir, Time.deltaTime);
            if (_localConnection != null)
            {
                _localConnection.MoveZ(dir, Time.deltaTime);
            }
        }
    }
}
