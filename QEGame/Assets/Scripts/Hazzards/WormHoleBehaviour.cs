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
            playerBase.transform.position = new Vector3(_wormHoleSpouse.transform.position.x, playerBase.transform.position.y, _wormHoleSpouse.transform.position.z);
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }
}
