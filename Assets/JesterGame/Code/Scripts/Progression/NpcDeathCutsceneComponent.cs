using System.Collections;
using System.Linq;
using JesterGame.Code.Scripts.Characters;
using JesterGame.Code.Scripts.Core;
using JesterGame.Code.Scripts.Dialogue.Data;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Player;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Progression
{
    public class NpcDeathCutsceneComponent : DayCutsceneComponentBase
    {
        [SerializeField] private float screenFadeDelay = 0.0f;

        protected override IEnumerator OnDayProgressionCutscene(ProgressionEventArgs cutsceneStruct)
        {
            // Get the game mode. If not, return.
            if (!UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
                yield break;

            // // Fade to black
            // if (dayProgressionScreenDataAsset)
            // {
            //     yield return dayProgressionScreenDataAsset.OpenScreen();
            //     if (screenFadeDelay > 0f)
            //         yield return new WaitForSecondsRealtime(screenFadeDelay);
            // }

            // Choose a random character (not the impostor or main character) to die
            var validCharacters = gameMode.characterNameToPawnMap.Keys
                .Where(charName => gameMode.characterNameToPawnMap.TryGetValue(charName, out var pawn) &&
                                   pawn != null && !pawn.IsDead &&
                                   gameMode.characterInstanceMap[charName].characterType == CharacterType.Normal
                )
                .ToArray();

            // Literally remove their pawn from the game and remove it from the map in the game mode
            if (validCharacters.Length > 0)
            {
                var randomCharacter = validCharacters.GetRandom();
                var pawn = gameMode.characterNameToPawnMap[randomCharacter];
                if (pawn != null)
                    pawn.Die();

                // Log a message to the screen.
                Debug.Log($"Killing character: {randomCharacter}");
            }

            // // Fade from black
            // if (dayProgressionScreenDataAsset)
            //     yield return dayProgressionScreenDataAsset.CloseScreen();

            yield break;
        }
    }
}