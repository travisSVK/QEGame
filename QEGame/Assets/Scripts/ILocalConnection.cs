public interface ILocalConnection
{
    void MoveX(int direction, float deltaTime);
    void MoveZ(int direction, float deltaTime);
    void NotifyTargetHit();
    void NotifyDeath();
    void NotifyGoalReached(bool hasGoalBeenReached);
}
