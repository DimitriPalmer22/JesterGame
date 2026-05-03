using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.Testing
{
    public class GameModeTest : GameMode.GameMode
    {
        /// <summary>
        /// The name of the game mode.
        /// </summary>
        [SerializeField] private string gameModeName;

        /// <summary>
        /// The number of rounds in the game mode.
        /// </summary>
        [SerializeField] private int numberOfRounds = 3;

        private void Start()
        {
            // Log the game mode name and number of rounds.
            Debug.Log($"Game Mode: {gameModeName}, Number of Rounds: {numberOfRounds}");
        }
    }
}