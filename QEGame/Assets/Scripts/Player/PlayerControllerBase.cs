using UnityEngine;

public abstract class PlayerControllerBase : MonoBehaviour, ILocalConnection
{
    // This field should only be assigned to if the game is played on one computer, without networking.
    [SerializeField] private GameObject _otherPlayerController;

    [SerializeField] protected float _movementSpeed = 5.0f;
    protected CharacterController _characterController;
    protected Vector2 _localOtherPlayerPos = Vector2.zero;
    protected ILocalConnection _localConnection = null;

    public float MovementSpeed
    {
        get { return _movementSpeed; }
        set { _movementSpeed = value; }
    }

    public abstract void SendDeltaXOrZ(float deltaXOrZ); // From 'ILocalConnection'

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
