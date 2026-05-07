using System.Collections;
using UnrealToUnity.Code.Scripts.Core.Utility.Interfaces;

namespace JesterGame.Code.Scripts.Characters
{
    public interface ICharacterBehavior : IOngoing<JesterGamePawn>

    {
    // /// <summary>
    // /// A coroutine that defines the behavior of a character.
    // /// This could include movement, interactions, or any other actions the character should perform.
    // /// This should really only be used for AI characters, but it can potentially be used for players, too.
    // /// </summary>
    // /// <param name="character">The character to act upon</param>
    // /// <returns></returns>
    // public IEnumerator PerformCharacterBehavior(JesterGamePawn character);
    }
}