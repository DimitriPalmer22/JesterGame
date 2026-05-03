using System;
using UnityEngine;
using UnityEngine.InputSystem;

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

        [NonSerialized] private PlayerInput _playerInput;

        protected virtual void Awake()
        {
            // Set up the player input reference
            _playerInput = GetComponent<PlayerInput>();
        }

        private void OnDisable()
        {
            // Disable the player input
            _playerInput.enabled = false;
        }

        private void OnEnable()
        {
            _playerInput.enabled = true;
        }
    }
}