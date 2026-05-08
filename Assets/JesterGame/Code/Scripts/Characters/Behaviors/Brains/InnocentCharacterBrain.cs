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
            throw new System.NotImplementedException();
        }
    }
}