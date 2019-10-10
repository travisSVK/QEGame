using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToScene : MonoBehaviour
{
    public string nameOfScene = "";

    public float delay = 0.0f;

    private void Update()
    {
        delay -= Time.deltaTime;

        if (delay <= 0.0f)
        {
            SceneManager.LoadScene(nameOfScene);
        }
    }
}
