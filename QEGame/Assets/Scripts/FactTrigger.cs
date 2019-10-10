using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactTrigger : MonoBehaviour
{
    public string swedishText = "";

    public string englishText = "";

    public TextCrossfade swedishTextUI = null;

    public TextCrossfade englishTextUI = null;

    private void OnCollisionEnter(Collision collision)
    {
        VerticalPlayerController verticalPlayer = collision.collider.GetComponent<VerticalPlayerController>();
        HorizontalPlayerController horizontalPlayer = collision.collider.GetComponent<HorizontalPlayerController>();
        if (verticalPlayer || horizontalPlayer)
        {
            swedishTextUI.SetText(swedishText);
            englishTextUI.SetText(englishText);
        }
    }
}
