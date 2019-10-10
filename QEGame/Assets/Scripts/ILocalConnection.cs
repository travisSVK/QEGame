public interface ILocalConnection
{
    void NotifyTargetHit();
    void NotifyDeath();
    void NotifyGoalReached(bool hasGoalBeenReached);
}
