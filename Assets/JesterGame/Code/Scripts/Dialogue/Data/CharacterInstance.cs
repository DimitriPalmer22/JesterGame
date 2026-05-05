using System;
using NaughtyAttributes;
using UnityEngine;

namespace JesterGame.Code.Scripts.Dialogue.Data
{
    [Serializable]
    public struct CharacterInstance
    {
        /// <summary>
        /// A reference to the dialogue character this instance handles.
        /// </summary>
        [SerializeField, Required] public DialogueCharacterAsset characterAsset;

        /// <summary>
        /// The type of the character, whether it's an impostor or not.
        /// </summary>
        [SerializeField] public CharacterType characterType;

        /// <summary>
        /// A bool indicating whether is character is alive or not.
        /// Should not really be changed in the inspector.
        /// </summary>
        [SerializeField, ReadOnly] private bool bIsAlive;

        public const int MAX_AFFECTION = 10;

        [SerializeField, ProgressBar("Affection", MAX_AFFECTION, EColor.Green)]
        public int affection;

        public CharacterInstance(DialogueCharacterAsset characterAsset, CharacterType characterType)
        {
            this.characterAsset = characterAsset;
            this.characterType = characterType;
            bIsAlive = true;
            affection = 0;
        }
    }
}