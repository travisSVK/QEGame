using UnityEngine;

public abstract class PlayerControllerBase : MonoBehaviour, ILocalConnection
{
    // This field should only be assigned to if the game is played on one computer, without networking.
    [SerializeField]
    private GameObject _otherPlayerController;

    [SerializeField]
    protected float _movementSpeed = 2.0f;
    protected bool _hasHitTarget = false;
    protected bool _hasOtherPlayerReachedGoal = false;
    protected Vector3 _input;

    // For local connections only
    protected ILocalConnection _localConnection = null;

    private Rigidbody _rigidBody;

    public float MovementSpeed
    {
        get { return _movementSpeed; }
        set { _movementSpeed = value; }
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

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();

        if (_otherPlayerController)
        {
            _localConnection = _otherPlayerController.GetComponent(typeof(ILocalConnection)) as ILocalConnection;
        }
    }

    private void FixedUpdate()
    {
        _rigidBody.MovePosition(_rigidBody.position + _input * _movementSpeed * Time.fixedDeltaTime);
    }
}
