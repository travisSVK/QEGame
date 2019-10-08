using UnityEngine;

public class VerticalPlayerController : PlayerControllerBase
{
    // Called when the other player changed its position.
    public override void SendDeltaXOrZ(float deltaX)
    {
        var newPos = transform.position;
        newPos.x += deltaX;
        transform.position = newPos;
    }

    public override void Start() { base.Start(); }

    public virtual void Update()
    {
        Vector3 _moveDirection = new Vector3(0.0f, 0.0f, Input.GetAxis("Vertical") * _movementSpeed);
        float deltaZ = transform.position.z;
        _characterController.Move(_moveDirection * Time.deltaTime);
        deltaZ = transform.position.z - deltaZ;

        if (_localConnection != null && _moveDirection.z != 0.0)
        {
            _localConnection.SendDeltaXOrZ(deltaZ);
        }
    }
}
