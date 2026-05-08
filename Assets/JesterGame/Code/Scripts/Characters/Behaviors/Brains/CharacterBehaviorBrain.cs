using AYellowpaper;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Characters.Behaviors.Brains
{
    public abstract class CharacterBehaviorBrain : ScriptableObject
    {
        #region Behaviors

        [SerializeField] protected CharacterBehaviorDefinition[] characterBehaviorDefinitions;

        #endregion

        public abstract InterfaceReference<ICharacterBehavior>[] DetermineBehavior(JesterGamePawn pawn);
    }
}