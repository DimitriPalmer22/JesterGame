using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnrealToUnity.Code.Scripts.Core.Pawns;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace UnrealToUnity.Code.Scripts.Core.Testing
{
    [RequireComponent(typeof(CharacterController))]
    public class TestPawn : Pawn
    {
        [SerializeField] private CinemachineCamera cinemachineCamera;

        [SerializeField, Min(0f)] private float moveSpeed = 5;
        [SerializeField, Min(0f)] private float rotationSpeed = 360;

        private CharacterController _characterController;

        private Vector2 _moveInput;

        protected override void Awake()
        {
            base.Awake();

            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            // Get the current camera (if any)
            var currentCamera = cinemachineCamera;

            // Get the camera forward & right
            var camForward = (currentCamera ? currentCamera.transform.forward : transform.forward).NoYNormalized();
            var camRight = (currentCamera ? currentCamera.transform.right : transform.right).NoYNormalized();

            // Calculate the move delta of the pawn.
            var forwardMovement = camForward * _moveInput.y;
            var rightMovement = camRight * _moveInput.x;
            var moveDelta = moveSpeed * (forwardMovement + rightMovement);

            // Rotate only when moving.
            if (!Mathf.Approximately(moveDelta.sqrMagnitude, 0f))
            {
                // Get the current rotation of the pawn
                // Get the desired rotation
                var currentRotation = transform.rotation;
                var desiredRotation = Quaternion.LookRotation(moveDelta, Vector3.up);

                // Rotate towards the desired rotation at a constant speed
                var calculatedRotation =
                    Quaternion.RotateTowards(currentRotation, desiredRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = calculatedRotation;
            }
            // Apply the movement to the pawn
            _characterController.SimpleMove(moveDelta);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            SetMovementInput(value);
        }

        private void SetMovementInput(Vector2 value)
        {
            _moveInput = value;
        }
    }
}