using AYellowpaper;
using UnityEngine;

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
            return new[] { waitBetweenBehavior };
        }
    }
}