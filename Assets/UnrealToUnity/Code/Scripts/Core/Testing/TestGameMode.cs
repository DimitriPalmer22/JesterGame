using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.Testing
{
    public class TestGameMode : GameMode.GameMode
    {
        /// <summary>
        /// The name of the game mode.
        /// </summary>
        [SerializeField] private string gameModeName;

        /// <summary>
        /// The number of rounds in the game mode.
        /// </summary>
        [SerializeField] private int numberOfRounds = 3;

        protected override void Start()
        {
            base.Start();

            // Log the game mode name and number of rounds.
            Debug.Log($"Game Mode: {gameModeName}, Number of Rounds: {numberOfRounds}");
        }

        public void StartGameMode()
        {
            Debug.Log($"Starting game mode: {gameModeName}");
        }

        public void EndGameMode()
        {
            Debug.Log($"Ending game mode: {gameModeName}");
        }
    }
}