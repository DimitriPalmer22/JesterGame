using System.Collections;
using UnityEngine;

namespace JesterGame.Code.Scripts.Characters.Behaviors
{
    public class TestNpcCharacterBehavior : MonoBehaviour, ICharacterBehavior
    {
        public IEnumerator OngoingCoroutine(JesterGamePawn pawn)
        {
            var randomWaitTime = UnityEngine.Random.Range(0.5f, 5f);
            yield return new WaitForSeconds(randomWaitTime);
        }

        public string GetBehaviorName => $"TestNpcCharacterBehavior";
    }
}