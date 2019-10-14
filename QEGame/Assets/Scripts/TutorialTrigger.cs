using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{
    [TextArea(5, 10)]
    public string swedishText = "";

    [TextArea(5, 10)]
    public string englishText = "";

    private Text _text = null;

    public void Start()
    {
        TutorialTextTag tutorialTag = FindObjectOfType<TutorialTextTag>();
        if (tutorialTag)
        {
            _text = tutorialTag.GetComponent<Text>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        VerticalPlayerController verticalPlayer = other.GetComponent<VerticalPlayerController>();
        HorizontalPlayerController horizontalPlayer = other.GetComponent<HorizontalPlayerController>();
        if (verticalPlayer || horizontalPlayer)
        {
            if (_text)
            {
                LanguageManager language = FindObjectOfType<LanguageManager>();
                if (language.activeLanguage == Language.Swedish)
                {
                    _text.text = swedishText;
                }
                else if (language.activeLanguage == Language.English)
                {
                    _text.text = englishText;
                }
            }
        }
    }
}
