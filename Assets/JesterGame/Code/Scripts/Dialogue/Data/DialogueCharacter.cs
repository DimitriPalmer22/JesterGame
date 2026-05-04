using System;
using UnityEngine;

namespace JesterGame.Code.Scripts.Dialogue.Data
{
    [Serializable]
    public struct DialogueCharacter
    {
        /// <summary>
        /// The display name of the character.
        /// </summary>
        [SerializeField] public string name;

        /// <summary>
        /// The portrait of the character to display in the dialogue UI.
        /// </summary>
        [SerializeField] public Texture2D portrait;
    }
}