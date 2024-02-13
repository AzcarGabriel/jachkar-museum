using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class CharacterController : MonoBehaviour
    {
        [SerializeField] private float playerSpeed = 5f;
        [SerializeField] private float jumpForce = 30f;
        [SerializeField] private float playerRotationSpeed = 10f;
        [SerializeField] private Transform playerCamera;
        
        private CharacterActions _characterActions;
        private Vector3 _movementInput;
        private Vector2 _mouseDelta;
        private bool _isGrounded = true;
        private const float GroundedDistance = 1.3f;
        private Rigidbody _rigidBody;
        private float _rotation = 0f;
        
        #region SetupRegion
        private void Awake()
        { 
            _characterActions = new CharacterActions();
            _rigidBody = GetComponent<Rigidbody>();
        }
        
        private void OnEnable()
        {
            _characterActions.Enable();
            _characterActions.FirstPerson.Move.performed += PerformMovement;
            _characterActions.FirstPerson.Move.canceled += CancelMovement;
            _characterActions.FirstPerson.Point.performed += PerformCameraRotation;
            _characterActions.FirstPerson.Point.canceled += CancelCameraRotation;
            _characterActions.FirstPerson.Jump.performed += Jump;
            _characterActions.FirstPerson.Interact.performed += Interact;
            _characterActions.FirstPerson.Ping.performed += Mark;
        }

        private void OnDisable()
        {
            _characterActions.FirstPerson.Move.performed -= PerformMovement;
            _characterActions.FirstPerson.Move.canceled -= CancelMovement;
            _characterActions.FirstPerson.Point.performed += PerformCameraRotation;
            _characterActions.FirstPerson.Point.canceled += CancelCameraRotation;
            _characterActions.FirstPerson.Jump.performed -= Jump;
            _characterActions.FirstPerson.Interact.performed -= Interact;
            _characterActions.FirstPerson.Ping.performed -= Mark;
        }
        #endregion

        private void Update()
        {
            float distanceToTheGround = GetComponent<Collider>().bounds.extents.y;
            _isGrounded = Physics.Raycast(transform.position, Vector3.down, distanceToTheGround + 0.1f);
        }

        private void FixedUpdate()
        {
            Vector3 movement = transform.TransformDirection(_movementInput) * playerSpeed;
            movement.y = _rigidBody.velocity.y;
            _rigidBody.velocity = movement;
            _rotation += _mouseDelta.x * playerRotationSpeed;
            _rigidBody.rotation = Quaternion.Euler(0, _rotation, 0);
            
            Quaternion localRotation = playerCamera.localRotation;
            localRotation *= Quaternion.Euler(-_mouseDelta.y, 0, 0);
            localRotation = ClampRotationAroundXAxis(localRotation);
            playerCamera.localRotation = localRotation;
            
           
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

            angleX = Mathf.Clamp (angleX, -90f, 90f);

            q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

        #region InputRegion

        private void PerformMovement(InputAction.CallbackContext ctx)
        {
            Vector2 value = ctx.ReadValue<Vector2>();
            _movementInput.x = value.x;
            _movementInput.z = value.y;
        }

        private void CancelMovement(InputAction.CallbackContext ctx)
        {
            _movementInput.x = 0f;
            _movementInput.z = 0f;
        }

        private void PerformCameraRotation(InputAction.CallbackContext ctx)
        {
            _mouseDelta = ctx.ReadValue<Vector2>();
        }

        private void CancelCameraRotation(InputAction.CallbackContext ctx)
        {
            _mouseDelta = Vector2.zero;
        }

        private void Jump(InputAction.CallbackContext ctx)
        {
            if (_isGrounded)
            {
                _rigidBody.AddForce(new Vector3(0, jumpForce, 0));
            }
        }
        
        private void Interact(InputAction.CallbackContext ctx)
        {
            
        }

        private void Mark(InputAction.CallbackContext ctx)
        {
            
        }

        #endregion
    }
}
