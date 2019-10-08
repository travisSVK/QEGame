using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(RoomManager))]
public class RoomManagerInspector : Editor
{
    private List<Scene> _loadedScenes = new List<Scene>();

    public override void OnInspectorGUI()
    {
        RoomManager roomManager = (RoomManager)target;

        for (int i = 0; i < roomManager.rooms.Count; ++i)
        {
            EditorGUILayout.BeginHorizontal();
            {
                roomManager.rooms[i] = EditorGUILayout.TextField(roomManager.rooms[i]);
                if (GUILayout.Button("Delete"))
                {
                    roomManager.rooms.RemoveAt(i);
                    break;
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Scene"))
        {
            roomManager.rooms.Add("");
        }
    }
}
