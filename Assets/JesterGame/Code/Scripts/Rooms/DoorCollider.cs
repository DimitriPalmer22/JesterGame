using System;
using System.Collections;
using JesterGame.Code.Scripts.Characters;
using JesterGame.Code.Scripts.Core;
using JesterGame.Code.Scripts.Progression.UI;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Player;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Rooms
{
    [RequireComponent(typeof(BoxCollider))]
    public class DoorCollider : MonoBehaviour
    {
        [SerializeField] private Transform otherSideSpawn;
        [SerializeField] private DoneTalkingScreenDataAsset doneTalkingScreenDataAsset;

        private void OnTriggerEnter(Collider other)
        {
            // Ignore if the other is not a pawn
            if (!other.TryGetComponent(out JesterGamePawn pawn))
                return;

            // If the pawn is player-controlled.
            if (!pawn.owningController || pawn.owningController is not PlayerController)
                return;

            // // Try to the game impostor game mode
            // if (!UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
            //     return;

            // Do the door cutscene
            StartCoroutine(DoorCutscene(pawn));
        }

        private IEnumerator DoorCutscene(JesterGamePawn pawn)
        {
            pawn.SetMovementEnabled(false);

            yield return doneTalkingScreenDataAsset.OpenScreen();

            // Move the pawn to the other side of the door
            pawn.transform.position = otherSideSpawn.position;
            pawn.transform.rotation = otherSideSpawn.rotation;

            yield return doneTalkingScreenDataAsset.CloseScreen();

            pawn.SetMovementEnabled(true);
        }

        private void OnDrawGizmos()
        {
            // Draw a sphere at the position of the other side spawn
            if (!otherSideSpawn)
                return;

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(otherSideSpawn.position, 0.5f);
        }
    }
}