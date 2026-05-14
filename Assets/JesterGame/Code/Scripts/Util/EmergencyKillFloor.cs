using System;
using JesterGame.Code.Scripts.Characters;
using JesterGame.Code.Scripts.Core;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Player;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Util
{
    [RequireComponent(typeof(BoxCollider))]
    public class EmergencyKillFloor : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            // Ignore if the other is not a pawn
            if (!other.TryGetComponent(out JesterGamePawn pawn))
                return;

            // If the pawn is player-controlled.
            if (!pawn.owningController || pawn.owningController is not PlayerController)
                return;

            // Try to the game impostor game mode
            if (!UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
                return;


            pawn.SetMovementEnabled(false);

            var playerStart = gameMode.GetPlayerStart();
            gameMode.MovePawnToPlayerStart(pawn, playerStart);

            pawn.SetMovementEnabled(true);
        }
    }
}