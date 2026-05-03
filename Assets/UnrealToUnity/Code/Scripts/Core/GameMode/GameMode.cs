using System.Collections.Generic;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Pawns;

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

        [SerializeField] public GameModePrefabs gameModePrefabs;

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
                    currentPawn = Instantiate(gameModePrefabs.pawnPrefab, null);

                // Possess if the pawn and controller were both instantiated.
                if (playerController && currentPawn)
                    playerController.Possess(currentPawn);
            }
        }

        protected virtual Transform GetPlayerStartTransform()
        {
            // TODO: make a player start functionality to determine where pawns start.
            return transform;
        }
    }
}