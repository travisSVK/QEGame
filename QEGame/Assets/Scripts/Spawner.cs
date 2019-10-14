using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab = null;
    public GameObject spawnEffect = null;

    public bool spawnOnStart = false;

    public void Spawn()
    {
        Instantiate(spawnEffect, transform.position, transform.rotation, transform.parent);
        Instantiate(prefab, transform.position, transform.rotation, transform.parent);
    }

    private void Start()
    {
        if (spawnOnStart)
        {
            Spawn();
        }
    }
}
