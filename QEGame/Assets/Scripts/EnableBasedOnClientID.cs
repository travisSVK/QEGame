using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBasedOnClientID : MonoBehaviour
{
    public int id;

    public bool Check()
    {
        Client client = FindObjectOfType<Client>();
        if (client)
        {
            int activeId = client.clientId;
            gameObject.SetActive(id == activeId);

            return id == activeId;
        }
        
        return false;
    }

    private void Awake()
    {
        Check();
    }
}
