using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab = null;
    public GameObject spawnEffect = null;

    private GameObject _trackedObject = null;

    public bool spawnOnStart = false;

    public void Spawn()
    {
        if (_trackedObject)
        {
            Destroy(_trackedObject);
        }

        Instantiate(spawnEffect, transform.position, transform.rotation, transform.parent);
        _trackedObject = Instantiate(prefab, transform.position, transform.rotation, transform.parent);
    }

    private void Start()
    {
        if (spawnOnStart)
        {
            Spawn();
        }
    }
}
