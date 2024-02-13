using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

[RequireComponent(typeof (CharacterController))]
[RequireComponent(typeof (AudioSource))]
public class FirstPersonController : NetworkBehaviour
{
    [FormerlySerializedAs("m_WalkSpeed")]
    [Header("Player settings")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] [Range(0f, 1f)] private float runStepLengthen;
    [SerializeField] private float stickToGroundForce;
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private bool useFovKick;
    [SerializeField] private float stepInterval;
    [SerializeField] private bool useHeadBob;
    
    [Header("Audio")]
    [SerializeField] private AudioClip[] footstepSounds;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip landSound;
    
    [Header("Components")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private MouseLook mouseLook;
    [SerializeField] private FOVKick fovKick = new();
    [SerializeField] private CurveControlledBob headBob = new();
    [SerializeField] private LerpControlledBob jumpBob = new();
    [SerializeField] private Animator animator;
    
    [Header("Network Components")]
    [SerializeField] private GameObject pingMarkPrefab;
    [SerializeField] private TMP_Text usernameTextField; 

   
    private bool _jump;
    private float _yRotation;
    private Vector3 _moveDirection = Vector3.zero;
    private CharacterController _characterController;
    private CollisionFlags _collisionFlags;
    private bool _previouslyGrounded;
    private Vector3 _originalCameraPosition;
    private float _stepCycle;
    private float _nextStep;
    private bool _jumping;
    private AudioSource _audioSource;
    private CharacterActions _playerActions;
    private Vector2 _moveInput;
    private float _speed;
    private bool _isWalking;

    #region NetworkFields
    private string _username;
    private static readonly int SpeedX = Animator.StringToHash("SpeedX");
    private static readonly int SpeedZ = Animator.StringToHash("SpeedZ");

    #endregion

    
    #region Initialization
    private void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        _playerActions = new CharacterActions();
        _characterController = GetComponent<CharacterController>();
        _audioSource = GetComponent<AudioSource>();
        _speed = walkSpeed;
        _stepCycle = 0f;
        _nextStep = _stepCycle/2f;
        _jumping = false;
    }
    
    private void OnEnable()
    {
        _playerActions.Enable();
        _playerActions.FirstPerson.Move.performed += OnMovePerformed;
        _playerActions.FirstPerson.Move.canceled += OnMoveCancelled;
        _playerActions.FirstPerson.Sprint.performed += OnSprintPerformed;
        _playerActions.FirstPerson.Sprint.canceled += OnSprintCancelled;
        _playerActions.FirstPerson.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        _playerActions.FirstPerson.Move.performed -= OnMovePerformed;
        _playerActions.FirstPerson.Move.canceled -= OnMoveCancelled;
        _playerActions.FirstPerson.Sprint.performed -= OnSprintPerformed;
        _playerActions.FirstPerson.Sprint.canceled -= OnSprintCancelled;
        _playerActions.FirstPerson.Jump.performed -= OnJump;
    }
    
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        _originalCameraPosition = playerCamera.transform.localPosition;
        fovKick.Setup(playerCamera);
        headBob.Setup(playerCamera, stepInterval);
        mouseLook.Init(transform , playerCamera.transform);
    }

    public override void OnNetworkSpawn()
    {
        _username = ServerManager.Instance.username;
    }

    // Update is called once per frame
    private void Update()
    {
        RotateView();
        
        if (StaticValues.should_lock) {
            StaticValues.should_lock = false;
            mouseLook.SetLocked(true);
        }
        if (!_previouslyGrounded && _characterController.isGrounded) OnLanding();
        
        if (!_characterController.isGrounded && !_jumping && _previouslyGrounded) _moveDirection.y = 0f;

        _previouslyGrounded = _characterController.isGrounded;

        if (!IsOwner) return;

        var cameraTransform = playerCamera.transform;
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Input.GetKeyDown("p")) {
            if (Physics.Raycast(ray, out RaycastHit hit)) {
                if (hit.collider != null) {
                    Vector3 spawnPosition = hit.point;
                    SpawnMarkServerRPC(spawnPosition);   
                }
            }
        }
    }
    #endregion
    
    private void PlayLandingSound()
    {
        _audioSource.clip = landSound;
        _audioSource.Play();
        _nextStep = _stepCycle + .5f;
    }

    private void OnLanding()
    {
        StartCoroutine(jumpBob.DoBobCycle());
        PlayLandingSound();
        _moveDirection.y = 0f;
        _jumping = false;
        animator.SetBool("Is Jumping", false);
    }
    
    private void FixedUpdate()
    {
        if (!IsOwner) return;
        // always move along the camera forward as it is the direction that it being aimed at
        var transform1 = transform;
        Vector3 desiredMove = transform1.forward*_moveInput.y + transform1.right*_moveInput.x;

        // get a normal for the surface that is being touched to move along it
        Physics.SphereCast(transform1.position, _characterController.radius, Vector3.down, out RaycastHit hitInfo,
            _characterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        _moveDirection.x = desiredMove.x*_speed;
        _moveDirection.z = desiredMove.z*_speed;


        if (_characterController.isGrounded)
        {
            _moveDirection.y = -stickToGroundForce;

            if (_jump)
            {
                _moveDirection.y = jumpSpeed;
                PlayJumpSound();
                _jump = false;
                _jumping = true;
                animator.SetBool("Is Jumping", true);
            }
        }
        else
        {
            _moveDirection += Physics.gravity * (gravityMultiplier * Time.fixedDeltaTime);
        }
        _collisionFlags = _characterController.Move(_moveDirection * Time.fixedDeltaTime);

        ProgressStepCycle();
        PerformHeadBob();
        animator.SetFloat("SpeedY", _moveDirection.y);
        ChangeNameTagServerRPC(_username); // This probably shouldn't be on update, but new users have to ask for the username when joining
    }
    
    private void PlayJumpSound()
    {
        _audioSource.clip = jumpSound;
        _audioSource.Play();
    }
    
    private void ProgressStepCycle()
    {
        if (_characterController.velocity.sqrMagnitude > 0 && (_moveInput.x != 0 || _moveInput.y != 0))
        {
            _stepCycle += (_characterController.velocity.magnitude 
                           + _speed * (_isWalking ? 1f : runStepLengthen))
                           * Time.fixedDeltaTime;
        }
        if (_stepCycle <= _nextStep) return;
        _nextStep = _stepCycle + stepInterval;
        PlayFootStepAudio();
    }
    
    private void PlayFootStepAudio()
    {
        if (!_characterController.isGrounded) return;
        // play random footstep excluding the one in index 0
        int n = Random.Range(1, footstepSounds.Length);
        _audioSource.clip = footstepSounds[n];
        _audioSource.PlayOneShot(_audioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        footstepSounds[n] = footstepSounds[0];
        footstepSounds[0] = _audioSource.clip;
    }
    
    private void PerformHeadBob()
    {
        if (!IsOwner || !useHeadBob) return;
        
        Vector3 newCameraPosition;

        if (_characterController.velocity.magnitude > 0 && _characterController.isGrounded)
        {
            playerCamera.transform.localPosition =
                headBob.DoHeadBob(_characterController.velocity.magnitude +
                                    _speed * (_isWalking ? 1f : runStepLengthen));
            newCameraPosition = playerCamera.transform.localPosition;
            newCameraPosition.y = playerCamera.transform.localPosition.y - jumpBob.Offset();
        }
        else
        {
            newCameraPosition = playerCamera.transform.localPosition;
            newCameraPosition.y = _originalCameraPosition.y - jumpBob.Offset();
        }
        playerCamera.transform.localPosition = newCameraPosition;
    }
    
    private void FOVKick()
    {
        if (!useFovKick || !(_characterController.velocity.sqrMagnitude > 0)) return;
        Debug.Log("Kicking");
        StopAllCoroutines();
        StartCoroutine(_isWalking ? fovKick.FOVKickDown() : fovKick.FOVKickUp());
    }
    
    private void RotateView()
    {
        mouseLook.LookRotation (transform, playerCamera.transform);
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        //dont move the rigidBody if the character is on top of it
        if (_collisionFlags == CollisionFlags.Below) return;
        if (body == null || body.isKinematic) return;
        body.AddForceAtPosition(_characterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }
    
    #region Input

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();
        if (_moveInput.sqrMagnitude > 1) _moveInput.Normalize();
        animator.SetFloat(SpeedX, _moveInput.x);
        animator.SetFloat(SpeedZ, _moveInput.y);
    }

    private void OnMoveCancelled(InputAction.CallbackContext ctx)
    {
        _moveInput = Vector2.zero;
    }

    private void OnSprintPerformed(InputAction.CallbackContext ctx)
    {
        _speed = runSpeed;
        _isWalking = false;
        FOVKick();
    }

    private void OnSprintCancelled(InputAction.CallbackContext ctx)
    {
        _speed = walkSpeed;
        _isWalking = true;
        FOVKick();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        _jump = true;
    }

    #endregion

    [ServerRpc(RequireOwnership = false)]
    public void SpawnMarkServerRPC(Vector3 position, ServerRpcParams serverRpcParams = default) 
    {
        string username = ServerManager.Instance.ClientData[serverRpcParams.Receive.SenderClientId].username;
        GameObject instantiatedPing = Instantiate(pingMarkPrefab, position, Quaternion.identity);
        instantiatedPing.GetComponent<MarkPing>().SetPlayerName(username);
        instantiatedPing.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId, true);
    }
    
    [ServerRpc]
    private void ChangeNameTagServerRPC(string username)
    {
        ChangeNameClientRPC(username);
    }

    [ClientRpc]
    private void ChangeNameClientRPC(string newUsername)
    {
        usernameTextField.text = newUsername;
    }
}