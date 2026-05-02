using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace UnrealToUnity.Code.Scripts.Core.GameMode
{
    public class GameModeTest : GameMode
    {
        /// <summary>
        /// The name of the game mode.
        /// </summary>
        [SerializeField] private string gameModeName;

        /// <summary>
        /// The number of rounds in the game mode.
        /// </summary>
        [SerializeField] private int numberOfRounds = 3;

        private void Awake()
        {
            Debug.Log($"Initializing Game Mode: {gameModeName}");
        }

        private void Start()
        {
            // Log the game mode name and number of rounds.
            Debug.Log($"Game Mode: {gameModeName}, Number of Rounds: {numberOfRounds}");
        }
    }
}