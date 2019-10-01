using UnityEngine;

public class ViewController : MonoBehaviour
{
    public GameObject[] levels;

    private uint currentVisibleLevelIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            levels[currentVisibleLevelIndex].SetActive(false);
            currentVisibleLevelIndex = (currentVisibleLevelIndex + 1) % 2;
            levels[currentVisibleLevelIndex].SetActive(true);
        }
    }
}
