using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestJoystick : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
       
    }

    // Update is called once per frame
    void Update()
    {
        float value = Input.GetAxis("Horizontal");
        Debug.Log(value);

        
        Debug.Log(Input.GetKeyDown("joystick button 3"));
    }
}
