using UnityEngine;
using UnityEngine.UI;

// TODO Fade instead of instant transitions

public class TutorialLevelController : MonoBehaviour
{
    private readonly string[] STRINGS_ENG = {
        "Welcome!", // 0
        "This is you.", // 1
        "You can move LEFT and RIGHT.", // 2, P1
        "You just moved yourself AND the other player.", // 3, P1
        "Who did that?", // 4, P2
        "It was the other player.", // 5, P2
        "Your movements are synchronized.", // 6
        "You can move UP and DOWN.", // 7, P2
        "You just moved yourself AND the other player.", // 8, P2
        "You were just moved along with the other player.", // 9, P1
        "You two share control of your characters.", // 10
        "This is your goal.", // 11
        "But your partner is somewhere else, and you both have to reach your goals.", // 12
        "To reach your goal, you need to move right. Maybe the other player can help?", // 13, P2
        "You just hit a wall, and can now move upwards.", // 14, P2
        "You walked into the left wall for a while, but what happened for your partner?", // 15, P1
        "To reach the goal, you need to cooperate.", // 16
        "You've reached the goal! But what about your partner?", // 17, P1
        "Your partner has reached the goal!", // 18, P2
        "You win!" // 19
    };

    public bool isPlayerTwo;
    public GameObject terrain;
    public GameObject player;
    public GameObject goal;
    public Text centeredText, textByGoal, leftOrientedText;

    private bool isUpdating = false;

    private System.Collections.IEnumerator AsyncUpdate()
    {
        string[] strings = STRINGS_ENG;

        centeredText.text = strings[0];
        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        centeredText.enabled = false;
        centeredText.text = strings[1];
        centeredText.enabled = true;
        player.SetActive(true);

        if (!isPlayerTwo) // P1
        {
            while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
            centeredText.text = strings[2];

            // TODO Wait until P1 has moved LEFT or RIGHT instead
            while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);

            centeredText.text = "(After P1 moved)\n" + strings[3];
        }

        else // P2
        {
            // TODO Wait until P1 has moved LEFT or RIGHT instead
            while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);

            centeredText.text = "(After P1 moved)\n" + strings[4];
            while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
            centeredText.text = strings[5];
        }

        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        centeredText.text = strings[6];

        if (isPlayerTwo) // P2
        {
            while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
            centeredText.text = strings[7];

            // TODO Wait until P2 has moved UP or DOWN instead
            while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
            centeredText.text = strings[8];
        }

        else // P1
        {
            // TODO Wait until P2 has moved UP or DOWN instead
            while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
            centeredText.text = strings[9];
        }

        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        centeredText.text = strings[10];

        // TODO Move the player back to its original position, or move the camera and level to the player
        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        terrain.SetActive(true);
        goal.SetActive(true);
        centeredText.enabled = false;
        textByGoal.text = strings[11];
        textByGoal.enabled = true;

        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        textByGoal.enabled = false;
        leftOrientedText.text = strings[12];
        leftOrientedText.enabled = true;

        if (isPlayerTwo) // P2
        {
            while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
            leftOrientedText.text = strings[13];

            // TODO Wait until P1 has moved until P2 has hit the right wall
            while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
            leftOrientedText.text = strings[14];
        }

        else // P1
        {
            // TODO Wait until P1 has moved until P2 has hit the right wall
            while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
            leftOrientedText.text = strings[15];
        }

        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        leftOrientedText.text = strings[16];

        if (!isPlayerTwo) // P1
        {
            // TODO Wait until P1 has reached the goal
            while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
            leftOrientedText.text = strings[17];
        }

        else // P2
        {
            // TODO Wait until P1 has reached the goal
            while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
            leftOrientedText.text = strings[18];
        }

        // If P1 leaves the goal, show for P1: "You have left the goal. Remember to communicate!"

        // TODO Wait until P2 has also reached the goal
        while (!Input.GetKeyUp(KeyCode.Return)) { yield return null; } yield return new WaitForSeconds(0.1f);
        leftOrientedText.text = strings[19];
    }

    private void Start()
    {
        terrain.SetActive(false);
        player.SetActive(false);
        goal.SetActive(false);
        textByGoal.enabled = false;
        leftOrientedText.enabled = false;
    }

    private void FixedUpdate()
    {
        if (isUpdating) return;
        isUpdating = true;
        StartCoroutine(AsyncUpdate());
    }
}
