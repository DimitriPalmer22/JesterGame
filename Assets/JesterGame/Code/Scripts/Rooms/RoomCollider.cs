using System;
using JesterGame.Code.Scripts.Characters;
using JesterGame.Code.Scripts.Core;
using UnityEngine;
using UnityEngine.Events;
using UnrealToUnity.Code.Scripts.Core;
using UnrealToUnity.Code.Scripts.Core.Pawns;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Rooms
{
    /// <summary>
    /// A class used to detect when the player enters a room, and to trigger any events associated with that room.
    /// For simplicity, a big collider encompassing most of the room should be used here.
    /// When the character touches this collider, their current room updates.
    /// </summary>
    public class RoomCollider : Actor
    {
        [SerializeField] public JesterLevelManager roomManager;

        [SerializeField] public BoxCollider boxCollider;

        /// <summary>
        /// ANOTHER unity event for when the room is entered, this one is on the collider itself instead of the data asset.
        /// This is for things that should happen when the room is entered, but aren't necessarily associated with the room itself.
        /// </summary>
        [SerializeField] public UnityEvent<RoomEventArgs> onRoomEntered;

        private void OnTriggerEnter(Collider other)
        {
            if (!CanTriggerCollider(other, out var pawn))
                return;

            roomManager.TryGetRoomDataAsset(out var roomDataAsset);

            var eventArgs = new RoomEventArgs
            {
                roomDataAsset = roomDataAsset,
                pawn = pawn
            };

            // Try to cast to npc character to get the dialogue character
            if (pawn is NpcCharacter npcCharacter)
                eventArgs.dialogueCharacter.FromTryGet(npcCharacter.TryGetCharacterData);

            // Call the event from the room data asset
            // Call the event from this script
            roomDataAsset?.onRoomEntered.Invoke(eventArgs);
            onRoomEntered.Invoke(eventArgs);
        }

        // private void OnTriggerStay(Collider other)
        // {
        //     if (!CanTriggerCollider(other, out var pawn))
        //         return;
        // }

        private bool CanTriggerCollider(Collider other, out JesterGamePawn pawn)
        {
            pawn = null;

            roomManager.TryGetRoomDataAsset(out var roomDataAsset);

            // If the room data asset is null, return
            if (roomDataAsset == null)
                return false;

            // If the other thing colliding with this is not a pawn, return
            if (!other.TryGetComponent(out pawn))
                return false;

            return true;
        }

        private void OnDrawGizmos()
        {
            // Draw the bounds of the box collider
            Gizmos.color = Color.yellow;
            var scaledSize = new Vector3(
                boxCollider.size.x * transform.localScale.x,
                boxCollider.size.y * transform.localScale.y,
                boxCollider.size.z * transform.localScale.z
            );

            var startPos = transform.position + boxCollider.center;

            Gizmos.DrawWireCube(startPos, scaledSize);
        }

        public void UpdatePawnRoomArea(RoomEventArgs args)
        {
            if (UtilLibrary.GetGameMode(out ImpostorGameMode gameMode) && args.pawn)
                gameMode.pawnToRoomMap[args.pawn] = args.roomDataAsset;
        }

        public void LogRoomEnter(RoomEventArgs args)
        {
            Debug.Log(
                args.dialogueCharacter
                    ? $"{args.roomDataAsset.name} entered {args.roomDataAsset.roomName} ({args.pawn.name}"
                    : $"{args.roomDataAsset.name} entered ({args.pawn.name}, {args.pawn.owningController.name})"
            );
        }
    }
}