using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormHoleBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject _wormHoleSpouse = null;

    private bool _canBeTirggered = true;

    private void OnTriggerEnter(Collider collider)
    {
        PlayerControllerBase playerBase = collider.GetComponent<PlayerControllerBase>();
        WormHoleBehaviour spouseHoleBehaviour = _wormHoleSpouse.GetComponent<WormHoleBehaviour>();
        if (playerBase && _canBeTirggered && spouseHoleBehaviour)
        {
            spouseHoleBehaviour.TriggerWormhole();
            playerBase.transform.position = new Vector3(_wormHoleSpouse.transform.position.x, playerBase.transform.position.y, _wormHoleSpouse.transform.position.z);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _canBeTirggered = true;
    }

    public void TriggerWormhole()
    {
        _canBeTirggered = false;
    }

    private void Update()
    {
    }
}
