using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndScreen : MonoBehaviour
{
    public HighscoreTable highscoreTableScript;
    public GameObject highscoreTableObject;

    public LetterSelectorBehavior letterSelectorScript;
    public GameObject letterSelectorObject;

    private float _animationTime = 1.5f;
    private Animator animator;

    private Text _pointsUIText;
    private Text _timeRemainingUIText;
    private Text _stagesCompletedUIText;

    private int _score;
    private float _desiredScoreNumber;
    private float _initialScoreNumber;
    private float _currentScoreNumber;

    private float _remainingTime;
    private float _desiredRemainingTimeNumber;
    private float _initialRemainingTimeNumber;
    private float _currentRemainingTimeNumber;

    private int _stagesCompleted;
    private float _desiredStagesCompletedNumber;
    private float _initialStagesCompletedNumber;
    private float _currentStagesCompletedNumber;

    private List<Transform> numberUIElements = new List<Transform>();

    public void SetScore(int value)
    {
        _score = value;
    }

    public void SetRemainingTime(float value)
    {
        if (value > 0)
        {
            _remainingTime = value;
        }
        else
        {
            _remainingTime = 0;
        }
    }

    public void SetCompletedStages(int value)
    {
        _stagesCompleted = value;
    }

    private void AnimationSetScoreNumber(float value)
    {
        _initialScoreNumber = _currentScoreNumber;
        _desiredScoreNumber = value;
    }

    private void AnimationSetRemainingTime(float value)
    {
        _initialRemainingTimeNumber = _desiredRemainingTimeNumber;
        _desiredRemainingTimeNumber = value;
    }

    private void AnimationSetStagesCompleted(int value)
    {
        _initialStagesCompletedNumber = _desiredStagesCompletedNumber;
        _desiredStagesCompletedNumber = value;
    }

    public void ActivateScreen()
    {
        _pointsUIText = numberUIElements[1].Find("ContainerAnimation").Find("Content").Find("Text Mask").Find("Content").Find("Text").GetComponent<Text>();
        _timeRemainingUIText = numberUIElements[3].Find("ContainerAnimation").Find("Content").Find("Text Mask").Find("Content").Find("Text").GetComponent<Text>();
        _stagesCompletedUIText = numberUIElements[6].Find("ContainerAnimation").Find("Content").Find("Text Mask").Find("Content").Find("Text").GetComponent<Text>();

        StartCoroutine(WaitForAppearing(numberUIElements));
    }

    public void LoadNextScreen()
    {
        if (_score > highscoreTableScript.GetLastScore())
        {
            //animator.SetTrigger("AutoCloseTrigger");
            letterSelectorObject.SetActive(true);
            letterSelectorScript.PassTheScore(_score);
            gameObject.SetActive(false);
        }
        else
        {
            highscoreTableObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    IEnumerator WaitForAppearing(List<Transform> UIElements)
    {
        for (int i = 0; i < numberUIElements.Count + 6; ++i)
        {
            yield return new WaitForSeconds(0.6f);
            switch(i)
            {
                case 0:
                    numberUIElements[i].gameObject.SetActive(true);
                    break;
                case 1:
                    numberUIElements[i].gameObject.SetActive(true);
                    break;
                case 2:
                    numberUIElements[i].gameObject.SetActive(true);
                    break;
                case 3:
                    numberUIElements[i].gameObject.SetActive(true);
                    AnimationSetRemainingTime(_remainingTime);
                    break;
                case 4:
                    numberUIElements[i].gameObject.SetActive(true);
                    break;
                case 5:
                    numberUIElements[i].gameObject.SetActive(true);
                    break;
                case 6:
                    AnimationSetRemainingTime(_remainingTime * 100);
                    numberUIElements[i].gameObject.SetActive(true);
                    AnimationSetStagesCompleted(_stagesCompleted);
                    break;
                case 7:
                    numberUIElements[i].gameObject.SetActive(true);
                    break;
                case 10:
                    AnimationSetStagesCompleted(_stagesCompleted * 100);
                    break;
                case 13:
                    _score = (int)(_remainingTime * 100) + (_stagesCompleted * 100);
                    AnimationSetRemainingTime(0);
                    AnimationSetStagesCompleted(0);
                    AnimationSetScoreNumber(_score);
                    break;
            }
        }
    }

    private void Awake()
    {
        numberUIElements.Add(gameObject.transform.Find("Content").Find("Text Mask").Find("EndScreen").Find("pointsText"));
        numberUIElements.Add(gameObject.transform.Find("Content").Find("Text Mask").Find("EndScreen").Find("points"));
        numberUIElements.Add(gameObject.transform.Find("Content").Find("Text Mask").Find("EndScreen").Find("timeText"));
        numberUIElements.Add(gameObject.transform.Find("Content").Find("Text Mask").Find("EndScreen").Find("timeRemaining"));
        numberUIElements.Add(gameObject.transform.Find("Content").Find("Text Mask").Find("EndScreen").Find("timeRemainingTimes100"));
        numberUIElements.Add(gameObject.transform.Find("Content").Find("Text Mask").Find("EndScreen").Find("stagesClearedText"));
        numberUIElements.Add(gameObject.transform.Find("Content").Find("Text Mask").Find("EndScreen").Find("stagesCleared"));
        numberUIElements.Add(gameObject.transform.Find("Content").Find("Text Mask").Find("EndScreen").Find("stagesClearedTimes100"));

        animator = gameObject.GetComponent<Animator>();
        StartCoroutine(closeWindowDelay());
        //===========
        //FOR TESTING
        //===========
        
        SetRemainingTime(260000f);
        SetCompletedStages(3);
        ActivateScreen();
        
    }

    private void Update()
    {
        if (_currentScoreNumber != _desiredScoreNumber)
        {
            if (_initialScoreNumber < _desiredScoreNumber)
            {
                _currentScoreNumber += (_animationTime * Time.deltaTime) * (_desiredScoreNumber - _initialScoreNumber);
                if (_currentScoreNumber >= _desiredScoreNumber)
                {
                    _currentScoreNumber = _desiredScoreNumber;
                }                
            }
            else
            {
                _currentScoreNumber -= (_animationTime * Time.deltaTime) * (_initialScoreNumber - _desiredScoreNumber);
                if ( _currentScoreNumber <= _desiredScoreNumber)
                {
                    _currentScoreNumber = _desiredScoreNumber;
                }
            }
            _pointsUIText.text = _currentScoreNumber.ToString("0");
        }

        if (_currentRemainingTimeNumber != _desiredRemainingTimeNumber)
        {
            if (_initialRemainingTimeNumber < _desiredRemainingTimeNumber)
            {
                _currentRemainingTimeNumber += (_animationTime * Time.deltaTime) * (_desiredRemainingTimeNumber - _initialRemainingTimeNumber);
                if (_currentRemainingTimeNumber >= _desiredRemainingTimeNumber)
                {
                    _currentRemainingTimeNumber = _desiredRemainingTimeNumber;
                }
            }
            else
            {
                _currentRemainingTimeNumber -= (_animationTime * Time.deltaTime) * (_initialRemainingTimeNumber - _desiredRemainingTimeNumber);
                if (_currentRemainingTimeNumber <= _desiredRemainingTimeNumber)
                {
                    _currentRemainingTimeNumber = _desiredRemainingTimeNumber;
                }
            }
            _timeRemainingUIText.text = _currentRemainingTimeNumber.ToString("0");
        }

        if (_currentStagesCompletedNumber != _desiredStagesCompletedNumber)
        {
            if (_initialStagesCompletedNumber < _desiredStagesCompletedNumber)
            {
                _currentStagesCompletedNumber += (_animationTime * Time.deltaTime) * (_desiredStagesCompletedNumber - _initialStagesCompletedNumber);
                if (_currentStagesCompletedNumber >= _desiredStagesCompletedNumber)
                {
                    _currentStagesCompletedNumber = _desiredStagesCompletedNumber;
                }
            }
            else
            {
                _currentStagesCompletedNumber -= (_animationTime * Time.deltaTime) * (_initialStagesCompletedNumber - _desiredStagesCompletedNumber);
                if (_currentStagesCompletedNumber <= _desiredStagesCompletedNumber)
                {
                    _currentStagesCompletedNumber = _desiredStagesCompletedNumber;
                }
            }
            _stagesCompletedUIText.text = _currentStagesCompletedNumber.ToString("0");
        }
    }

    IEnumerator closeWindowDelay()
    {
        yield return new WaitForSeconds(11.0f);
        animator.SetTrigger("AutoCloseTrigger");
    }
}
