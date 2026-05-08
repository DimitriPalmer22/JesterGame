using AYellowpaper;
using UnityEngine;

namespace JesterGame.Code.Scripts.Characters.Behaviors.Brains
{
    [CreateAssetMenu(
        fileName = "Brain_Innocent",
        menuName = "JesterGame/Character Behaviors/Brains/Innocent Character Brain")
    ]
    public class InnocentCharacterBrain : CharacterBehaviorBrain
    {

        public override InterfaceReference<ICharacterBehavior>[] DetermineBehavior(JesterGamePawn pawn)
        {
            var behaviorDelegates = BehaviorDelegates();

            // Get a random behavior delegate and execute it to get the behavior(s) to perform
            var randomDelegate = behaviorDelegates[Random.Range(0, behaviorDelegates.Length)];
            return randomDelegate(pawn);
        }
    }
}