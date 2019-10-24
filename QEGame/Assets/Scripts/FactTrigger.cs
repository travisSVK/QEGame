using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FactTrigger : MonoBehaviour
{
    public string swedishText = "";

    public string englishText = "";

    public Sprite sprite = null;

    [HideInInspector]
    public TextCrossfade swedishTextUI = null;

    [HideInInspector]
    public TextCrossfade englishTextUI = null;

    [HideInInspector]
    public ImageCrossfade image = null;

    private void Update()
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

        FactImageTag imgTag = FindObjectOfType<FactImageTag>();
        if (imgTag)
        {
            image = imgTag.GetComponent<ImageCrossfade>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("FUck you UYNITY");

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

            if (image)
            {
                image.SetSprite(sprite);
            }
        }
    }
}
