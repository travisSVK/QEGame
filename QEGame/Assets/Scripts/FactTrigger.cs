using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactTrigger : MonoBehaviour
{
    public string swedishText = "";

    public string englishText = "";

    public TextCrossfade swedishTextUI = null;

    public TextCrossfade englishTextUI = null;

    private void OnTriggerEnter(Collider other)
    {
        VerticalPlayerController verticalPlayer = other.GetComponent<VerticalPlayerController>();
        HorizontalPlayerController horizontalPlayer = other.GetComponent<HorizontalPlayerController>();
        if (verticalPlayer || horizontalPlayer)
        {
            swedishTextUI.SetText(swedishText);
            englishTextUI.SetText(englishText);
        }
    }
}
