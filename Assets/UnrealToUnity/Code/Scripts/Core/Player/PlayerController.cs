using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnrealToUnity.Code.Scripts.Core.Pawns;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace UnrealToUnity.Code.Scripts.Core.Player
{
    /// <summary>
    /// Use prefabs as fully-featured "blueprint classes".
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : Controller
    {
        [NonSerialized] private PlayerInput _playerInput;

        /// <summary>
        /// A bool token manager used to control whether input is available or not.
        /// </summary>
        private readonly BoolTokenManager _inputTokenManager = new();

        private void Awake()
        {
            // Set up the player input reference
            _playerInput = GetComponent<PlayerInput>();

            _inputTokenManager.OnTokensChanged += InputTokenManagerOnOnTokensChanged;
        }

        private void InputTokenManagerOnOnTokensChanged(bool hasTokens)
        {
            // If there are any tokens, disable input. Otherwise, enable it.
            _playerInput.enabled = !hasTokens;
            if (ControlledPawn)
            {
                if (hasTokens)
                    ControlledPawn.AddInputBlocker(this);
                else
                    ControlledPawn.RemoveInputBlocker(this);
            }
        }

        private void OnDisable()
        {
            // Disable the player input
            AddInputBlocker(this);
        }

        private void OnEnable()
        {
            RemoveInputBlocker(this);
        }

        public void AddInputBlocker(object token)
        {
            _inputTokenManager.AddToken(token);
        }

        public void RemoveInputBlocker(object token)
        {
            _inputTokenManager.RemoveToken(token);
        }

        public bool IsInputBlocked() => _inputTokenManager.HasTokens;

        protected override void CustomPossess(Pawn pawn)
        {
        }

        protected override void CustomUnPossess(Pawn pawn)
        {
            // Remove this as an input blocker from the pawn, since we are no longer controlling it.
            pawn?.RemoveInputBlocker(this);
        }
    }
}