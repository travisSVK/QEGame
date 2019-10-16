using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    private bool _enabled = true;

    public void OnTriggerEnter(Collider other)
    {
        PlayerControllerBase playerBase = other.GetComponent<PlayerControllerBase>();
        if (playerBase && _enabled)
        {
            Server server = FindObjectOfType<Server>();
            if (server && server.LevelFinished())
            {
                _enabled = false;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        PlayerControllerBase playerBase = other.GetComponent<PlayerControllerBase>();
        if (playerBase && _enabled)
        {
            Server server = FindObjectOfType<Server>();
            if (server)
            {
                server.LevelFinishedRetracted();
            }
        }
    }
}
