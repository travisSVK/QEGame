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
        gameObject.SetActive(false);
    }
}
