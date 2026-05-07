using System;
using System.Collections;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Levels;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace UnrealToUnity.Code.Scripts.Core.Testing
{
    public class TestLevelManager : LevelManager
    {
        private void Start()
        {
            StartCoroutine(IntroCutscene());
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