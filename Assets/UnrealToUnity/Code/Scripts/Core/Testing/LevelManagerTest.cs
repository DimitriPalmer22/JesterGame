using System;
using System.Collections;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Levels;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace UnrealToUnity.Code.Scripts.Core.Testing
{
    public class LevelManagerTest : LevelManager
    {
        private void Start()
        {
            StartCoroutine(IntroCutscene());
        }

        IEnumerator CoroutineTest()
        {
            // Get the game mode.
            if (!UtilLibrary.GetGameMode(out GameModeTest gameMode))
                yield break;

            gameMode.StartGameMode();

            yield return new WaitForSeconds(10f);

            gameMode.EndGameMode();
        }

        /// <summary>
        /// Enumerator responsible for playing the cutscene at the start of the game.
        /// </summary>
        private IEnumerator IntroCutscene()
        {
            // Disable input
            var playerController = UtilLibrary.GetPlayerController();
            playerController?.AddInputBlocker(this);

            yield return new WaitForSeconds(1f);

            // re-enable input
            playerController?.RemoveInputBlocker(this);

            Debug.Log("Intro cutscene finished, input re-enabled.");
        }
    }
}