using System;
using System.Collections.Generic;
using JesterGame.Code.Scripts.Dialogue.Data;
using NaughtyAttributes;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.GameMode;

namespace JesterGame.Code.Scripts.Core
{
    public class ImpostorGameMode : GameMode
    {
        public delegate void ProgressionEventHandler(ImpostorGameMode mode, int prevProgress, int currentProgress);

        #region Inspector Fields

        /// <summary>
        /// The number of interactions the player must go through until they reach the end of the game.
        /// </summary>
        [SerializeField, BoxGroup("Progression")]
        private int maxInteractionsProgression = 15;

        /// <summary>
        /// The number of interactions the player has currently gone through.
        /// </summary>
        [SerializeField, ReadOnly, ProgressBar("Current Interactions Progression", "maxInteractionsProgression"),
         BoxGroup("Progression")
        ]
        private int currentInteractionProgression;

        // [SerializeField] private Dictionary<DialogueCharacterAsset, CharacterInstance> characterInstanceMap;
        [SerializeField] private CharacterInstance characterInstanceTest;

        #endregion

        public event ProgressionEventHandler OnProgressionChanged;

        #region Functions

        #region Progress Functions

        public void SetProgress(int progress)
        {
            // If the new progress is out of bounds, return.
            if (progress < 0 || progress > maxInteractionsProgression)
                return;

            var prevProgress = currentInteractionProgression;
            currentInteractionProgression = progress;

            OnProgressionChanged?.Invoke(this, prevProgress, currentInteractionProgression);
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void IncrementProgress() => SetProgress(currentInteractionProgression + 1);

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void DecrementProgress() => SetProgress(currentInteractionProgression - 1);

        private void TestForGameFinished_Event(ImpostorGameMode mode, int prevProgress, int currentProgress)
        {
            if (currentProgress >= maxInteractionsProgression)
            {
                Debug.Log("Game finished!");

                // TODO: Add a function or something here to handle the game mode ending.

            }
        }

        private void LogProgress_Event(ImpostorGameMode mode, int prevProgress, int currentProgress)
        {
            Debug.Log($"Day progressed to {currentProgress} from {prevProgress}");
        }

        #endregion

        #endregion

        protected override void Awake()
        {
            base.Awake();

            // Bind to the progress event
            OnProgressionChanged += LogProgress_Event;
            OnProgressionChanged += TestForGameFinished_Event;
        }

        private void Start()
        {
            // Set the progress to 0
            SetProgress(0);
        }
    }
}