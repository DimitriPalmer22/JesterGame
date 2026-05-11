using JesterGame.Code.Scripts.Characters;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Player
{
    public class JesterPlayerPawn : JesterGamePawn
    {
        [SerializeField] private CinemachineCamera cinemachineCamera;

        [SerializeField, Min(0f)] private float moveSpeed = 5;
        [SerializeField, Min(0f)] private float rotationSpeed = 720;

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

            var camTrans = currentCamera ? currentCamera.transform : Camera.main!.transform;

            // Get the camera forward & right
            var camForward = camTrans.forward.NoYNormalized();
            var camRight = camTrans.right.NoYNormalized();

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

            animator?.SetFloat(APCurrentVelocity, value.magnitude);
        }

        private void SetMovementInput(Vector2 value)
        {
            _moveInput = value;
        }
    }
}