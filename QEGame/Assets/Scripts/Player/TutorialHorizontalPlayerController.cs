using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHorizontalPlayerController : HorizontalPlayerController
{
    [SerializeField] private GameObject _terrain, _goal;
    [SerializeField] private Text _centeredText, _textByGoal, _leftOrientedText;
    private bool _hasTutorialStarted = false;
    private Vector3 _startPosition;

    private IEnumerator AsyncUpdate()
    {
        bool isEnglish = true;
        var tmm = TutorialMessageManager.GetInstance();

        // (Both players)
        _centeredText.text = tmm.GetString(0, isEnglish);
        yield return Wait();
        _centeredText.enabled = false;
        _centeredText.text = tmm.GetString(1, isEnglish);
        _centeredText.enabled = true;

        yield return Wait();
        _centeredText.text = tmm.GetString(2, isEnglish);
        float currentX = transform.position.x;
        while (transform.position.x == currentX) { yield return null; }
        _centeredText.text = tmm.GetString(3, isEnglish);

        // (Both players)
        yield return Wait();
        _centeredText.text = tmm.GetString(6, isEnglish);
        float currentZ = transform.position.z;

        while (transform.position.z == currentZ) { yield return null; }
        _centeredText.text = tmm.GetString(9, isEnglish);

        // (Both players)
        yield return Wait();
        _centeredText.text = tmm.GetString(10, isEnglish);

        // (Both players)
        yield return Wait();
        // TODO Fade out the player
        transform.position = _startPosition;
        // TODO Fade in everything, including the player
        _terrain.SetActive(true);
        _goal.SetActive(true);
        _centeredText.enabled = false;
        _textByGoal.text = tmm.GetString(11, isEnglish);
        _textByGoal.enabled = true;

        // (Both players)
        yield return Wait();
        _textByGoal.enabled = false;
        _leftOrientedText.text = tmm.GetString(12, isEnglish);
        _leftOrientedText.enabled = true;

        // Wait until you have moved so that P2 hits the right wall.
        while (!_hasHitTarget) { yield return null; }
        _hasHitTarget = false;
        _leftOrientedText.text = tmm.GetString(15, isEnglish);

        // (Both players)
        yield return Wait();
        _leftOrientedText.text = tmm.GetString(16, isEnglish);

        // Wait until you have reached your goal.
        while (!IsInGoal()) { yield return null; }
        if (_localConnection != null) { _localConnection.NotifyGoalReached(true); }
        _leftOrientedText.text = tmm.GetString(17, isEnglish);

        bool wasPlayerInGoal = true;
        while (!_hasOtherPlayerReachedGoal)
        {
            // If you were in the goal...
            if (wasPlayerInGoal)
            {
                // ... but have now left, notify the other player and change the text.
                if (!IsInGoal())
                {
                    wasPlayerInGoal = false;
                    if (_localConnection != null) { _localConnection.NotifyGoalReached(false); }
                    _leftOrientedText.text = tmm.GetString(20, isEnglish);
                }
            }

            // If you had left the goal...
            else
            {
                // ... but have now reached it again, notify the other player and change the text.
                if (IsInGoal())
                {
                    wasPlayerInGoal = true;
                    _leftOrientedText.text = tmm.GetString(17, isEnglish);
                    if (_localConnection != null) { _localConnection.NotifyGoalReached(true); }
                }
            }

            yield return null;
        }

        // (Both players)
        _leftOrientedText.text = tmm.GetString(19, isEnglish);
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(3f);
    }

    private bool IsInGoal()
    {
        return (
            transform.position.x >= 4.8f &&
            transform.position.x <= 5.2f &&
            transform.position.z >= 0.6f &&
            transform.position.z <= 1.2f
        );
    }

    public override void Start()
    {
        base.Start();
        _startPosition = transform.position;
        _terrain.SetActive(false);
        _goal.SetActive(false);
        _textByGoal.enabled = false;
        _leftOrientedText.enabled = false;
    }

    public override void Update()
    {
        base.Update();
        if (_hasTutorialStarted) return;
        _hasTutorialStarted = true;
        StartCoroutine(AsyncUpdate());
    }
}
