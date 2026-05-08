using System.Linq;
using JesterGame.Code.Scripts.Core;
using JesterGame.Code.Scripts.Rooms;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Characters.Behaviors.Assets
{
    [CreateAssetMenu(
        fileName = "CB_GoToRoom_Random",
        menuName = "JesterGame/Character Behaviors/Room/Go To Random")
    ]
    public class GoToRandomRoomBehavior : GoToRoomBehavior
    {
        public override string GetBehaviorName => $"Go To Random Room";

        protected override RoomDataAsset DetermineRoom(JesterGamePawn pawn)
        {
            // From the game mode, get the list of all rooms.
            if (!UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
                return null;

            // Get only the valid rooms to go to.
            var roomsSet = gameMode.roomToLevelManagerMap.Keys
                .Where(n => IsRoomValidToGoTo(gameMode, pawn, n))
                .ToHashSet();

            // Return a random element from the set of rooms
            if (roomsSet.Count > 0)
                return roomsSet.ElementAt(Random.Range(0, roomsSet.Count));

            return null;
        }

        private bool IsRoomValidToGoTo(ImpostorGameMode gameMode, JesterGamePawn pawn, RoomDataAsset room)
        {
            // If the pawn is currently in the room, return false
            if (gameMode.GetCurrentRoom(pawn, out var currentRoom) && currentRoom == room)
                return false;

            // If the room has no points of interests, return false
            if (gameMode.GetRoomLevelManager(room, out var levelManager) &&
                levelManager.GetAllPointsOfInterest().Length <= 0)
                return false;

            return true;
        }
    }
}