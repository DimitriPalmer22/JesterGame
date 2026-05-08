using System.Collections;
using System.Collections.Generic;
using JesterGame.Code.Scripts.Core;
using JesterGame.Code.Scripts.Rooms;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Characters.Behaviors.Assets
{
    public abstract class GoToRoomBehavior : CharacterBehaviorDataAsset
    {
        protected abstract RoomDataAsset DetermineRoom(JesterGamePawn pawn);

        public override IEnumerator OngoingCoroutine(JesterGamePawn pawn)
        {
            var roomToGoTo = DetermineRoom(pawn);
            if (roomToGoTo == null)
                yield break;

            if (!UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
                yield break;

            // Get the new room's level manager
            if (!gameMode.roomToLevelManagerMap.TryGetValue(roomToGoTo, out var levelManager))
                yield break;

            // Get a random point of interest from that room.
            var randomPoint = levelManager.GetRandomPointOfInterest();
            if (randomPoint == null)
                yield break;

            yield return randomPoint.OngoingCoroutine(pawn);
        }
    }
}