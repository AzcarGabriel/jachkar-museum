using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class TopViewController : MonoBehaviour
    {
        [SerializeField] private float _horizontalSpeed;
        
        private Camera _playerCamera;
        private CharacterActions _playerActions;
        private Vector3 _moveInput;
        
        #region Initialization
        private void Awake()
        {
            _playerCamera = GetComponent<Camera>();
            _playerActions = new CharacterActions();
        }
    
        private void OnEnable()
        {
            _playerActions.Enable();
            _playerActions.TopCamera.Move.performed += OnMovePerformed;
            _playerActions.TopCamera.Move.canceled += OnMoveCancelled;

        }

        private void OnDisable()
        {
            _playerActions.TopCamera.Move.performed -= OnMovePerformed;
            _playerActions.TopCamera.Move.canceled -= OnMoveCancelled;
        }

        #endregion
        
        #region Input
        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            _moveInput.x = ctx.ReadValue<Vector2>().x;
            _moveInput.z = ctx.ReadValue<Vector2>().y;
        }
        private void OnMoveCancelled(InputAction.CallbackContext ctx)
        {
            _moveInput = Vector2.zero;
        }
        
        #endregion

        private void Update()
        {
            transform.Translate(_moveInput * _horizontalSpeed);
        }
    }
    
}
