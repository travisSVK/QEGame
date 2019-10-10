using UnityEngine;

public class VerticalPlayerController : PlayerControllerBase
{
    public override void Start() { base.Start(); }

    public virtual void Update()
    {
        Vector3 _moveDirection = new Vector3(0.0f, 0.0f, Input.GetAxis("Vertical"));
        if (_moveDirection.z != 0.0)
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
