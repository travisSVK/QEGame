using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private bool _used = false;
    private bool _enabled = true;

    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        PlayerControllerBase playerBase = other.GetComponent<PlayerControllerBase>();
        if (playerBase && !_used && _enabled)
        {
            Server server = FindObjectOfType<Server>();
            if (server && server.LevelFinished())
            {
                _enabled = false;
            }
            _used = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        PlayerControllerBase playerBase = other.GetComponent<PlayerControllerBase>();
        if (playerBase)
        {
            Server server = FindObjectOfType<Server>();
            if (server)
            {
                server.LevelFinishedRetracted();
            }
            _used = false;
        }
    }
}
