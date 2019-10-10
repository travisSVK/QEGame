using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialVerticalPlayerController : VerticalPlayerController
{
    [SerializeField] private GameObject _terrain, _goal;
    [SerializeField] private Text _centeredText, _textByGoal, _leftOrientedText;
    private bool _hasTutorialStarted = false;
    private Vector3 _startPosition;

    //private IEnumerator AsyncUpdate()
    //{
    //    bool isEnglish = true;
    //    var tmm = TutorialMessageManager.GetInstance();

    //    // (Both players)
    //    _centeredText.text = tmm.GetString(0, isEnglish);
    //    yield return Wait();
    //    _centeredText.enabled = false;
    //    _centeredText.text = tmm.GetString(1, isEnglish);
    //    _centeredText.enabled = true;
    //    float currentX = transform.position.x;

    //    while (transform.position.x == currentX) { yield return null; }
    //    _centeredText.text = tmm.GetString(4, isEnglish);
    //    yield return Wait();
    //    _centeredText.text = tmm.GetString(5, isEnglish);

    //    // (Both players)
    //    yield return Wait();
    //    _centeredText.text = tmm.GetString(6, isEnglish);

    //    yield return Wait();
    //    _centeredText.text = tmm.GetString(7, isEnglish);
    //    float currentZ = transform.position.z;
    //    while (transform.position.z == currentZ) { yield return null; }
    //    _centeredText.text = tmm.GetString(8, isEnglish);

    //    // (Both players)
    //    yield return Wait();
    //    _centeredText.text = tmm.GetString(10, isEnglish);

    //    // (Both players)
    //    yield return Wait();
    //    // TODO Fade out the player
    //    transform.position = _startPosition;
    //    // TODO Fade in everything, including the player
    //    _terrain.SetActive(true);
    //    _goal.SetActive(true);
    //    _centeredText.enabled = false;
    //    _textByGoal.text = tmm.GetString(11, isEnglish);
    //    _textByGoal.enabled = true;

    //    // (Both players)
    //    yield return Wait();
    //    _textByGoal.enabled = false;
    //    _leftOrientedText.text = tmm.GetString(12, isEnglish);
    //    _leftOrientedText.enabled = true;

    //    yield return Wait();
    //    _leftOrientedText.text = tmm.GetString(13, isEnglish);

    //    // Wait until P1 has moved so that you hit the right wall.
    //    while (transform.position.x < 2.8f) { yield return null; }
    //    if (_localConnection != null) { _localConnection.NotifyTargetHit(); }
    //    _leftOrientedText.text = tmm.GetString(14, isEnglish);

    //    // (Both players)
    //    yield return Wait();
    //    _leftOrientedText.text = tmm.GetString(16, isEnglish);

    //    while (!_hasOtherPlayerReachedGoal) { yield return null; }
    //    _leftOrientedText.text = tmm.GetString(18, isEnglish);

    //    // (Both players)
    //    while (!IsInGoal()) { yield return null; }
    //    if (_localConnection != null) { _localConnection.NotifyGoalReached(true); }
    //    _leftOrientedText.text = tmm.GetString(19, isEnglish);
    //}

    //private IEnumerator Wait()
    //{
    //    yield return new WaitForSeconds(3f);
    //}

    //private bool IsInGoal()
    //{
    //    return (transform.position.z >= 6.8f);
    //}

    //public override void Start()
    //{
    //    base.Start();
    //    _startPosition = transform.position;
    //    _terrain.SetActive(false);
    //    _goal.SetActive(false);
    //    _textByGoal.enabled = false;
    //    _leftOrientedText.enabled = false;
    //}

    //public override void Update()
    //{
    //    base.Update();
    //    if (_hasTutorialStarted) return;
    //    _hasTutorialStarted = true;
    //    StartCoroutine(AsyncUpdate());
    //}
}
