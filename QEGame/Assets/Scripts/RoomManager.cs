using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    [SerializeField]
    private List<string> _rooms = new List<string>();

    private List<Scene> _loadedScenes = new List<Scene>();

    public List<string> rooms
    {
        get { return _rooms; }
        set { _rooms = value; }
    }

    public void Refresh()
    {
        for (int i = 0; i < _loadedScenes.Count; ++i)
        {
            SceneManager.UnloadSceneAsync(_loadedScenes[i]);
        }

        _loadedScenes.Clear();

        for (int i = 0; i < _rooms.Count; ++i)
        {
            SceneManager.LoadScene(_rooms[i], LoadSceneMode.Additive);
            _loadedScenes.Add(SceneManager.GetSceneByName(_rooms[i]));
        }
    }

    private void Awake()
    {
        EnableBasedOnClientID enableBasedOnClientID = GetComponent<EnableBasedOnClientID>();
        if (enableBasedOnClientID && enableBasedOnClientID.Check())
        {
            Refresh();
        }
    }
}
