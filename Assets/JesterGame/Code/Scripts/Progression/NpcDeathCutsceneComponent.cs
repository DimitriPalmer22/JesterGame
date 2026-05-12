using System.Collections;
using System.Linq;
using JesterGame.Code.Scripts.Core;
using JesterGame.Code.Scripts.Dialogue.Data;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Progression
{
    public class NpcDeathCutsceneComponent : DayCutsceneComponentBase
    {
        [SerializeField] private float screenFadeDelay = 0.0f;

        protected override IEnumerator CustomRunCutscene(ProgressionEventArgs cutsceneStruct)
        {
            // Get the game mode. If not, return.
            if (!UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
                yield break;

            // Fade to black
            if (dayProgressionScreenDataAsset)
            {
                yield return dayProgressionScreenDataAsset.OpenScreen();
                if (screenFadeDelay > 0f)
                    yield return new WaitForSecondsRealtime(screenFadeDelay);
            }


            // Choose a random character (not the impostor or main character) to die
            var validCharacters = gameMode.characterNameToPawnMap.Keys
                .Where(charName => gameMode.characterInstanceMap[charName].characterType == CharacterType.Normal)
                .ToArray();

            var randomCharacter = validCharacters.GetRandom();

            // Literally remove their pawn from the game and remove it from the map in the game mode
            var pawn = gameMode.characterNameToPawnMap[randomCharacter];
            if (pawn)
            {
                Destroy(pawn.gameObject);
                gameMode.characterNameToPawnMap.Remove(randomCharacter);
            }

            // Log a message to the screen.
            Debug.Log($"Killing character: {randomCharacter}");

            // Fade from black
            if (dayProgressionScreenDataAsset)
                yield return dayProgressionScreenDataAsset.CloseScreen();

            yield break;
        }
    }
}