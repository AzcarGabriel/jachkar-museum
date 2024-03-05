using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    [Serializable]
    public class PlayerLook
    {
        [SerializeField] public Vector2 sensitivity;
        [SerializeField] public float clampX = 85.0f;

        private Vector2 _mouseDelta;
        public Transform playerCamera { private get;  set; }

        private float _rotationX;

        public void Update()
        {
            playerCamera.transform.Rotate(new Vector3(-_mouseDelta.y, _mouseDelta.x, 0) * Time.deltaTime);

            //_rotationX = -_mouseDelta.y;
            //_rotationX = Mathf.Clamp(_rotationX, -clampX, clampX);
            //Vector3 targetRotation = playerCamera.transform.eulerAngles;
            //targetRotation.x = _rotationX;
            //playerCamera.eulerAngles = targetRotation;
        }


        public void ReceiveInput(Vector2 mouseInput)
        {
            _mouseDelta = new Vector2(mouseInput.x * sensitivity.x, mouseInput.y * sensitivity.y);
        }


    }
}
