using System;
using System.Collections.Generic;
using JesterGame.Code.Scripts.Dialogue.Data;
using NaughtyAttributes;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.DataTables;
using UnrealToUnity.Code.Scripts.Core.GameMode;

namespace JesterGame.Code.Scripts.Core
{
    public class ImpostorGameMode : GameMode
    {
        public delegate void ProgressionEventHandler(ImpostorGameMode mode, int prevProgress, int currentProgress);

        #region Inspector Fields

        [SerializeField, ReadOnly, Label("Impostor Name"), BoxGroup("Debug")]
        private string impostorRowName;

        /// <summary>
        /// The number of interactions the player must go through until they reach the end of the game.
        /// </summary>
        [SerializeField, BoxGroup("Debug")]
        private int maxInteractionsProgression = 15;

        /// <summary>
        /// The number of interactions the player has currently gone through.
        /// </summary>
        [SerializeField, ReadOnly, ProgressBar("Current Interactions Progression", "maxInteractionsProgression"),
         BoxGroup("Debug")
        ]
        private int currentInteractionProgression;

        [SerializeField] private DataTable<DialogueCharacter> characterDataTable;

        private readonly Dictionary<string, CharacterInstance> _characterInstanceMap = new();

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

        #region Awake

        protected override void Awake()
        {
            base.Awake();

            // Bind to the progress event
            OnProgressionChanged += LogProgress_Event;
            OnProgressionChanged += TestForGameFinished_Event;

            InitializeCharacters();
        }

        private void InitializeCharacters()
        {
            // Add all the characters to the map
            _characterInstanceMap.Clear();
            if (characterDataTable)
            {
                foreach (var rowHandle in characterDataTable.GetAllRowHandles())
                    _characterInstanceMap[rowHandle.rowName] = new CharacterInstance(rowHandle, CharacterType.Normal);
            }

            // Determine who the impostor is.
            // Randomly select an impostor.
            var impostorIndex = UnityEngine.Random.Range(0, _characterInstanceMap.Count);
            var currentIndex = 0;
            foreach (var rowHandle in _characterInstanceMap.Values)
            {
                // If not the impostor, continue.
                if (currentIndex != impostorIndex && currentIndex < _characterInstanceMap.Count)
                {
                    currentIndex++;
                    continue;
                }

                // Set the impostor.
                _characterInstanceMap[rowHandle.characterAsset.rowName] =
                    new CharacterInstance(rowHandle.characterAsset, CharacterType.Impostor);
                impostorRowName = rowHandle.characterAsset.rowName;

                Debug.Log($"The impostor is {rowHandle.characterAsset.rowName}!");

                break;
            }
        }

        #endregion


        private void Start()
        {
            // Set the progress to 0
            SetProgress(0);
        }
    }
}