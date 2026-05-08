using System.Collections;
using UnityEngine;

namespace JesterGame.Code.Scripts.Characters.Behaviors.Assets
{
    public abstract class CharacterBehaviorDataAsset : ScriptableObject, ICharacterBehavior
    {
        public abstract IEnumerator OngoingCoroutine(JesterGamePawn pawn);

        public abstract string GetBehaviorName { get; }
    }
}