using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraController))]
public class CameraControllerInspector : Editor
{
    private enum Mode
    {
        Translate,
        Rotate
    }

    private Mode _mode = Mode.Translate;

    private List<bool> _foldouts = new List<bool>();

    private static readonly string[] _modes = { "Translate", "Rotate" };

    public override void OnInspectorGUI()
    {
        CameraController cameraController = (CameraController)target;
        if (cameraController.controlPoints.Count != _foldouts.Count)
        {
            ResetFoldout(cameraController);
        }

        EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel);

        cameraController.speed = EditorGUILayout.FloatField("Speed", cameraController.speed);

        EditorGUILayout.LabelField("Mode", EditorStyles.boldLabel);

        _mode = (Mode)GUILayout.SelectionGrid((int)_mode, _modes, 2);

        EditorGUILayout.LabelField("Control Points", EditorStyles.boldLabel);

        for (int i = 0; i < cameraController.controlPoints.Count; ++i)
        {
            _foldouts[i] = EditorGUILayout.BeginFoldoutHeaderGroup(_foldouts[i], cameraController.controlPoints[i].name);
            if (_foldouts[i])
            {
                string name = EditorGUILayout.TextField("Name", cameraController.controlPoints[i].name);
                Vector3 position = EditorGUILayout.Vector3Field("Position", cameraController.controlPoints[i].position);
                Quaternion rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", cameraController.controlPoints[i].rotation.eulerAngles));
                cameraController.controlPoints[i] = new CameraController.ControlPoint(name, position, rotation);

                GUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginDisabledGroup(i == 0);
                    {
                        if (GUILayout.Button("Up"))
                        {
                            CameraController.ControlPoint temp = cameraController.controlPoints[i];
                            cameraController.controlPoints[i] = cameraController.controlPoints[i - 1];
                            cameraController.controlPoints[i - 1] = temp;
                            GUILayout.EndHorizontal();
                            break;
                        }
                    }
                    EditorGUI.EndDisabledGroup();

                    EditorGUI.BeginDisabledGroup(i >= cameraController.controlPoints.Count - 1);
                    {
                        if (GUILayout.Button("Down"))
                        {
                            CameraController.ControlPoint temp = cameraController.controlPoints[i];
                            cameraController.controlPoints[i] = cameraController.controlPoints[i + 1];
                            cameraController.controlPoints[i + 1] = temp;
                            GUILayout.EndHorizontal();
                            break;
                        }
                    }
                    EditorGUI.EndDisabledGroup();

                    if (GUILayout.Button("Delete"))
                    {
                        cameraController.controlPoints.RemoveAt(i);
                        GUILayout.EndHorizontal();
                        break;
                    }
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Add Control Point"))
        {
            if (cameraController.controlPoints.Count <= 0)
            {
                cameraController.controlPoints.Add(new CameraController.ControlPoint("New Control Point", Vector3.zero, Quaternion.identity));
            }
            else if (cameraController.controlPoints.Count == 1)
            {
                cameraController.controlPoints.Add(new CameraController.ControlPoint("New Control Point", new Vector3(0.0f, 0.0f, 5.0f), Quaternion.identity));
            }
            else
            {
                Vector3 normal = cameraController.controlPoints[cameraController.controlPoints.Count - 1].position - cameraController.controlPoints[cameraController.controlPoints.Count - 2].position;
                cameraController.controlPoints.Add(new CameraController.ControlPoint("New Control Point", cameraController.controlPoints[cameraController.controlPoints.Count - 1].position + normal.normalized * 5.0f, Quaternion.identity));
            }
            _foldouts.Add(true);
        }

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        {
            EditorGUI.BeginDisabledGroup(!cameraController.canMoveForward);
            {
                if (GUILayout.Button("Move Forward"))
                {
                    if (EditorApplication.isPlaying)
                    {
                        cameraController.MoveForward();
                    }
                    else
                    {
                        cameraController.SnapForward();
                    }
                }
            }
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!cameraController.canMoveBack);
            {
                if (GUILayout.Button("Move Back"))
                {
                    if (EditorApplication.isPlaying)
                    {
                        cameraController.MoveBack();
                    }
                    else
                    {
                        cameraController.SnapBack();
                    }
                }
            }
            EditorGUI.EndDisabledGroup();
        }
        GUILayout.EndHorizontal();
    }

    protected virtual void OnSceneGUI()
    {
        CameraController cameraController = (CameraController)target;

        Handles.color = Color.cyan;
        for (int i = 0; i < cameraController.controlPoints.Count - 1; ++i)
        {
            Handles.DrawLine(cameraController.controlPoints[i].position, cameraController.controlPoints[i + 1].position);
        }

        GUIStyle textStyle = new GUIStyle();
        textStyle.normal.textColor = Color.white;

        for (int i = 0; i < cameraController.controlPoints.Count; ++i)
        {
            Handles.Label(cameraController.controlPoints[i].position, cameraController.controlPoints[i].name, textStyle);

            if (_mode == Mode.Translate)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newTargetPosition = Handles.PositionHandle(
                    cameraController.controlPoints[i].position, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    cameraController.controlPoints[i] = new CameraController.ControlPoint(
                        cameraController.controlPoints[i].name, newTargetPosition, cameraController.controlPoints[i].rotation);
                }
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                Quaternion rotation = Handles.RotationHandle(
                    cameraController.controlPoints[i].rotation, cameraController.controlPoints[i].position);
                cameraController.controlPoints[i] = new CameraController.ControlPoint(
                    cameraController.controlPoints[i].name, cameraController.controlPoints[i].position, rotation);
                cameraController.UpdateTransformations();
            }
        }
    }

    private void ResetFoldout(CameraController cameraController)
    {
        _foldouts.Clear();
        for (int i = 0; i < cameraController.controlPoints.Count; ++i)
        {
            _foldouts.Add(true);
        }
    }

    private void OnEnable()
    {
        CameraController cameraController = (CameraController)target;
        if (cameraController.controlPoints.Count != _foldouts.Count)
        {
            ResetFoldout(cameraController);
        }

        cameraController.Restart();
    }
}
