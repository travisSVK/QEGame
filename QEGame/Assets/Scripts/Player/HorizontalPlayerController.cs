using UnityEngine;

public class HorizontalPlayerController : PlayerControllerBase
{
    // Called when the other player changed its position.
    public override void SendDeltaXOrZ(float deltaZ)
    {
        var newPos = transform.position;
        newPos.z += deltaZ;
        transform.position = newPos;
    }

    public override void Start() { base.Start(); }

    public virtual void Update()
    {
        Vector3 _moveDirection = new Vector3(Input.GetAxis("Horizontal") * _movementSpeed, 0.0f, 0.0f);
        float deltaX = transform.position.x;
        _characterController.Move(_moveDirection * Time.deltaTime);
        deltaX -= transform.position.x;

        if (_localConnection != null && _moveDirection.x != 0.0)
        {
            _localConnection.SendDeltaXOrZ(deltaX);
        }
    }
}
