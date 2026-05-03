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
            StartCoroutine(CoroutineTest());
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
    }
}