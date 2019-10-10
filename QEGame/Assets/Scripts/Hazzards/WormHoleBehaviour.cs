using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormHoleBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject _wormHoleSpouse;

    private void OnTriggerEnter(Collider collider)
    {
        PlayerTag playerTag = collider.GetComponent<PlayerTag>();
        if (playerTag)
        {
            playerTag.transform.position = _wormHoleSpouse.transform.position;
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
