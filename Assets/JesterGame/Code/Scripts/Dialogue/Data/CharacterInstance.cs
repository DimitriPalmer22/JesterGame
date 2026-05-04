using System;
using UnityEngine;

namespace JesterGame.Code.Scripts.Dialogue.Data
{
    [Serializable]
    public struct CharacterInstance
    {
        /// <summary>
        /// A reference to the dialogue character this instance handles.
        /// </summary>
        [SerializeField] public DialogueCharacterAsset characterAsset;

        /// <summary>
        /// The type of the character, whether it's an impostor or not.
        /// </summary>
        [SerializeField] public CharacterType characterType;

        /// <summary>
        /// A bool indicating whether is character is alive or not.
        /// Should not really be changed in the inspector.
        /// </summary>
        [SerializeField] public bool bIsAlive;

        public CharacterInstance(DialogueCharacterAsset characterAsset, CharacterType characterType)
        {
            this.characterAsset = characterAsset;
            this.characterType = characterType;
            bIsAlive = true;
        }
    }
}