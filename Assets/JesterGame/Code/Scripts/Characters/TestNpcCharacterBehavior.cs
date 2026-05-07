using System.Collections;
using UnityEngine;

namespace JesterGame.Code.Scripts.Characters
{
    public class TestNpcCharacterBehavior : MonoBehaviour, ICharacterBehavior
    {
        public IEnumerator OngoingCoroutine(JesterGamePawn character)
        {
            var randomWaitTime = UnityEngine.Random.Range(0.5f, 5f);
            Debug.LogWarning($"{character.name} is waiting for {randomWaitTime} seconds");

            yield return new WaitForSeconds(randomWaitTime);

            Debug.LogWarning($"{character.name} is done waiting!");
        }
    }
}