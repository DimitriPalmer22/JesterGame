using AYellowpaper;
using UnityEngine;

namespace JesterGame.Code.Scripts.Characters.Behaviors.Brains
{
    public abstract class CharacterBehaviorBrain : ScriptableObject
    {
        protected delegate InterfaceReference<ICharacterBehavior>[] BehaviorDelegate(JesterGamePawn pawn);

        #region Behaviorsa

        [SerializeField] protected InterfaceReference<ICharacterBehavior> idleBehavior;
        [SerializeField] protected InterfaceReference<ICharacterBehavior> waitBetweenBehavior;
        [SerializeField] protected InterfaceReference<ICharacterBehavior> randomPointBehavior;

        #endregion

        protected virtual BehaviorDelegate[] BehaviorDelegates()
        {
            return new BehaviorDelegate[]
            {
                Behavior_Wait,
                Behavior_RandomPointOfInterest
            };
        }

        public abstract InterfaceReference<ICharacterBehavior>[] DetermineBehavior(JesterGamePawn pawn);

        #region MyRegion

        protected InterfaceReference<ICharacterBehavior>[] Behavior_Wait(JesterGamePawn pawn)
        {
            return new[] { idleBehavior };
        }

        protected InterfaceReference<ICharacterBehavior>[] Behavior_RandomPointOfInterest(JesterGamePawn pawn)
        {
            return new[] { randomPointBehavior, waitBetweenBehavior };
        }

        #endregion
    }
}