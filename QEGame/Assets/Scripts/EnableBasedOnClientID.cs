using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBasedOnClientID : MonoBehaviour
{
    public int id;

    private void Awake()
    {
        Client client = FindObjectOfType<Client>();
        if (client)
        {
            int activeId = client.clientId;
            gameObject.SetActive(id == activeId);
        }
        else
        {
            Debug.Log("No client in this scene.");
        }
    }
}
