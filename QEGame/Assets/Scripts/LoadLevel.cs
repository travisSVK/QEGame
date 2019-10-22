using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public Text playButtonText;

    public GameObject firstScreen = null;
    public GameObject waitingForPlayersSwedish = null;
    public GameObject waitingForPlayersEnglish = null;
    public GameObject playerOrb1 = null;
    public GameObject playerOrb2 = null;

    public bool isSwedish = true;

    private void Start()
    {
        FindObjectOfType<LanguageManager>().activeLanguage = Language.Swedish;
        playButtonText.text = "Spela";

        waitingForPlayersSwedish.SetActive(false);
        waitingForPlayersEnglish.SetActive(false);
    }

    public void SetToEnglish()
    {
        isSwedish = false;
        playButtonText.text = "PLAY";
    }

    public void SetToSwedish()
    {
        isSwedish = true;
        playButtonText.text = "SPELA";
    }

    private void StartClientConnection()
    {
        Client clientScript = FindObjectOfType<Client>();
        if (clientScript)
        {
            clientScript.ConnectToServer();
        }
    }

    public void StartFirstScene()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void LoadNextLevel()
    {
        firstScreen.SetActive(false);
        playerOrb1.SetActive(false);
        playerOrb2.SetActive(false);

        if (isSwedish)
        {
            waitingForPlayersSwedish.SetActive(true);        
            FindObjectOfType<LanguageManager>().activeLanguage = Language.Swedish;
            StartClientConnection();
        }
        else
        {
            waitingForPlayersEnglish.SetActive(true);            
            FindObjectOfType<LanguageManager>().activeLanguage = Language.English;
            StartClientConnection();
        }
        //SceneManager.LoadScene("Menu");
    }
}
