using AYellowpaper;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Characters.Behaviors.Brains
{
    [CreateAssetMenu(
        fileName = "Brain_Impostor",
        menuName = "JesterGame/Character Behaviors/Brains/Impostor Brain")
    ]
    public class ImpostorBrain : CharacterBehaviorBrain
    {
        public override InterfaceReference<ICharacterBehavior>[] DetermineBehavior(JesterGamePawn pawn)
        {
            // TODO: Create a more intricate version of this later on.

            // Get a random option from the list of weighted options.
            var randomBehaviorDefinition = characterBehaviorDefinitions.GetRandomWeightedOption();
            return randomBehaviorDefinition.behaviors;
        }
    }
}