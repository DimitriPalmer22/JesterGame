using System;
using System.Collections;
using System.Collections.Generic;
using JesterGame.Code.Scripts.Dialogue.Data;
using JesterGame.Code.Scripts.Progression;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnrealToUnity.Code.Scripts.Core.Cutscenes;
using UnrealToUnity.Code.Scripts.Core.DataTables;
using UnrealToUnity.Code.Scripts.Core.GameMode;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Core
{
    public class ImpostorGameMode : GameMode
    {
        public delegate void ProgressionEventHandler(ImpostorGameMode mode, int prevProgress, int currentProgress);

        #region Inspector Fields

        [SerializeField, ReadOnly, Label("Impostor Name"), BoxGroup("Debug")]
        private string impostorRowName;

        [SerializeField, BoxGroup("Progression")]
        public DayProgressionStruct[] dayProgressions;

        [
            SerializeField, ReadOnly,
            ProgressBar("Current Day", "GetMaxDays"),
            BoxGroup("Progression")
        ]
        public int currentDayIndex;

        /// <summary>
        /// The number of interactions the player has currently gone through.
        /// </summary>
        [
            SerializeField, ReadOnly,
            ProgressBar("Current Interactions Progression", "GetCurrentMaxProgressions"),
            BoxGroup("Progression")
        ]
        private int currentInteractionProgression;

        [SerializeField] private DataTable<DialogueCharacter> characterDataTable;

        [SerializeField] public UnityEvent<AffectionEventArgs> onAffectionChanged;
        [SerializeField] public UnityEvent<ProgressionEventArgs> onProgressionChanged;
        [SerializeField] public UnityEvent<ProgressionEventArgs> onDayProgressed;

        #endregion

        private readonly Dictionary<string, CharacterInstance> _characterInstanceMap = new();

        #region Functions

        #region Progress Functions

        public void SetProgress(int progress)
        {
            // If the new progress is out of bounds, return.
            if (progress < 0 || progress > GetCurrentMaxProgressions())
                return;

            // If the current day index is NOT valid, return
            if (!dayProgressions.IsValidIndex(currentDayIndex))
                return;

            var prevProgress = currentInteractionProgression;
            currentInteractionProgression = progress;

            // Only invoke the event if the progress has actually changed.
            if (prevProgress != currentInteractionProgression)
            {
                // Create the progression args
                var args = new ProgressionEventArgs(prevProgress, progress, currentDayIndex);
                onProgressionChanged?.Invoke(args);
            }

            // TODO: Maybe have some different event for when the progress is reset.
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void IncrementProgress() => SetProgress(currentInteractionProgression + 1);

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void DecrementProgress() => SetProgress(currentInteractionProgression - 1);

        public void CheckDayProgression(ProgressionEventArgs args)
        {
            if (args.currentProgress < GetCurrentMaxProgressions())
                return;

            // Increment the day index and call the day progressed event.
            currentDayIndex += 1;
            onDayProgressed?.Invoke(
                new ProgressionEventArgs(args.previousProgress, args.currentProgress, currentDayIndex));

            // Reset the interaction progress.
            currentInteractionProgression = 0;
        }

        public void LogProgress_Event(ProgressionEventArgs args)
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

        public int GetCurrentMaxProgressions()
        {
            if (!dayProgressions.IsValidIndex(currentDayIndex))
                return 0;

            var validDayIndex = Mathf.Clamp(currentDayIndex, 0, dayProgressions.Length - 1);
            return dayProgressions[validDayIndex].numProgressionsInDay;
        }

        public int GetMaxDays => dayProgressions.Length;

        public void OnDayProgressed_Event(ProgressionEventArgs args)
        {
            Debug.Log($"Day progressed to {args.currentDay}!");
            StartCoroutine(DayProgressedCoroutine(args));
        }

        private IEnumerator DayProgressedCoroutine(ProgressionEventArgs args)
        {
            // Try to play the previous day end's cutscene
            var previousDay = args.currentDay - 1;
            if (previousDay >= 0 && dayProgressions.IsValidIndex(previousDay))
            {
                var previousDayStruct = dayProgressions[previousDay];

                // yield the cutscene with events
                yield return PlayDayCutsceneWithEvents(
                    previousDayStruct.dayEndCutscene,
                    previousDayStruct.dayEndEvents,
                    args
                );
            }

            // Try to play the current day start's cutscene
            var currentDay = args.currentDay;
            if (currentDay >= 0 && dayProgressions.IsValidIndex(currentDay))
            {
                var currentDayStruct = dayProgressions[currentDay];

                yield return PlayDayCutsceneWithEvents(
                    currentDayStruct.dayStartCutscene,
                    currentDayStruct.dayStartEvents,
                    args
                );
            }

            yield return null;
        }

        private IEnumerator PlayDayCutsceneWithEvents(
            CutsceneComponent<ProgressionEventArgs> cutsceneComponent,
            PrePostEvent<ProgressionEventArgs> dayEvents,
            ProgressionEventArgs args
        )
        {
            if (!cutsceneComponent)
                yield break;

            // Call the pre-event
            dayEvents.preEvent?.Invoke(args);

            // Play the cutscene
            yield return StartCoroutine(cutsceneComponent.RunCutsceneEnumerator(args));

            // Call the post-event
            dayEvents.postEvent?.Invoke(args);
        }
    }
}