using UnityEngine;

public class VerticalPlayerController : PlayerControllerBase
{
    private void Update()
    {
        _input = new Vector3(0.0f, 0.0f, Input.GetAxis("Vertical"));
        if (transform.position.y < -10.0f)
        {
            OnPlayerDeath();
        }
    }
}
