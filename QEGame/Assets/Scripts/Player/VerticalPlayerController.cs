using UnityEngine;

public class VerticalPlayerController : PlayerControllerBase
{
    private void Update()
    {
        _input = new Vector3(0.0f, 0.0f, Input.GetAxis("Vertical"));
    }
}
