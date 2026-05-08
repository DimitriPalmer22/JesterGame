using System.Collections;
using JesterGame.Code.Scripts.Core;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Characters.Behaviors.Assets
{
    public abstract class FollowCharacterBehavior : CharacterBehaviorDataAsset
    {
        /// <summary>
        /// How long before the pawn should stop following the character and pick a new behavior?
        /// If left empty, the pawn will follow indefinitely.
        /// </summary>
        [SerializeField] private Optional<float> timeOut = 10f;

        [SerializeField] private float repathInterval = 0.5f;

        protected abstract string DetermineCharacter(JesterGamePawn pawn);

        public override IEnumerator OngoingCoroutine(JesterGamePawn pawn)
        {
            var characterName = DetermineCharacter(pawn);
            if (string.IsNullOrEmpty(characterName))
                yield break;

            if (!UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
                yield break;

            // Get the character pawn from the game mode
            if (!gameMode.GetCharacterPawn(characterName, out var targetPawn))
                yield break;

            // Try to get the nav mesh agent component from the pawn
            if (!pawn.TryGetComponent(out UnityEngine.AI.NavMeshAgent navMeshAgent))
                yield break;

            var startTime = Time.time;
            var endTime = startTime + timeOut.GetValueUnsafe;

            var currentMoveCoroutine = pawn.StartCoroutine(navMeshAgent.MoveToCoroutine(targetPawn.transform.position));

            while (
                !navMeshAgent.IsAtDestination() &&
                targetPawn != null && pawn != null && navMeshAgent &&
                (timeOut.HasValue && Time.time < endTime || !timeOut.HasValue)
            )
            {
                // Wait a little before repathing
                yield return new WaitForSecondsRealtime(repathInterval);

                // Update the destination of the nav mesh agent
                navMeshAgent.SetDestination(targetPawn.transform.position);
            }

            // Stop the move coroutine if it is currently active.
            if (currentMoveCoroutine != null)
                pawn.StopCoroutine(currentMoveCoroutine);
        }
    }
}