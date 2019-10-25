using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarFlareMovement : MonoBehaviour
{
    private Vector3[] _points = new Vector3[3];

    private float _prevTime = 0.0f;
    private float _currentTime = 0.0f;
    private float _alpha = 0.0f;
    private float _realTime = 0.0f;

    public void NewTime(float newTime)
    {
        _prevTime = _currentTime;
        _currentTime = newTime;
        _alpha = 0.0f;
    }

    void Awake()
    {
        _points[0] = gameObject.transform.Find("MovePoint0").position;
        _points[1] = gameObject.transform.Find("MovePoint1").position;
        _points[2] = gameObject.transform.Find("MovePoint2").position;
    }

    private void Update()
    {
        float t = Mathf.Lerp(_prevTime, _currentTime, Mathf.Min(1.0f, _alpha));
        _alpha += Time.deltaTime;

        float finalTime = (t / 10.0f) % 3.0f;
        int currentPoint = (int)finalTime;
        int prevPoint = currentPoint - 1;
        prevPoint = prevPoint < 0 ? 2 : prevPoint;
        finalTime %= 1.0f;

        transform.position = Vector3.Lerp(_points[prevPoint], _points[currentPoint], Mathf.SmoothStep(0.0f, 1.0f, finalTime));
    }
}
