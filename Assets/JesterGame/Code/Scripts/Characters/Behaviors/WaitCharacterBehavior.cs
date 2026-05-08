using System.Collections;
using UnityEngine;

namespace JesterGame.Code.Scripts.Characters.Behaviors
{
    [CreateAssetMenu(fileName = "CB_Wait_", menuName = "JesterGame/Character Behaviors/Wait Character Behavior")]
    public class WaitCharacterBehavior : CharacterBehaviorDataAsset
    {
        [SerializeField, Delayed] public float minWaitTime = 1;
        [SerializeField, Delayed] public float maxWaitTime = 1;

        public override IEnumerator OngoingCoroutine(JesterGamePawn pawn)
        {
            var randomWaitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(randomWaitTime);
        }

        public override string GetBehaviorName => $"WaitCharacterBehavior ([{minWaitTime} - {maxWaitTime}] seconds)";
    }
}