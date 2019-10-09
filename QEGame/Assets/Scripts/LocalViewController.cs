using UnityEngine;

public class LocalViewController : MonoBehaviour
{
    public GameObject[] players;
    public GameObject[] levels;
    public GameObject[] textCanvases = null; // Only set for the tutorial level

    private uint currentVisibleLevelIndex;

    private void Start()
    {
        currentVisibleLevelIndex = 1;
        SetLevelVisibility(false);
        currentVisibleLevelIndex = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetLevelVisibility(false);
            currentVisibleLevelIndex = (currentVisibleLevelIndex + 1) % 2;
            SetLevelVisibility(true);
        }
    }

    private void SetLevelVisibility(bool isVisible)
    {
        if (textCanvases != null && textCanvases.Length == levels.Length)
        {
            textCanvases[currentVisibleLevelIndex].SetActive(isVisible);
        }

        Renderer[] array = levels[currentVisibleLevelIndex].GetComponentsInChildren<Renderer>();
        foreach (var renderer in array)
        {
            renderer.enabled = isVisible;
        }
    }
}
