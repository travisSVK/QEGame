﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarFlareBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Server server = FindObjectOfType<Server>();
        if (server)
        {
            PlayerControllerBase[] playerControllers = FindObjectsOfType<PlayerControllerBase>();
            foreach (PlayerControllerBase playerControllerBase in playerControllers)
            {
                float distance = Vector3.Distance(transform.position, playerControllerBase.transform.position);
                if (distance <= 0.2f)
                {
                    Debug.Log("Dead");
                    playerControllerBase.OnPlayerDeath();
                }
            }
        }
    }
}