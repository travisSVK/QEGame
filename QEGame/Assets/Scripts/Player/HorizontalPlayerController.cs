using UnityEngine;

public class HorizontalPlayerController : PlayerControllerBase
{
    private void Update()
    {
        _input = new Vector3(Input.GetAxis("Horizontal"), 0.0f, 0.0f);
    }
}