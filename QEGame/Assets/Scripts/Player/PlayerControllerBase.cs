using UnityEngine;

public abstract class PlayerControllerBase : MonoBehaviour, ILocalConnection
{
    // This field should only be assigned to if the game is played on one computer, without networking.
    [SerializeField] private GameObject _otherPlayerController;

    [SerializeField] protected float _movementSpeed = 2.0f;

    protected bool _hasHitTarget = false;
    protected bool _hasOtherPlayerReachedGoal = false;

    // For local connections only
    protected ILocalConnection _localConnection = null;

    private CharacterController _characterController;

    public float MovementSpeed
    {
        get { return _movementSpeed; }
        set { _movementSpeed = value; }
    }

    public virtual void MoveX(int direction, float deltaTime) // From 'ILocalConnection'
    {
        _characterController.Move(new Vector3(direction * _movementSpeed * deltaTime, 0.0f, 0.0f));
    }

    public virtual void MoveZ(int direction, float deltaTime) // From 'ILocalConnection'
    {
        _characterController.Move(new Vector3(0.0f, 0.0f, direction * _movementSpeed * deltaTime));
    }

    public void NotifyTargetHit() // From 'ILocalConnection'
    {
        _hasHitTarget = true;
    }

    // TODO Implement (by restarting the level)
    public void NotifyDeath() // From 'ILocalConnection'
    {
        Debug.Log("Death");
    }

    public void NotifyGoalReached(bool hasGoalBeenReached)  // From 'ILocalConnection'
    {
        _hasOtherPlayerReachedGoal = hasGoalBeenReached;
    }

    public virtual void Start()
    {
        _characterController = GetComponent<CharacterController>();

        if (_otherPlayerController != null)
        {
            _localConnection = _otherPlayerController.
                GetComponent(typeof(ILocalConnection)) as ILocalConnection;
        }
    }
}
