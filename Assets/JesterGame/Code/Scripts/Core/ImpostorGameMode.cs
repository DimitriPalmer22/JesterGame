using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using JesterGame.Code.Scripts.Characters;
using JesterGame.Code.Scripts.Characters.Behaviors.PointsOfInterest;
using JesterGame.Code.Scripts.Dialogue.Data;
using JesterGame.Code.Scripts.Dialogue.DialogueGraph.Runtime;
using JesterGame.Code.Scripts.Dialogue.UI;
using JesterGame.Code.Scripts.Progression;
using JesterGame.Code.Scripts.Rooms;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnrealToUnity.Code.Scripts.Core.DataTables;
using UnrealToUnity.Code.Scripts.Core.GameMode;
using UnrealToUnity.Code.Scripts.Core.Subsystems;
using UnrealToUnity.Code.Scripts.Core.Utility;
using UnrealToUnity.Code.Scripts.Core.Utility.Scenes;

namespace JesterGame.Code.Scripts.Core
{
    public class ImpostorGameMode : GameMode
    {
        #region Inspector Fields

        [SerializeField] private EnsureSubscenesLoaded ensureSubscenesLoadedComponent;

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

        [SerializeField] public DayProgressionStruct[] dayProgressions;

        [
            SerializeField, ReadOnly,
            ProgressBar("Current Day", "GetMaxDays"),
            // BoxGroup("Progression")
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

        [SerializeField] private NpcDeathCutsceneComponent npcDeathCutsceneComponent;

        [SerializeField] private DayCutsceneComponentBase gameEndCutscene;

        [SerializeField] private StartGameCutsceneComponent startGameCutsceneComponent;

        #endregion

        [NonSerialized] public readonly SerializedDictionary<string, JesterGamePawn> characterNameToPawnMap = new();

        [NonSerialized] private readonly ManualYield freeRoamPhaseYield = new();

        [NonSerialized] private Coroutine gameLoopCoroutine;

        #region Properties

        public int GetMaxDays => dayProgressions.Length;

        #endregion

        #region Functions

        #region Awake

        protected override void Awake()
        {
            base.Awake();

            InitializeCharacters();

#if UNITY_EDITOR
#else
                // If in a build, we need to ensure all the subscenes are loaded before we initialize the characters.
                // This is because the characters are initialized based on the character data table, which is in a subscene.
                if (ensureSubscenesLoadedComponent)
                    ensureSubscenesLoadedComponent.LoadScenes();
#endif
        }

        private void InitializeCharacters()
        {
            // Add all the characters to the map
            characterInstanceMap.Clear();
            if (characterDataTable)
            {
                foreach (var rowHandle in characterDataTable.GetAllRowHandles())
                {
                    var characterType = CharacterType.Normal;

                    if (rowHandle.GetValue(out DialogueCharacter characterAsset) && characterAsset.bIsMainCharacter)
                        characterType = CharacterType.Player;

                    characterInstanceMap[rowHandle.RowName] = new CharacterInstance(rowHandle, characterType);
                }
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
                characterInstanceMap[characterInstance.characterAsset.RowName] =
                    new CharacterInstance(characterInstance.characterAsset, CharacterType.Impostor);
                impostorRowName = characterInstance.characterAsset.RowName;

                Debug.Log($"The impostor is {characterInstance.characterAsset.RowName}!");

                break;
            }
        }

        private bool CanBeImpostor(string characterName)
        {
            // If the character is not in the table, return false
            if (!characterDataTable.GetRow(characterName, out var characterAsset))
                return false;

            if (characterAsset.bIsMainCharacter || !characterAsset.bCanBeImpostor || characterAsset.bIsDisabled)
                return false;

            return true;
        }

        #endregion

        protected override IEnumerator Start()
        {
#if UNITY_EDITOR

            // If in the editor,
            // we need to check if we have multiple scenes open first.
            if (ensureSubscenesLoadedComponent)
                ensureSubscenesLoadedComponent.LoadScenes();

            // Wait a frame to ensure all the subscenes are loaded and Awake has been called on all the components?
            yield return null;
#endif

            yield return base.Start();

            // Set the progress to 0
            SetProgress(0);

            // Start the game loop coroutine
            gameLoopCoroutine = StartCoroutine(GameLoop());
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

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void ForceNextDay()
        {
            if (!dayProgressions.IsValidIndex(currentDayIndex))
                return;

            var currentDay = dayProgressions[currentDayIndex];

            // If we are not at the end of the progression for the day, set it to the end.
            if (currentInteractionProgression < currentDay.numProgressionsInDay)
            {
                SetProgress(currentDay.numProgressionsInDay);
                return;
            }

            // // If we are already at the end of the progression for the day, increment the day index and reset progress.
            // if (currentInteractionProgression >= currentDay.numProgressionsInDay)
            // {
            //     SetProgress(0);
            //     currentDayIndex += 1;
            // }
        }

        public void CheckDayProgression(ProgressionEventArgs args)
        {
            if (args.currentProgress < GetCurrentMaxProgressions())
                return;

            // Reset the manual yield for the free roam phase
            freeRoamPhaseYield.Reset();

            // // Increment the day index and call the day progressed event.
            // currentDayIndex += 1;
            // onDayProgressed?.Invoke(
            //     new ProgressionEventArgs(args.previousProgress, args.currentProgress, currentDayIndex));
            //
            // // Reset the interaction progress.
            // currentInteractionProgression = 0;
        }

        public void LogProgress_Event(ProgressionEventArgs args)
        {
            Debug.Log($"Progression from {args.previousProgress} to {args.currentProgress}");
        }

        public void OnDayProgressed_Event(ProgressionEventArgs args)
        {
            Debug.Log($"Day progressed to {args.currentDay}!");
        }

        public void PlayParticles_AffectionChanged(AffectionEventArgs args)
        {
            if (args.affectionDelta == 0)
                return;

            if (!characterNameToPawnMap.TryGetValue(args.characterName, out var pawn))
                return;

            Debug.Log($"Affection changed for {args.characterName} by {args.affectionDelta}. Playing particles!");

            var vfxHelper = pawn.GetVfxHelper();
            if (vfxHelper == null)
                return;

            if (args.affectionDelta > 0)
                vfxHelper.OnAffectionUp();
            else
                vfxHelper.OnAffectionDown();
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
            return roomToLevelManagerMap.Values
                .SelectMany(room => room.GetAllPointsOfInterest().Where(n => n != null))
                .ToArray();
        }

        public PointOfInterest GetPointOfInterestByRowHandle(DataTableRowHandle rowHandle)
        {
            var allPoints = GetAllPointsOfInterest();

            return allPoints.FirstOrDefault(poi => poi.PointOfInterestDataHandle == rowHandle);
        }

        public void MoveNpcsToRandomPointsOfInterest()
        {
            var allPointsOfInterest = GetAllPointsOfInterest().ToHashSet();
            var usedPoints = new HashSet<PointOfInterest>();

            // If there are no valid points of interest, return
            if (allPointsOfInterest.Count == 0 && usedPoints.Count == 0)
                return;

            var validCharacters = characterNameToPawnMap.Values
                .Where(n => n is NpcCharacter npc && !npc.IsDead)
                .Cast<NpcCharacter>()
                .ToArray();

            foreach (var npc in validCharacters)
                MoveNpcToRandomPointOfInterest(npc, allPointsOfInterest, usedPoints);
        }

        public void MoveNpcToRandomPointOfInterest(JesterGamePawn npc)
        {
            var randomPoint = GetAllPointsOfInterest().GetRandom();

            if (randomPoint == null)
            {
                Debug.LogError($"No valid points of interest found to move {npc.name} to.");
                return;
            }

            // Move the npc to the random point
            npc.transform.position = randomPoint.GetTransform.position;
            npc.transform.rotation = randomPoint.GetTransform.rotation;
        }

        private void MoveNpcToRandomPointOfInterest(
            NpcCharacter npc,
            HashSet<PointOfInterest> allPointsOfInterest, HashSet<PointOfInterest> usedPoints
        )
        {
            // when the valid list is empty, add all the used points back
            if (allPointsOfInterest.Count == 0)
            {
                allPointsOfInterest.UnionWith(usedPoints);
                usedPoints.Clear();
            }

            // Get a random point from the valid points
            var randomPoint = allPointsOfInterest.GetRandom();
            allPointsOfInterest.Remove(randomPoint);
            usedPoints.Add(randomPoint);

            // Move the npc to the random point
            npc.transform.position = randomPoint.GetTransform.position;
            npc.transform.rotation = randomPoint.GetTransform.rotation;
        }

        #endregion

        private void OnDebugTimeScaleChanged()
        {
            // Get the timescale subsystem
            if (!UtilLibrary.GetSubsystem(out TimeScaleSubsystem timeScaleSubsystem))
                return;

            timeScaleSubsystem.SetDebugSpeed(debugTimeScale);
        }

        public int GetImpostorAffection(out string impostorName)
        {
            impostorName = string.Empty;

            var impostorData = characterInstanceMap
                .FirstOrDefault(n => n.Value.characterType == CharacterType.Impostor);

            impostorName = impostorData.Key;

            return impostorData.Value.currentAffection;
        }

        #endregion

        private void StopNpcBehaviors(bool bClear = true)
        {
            var validCharacters = characterNameToPawnMap.Values
                .Where(n => n is NpcCharacter npc && !npc.IsDead)
                .Cast<NpcCharacter>()
                .ToArray();

            foreach (var npc in validCharacters)
                npc.StopMainBehaviorCoroutine(bClear);
        }

        private void StartNpcBehaviors(bool bClear = true)
        {
            var validCharacters = characterNameToPawnMap.Values
                .Where(n => n is NpcCharacter npc && !npc.IsDead)
                .Cast<NpcCharacter>()
                .ToArray();


            foreach (var npc in validCharacters)
                npc.StartMainBehaviorCoroutine(bClear);
        }

        private IEnumerator GameLoop()
        {
            for (currentDayIndex = 0; currentDayIndex < dayProgressions.Length; currentDayIndex++)
            {
                var currentDay = dayProgressions[currentDayIndex];

                #region Intro Cutscene

                // Move each of the characters to a random point of interest at the start of the game.
                MoveNpcsToRandomPointsOfInterest();

                // Move the player pawn to the player start
                var playerPawn = UtilLibrary.GetPlayerController(0)?.ControlledPawn as JesterGamePawn;
                playerPawn!.SetMovementEnabled(false);
                MovePawnToPlayerStart(playerPawn, GetPlayerStart());
                playerPawn!.SetMovementEnabled(true);

                // Yield the day start cutscene
                if (currentDay.dayStartCutscene)
                {
                    var currentProgressionArgs = new ProgressionEventArgs
                    {
                        currentDay = currentDayIndex,
                        currentProgress = 0,
                        previousProgress = 0
                    };
                    yield return currentDay.dayStartCutscene.OngoingCoroutine(currentProgressionArgs);
                }

                Debug.Log($"Day #{currentDayIndex} - Day Start Cutscene Finished!");

                #endregion

                // Intro cutscene.
                if (currentDayIndex == 0)
                    yield return startGameCutsceneComponent.OngoingCoroutine(new ProgressionEventArgs());

                #region Free Roam Phase

                // Start the NPC coroutines
                StartNpcBehaviors(true);

                // Yield the free roam phase (all progression is done for the day)
                freeRoamPhaseYield.Reset();
                freeRoamPhaseYield.StartYield();
                yield return freeRoamPhaseYield;

                Debug.Log($"Day #{currentDayIndex} - Free Roam Phase Finished!");

                #endregion

                #region Day End Cutscene

                // Stop the NPC behaviors
                StopNpcBehaviors(false);

                // Yield the day end cutscene
                if (currentDay.dayEndCutscene)
                {
                    var currentProgressionArgs = new ProgressionEventArgs
                    {
                        currentDay = currentDayIndex,
                        currentProgress = currentDay.numProgressionsInDay,
                        previousProgress = currentDay.numProgressionsInDay - 1
                    };
                    yield return currentDay.dayEndCutscene.OngoingCoroutine(currentProgressionArgs);
                }

                Debug.Log($"Day #{currentDayIndex} - Day End Cutscene Finished!");

                #endregion

                #region Death Cutscene

                // Yield the someone dies cutscene
                var impostorAffection = GetImpostorAffection(out var impostorName);
                Debug.Log(
                    $"Current impostor affection: {impostorName} - {impostorAffection} - {currentDay.minimumImpostorAffection}"
                );
                if (impostorAffection < currentDay.minimumImpostorAffection)
                {
                    var currentProgressionArgs = new ProgressionEventArgs
                    {
                        currentDay = currentDayIndex,
                        currentProgress = currentDay.numProgressionsInDay,
                        previousProgress = currentDay.numProgressionsInDay - 1
                    };

                    yield return npcDeathCutsceneComponent.OngoingCoroutine(currentProgressionArgs);
                }

                #endregion

                // Increment the day index and call the day progressed event.
                var previousProgress = currentInteractionProgression;
                currentInteractionProgression = 0;
                onDayProgressed?.Invoke(
                    new ProgressionEventArgs(previousProgress, currentInteractionProgression, currentDayIndex + 1)
                );
            }

            var progressionArgs = new ProgressionEventArgs(
                previousProgress: currentInteractionProgression,
                currentProgress: currentInteractionProgression,
                currentDay: currentDayIndex
            );
            yield return gameEndCutscene.OngoingCoroutine(progressionArgs);

            yield break;
        }
    }
}