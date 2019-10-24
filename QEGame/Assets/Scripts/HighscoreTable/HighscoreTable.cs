using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighscoreTable : MonoBehaviour
{
    private Transform _entryContainer;
    private Transform _entryTemplate;
    private List<Transform> _highscoreEntryTransformList;
    public Animator _containerAnimator;
    public bool isHighScoreClosed = false;
    public string lastName = "";
    public int lastScore = 0;

    private void Awake()
    {
        _entryContainer = transform.Find("highscoreEntryContainer");
        _entryTemplate = _entryContainer.Find("ContainerAnimation");
        _entryTemplate.gameObject.SetActive(false);

        //Load saved Highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores.highscoreEntryList.Count > 0)
        {
            SortList(highscores);
        }

        // Keep only top 8
        if (highscores.highscoreEntryList.Count > 8)
        {
            int limit = highscores.highscoreEntryList.Count;
            for (int i = limit; i > 8; --i)
            {
                highscores.highscoreEntryList.RemoveAt(i - 1);
            }
        }

        StartCoroutine(WaitForAppearing(highscores.highscoreEntryList));
        //StartCoroutine(closeWindowDelay());
    }

    private void Update()
    {
        if (Input.GetKeyDown("joystick button 0") || Input.GetKeyDown("joystick button 1") || Input.GetKeyDown("joystick button 2") || Input.GetKeyDown("joystick button 3"))
        {
            isHighScoreClosed = true;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            CanvasGroup canvasGroup = GetComponentInParent<CanvasGroup>();
            if (canvasGroup)
            {
                canvasGroup.alpha = canvasGroup.alpha == 1.0f ? 0.0f : 1.0f;
            }
        }
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry hightscoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 40f;
        Transform entryTransform = Instantiate(_entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        GameObject trophyIcon1st = entryTransform.Find("Content").Find("Text Mask").Find("highscoreEntryTemplate").Find("trophyIcon1st").gameObject;
        GameObject trophyIcon2nd = entryTransform.Find("Content").Find("Text Mask").Find("highscoreEntryTemplate").Find("trophyIcon2nd").gameObject;
        GameObject trophyIcon3rd = entryTransform.Find("Content").Find("Text Mask").Find("highscoreEntryTemplate").Find("trophyIcon3rd").gameObject;
        switch (rank)
        {
            default:
                rankString = rank + "TH";
                trophyIcon1st.SetActive(false);
                trophyIcon2nd.SetActive(false);
                trophyIcon3rd.SetActive(false);
                break;
            case 1:
                rankString = "1ST";
                trophyIcon1st.SetActive(true);
                trophyIcon2nd.SetActive(false);
                trophyIcon3rd.SetActive(false);
                break;
            case 2:
                rankString = "2ND";
                trophyIcon1st.SetActive(false);
                trophyIcon2nd.SetActive(true);
                trophyIcon3rd.SetActive(false);
                break;
            case 3:
                rankString = "3RD";
                trophyIcon1st.SetActive(false);
                trophyIcon2nd.SetActive(false);
                trophyIcon3rd.SetActive(true);
                break;
        }

        entryTransform.Find("Content").Find("Text Mask").Find("highscoreEntryTemplate").Find("posText").GetComponent<Text>().text = rankString;

        int score = hightscoreEntry.score;
        entryTransform.Find("Content").Find("Text Mask").Find("highscoreEntryTemplate").Find("scoreText").GetComponent<Text>().text = score.ToString();

        string name = hightscoreEntry.name;
        entryTransform.Find("Content").Find("Text Mask").Find("highscoreEntryTemplate").Find("nameText").GetComponent<Text>().text = name;

        if (rank == 1)
        {
            entryTransform.Find("Content").Find("Text Mask").Find("highscoreEntryTemplate").Find("posText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("Content").Find("Text Mask").Find("highscoreEntryTemplate").Find("scoreText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("Content").Find("Text Mask").Find("highscoreEntryTemplate").Find("nameText").GetComponent<Text>().color = Color.green;
        }

        
        transformList.Add(entryTransform);

    }

    public void AddHighscoreEntry(int score, string name)
    {
        lastName = name;
        lastScore = score;
        //Create HighscoreEntry
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, name = name };
        
        //Load saved Highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        //Add new entry to Highscores
        highscores.highscoreEntryList.Add(highscoreEntry);

        //Save updated Highscores
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
        
    }

    public int GetLastScore()
    {
        //Load saved Highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        

        if (highscores.highscoreEntryList.Count > 7)
        {
            SortList(highscores);
            return highscores.highscoreEntryList[highscores.highscoreEntryList.Count - 1].score;            
        }

        return -1;

    }

    private void SortList(Highscores highscores)
    {
        //Sort entry list by score
        for (int i = 0; i < highscores.highscoreEntryList.Count; ++i)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; ++j)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    //Swap
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }
    }

    public void SetDefaultValues()
    {
        PlayerPrefs.SetString("highscoreTable", JsonUtility.ToJson(new Highscores()));
        PlayerPrefs.Save();
    }

    IEnumerator WaitForAppearing(List<HighscoreEntry> _highscoreEntryList)
    {
        _highscoreEntryTransformList = new List<Transform>();
        foreach (HighscoreEntry highscoreEntry in _highscoreEntryList)
        {
            yield return new WaitForSeconds(0.3f);
            CreateHighscoreEntryTransform(highscoreEntry, _entryContainer, _highscoreEntryTransformList);
        }
    }

    IEnumerator closeWindowDelay()
    {
        yield return new WaitForSeconds(8.0f);
        _containerAnimator.SetTrigger("AutoCloseTrigger");
    }

    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList = new List<HighscoreEntry>();
    }

    /*
     * Represents a single  High score entry 
     * */
     [System.Serializable]
    private class HighscoreEntry
    {
        public int score;
        public string name;
    }
}
