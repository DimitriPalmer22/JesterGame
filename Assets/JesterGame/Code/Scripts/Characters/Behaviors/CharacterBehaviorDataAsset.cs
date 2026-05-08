using System.Collections;
using UnityEngine;

namespace JesterGame.Code.Scripts.Characters.Behaviors
{
    public abstract class CharacterBehaviorDataAsset : ScriptableObject, ICharacterBehavior
    {
        public abstract IEnumerator OngoingCoroutine(JesterGamePawn args);

        public abstract string GetBehaviorName { get; }
    }
}