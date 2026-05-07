using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace UnrealToUnity.Code.Scripts.Core.Pawns
{
    /// <summary>
    /// Just like Unreal Engine, a Pawn is an entity that can be possessed and controlled by a player or AI.
    /// It represents the physical embodiment of a character in the game world, and can be used to implement player characters, NPCs, and other entities that require movement and interaction.
    /// In Unreal Engine, Pawns are typically used in conjunction with Controllers, which handle the input and decision-making for the Pawn.
    /// In our Unity implementation, we can use a similar approach by creating a Pawn class that can be controlled by a PlayerController or AIController,
    /// allowing us to manage the behavior and interactions of our characters in a way that closely mimics Unreal Engine's architecture.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class Pawn : Actor
    {
        /// <summary>
        /// A reference to the controller currently controlling this pawn.
        /// Should not be edited directly; should only be set by controller class.
        /// </summary>
        [NonSerialized] internal Controller owningController;

        internal BoolTokenManager InputTokenManager { get; private set; } = new();

        private PlayerInput _playerInput;

        protected virtual void Awake()
        {
            // Set up the player input reference
            _playerInput = GetComponent<PlayerInput>();

            InputTokenManager.OnTokensChanged += InputTokenManagerOnOnTokensChanged;
        }

        private void OnDisable()
        {
            // Disable the player input
            AddInputBlocker(this);

            CustomOnDisable();
        }

        protected virtual void CustomOnDisable()
        {
        }

        private void OnEnable()
        {
            RemoveInputBlocker(this);

            CustomOnEnable();
        }

        protected virtual void CustomOnEnable()
        {
        }

        private void InputTokenManagerOnOnTokensChanged(bool hasTokens)
        {
            // If there are any tokens, disable input. Otherwise, enable it.
            _playerInput.enabled = !hasTokens;
        }

        internal void AddInputBlocker(object token)
        {
            InputTokenManager.AddToken(token);
        }

        internal void RemoveInputBlocker(object token)
        {
            InputTokenManager.RemoveToken(token);
        }
    }
}