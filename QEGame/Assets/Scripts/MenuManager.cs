using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Button swedishButton = null;

    public Button englishButton = null;

    public GameObject waitingForPlayersSwedish = null;

    public GameObject waitingForPlayersEnglish = null;

    private void Awake()
    {
        swedishButton.gameObject.SetActive(true);
        englishButton.gameObject.SetActive(true);
        waitingForPlayersSwedish.SetActive(false);
        waitingForPlayersEnglish.SetActive(false);

        swedishButton.onClick.AddListener(PressedSwedish);
        englishButton.onClick.AddListener(PressedEnglish);
    }

    private void PressedEnglish()
    {
        swedishButton.gameObject.SetActive(false);
        englishButton.gameObject.SetActive(false);
        waitingForPlayersEnglish.SetActive(true);
    }

    private void PressedSwedish()
    {
        swedishButton.gameObject.SetActive(false);
        englishButton.gameObject.SetActive(false);
        waitingForPlayersSwedish.SetActive(true);
    }
}
