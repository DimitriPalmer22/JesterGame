using System;
using System.Collections;
using System.Linq;
using AYellowpaper.SerializedCollections;
using JesterGame.Code.Scripts.Characters;
using JesterGame.Code.Scripts.Characters.Behaviors.PointsOfInterest;
using JesterGame.Code.Scripts.Dialogue.Data;
using JesterGame.Code.Scripts.Progression;
using JesterGame.Code.Scripts.Rooms;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnrealToUnity.Code.Scripts.Core.Cutscenes;
using UnrealToUnity.Code.Scripts.Core.DataTables;
using UnrealToUnity.Code.Scripts.Core.GameMode;
using UnrealToUnity.Code.Scripts.Core.Subsystems;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Core
{
    public class ImpostorGameMode : GameMode
    {
        #region Inspector Fields

        [SerializeField, Foldout("Debug"), Delayed, OnValueChanged(nameof(OnDebugTimeScaleChanged))]
        private float debugTimeScale = 1f;

        [SerializeField, ReadOnly, Label("Impostor Name"), Foldout("Read Only")]
        private string impostorRowName;

        [SerializedDictionary("Character Name", "Character Instance"), ReadOnly, Foldout("Read Only")]
        public SerializedDictionary<string, CharacterInstance> characterInstanceMap = new();

        [SerializedDictionary("Room Data Asset", "Level Manager"), ReadOnly, Foldout("Read Only")]
        public SerializedDictionary<RoomDataAsset, JesterLevelManager> roomToLevelManagerMap = new();

        [SerializedDictionary("Pawn", "Room Data Asset"), ReadOnly, Foldout("Read Only")]
        public SerializedDictionary<JesterGamePawn, RoomDataAsset> pawnToRoomMap = new();

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

        [SerializeField, Foldout("Progression Events")]
        public UnityEvent<AffectionEventArgs> onAffectionChanged;

        [SerializeField, Foldout("Progression Events")]
        public UnityEvent<ProgressionEventArgs> onProgressionChanged;

        [SerializeField, Foldout("Progression Events")]
        public UnityEvent<ProgressionEventArgs> onDayProgressed;

        #endregion

        [NonSerialized] public readonly SerializedDictionary<string, JesterGamePawn> characterNameToPawnMap = new();

        #region Properties

        public int GetMaxDays => dayProgressions.Length;

        #endregion

        #region Functions

        #region Awake

        protected override void Awake()
        {
            base.Awake();

            InitializeCharacters();
        }

        private void InitializeCharacters()
        {
            // Add all the characters to the map
            characterInstanceMap.Clear();
            if (characterDataTable)
            {
                foreach (var rowHandle in characterDataTable.GetAllRowHandles())
                    characterInstanceMap[rowHandle.rowName] = new CharacterInstance(rowHandle, CharacterType.Normal);
            }

            var validImpostors = characterInstanceMap
                .Where(n => CanBeImpostor(n.Key))
                .Select(n => n.Value)
                .ToArray();

            // Determine who the impostor is.
            // Randomly select an impostor.
            var impostorIndex = UnityEngine.Random.Range(0, validImpostors.Length);
            var currentIndex = 0;
            foreach (var characterInstance in validImpostors)
            {
                // If not the impostor, continue.
                if (currentIndex != impostorIndex && currentIndex < characterInstanceMap.Count)
                {
                    currentIndex++;
                    continue;
                }

                // Set the impostor.
                characterInstanceMap[characterInstance.characterAsset.rowName] =
                    new CharacterInstance(characterInstance.characterAsset, CharacterType.Impostor);
                impostorRowName = characterInstance.characterAsset.rowName;

                Debug.Log($"The impostor is {characterInstance.characterAsset.rowName}!");

                break;
            }
        }

        private bool CanBeImpostor(string characterName)
        {
            // If the character is not in the table, return false
            if (!characterDataTable.GetRow(characterName, out var characterAsset))
                return false;

            if (characterAsset.bIsMainCharacter)
                return false;

            return true;
        }

        #endregion

        protected override void Start()
        {
            base.Start();

            // Set the progress to 0
            SetProgress(0);
        }

        #region Progress Functions

        public int GetCurrentMaxProgressions()
        {
            if (!dayProgressions.IsValidIndex(currentDayIndex))
                return 0;

            var validDayIndex = Mathf.Clamp(currentDayIndex, 0, dayProgressions.Length - 1);
            return dayProgressions[validDayIndex].numProgressionsInDay;
        }


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
            Debug.Log($"Progression from {args.previousProgress} to {args.currentProgress}");
        }

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

        #endregion

        public void ModifyCharacterAffection(string characterName, int affectionValue)
        {
            if (!characterInstanceMap.TryGetValue(characterName, out var characterInstance))
                return;

            characterInstance.currentAffection += affectionValue;

            // Update the value within the map
            characterInstanceMap[characterName] = characterInstance;

            // Create affection event args
            var affectionEventArgs =
                new AffectionEventArgs(characterName, characterInstance.currentAffection, affectionValue);

            // Broadcast affection event here.
            onAffectionChanged?.Invoke(affectionEventArgs);
        }

        public void LogAffectionStatus(AffectionEventArgs args)
        {
            // Return if no change.
            if (args.affectionDelta == 0)
                return;

            Debug.Log($"{args.characterName}'s affection is now {args.affectionValue} ({args.affectionDelta})");
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
            yield return StartCoroutine(cutsceneComponent.OngoingCoroutine(args));

            // Call the post-event
            dayEvents.postEvent?.Invoke(args);
        }

        #region Get Functions

        public bool GetCurrentRoom(JesterGamePawn pawn, out RoomDataAsset dataAsset)
        {
            return pawnToRoomMap.TryGetValue(pawn, out dataAsset);
        }

        public bool GetRoomLevelManager(RoomDataAsset dataAsset, out JesterLevelManager levelManager)
        {
            return roomToLevelManagerMap.TryGetValue(dataAsset, out levelManager);
        }

        public bool GetCharacterInstance(string characterAssetName, out CharacterInstance characterInstance)
        {
            return characterInstanceMap.TryGetValue(characterAssetName, out characterInstance);
        }

        public bool GetCharacterPawn(string characterName, out JesterGamePawn pawn)
        {
            return characterNameToPawnMap.TryGetValue(characterName, out pawn);
        }

        #endregion

        #region Point of Interest Functions

        /// <summary>
        /// A function that goes through each of the active levels and returns all the registered points of interest from each.
        /// </summary>
        /// <returns></returns>
        public PointOfInterest[] GetAllPointsOfInterest()
        {
            return roomToLevelManagerMap.Values.SelectMany(room => room.GetAllPointsOfInterest().Where(n => n != null))
                .ToArray();
        }

        public PointOfInterest GetPointOfInterestByRowHandle(DataTableRowHandle rowHandle)
        {
            var allPoints = GetAllPointsOfInterest();

            return allPoints.FirstOrDefault(poi => poi.PointOfInterestDataHandle == rowHandle);
        }

        #endregion

        private void OnDebugTimeScaleChanged()
        {
            // Get the time scale subsystem
            if (!UtilLibrary.GetSubsystem(out TimeScaleSubsystem timeScaleSubsystem))
                return;

            timeScaleSubsystem.SetDebugSpeed(debugTimeScale);
        }

        #endregion
    }
}