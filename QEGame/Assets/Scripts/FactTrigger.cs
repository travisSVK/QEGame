using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactTrigger : MonoBehaviour
{
    public string swedishText = "";

    public string englishText = "";

    [HideInInspector]
    public TextCrossfade swedishTextUI = null;

    [HideInInspector]
    public TextCrossfade englishTextUI = null;

    private void Start()
    {
        SwedishFactTag sweFact = FindObjectOfType<SwedishFactTag>();
        if (sweFact)
        {
            swedishTextUI = sweFact.GetComponent<TextCrossfade>();
        }

        EnglishFactTag engFact = FindObjectOfType<EnglishFactTag>();
        if (engFact)
        {
            englishTextUI = engFact.GetComponent<TextCrossfade>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        VerticalPlayerController verticalPlayer = other.GetComponent<VerticalPlayerController>();
        HorizontalPlayerController horizontalPlayer = other.GetComponent<HorizontalPlayerController>();
        if (verticalPlayer || horizontalPlayer)
        {
            if (swedishTextUI)
            {
                swedishTextUI.SetText(swedishText);
            }

            if (englishTextUI)
            {
                englishTextUI.SetText(englishText);
            }
        }
    }
}
