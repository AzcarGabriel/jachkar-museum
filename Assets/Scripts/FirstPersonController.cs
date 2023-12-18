using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;
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
    [SerializeField] [Range(0f, 1f)] private float runstepLenghten;
    [SerializeField] private float stickToGroundForce;
    [SerializeField] private float gravityMultiplier;
    [SerializeField] private bool useFovKick;
    [SerializeField] private float stepInterval;
    [SerializeField] private bool useHeadBob;
    [SerializeField] private bool isWalking;
        
    [Header("Audio")]
    [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
    [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.
        
    [Header("Components")]
    [SerializeField] private Camera m_Camera;
    [SerializeField] private MouseLook m_MouseLook;
    [SerializeField] private FOVKick m_FovKick = new FOVKick();
    [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
    [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
    [SerializeField] private Animator animator;
    
    
    [Header("Network Components")]
    [SerializeField] private GameObject pingMarkPrefab;
    [SerializeField] private TMP_Text usernameTextField; 

   
    private bool jump;
    private float yRotation;
    private Vector2 input;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;
    private CollisionFlags collisionFlags;
    private bool previouslyGrounded;
    private Vector3 originalCameraPosition;
    private float stepCycle;
    private float nextStep;
    private bool jumping;
    private AudioSource audioSource;

    #region NetworkFields
    private string _username;
    #endregion

    // Use this for initialization
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        characterController = GetComponent<CharacterController>();
        m_Camera = GetComponentInChildren<Camera>();
        originalCameraPosition = m_Camera.transform.localPosition;
        m_FovKick.Setup(m_Camera);
        m_HeadBob.Setup(m_Camera, stepInterval);
        stepCycle = 0f;
        nextStep = stepCycle/2f;
        jumping = false;
        audioSource = GetComponent<AudioSource>();
        m_MouseLook.Init(transform , m_Camera.transform);
    }


    // Update is called once per frame
    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null) return;
        RotateView();

        if (StaticValues.should_lock) {
            StaticValues.should_lock = false;
            m_MouseLook.SetLocked(true);
            Debug.Log("Locking...");
        }
        // the jump state needs to read here to make sure it is not missed
        if (!jump)
        {
            jump = CrossPlatformInputManager.GetButtonDown("Jump");
        }

        if (!previouslyGrounded && characterController.isGrounded)
        {
            StartCoroutine(m_JumpBob.DoBobCycle());
            PlayLandingSound();
            moveDirection.y = 0f;
            jumping = false;
            animator.SetBool("Is Jumping", false);
        }
        if (!characterController.isGrounded && !jumping && previouslyGrounded)
        {
            moveDirection.y = 0f;
        }

        previouslyGrounded = characterController.isGrounded;

        if (!IsOwner) return;


        Transform camera = Camera.main.transform;

        Ray ray = new Ray(camera.position, camera.forward);
        RaycastHit hit;

        if (Input.GetKeyDown("p")) {
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider != null) {
                    Vector3 spawnPosition = hit.point;
                    SpawnMarkServerRPC(spawnPosition);   
                }
            }
        }
    }


    private void PlayLandingSound()
    {
        audioSource.clip = m_LandSound;
        audioSource.Play();
        nextStep = stepCycle + .5f;
    }


    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (EventSystem.current.currentSelectedGameObject != null) return;

        GetInput(out float speed);
        // always move along the camera forward as it is the direction that it being aimed at
        Vector3 desiredMove = transform.forward*input.y + transform.right*input.x;

        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo,
            characterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        moveDirection.x = desiredMove.x*speed;
        moveDirection.z = desiredMove.z*speed;


        if (characterController.isGrounded)
        {
            moveDirection.y = -stickToGroundForce;

            if (jump)
            {
                moveDirection.y = jumpSpeed;
                PlayJumpSound();
                jump = false;
                jumping = true;
                animator.SetBool("Is Jumping", true);
            }
        }
        else
        {
            moveDirection += Physics.gravity * (gravityMultiplier * Time.fixedDeltaTime);
        }
        collisionFlags = characterController.Move(moveDirection*Time.fixedDeltaTime);

        ProgressStepCycle(speed);
        UpdateCameraPosition(speed);
        animator.SetFloat("SpeedY", moveDirection.y);
        ChangeNameTagServerRPC(); // This probably shouldnt be on update, but new users have to ask for the username when joining
    }


    private void PlayJumpSound()
    {
        audioSource.clip = m_JumpSound;
        audioSource.Play();
    }


    private void ProgressStepCycle(float speed)
    {
        if (characterController.velocity.sqrMagnitude > 0 && (input.x != 0 || input.y != 0))
        {
            stepCycle += (characterController.velocity.magnitude + (speed*(isWalking ? 1f : runstepLenghten)))*
                           Time.fixedDeltaTime;
        }

        if (!(stepCycle > nextStep))
        {
            return;
        }

        nextStep = stepCycle + stepInterval;

        PlayFootStepAudio();
    }


    private void PlayFootStepAudio()
    {
        if (!characterController.isGrounded)
        {
            return;
        }
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, m_FootstepSounds.Length);
        audioSource.clip = m_FootstepSounds[n];
        audioSource.PlayOneShot(audioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        m_FootstepSounds[n] = m_FootstepSounds[0];
        m_FootstepSounds[0] = audioSource.clip;
    }


    private void UpdateCameraPosition(float speed)
    {
        if (!IsOwner) return;
        Vector3 newCameraPosition;
        if (!useHeadBob)
        {
            return;
        }
        if (characterController.velocity.magnitude > 0 && characterController.isGrounded)
        {
            m_Camera.transform.localPosition =
                m_HeadBob.DoHeadBob(characterController.velocity.magnitude +
                                    (speed*(isWalking ? 1f : runstepLenghten)));
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
        }
        else
        {
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = originalCameraPosition.y - m_JumpBob.Offset();
        }
        m_Camera.transform.localPosition = newCameraPosition;
    }


    private void GetInput(out float speed)
    {
        // Read input
        float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        float vertical = CrossPlatformInputManager.GetAxis("Vertical");

        bool waswalking = isWalking;

#if !MOBILE_INPUT
        // On standalone builds, walk/run speed is modified by a key press.
        // keep track of whether or not the character is walking or running
        isWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
        // set the desired speed to be walking or running
        speed = isWalking ? walkSpeed : runSpeed;
        input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (input.sqrMagnitude > 1)
        {
            input.Normalize();
        }

        // handle speed change to give an fov kick
        // only if the player is going to a run, is running and the fovkick is to be used
        if (isWalking != waswalking && useFovKick && characterController.velocity.sqrMagnitude > 0)
        {
            StopAllCoroutines();
            StartCoroutine(!isWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
        }

        animator.SetFloat("SpeedX", input.x);
        animator.SetFloat("SpeedZ", input.y);
    }


    private void RotateView()
    {
        m_MouseLook.LookRotation (transform, m_Camera.transform);
    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        //dont move the rigidbody if the character is on top of it
        if (collisionFlags == CollisionFlags.Below)
        {
            return;
        }

        if (body == null || body.isKinematic)
        {
            return;
        }
        body.AddForceAtPosition(characterController.velocity*0.1f, hit.point, ForceMode.Impulse);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnMarkServerRPC(Vector3 position, ServerRpcParams serverRpcParams = default) 
    {
        string username = ServerManager.Instance.ClientData[serverRpcParams.Receive.SenderClientId].username;
        GameObject instantiatedPing = Instantiate(pingMarkPrefab, position, Quaternion.identity);
        instantiatedPing.GetComponent<MarkPing>().SetPlayerName(username);
        instantiatedPing.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId, true);
    }

    [ServerRpc]
    private void ChangeNameTagServerRPC(ServerRpcParams serverRpcParams = default)
    {
        string username = ServerManager.Instance.ClientData[serverRpcParams.Receive.SenderClientId].username;
        ChangeNameClientRPC(username);
    }

    [ClientRpc]
    private void ChangeNameClientRPC(string newUsername)
    {
        usernameTextField.text = newUsername;
    }
}