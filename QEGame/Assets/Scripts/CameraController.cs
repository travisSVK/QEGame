using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Serializable]
    public struct ControlPoint
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;

        public ControlPoint(string name, Vector3 position, Quaternion rotation)
        {
            this.name = name;
            this.position = position;
            this.rotation = rotation;
        }
    }

    [SerializeField]
    private float _speed = 5.0f;

    [SerializeField]
    private List<ControlPoint> _controlPoints = new List<ControlPoint>();

    private bool _isMovingForward = true;
    private int _currentControlPoint = 0;
    private float _movementProgress = 0.0f;

    public float speed
    {
        get { return _speed; }
        set { _speed = value; }
    }

    public List<ControlPoint> controlPoints
    {
        get { return _controlPoints; }
        set { _controlPoints = value; }
    }

    public bool canMoveBack
    {
        get
        {
            return _currentControlPoint - 1 >= 0;
        }
    }

    public bool canMoveForward
    {
        get
        {
            return _currentControlPoint + 1 < _controlPoints.Count;
        }
    }

    /**
     * @brief Move to the previous control point.
     */
    public void MoveBack()
    {
        // If enabled, currently moving.
        if (enabled)
        {
            SnapToNextControlPoint();
        }

        if (canMoveBack)
        {
            enabled = true;
            _isMovingForward = false;
            _movementProgress = 0.0f;
        }
    }

    /**
     * @brief Move to the next control point.
     */
    public void MoveForward()
    {
        // If enabled, currently moving.
        if (enabled)
        {
            SnapToNextControlPoint();
        }

        if (canMoveForward)
        {
            enabled = true;
            _isMovingForward = true;
            _movementProgress = 0.0f;
        }
    }

    public void Restart()
    {
        _currentControlPoint = 0;
        UpdateTransformations();
    }

    public void SnapBack()
    {
        if (canMoveBack)
        {
            --_currentControlPoint;
            UpdateTransformations();
        }
    }

    public void SnapForward()
    {
        if (canMoveForward)
        {
            ++_currentControlPoint;
            UpdateTransformations();
        }
    }

    /**
     * @brief Update the position and position in accordance to the current control point.
     *
     * Call this function from the editor only since the update function is not called.
     */
    public void UpdateTransformations()
    {
        transform.position = _controlPoints[_currentControlPoint].position;
        transform.rotation = _controlPoints[_currentControlPoint].rotation;
    }

    private void SnapToNextControlPoint()
    {
        if (_isMovingForward)
        {
            _currentControlPoint = Mathf.Min(_controlPoints.Count - 1, _currentControlPoint + 1);
        }
        else
        {
            _currentControlPoint = Mathf.Max(0, _currentControlPoint - 1);
        }

        transform.position = _controlPoints[_currentControlPoint].position;
        enabled = false;
    }

    private void LateUpdate()
    {
        if (_isMovingForward)
        {
            _movementProgress += (Time.deltaTime / Vector3.Distance(_controlPoints[_currentControlPoint].position, _controlPoints[_currentControlPoint + 1].position)) * _speed;

            if (_movementProgress >= 1.0f)
            {
                ++_currentControlPoint;
                enabled = false;
                transform.position = _controlPoints[_currentControlPoint].position;
                transform.rotation = _controlPoints[_currentControlPoint].rotation;
            }
            else
            {
                transform.position = Vector3.Lerp(_controlPoints[_currentControlPoint].position, _controlPoints[_currentControlPoint + 1].position, Mathf.SmoothStep(0.0f, 1.0f, _movementProgress));
                transform.rotation = Quaternion.Slerp(_controlPoints[_currentControlPoint].rotation, _controlPoints[_currentControlPoint + 1].rotation, Mathf.SmoothStep(0.0f, 1.0f, _movementProgress));
            }
        }
        else
        {
            _movementProgress += (Time.deltaTime / Vector3.Distance(_controlPoints[_currentControlPoint].position, _controlPoints[_currentControlPoint - 1].position)) * _speed;

            if (_movementProgress >= 1.0f)
            {
                --_currentControlPoint;
                enabled = false;
                transform.position = _controlPoints[_currentControlPoint].position;
                transform.rotation = _controlPoints[_currentControlPoint].rotation;
            }
            else
            {
                transform.position = Vector3.Lerp(_controlPoints[_currentControlPoint].position, _controlPoints[_currentControlPoint - 1].position, Mathf.SmoothStep(0.0f, 1.0f, _movementProgress));
                transform.rotation = Quaternion.Slerp(_controlPoints[_currentControlPoint].rotation, _controlPoints[_currentControlPoint - 1].rotation, Mathf.SmoothStep(0.0f, 1.0f, _movementProgress));
            }
        }
    }

    private void Start()
    {
        if (_controlPoints.Count > 0)
        {
            transform.position = _controlPoints[0].position;
            transform.rotation = _controlPoints[0].rotation;
        }

        enabled = false;
    }
}
