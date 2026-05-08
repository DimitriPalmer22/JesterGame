using System.Collections;
using UnityEngine;

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
            yield return new WaitForEndOfFrame();
        }

        protected abstract IEnumerator DoAtPointOfInterest(JesterGamePawn args);

        public string GetBehaviorName => $"Point of interest: {gameObject.name}";
    }
}