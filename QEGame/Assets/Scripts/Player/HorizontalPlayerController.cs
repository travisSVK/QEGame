using UnityEngine;

public class HorizontalPlayerController : PlayerControllerBase
{
    public override void Start() { base.Start(); }

    public virtual void Update()
    {
        Vector3 _moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, 0.0f);
        if (_moveDirection.x != 0.0)
        {
            int dir = System.Math.Sign(_moveDirection.x);
            MoveX(dir, Time.deltaTime);
            if (_localConnection != null)
            {
                _localConnection.MoveX(dir, Time.deltaTime);
            }
        }
    }
}
