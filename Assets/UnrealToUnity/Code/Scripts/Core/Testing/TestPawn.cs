using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnrealToUnity.Code.Scripts.Core.Pawns;

namespace UnrealToUnity.Code.Scripts.Core.Testing
{
    [RequireComponent(typeof(CharacterController))]
    public class TestPawn : Pawn
    {
        [SerializeField, Min(0f)] private float moveSpeed = 5;

        private CharacterController _characterController;

        private Vector2 _moveInput;

        protected override void Awake()
        {
            base.Awake();

            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            var deltaTime = Time.deltaTime;


            // Calculate the move delta of the pawn.
            var forwardMovement = transform.forward * _moveInput.y;
            var rightMovement = transform.right * _moveInput.x;
            var moveDelta = moveSpeed * (forwardMovement + rightMovement);

            // // Apply the movement to the pawn
            // transform.position += moveDelta;
            // _characterController.Move(moveDelta * deltaTime);
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