using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace JesterGame.Code.Scripts.Characters.Behaviors.PointsOfInterest
{
    public abstract class PointOfInterest : MonoBehaviour, ICharacterBehavior
    {
        /// <summary>
        /// Move toward this point of interest.
        /// Then, do whatever at the point of interest.
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public IEnumerator OngoingCoroutine(JesterGamePawn pawn)
        {
            yield return StartCoroutine(MoveToPointOfInterest(pawn));

            yield return StartCoroutine(DoAtPointOfInterest(pawn));
        }

        private IEnumerator MoveToPointOfInterest(JesterGamePawn args)
        {
            // Try to get the nav mesh agent component from the pawn
            if (!args.TryGetComponent(out NavMeshAgent navMeshAgent))
                yield break;

            // Set the destination of the nav mesh agent to the position of this point of interest
            navMeshAgent.SetDestination(transform.position);

            // Wait until the nav mesh agent has reached the destination
            while (navMeshAgent.pathPending || navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
                yield return null;
        }

        protected abstract IEnumerator DoAtPointOfInterest(JesterGamePawn args);

        public string GetBehaviorName => $"Point of interest: {gameObject.name}";

        private void OnDrawGizmosSelected()
        {
            // Draw a small sphere at the point
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}