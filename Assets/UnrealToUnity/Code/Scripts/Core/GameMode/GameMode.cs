using System;
using System.Collections.Generic;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Pawns;
using UnrealToUnity.Code.Scripts.Core.Player;
using UnrealToUnity.Code.Scripts.Core.Subsystems;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace UnrealToUnity.Code.Scripts.Core.GameMode
{
    /// <summary>
    /// Monobehaviour class that represents the game mode.
    /// Supposed to mimic behavior of Unreal Engine's GameMode class, which is responsible for managing the rules and flow of the game.
    /// Since this class *needs* to attach to a GameObject, we can use it to manage the lifecycle of the game mode,
    /// and also to store any data that needs to persist across scenes.
    /// We can also abstractly define any components that *also* need to be on the game mode.
    /// </summary>
    public abstract class GameMode : MonoBehaviour
    {
        #region Setup

        [SerializeField] protected GameModePrefabs gameModePrefabs;

        #endregion

        #region Runtime Fields

        /// <summary>
        /// The list of active player controllers.
        /// </summary>
        private readonly List<PlayerController> _playerControllers = new();

        #endregion

        protected virtual void Awake()
        {
            // Instantiate the game mode prefabs.
            InstantiateGameModePrefabs();
        }

        private void OnEnable()
        {
            // Get the game instance subsystem
            // Set the current game mode in the subsystem
            if (UtilLibrary.GetSubsystem(out GameInstanceSubsystem gameInstanceSubsystem))
                gameInstanceSubsystem.CurrentGameMode = this;
        }

        private void InstantiateGameModePrefabs()
        {
            // Create the player controllers & pawns
            for (var i = 0; i < gameModePrefabs.numberOfPlayers; i++)
            {
                PlayerController playerController = null;

                if (gameModePrefabs.playerControllerPrefab)
                {
                    playerController = Instantiate(gameModePrefabs.playerControllerPrefab, null);
                    _playerControllers.Add(playerController);
                }

                // Also spawn the pawns as well.
                Pawn currentPawn = null;
                if (gameModePrefabs.pawnPrefab)
                {
                    currentPawn = Instantiate(gameModePrefabs.pawnPrefab, null);

                    // Get the player start & move the pawn there
                    var playerStart = GetPlayerStart();
                    if (playerStart)
                    {
                        var spawnTransform = playerStart.GetSpawnTransform();
                        currentPawn.transform.position = spawnTransform.position;
                        currentPawn.transform.forward = spawnTransform.forward;
                    }
                    else
                    {
                        currentPawn.transform.position = Vector3.zero;
                        currentPawn.transform.forward = Vector3.forward;
                    }
                }

                // Possess if the pawn and controller were both instantiated.
                if (playerController && currentPawn)
                    playerController.Possess(currentPawn);

                // Instantiate the cheat menu prefab if possible
                if (gameModePrefabs.cheatMenuPrefab && playerController)
                {
                    var cheatMenu = Instantiate(gameModePrefabs.cheatMenuPrefab, null);
                    cheatMenu.Initialize(playerController);
                }
            }
        }

        private PlayerStart[] GetAllPlayerStarts()
        {
            // Find all the player starts within the scene.
            // var allStarts = FindObjectsByType<PlayerStart>(FindObjectsInactive.Exclude);
            // return allStarts.Where(playerStart => playerStart && playerStart.enabled).ToArray();

            var allPlayerStarts = FindObjectsByType<PlayerStart>(FindObjectsInactive.Exclude);

            return allPlayerStarts;
        }

        protected virtual PlayerStart GetPlayerStart()
        {
            var allStarts = GetAllPlayerStarts();
            if (allStarts.Length == 0)
                return null;

            // Use the first one as the chosen player start.
            return allStarts[0];
        }

        public PlayerController GetPlayerController(int index)
        {
            if (index >= 0 && index < _playerControllers.Count)
                return _playerControllers[index];

            Debug.LogError($"Player controller index `{index}` is out of range.");
            return null;
        }
    }
}