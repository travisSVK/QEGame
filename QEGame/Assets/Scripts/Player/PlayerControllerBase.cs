using UnityEngine;

public abstract class PlayerControllerBase : MonoBehaviour, ILocalConnection
{
    // This field should only be assigned to if the game is played on one computer, without networking.
    [SerializeField]
    private GameObject _otherPlayerController = null;
    [SerializeField]
    private GameObject _deathEffect = null;
    [SerializeField]
    private GameObject _winEffect = null;
    [SerializeField]
    private int _clientId = 0;

    [SerializeField]
    protected float _movementSpeed = 2.0f;
    protected bool _hasHitTarget = false;
    protected bool _hasOtherPlayerReachedGoal = false;
    protected Vector3 _input;

    private Vector3 _movementIncrement = Vector3.zero;

    // For local connections only
    protected ILocalConnection _localConnection = null;

    private Rigidbody _rigidBody;

    public int ClientId
    {
        get { return _clientId; }
    }

    public Vector3 movementIncrement
    {
        get { return _movementIncrement; }
        set { _movementIncrement = value; }
    }

    public Vector3 input
    {
        get { return _input; }
    }

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

    public void OnPlayerWin()
    {
        Instantiate(_winEffect, transform.position, transform.rotation, transform.parent);
    }

    public void OnPlayerDeath()
    {
        Server server = FindObjectOfType<Server>();
        if (server)
        {
            InstantiateDeath();
            server.RestartLevel();
        }
    }

    public void InstantiateDeath()
    {
        Instantiate(_deathEffect, transform.position, transform.rotation, transform.parent);
    }

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        Client client = FindObjectOfType<Client>();
        if (_rigidBody && client)
        {
            client.rb = _rigidBody;
        }

        if (_otherPlayerController)
        {
            _localConnection = _otherPlayerController.GetComponent(typeof(ILocalConnection)) as ILocalConnection;
        }
    }

    private void FixedUpdate()
    {
        _movementIncrement += _input * _movementSpeed * Time.fixedDeltaTime;
        _rigidBody.MovePosition(_rigidBody.position + _input * _movementSpeed * Time.fixedDeltaTime);
    }
}
