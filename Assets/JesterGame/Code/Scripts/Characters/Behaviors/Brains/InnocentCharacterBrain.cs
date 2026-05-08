using AYellowpaper;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Utility;

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
            // Get a random option from the list of weighted options.
            var randomBehaviorDefinition = characterBehaviorDefinitions.GetRandomWeightedOption();
            return randomBehaviorDefinition.behaviors;
        }
    }
}