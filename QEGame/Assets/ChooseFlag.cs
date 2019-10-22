using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseFlag : MonoBehaviour
{
    private Button swedishFlag;
    private Button englishFlag;

    private Button currentSelection;

    private void Awake()
    {
        swedishFlag = gameObject.transform.Find("Swedish").GetComponent<Button>();
        englishFlag = gameObject.transform.Find("English").GetComponent<Button>();

        currentSelection = swedishFlag;
    }

    // Update is called once per frame
    void Update()
    {
        float axisX = Input.GetAxis("Horizontal");

        if (axisX < 0)
        {
            currentSelection = swedishFlag;
        }
        else if (axisX > 0)
        {
            currentSelection = englishFlag;
        }

        currentSelection.Select();
        currentSelection.onClick.Invoke();
    }
}
