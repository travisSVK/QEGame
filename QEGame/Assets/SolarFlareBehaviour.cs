using System.Collections;
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
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Server server = FindObjectOfType<Server>();
        PlayerControllerBase playerBase = other.GetComponent<PlayerControllerBase>();
        if (server && playerBase)
        {
            Debug.Log("Dead");
            playerBase.OnPlayerDeath();
        }
    }
}
