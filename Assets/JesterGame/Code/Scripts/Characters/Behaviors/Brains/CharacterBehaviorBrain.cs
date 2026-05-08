using AYellowpaper;
using UnityEngine;

namespace JesterGame.Code.Scripts.Characters.Behaviors.Brains
{
    public abstract class CharacterBehaviorBrain : ScriptableObject
    {
        public abstract InterfaceReference<ICharacterBehavior>[] DetermineBehavior(JesterGamePawn pawn);
    }
}