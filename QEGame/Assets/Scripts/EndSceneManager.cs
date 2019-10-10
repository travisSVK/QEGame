using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneManager : MonoBehaviour
{
    private bool _finished;
    public GameObject clientObject = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_finished)
        {
            Client client = FindObjectOfType<Client>();
            int clientId = 0;
            if (client)
            {
                clientId = client.clientId;
                DestroyImmediate(client.gameObject);
            }
            _finished = false;
            SceneManager.LoadScene("StartScene" + clientId);
        }
    }

    public void Restart()
    {
        _finished = true;
    }
}
