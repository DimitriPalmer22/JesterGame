using System.Collections;
using AYellowpaper;
using UnityEngine;

namespace JesterGame.Code.Scripts.Characters.Behaviors.PointsOfInterest
{
    public class InspectorPointOfInterest : PointOfInterest
    {
        [SerializeField] private InterfaceReference<ICharacterBehavior> behaviorAtPointOfInterest;

        protected override IEnumerator DoAtPointOfInterest(JesterGamePawn args)
        {
            if (behaviorAtPointOfInterest != null && behaviorAtPointOfInterest.Value != null)
                yield return StartCoroutine(behaviorAtPointOfInterest.Value.OngoingCoroutine(args));
        }
    }
}