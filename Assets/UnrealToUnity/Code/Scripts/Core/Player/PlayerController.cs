using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnrealToUnity.Code.Scripts.Core.Pawns;

namespace UnrealToUnity.Code.Scripts.Core.Player
{
    /// <summary>
    /// Use prefabs as fully-featured "blueprint classes".
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : Controller
    {
        [NonSerialized] private PlayerInput playerInput;

        private void Awake()
        {
            // Set up the player input reference
            playerInput = GetComponent<PlayerInput>();
        }

        private void OnDisable()
        {
            // Disable the player input
            playerInput.enabled = false;
        }

        private void OnEnable()
        {
            playerInput.enabled = true;
        }
    }
}