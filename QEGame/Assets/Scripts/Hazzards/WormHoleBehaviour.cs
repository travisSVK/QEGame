using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormHoleBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject _wormHoleSpouse = null;

    private void OnTriggerEnter(Collider collider)
    {
        PlayerControllerBase playerBase = collider.GetComponent<PlayerControllerBase>();
        if (playerBase)
        {
            playerBase.transform.position = _wormHoleSpouse.transform.position;
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
