using JesterGame.Code.Scripts.Rooms;
using UnityEngine;

namespace JesterGame.Code.Scripts.Characters.Behaviors.Assets
{
    [CreateAssetMenu(fileName = "CB_GoToRoom_", menuName = "JesterGame/Character Behaviors/Go To Specific Room")]
    public class GoToSpecificRoomBehavior : GoToRoomBehavior
    {
        [SerializeField] private RoomDataAsset roomToGoTo;

        public override string GetBehaviorName => $"Go To Room: {roomToGoTo.roomName}";
        protected override RoomDataAsset DetermineRoom(JesterGamePawn pawn) => roomToGoTo;
    }
}