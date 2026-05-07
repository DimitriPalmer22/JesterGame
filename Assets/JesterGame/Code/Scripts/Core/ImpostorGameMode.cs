using System;
using System.Collections.Generic;
using JesterGame.Code.Scripts.Dialogue.Data;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
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
        [SerializeField, BoxGroup("Debug")] private int maxInteractionsProgression = 15;

        /// <summary>
        /// The number of interactions the player has currently gone through.
        /// </summary>
        [SerializeField, ReadOnly, ProgressBar("Current Interactions Progression", "maxInteractionsProgression"),
         BoxGroup("Debug")
        ]
        private int currentInteractionProgression;

        [SerializeField] private DataTable<DialogueCharacter> characterDataTable;

        [SerializeField] public UnityEvent<AffectionEventArgs> onAffectionChanged;
        [SerializeField] public UnityEvent<ProgressionEventArgs> onProgressionChanged;

        #endregion

        private readonly Dictionary<string, CharacterInstance> _characterInstanceMap = new();

        #region Functions

        #region Progress Functions

        public void SetProgress(int progress)
        {
            // If the new progress is out of bounds, return.
            if (progress < 0 || progress > maxInteractionsProgression)
                return;

            var prevProgress = currentInteractionProgression;
            currentInteractionProgression = progress;

            // Only invoke the event if the progress has actually changed.
            if (prevProgress != currentInteractionProgression)
            {
                // Create the progression args
                var args = new ProgressionEventArgs(prevProgress, progress, 0);
                onProgressionChanged?.Invoke(args);
            }

            // TODO: Maybe have some different event for when the progress is reset.
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void IncrementProgress() => SetProgress(currentInteractionProgression + 1);

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void DecrementProgress() => SetProgress(currentInteractionProgression - 1);

        private void TestForGameFinished_Event(ProgressionEventArgs args)
        {
            if (args.currentProgress >= maxInteractionsProgression)
            {
                Debug.Log("Game finished!");

                // TODO: Add a function or something here to handle the game mode ending.
            }
        }

        private void LogProgress_Event(ProgressionEventArgs args)
        {
            Debug.Log($"Day progressed from {args.previousProgress} to {args.currentProgress}");
        }

        public void ModifyCharacterAffection(string characterName, int affectionValue)
        {
            if (!_characterInstanceMap.TryGetValue(characterName, out var characterInstance))
                return;

            characterInstance.currentAffection += affectionValue;

            // Update the value within the map
            _characterInstanceMap[characterName] = characterInstance;

            // Create affection event args
            var affectionEventArgs =
                new AffectionEventArgs(characterName, characterInstance.currentAffection, affectionValue);

            // Broadcast affection event here.
            onAffectionChanged?.Invoke(affectionEventArgs);
        }

        #endregion

        #endregion

        #region Awake

        protected override void Awake()
        {
            base.Awake();

            // Bind to the progress event
            onProgressionChanged.AddListener(LogProgress_Event);
            onProgressionChanged.AddListener(TestForGameFinished_Event);

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

        public void LogAffectionStatus(AffectionEventArgs args)
        {
            // Return if no change.
            if (args.affectionDelta == 0)
                return;

            Debug.Log($"{args.characterName}'s affection is now {args.affectionValue} ({args.affectionDelta})");
        }
    }
}