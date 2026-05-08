using System.Collections;
using JesterGame.Code.Scripts.Core;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Characters.Behaviors
{
    [CreateAssetMenu(
        fileName = "CB_RandomPointOfInterest",
        menuName = "JesterGame/Character Behaviors/Random Point Of Interest")
    ]
    public class RandomPointOfInterestBehavior : CharacterBehaviorDataAsset
    {
        public override IEnumerator OngoingCoroutine(JesterGamePawn pawn)
        {
            if (!UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
                yield break;

            if (!gameMode.GetCurrentRoom(pawn, out var roomDataAsset))
                yield break;

            if (!gameMode.GetRoomLevelManager(roomDataAsset, out var levelManager))
                yield break;

            var randomPoint = levelManager.GetRandomPointOfInterest();

            if (randomPoint == null)
                yield break;

            // // Do the thing
            // yield return pawn.StartCoroutine(randomPoint.OngoingCoroutine(pawn));

            yield return randomPoint.OngoingCoroutine(pawn);
        }

        public override string GetBehaviorName => $"RandomPointOfInterestBehavior";
    }
}