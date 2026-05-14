using System;
using System.Collections;
using System.Linq;
using Eflatun.SceneReference;
using JesterGame.Code.Scripts.Characters;
using JesterGame.Code.Scripts.Core;
using JesterGame.Code.Scripts.Dialogue.Data;
using JesterGame.Code.Scripts.Dialogue.DialogueGraph.Runtime;
using JesterGame.Code.Scripts.Player;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnrealToUnity.Code.Scripts.Core.Subsystems;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Progression
{
    public class GameEndCutscene : DayCutsceneComponentBase
    {
        [SerializeField] private CineCameraDataAsset cineCameraDataAsset;

        [SerializeField] private SceneReference impostorRevealScene;
        [SerializeField] private SceneReference[] subscenesToUnload;

        // [SerializeField, Foldout("Dialogues")] private RuntimeDialogueGraph impostorKillsEveryoneDg;
        // [SerializeField, Foldout("Dialogues")] private RuntimeDialogueGraph impostorDoesntKillYouDg;
        // [SerializeField, Foldout("Dialogues")] private RuntimeDialogueGraph impostorKillsNobodyDg;

        protected override IEnumerator OnDayProgressionCutscene(ProgressionEventArgs cutsceneStruct)
        {
            TimeScaleSubsystem timeScaleSubsystem = null;

            if (!UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
            {
                throw new Exception("Could not find ImpostorGameMode in the scene.");
                yield break;
            }

            foreach (var pawn in gameMode.characterNameToPawnMap.Values)
            {
                pawn.StopMainBehaviorCoroutine(true);
                pawn.SetMovementEnabled(false);
            }

            // Disable the player's input
            var playerController = UtilLibrary.GetPlayerController(0);
            playerController?.AddInputBlocker(this);

            if (UtilLibrary.GetSubsystem(out TimeScaleSubsystem ts))
            {
                timeScaleSubsystem = ts;
                timeScaleSubsystem.SetGameSpeed(0f);
            }

            // Impostor stuff?

            // Unload all the scenes to unload
            foreach (var sceneReference in subscenesToUnload)
                SceneManager.UnloadSceneAsync(sceneReference.LoadedScene);

            // Load the scene to load
            SceneManager.LoadScene(impostorRevealScene.Path, LoadSceneMode.Additive);

            // Wait a frame for the scene to awake
            yield return null;

            var gameEndLevelManager =
                GameObject.FindWithTag("GameEndTimeline")?.GetComponent<GameEndSceneLevelManager>();

            if (gameEndLevelManager == null)
                throw new Exception("Could not find GameEndSceneLevelManager in the scene.");

            // Move the pawns to their respective positions in the cutscene
            var playerPawn = playerController!.ControlledPawn as JesterPlayerPawn;
            var impostorName = gameMode.characterInstanceMap
                .FirstOrDefault(n => n.Value.characterType == CharacterType.Impostor)
                .Value.characterAsset;
            var impostorPawn = gameMode.characterNameToPawnMap[impostorName.RowName];

            if (playerPawn != null)
                playerPawn!.SetMovementEnabled(false);

            if (impostorPawn != null)
                impostorPawn.SetMovementEnabled(false);

            yield return gameEndLevelManager.PrepareCutscene();

            gameEndLevelManager.SetPlayerAndImpostorTransforms(playerController?.ControlledPawn, impostorPawn);

            // Unpause the game.
            timeScaleSubsystem?.SetGameSpeed(1);

            // Wait for the blend time of the cine camera data asset
            yield return new WaitForSeconds(cineCameraDataAsset.BlendTime);

            // Fade from black
            yield return dayProgressionScreenDataAsset?.CloseScreen();

            // Start the timeline and await its finish
            gameEndLevelManager.playableDirector.Play();

            yield return new WaitForSeconds((float)gameEndLevelManager.playableDirector.duration);

            Debug.Log($"GAME END CUTSCENE OVER");
            // Play some dialogue or something...
            yield break;
        }
    }
}