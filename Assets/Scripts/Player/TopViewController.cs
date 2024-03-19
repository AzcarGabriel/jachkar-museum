using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class TopViewController : MonoBehaviour
    {
        [SerializeField] private float horizontalSpeed;
        [SerializeField] private float zoomSpeed;
        
        private Camera _playerCamera;
        private CharacterActions _playerActions;
        private Vector3 _moveInput;
        private Vector2 _mouseLook;
        private float _sizeInput;
        //private SelectionScript _selector;
        
        public FirstPersonController FirstPersonController { set; private get; }
        public PlayerLook PlayerLook { set; private get; }


        #region UIEvents

        public delegate void OnEditorOpen();

        public delegate void OnEditorClose();

        public static event OnEditorOpen EditorOpenEvent;

        public static event OnEditorClose EditorCloseEvent;
        

        #endregion
        
        #region Initialization
        private void Awake()
        {
            _playerCamera = GetComponent<Camera>();
            _playerActions = new CharacterActions();
            //_selector = gameObject.AddComponent<SelectionScript>();
            //_selector.topCamera = _playerCamera;
            StaticValues.TopCamera = _playerCamera;
            DontDestroyOnLoad(this);
        }

        public void Init(PlayerLook playerLook)
        {
            PlayerLook = playerLook;
            PlayerLook.playerCamera = _playerCamera.transform;
        }
    
        private void OnEnable()
        {
            EditorOpenEvent?.Invoke();
            _playerCamera.enabled = true;
            _playerActions.Enable();
            _playerActions.TopCamera.Move.performed += OnMovePerformed;
            _playerActions.TopCamera.Move.canceled += OnMoveCancelled;
            _playerActions.TopCamera.SwitchCamera.performed += SwitchCamera;
            _playerActions.TopCamera.Float.performed += FloatPerformed;
            _playerActions.TopCamera.Float.canceled += FloatCancelled;
            _playerActions.TopCamera.SwitchMode.performed += SwitchMode;
            _playerActions.TopCamera.Look.performed += ctx => _mouseLook = ctx.ReadValue<Vector2>();
            _playerActions.TopCamera.Look.canceled += _ => _mouseLook = Vector2.zero;
            _playerActions.TopCamera.Select.performed += OnMouseSelect;
            Cursor.lockState = CursorLockMode.None;
        }

        private void OnDisable()
        {
            EditorCloseEvent?.Invoke();
            _playerCamera.enabled = false;
            _playerActions.Disable();
            _playerActions.TopCamera.Move.performed -= OnMovePerformed;
            _playerActions.TopCamera.Move.canceled -= OnMoveCancelled;
            _playerActions.TopCamera.SwitchCamera.performed -= SwitchCamera;
            _playerActions.TopCamera.Float.performed -= FloatPerformed;
            _playerActions.TopCamera.Float.canceled -= FloatCancelled;
            _playerActions.TopCamera.SwitchMode.performed -= SwitchMode;
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
            _moveInput.x = 0;
            _moveInput.z = 0;
        }

        private void FloatPerformed(InputAction.CallbackContext ctx)
        {
            if (!_playerCamera.orthographic)
                _moveInput.y = ctx.ReadValue<float>();
            else
               _sizeInput = ctx.ReadValue<float>();
        }
        
        private void FloatCancelled(InputAction.CallbackContext ctx)
        {
            if (!_playerCamera.orthographic)
                _moveInput.y = 0;
            else
                _sizeInput = 0;
        }

        private void SwitchCamera(InputAction.CallbackContext ctx)
        {
            this.enabled = false;
            FirstPersonController.enabled = true;
        }

        private void SwitchMode(InputAction.CallbackContext ctx)
        {
            _playerCamera.orthographic = !_playerCamera.orthographic;
        }

        private void OnMouseSelect(InputAction.CallbackContext ctx)
        {
            //_selector.OnStoneSelect();
        }
        
        
        #endregion

        private void Update()
        {
            transform.Translate(_moveInput * horizontalSpeed, Space.World);

            if (_playerCamera.orthographic)
            {
                _playerCamera.orthographicSize += _sizeInput * zoomSpeed * Time.deltaTime;
            }
            else
            {
                PlayerLook.ReceiveInput(_mouseLook);
                PlayerLook.Update();
            }
        }
    }
    
}
