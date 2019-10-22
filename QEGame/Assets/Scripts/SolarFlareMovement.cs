using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarFlareMovement : MonoBehaviour
{
    private Vector3 movePoint0;
    private Vector3 movePoint1;
    private Vector3 movePoint2;
    private Vector3 newPoint;

    private float smooth = 0.7f;
    private float resetTime = 7.0f;

    public int currentState = 0;


    // Start is called before the first frame update
    void Awake()
    {
        movePoint0 = gameObject.transform.Find("MovePoint0").position;
        movePoint1 = gameObject.transform.Find("MovePoint1").position;
        movePoint2 = gameObject.transform.Find("MovePoint2").position;

        ChangeTarget();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, newPoint, smooth * Time.deltaTime);
    }

    void ChangeTarget()
    {
        if (currentState == 0)
        {
            currentState = 1;
            newPoint = movePoint1;
        }
        else if (currentState == 1)
        {
            currentState = 2;
            newPoint = movePoint2;
        }
        else if (currentState == 2)
        {
            currentState = 3;
            newPoint = movePoint1;
        }
        else if (currentState == 3)
        {
            currentState = 0;
            newPoint = movePoint0;
        }
        Invoke("ChangeTarget", resetTime);
    }
}
