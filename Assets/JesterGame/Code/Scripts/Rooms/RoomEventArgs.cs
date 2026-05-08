using System;
using JesterGame.Code.Scripts.Characters;
using JesterGame.Code.Scripts.Dialogue.Data;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Pawns;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Rooms
{
    [Serializable]
    public struct RoomEventArgs
    {
        [SerializeField] public RoomDataAsset roomDataAsset;
        [SerializeField] public Optional<DialogueCharacter> dialogueCharacter;
        [SerializeField] public JesterGamePawn pawn;
    }
}