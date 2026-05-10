using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnrealToUnity.Code.Scripts.Core.DataTables;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Characters.Behaviors.PointsOfInterest
{
    public abstract class PointOfInterest : MonoBehaviour, ICharacterBehavior
    {
        [SerializeField] private Transform pointTransform;
        [SerializeField] private DataTableRowHandle pointOfInterestDataHandle;

        public DataTableRowHandle PointOfInterestDataHandle => pointOfInterestDataHandle;

        /// <summary>
        /// Move toward this point of interest.
        /// Then, do whatever at the point of interest.
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns></returns>
        public IEnumerator OngoingCoroutine(JesterGamePawn pawn)
        {
            yield return MoveToPointOfInterest(pawn);

            yield return DoAtPointOfInterest(pawn);
        }

        private IEnumerator MoveToPointOfInterest(JesterGamePawn args)
        {
            // Try to get the nav mesh agent component from the pawn
            if (!args.TryGetComponent(out NavMeshAgent navMeshAgent))
                yield break;

            // Wait until the nav mesh agent is done with its current path
            yield return navMeshAgent.MoveToCoroutine(GetTransform.position);
        }

        protected abstract IEnumerator DoAtPointOfInterest(JesterGamePawn args);

        public string GetBehaviorName => $"Point of interest: {gameObject.name}";

        private void OnDrawGizmosSelected()
        {
            // Draw a small sphere at the point
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GetTransform.position, 0.5f);
        }

        public Transform GetTransform => pointTransform != null ? pointTransform : transform;
    }
}