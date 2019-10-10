using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableBasedOnLanguage : MonoBehaviour
{
    public Language language;

    private void Start()
    {
        LanguageManager languageManager = FindObjectOfType<LanguageManager>();
        if (languageManager)
        {
            if (languageManager.activeLanguage == language)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogError("Could not find LanguageManager in the scene.");
        }
    }
}
