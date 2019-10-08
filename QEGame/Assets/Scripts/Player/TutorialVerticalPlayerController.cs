using UnityEngine;
using UnityEngine.UI;

public class TutorialVerticalPlayerController : VerticalPlayerController
{
    [SerializeField] private GameObject _level;
    [SerializeField] private Text _centeredText, _textByGoal, _leftOrientedText;
    private bool _hasTutorialStarted = false;

    private System.Collections.IEnumerator AsyncUpdate()
    {
        bool isEnglish = true;
        var tmm = TutorialMessageManager.GetInstance();

        // Both players
        _centeredText.text = tmm.GetString(0, isEnglish);
        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        _centeredText.enabled = false;
        _centeredText.text = tmm.GetString(1, isEnglish);
        _centeredText.enabled = true;
        float currentX = transform.position.x;

        // P2
        while (transform.position.x == currentX) { yield return null; }
        _centeredText.text = tmm.GetString(4, isEnglish);
        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        _centeredText.text = tmm.GetString(5, isEnglish);

        // Both players
        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        _centeredText.text = tmm.GetString(6, isEnglish);

        // P2
        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        _centeredText.text = tmm.GetString(7, isEnglish);
        float currentZ = transform.position.z;
        while (transform.position.z == currentZ) { yield return null; }
        _centeredText.text = tmm.GetString(8, isEnglish);

        // Both players
        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        _centeredText.text = tmm.GetString(10, isEnglish);

        // Both players
        _level.transform.position += transform.position;
        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        _level.SetActive(true);
        _centeredText.enabled = false;
        _textByGoal.text = tmm.GetString(11, isEnglish);
        _textByGoal.enabled = true;

        // Both players
        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        _textByGoal.enabled = false;
        _leftOrientedText.text = tmm.GetString(12, isEnglish);
        _leftOrientedText.enabled = true;

        // P2
        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        _leftOrientedText.text = tmm.GetString(13, isEnglish);
        // TODO Wait until P1 has moved until P2 has hit the right wall
        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        _leftOrientedText.text = tmm.GetString(14, isEnglish);

        // Both players
        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        _leftOrientedText.text = tmm.GetString(16, isEnglish);

        // P2
        // TODO Wait until P1 has reached the goal
        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        _leftOrientedText.text = tmm.GetString(18, isEnglish);

        // If P1 leaves the goal, show for P1: "You have left the goal. Remember to communicate!"

        // Both players
        // TODO Wait until P2 has also reached the goal
        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        _leftOrientedText.text = tmm.GetString(19, isEnglish);
    }

    public override void Start()
    {
        base.Start();
        //_terrain.SetActive(false);
        //_goal.SetActive(false);
        _textByGoal.enabled = false;
        _leftOrientedText.enabled = false;
    }

    // Called when the other player changed its position.
    public override void SendDeltaXOrZ(float deltaX)
    {
        base.SendDeltaXOrZ(deltaX);
    }

    public override void Update()
    {
        base.Update();
        if (_hasTutorialStarted) return;
        _hasTutorialStarted = true;
        StartCoroutine(AsyncUpdate());
    }
}
