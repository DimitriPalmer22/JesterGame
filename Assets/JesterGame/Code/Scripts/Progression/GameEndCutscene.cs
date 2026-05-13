using System.Collections;
using Eflatun.SceneReference;
using JesterGame.Code.Scripts.Core;
using UnityEngine;
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

        protected override IEnumerator OnDayProgressionCutscene(ProgressionEventArgs cutsceneStruct)
        {
            TimeScaleSubsystem timeScaleSubsystem = null;

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

            timeScaleSubsystem?.SetGameSpeed(1f);

            // Move the player pawn to the origin
            // TODO: Specify a spot where the player should be moved to in the inspector
            playerController!.ControlledPawn.transform.position = Vector3.zero + (Vector3.up * .5f);


            // Wait for the blend time of the cine camera data asset
            yield return cineCameraDataAsset.BlendTime;

            // Fade from black
            yield return dayProgressionScreenDataAsset?.CloseScreen();
        }
    }
}