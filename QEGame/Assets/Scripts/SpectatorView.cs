using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorView : MonoBehaviour
{
    private bool _finished;
    public GameObject serverObject = null;
    public Camera player1Camera = null;
    public Camera player2Camera = null;

    private void Start()
    {
        _finished = false;

        AudioListener audioListener0 = player1Camera.GetComponent<AudioListener>();
        AudioListener audioListener1 = player1Camera.GetComponent<AudioListener>();

        if (audioListener0)
        {
            Destroy(audioListener0);
        }

        if (audioListener1)
        {
            Destroy(audioListener1);
        }

        float height = (float)Screen.height;
        float width = (float)Screen.width;

        player1Camera.rect = new Rect(0.0f + (40 / width), 0.2f, 0.5f - (60 / width), 0.8f - (40 / height));
        player2Camera.rect = new Rect(0.5f + (20 / width), 0.2f, 0.5f - (60 / width), 0.8f - (40 / height));
    }

    private void Update()
    {
        if (_finished)
        {
            Server server = FindObjectOfType<Server>();
            if (server)
            {
                Destroy(server.gameObject);
            }
            Instantiate(serverObject);
            _finished = false;
        }
    }

    public void Restart()
    {
        _finished = true;
    }
}
