using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomOnClick : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("joystick button 0") || Input.GetKeyDown("joystick button 1") || Input.GetKeyDown("joystick button 2") || Input.GetKeyDown("joystick button 3"))
        {
            GetComponent<Button>().Select();
        }

        if (Input.GetKeyUp("joystick button 0") || Input.GetKeyDown("joystick button 1") || Input.GetKeyDown("joystick button 2") || Input.GetKeyDown("joystick button 3"))
        {
            GetComponent<Button>().onClick.Invoke();
        }
    }
}
