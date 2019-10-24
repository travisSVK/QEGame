using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterSelectorBehavior : MonoBehaviour
{
    public HighscoreTable highscoreTableScript;
    public GameObject highscoreTableObject;

    private Dropdown firstLetter;
    private Dropdown secondLetter;
    private Dropdown thirdLetter;

    private Animator animator;

    private string firstLetterText;
    private string secondLetterText;
    private string thirdLetterText;

    private int oneSelection = 0;
    private int twoSelection = 0;
    private int threeSelection = 0;
    private int activeSelectionIndex = 0;

    private bool verticalSwitch = false;
    private bool horizontalSwitch = false;
    private int currentSelectionIndex = 0;
    private RectTransform selectionBackground;
    private Dropdown currentSelection;
    private Button nextButton;

    private int _score;

    // Start is called before the first frame update
    void Awake()
    {
        firstLetter = gameObject.transform.Find("Content").Find("Text Mask").Find("InputName").Find("FirstLetter").GetComponent<Dropdown>();
        firstLetterText = firstLetter.options[0].text;

        secondLetter = gameObject.transform.Find("Content").Find("Text Mask").Find("InputName").Find("SecondLetter").GetComponent<Dropdown>();
        secondLetterText = secondLetter.options[0].text;

        thirdLetter = gameObject.transform.Find("Content").Find("Text Mask").Find("InputName").Find("ThirdLetter").GetComponent<Dropdown>();
        thirdLetterText = thirdLetter.options[0].text;

        nextButton = gameObject.transform.Find("Content").Find("Text Mask").Find("Style Button").GetComponent<Button>();

        selectionBackground = gameObject.transform.Find("Content").Find("Text Mask").Find("InputName").Find("selectionBackground").GetComponent<RectTransform>();

        animator = gameObject.GetComponent<Animator>();
    }

    public void PassTheScore(int score)
    {
        _score = score;
    }

    public void ChangeFirstLetter()
    {
        firstLetterText = firstLetter.options[firstLetter.value].text;
    }

    public void ChangeSecondLetter()
    {
        secondLetterText = secondLetter.options[secondLetter.value].text;
    }

    public void ChangeThirdLetter()
    {
        thirdLetterText = thirdLetter.options[thirdLetter.value].text;
    }

    public void NextScreen()
    {
        string fullName = firstLetterText + secondLetterText + thirdLetterText;

        animator.SetTrigger("CloseTrigger");
        highscoreTableScript.AddHighscoreEntry(_score, fullName);
        highscoreTableObject.SetActive(true);
        highscoreTableObject.GetComponent<CanvasGroup>().alpha = 1.0f;
        gameObject.SetActive(false);
    }

    private void Update()
    {
        float axisX = Input.GetAxis("Horizontal");
        float axisY = Input.GetAxis("Vertical");


        if (axisX < 0 && horizontalSwitch)
        {
            currentSelectionIndex--;
            if (currentSelectionIndex < 0)
            {
                currentSelectionIndex = 2;
            }
            horizontalSwitch = false;
        }
        else if (axisX > 0 && horizontalSwitch)
        {
            currentSelectionIndex++;
            if (currentSelectionIndex > 2)
            {
                currentSelectionIndex = 0;
            }
            horizontalSwitch = false;
        }
        else if (axisX == 0)
        {
            horizontalSwitch = true;
        }

        switch (currentSelectionIndex)
        {
            case 0:
                currentSelection = firstLetter;
                selectionBackground.anchoredPosition = new Vector3(-220,-77,0);
                break;
            case 1:
                currentSelection = secondLetter;
                selectionBackground.anchoredPosition = new Vector3(-53, -77, 0);
                break;
            case 2:
                currentSelection = thirdLetter;
                selectionBackground.anchoredPosition = new Vector3(107, -77, 0);
                break;
            default:
                currentSelection = firstLetter;
                selectionBackground.anchoredPosition = new Vector3(-220, -77, 0);
                break;
        }

        

        if (axisY < 0 && verticalSwitch)
        {            

            if (currentSelectionIndex == 0)
            {
                oneSelection++;
                if (oneSelection > currentSelection.options.Count - 1)
                {
                    oneSelection = 0;
                }
                firstLetterText = currentSelection.options[currentSelection.value].text;
                currentSelection.value = oneSelection;
            }
            if (currentSelectionIndex == 1)
            {
                twoSelection++;
                if (twoSelection > currentSelection.options.Count - 1)
                {
                    twoSelection = 0;
                }
                secondLetterText = currentSelection.options[currentSelection.value].text;
                currentSelection.value = twoSelection;
            }
            if (currentSelectionIndex == 2)
            {
                threeSelection++;
                if (threeSelection > currentSelection.options.Count - 1)
                {
                    threeSelection = 0;
                }
                thirdLetterText = currentSelection.options[currentSelection.value].text;
                currentSelection.value = threeSelection;
            }

            verticalSwitch = false;
        }
        else if (axisY > 0 && verticalSwitch)
        {
            if (currentSelectionIndex == 0)
            {
                oneSelection--;
                if (oneSelection < 0)
                {
                    oneSelection = currentSelection.options.Count - 1;
                }
                firstLetterText = currentSelection.options[currentSelection.value].text;
                currentSelection.value = oneSelection;
            }
            if (currentSelectionIndex == 1)
            {
                twoSelection--;
                if (twoSelection < 0)
                {
                    twoSelection = currentSelection.options.Count - 1;
                }
                secondLetterText = currentSelection.options[currentSelection.value].text;
                currentSelection.value = twoSelection;
            }
            if (currentSelectionIndex == 2)
            {
                threeSelection--;
                if (threeSelection < 0)
                {
                    threeSelection = currentSelection.options.Count - 1;
                }
                thirdLetterText = currentSelection.options[currentSelection.value].text;
                currentSelection.value = threeSelection;
            }

            verticalSwitch = false;
        }
        else if (axisY == 0)
        {
            verticalSwitch = true;
        }

        

        if (Input.GetKey("joystick button 0") || Input.GetKey("joystick button 1") || Input.GetKey("joystick button 2") || Input.GetKey("joystick button 3"))
        {
            nextButton.Select();
        }

        if (Input.GetKeyUp("joystick button 0") || Input.GetKeyDown("joystick button 1") || Input.GetKeyDown("joystick button 2") || Input.GetKeyDown("joystick button 3"))
        {
            NextScreen();
        }
    }
}
