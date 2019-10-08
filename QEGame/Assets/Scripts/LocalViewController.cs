using UnityEngine;

public class LocalViewController : MonoBehaviour
{
    public GameObject[] players;
    public GameObject[] levels;

    private uint currentVisibleLevelIndex = 0;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            levels[currentVisibleLevelIndex].SetActive(false);
            players[currentVisibleLevelIndex].SetActive(false);
            currentVisibleLevelIndex = (currentVisibleLevelIndex + 1) % 2;
            levels[currentVisibleLevelIndex].SetActive(true);
            players[currentVisibleLevelIndex].SetActive(true);
        }
    }
}
