public class TutorialMessageManager
{
    public readonly string[] _stringsEnglish = {
        "Welcome!", // 0
        "This is you.", // 1
        "You can move LEFT and RIGHT.", // 2, P1
        "You just moved yourself AND the other player.", // 3, P1
        "Who moved you?", // 4, P2
        "It was the other player.", // 5, P2
        "Your movements are synchronized.", // 6
        "You can move UP and DOWN.", // 7, P2
        "You just moved yourself AND the other player.", // 8, P2
        "You were just moved along with the other player.", // 9, P1
        "You two share control of your characters.", // 10
        "This is your goal.", // 11
        "But your friend is somewhere else, and you both have to reach your goals.", // 12
        "To reach your goal, you need to move right. Maybe the other player can help?", // 13, P2
        "You just hit a wall, and can now move upwards.", // 14, P2
        "You walked into the left wall for a while, but what happened for your friend?", // 15, P1
        "To reach the goal, you need to cooperate.", // 16
        "You've reached the goal! But what about your friend?", // 17, P1
        "Your friend has reached the goal!", // 18, P2
        "You win!", // 19
        "You have left the goal. Remember to communicate!" // 20, P1
    };

    public readonly string[] _stringsSwedish = {
        "Välkommen!", // 0
        "Detta är du.", // 1
        "Du kan röra dig till VÄNSTER och HÖGER.", // 2, P1
        "Du flyttade just dig själv OCH den andra spelaren.", // 3, P1
        "Vem flyttade dig?", // 4, P2
        "Det var den andra spelaren.", // 5, P2
        "Era rörelser är synkroniserade.", // 6
        "Du kan röra dig UPP och NER.", // 7, P2
        "Du flyttade just dig själv och den andra spelaren.", // 8, P2
        "Du flyttades just tillsammans med den andra spelaren.", // 9, P1
        "Ni två delar kontroll över era karaktärer.", // 10
        "Detta är ditt mål.", // 11
        "Men din vän är någon annan stans, och ni måste båda nå era mål.", // 12
        "För att nå ditt mål måste du gå åt höger. Din vän kanske kan hjälpa till?", // 13, P2
        "Du gick just in i en vägg, och kan nu röra dig uppåt.", // 14, P2
        "Du gick in i den vänstra väggen ett tag, men vad hände med din vän?", // 15, P1
        "För att nå målet måste ni samarbeta.", // 16
        "Du har nått målet! Men hur går det för din vän?", // 17, P1
        "Din vän har också nått målet!", // 18, P2
        "Ni vann!", // 19
        "Du har lämnat målet. Kom ihåg att kommunicera!" // 20, P1
    };

    private static readonly TutorialMessageManager _instance = new TutorialMessageManager();

    private TutorialMessageManager() {}

    public static TutorialMessageManager GetInstance() { return _instance; }

    public string GetString(uint index, bool isEnglish)
    {
        switch (isEnglish)
        {
            case true: return _stringsEnglish[index];
            default: return _stringsSwedish[index];
        }
    }
}
