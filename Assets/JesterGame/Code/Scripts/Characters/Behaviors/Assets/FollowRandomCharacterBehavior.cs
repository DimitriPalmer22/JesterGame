using System.Linq;
using JesterGame.Code.Scripts.Core;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Characters.Behaviors.Assets
{
    [CreateAssetMenu(
        fileName = "CB_Follow_RandomCharacter",
        menuName = "JesterGame/Character Behaviors/Follow/Random Character")
    ]
    public class FollowRandomCharacterBehavior : FollowCharacterBehavior
    {
        public override string GetBehaviorName => $"FollowRandomCharacterBehavior";

        protected override string DetermineCharacter(JesterGamePawn pawn)
        {
            if (!UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
                return null;

            var characterPawns = gameMode
                .characterNameToPawnMap
                // Only get alive, non-main characters and characters that are not the current pawn.
                .Where(n =>
                    n.Value.TryGetCharacterData(out var characterData) &&
                    !characterData.bIsMainCharacter &&
                    !n.Value.IsDead &&
                    n.Value != pawn
                )
                .ToArray();

            if (characterPawns.Length == 0)
                return null;

            var randomIndex = Random.Range(0, characterPawns.Length);
            return characterPawns[randomIndex].Key;
        }
    }
}