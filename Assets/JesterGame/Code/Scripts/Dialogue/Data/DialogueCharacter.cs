using System;
using NaughtyAttributes;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.DataTables;

namespace JesterGame.Code.Scripts.Dialogue.Data
{
    [Serializable]
    public struct DialogueCharacter
    {
        /// <summary>
        /// The display name of the character.
        /// </summary>
        [SerializeField] public string name;

        [SerializeField] public bool bIsMainCharacter;

        /// <summary>
        /// The portrait of the character to display in the dialogue UI.
        /// </summary>
        [SerializeField] public Sprite portrait;

        /// <summary>
        /// Idea?
        /// Small, tiny little portrait that appears next to dialogue box.
        /// </summary>
        [SerializeField] public Sprite miniPortrait;

        /// <summary>
        /// The dialogue pool to use for this character.
        /// This will determine the dialogue interactions that can occur with this character.
        /// </summary>
        [SerializeField] public DialoguePoolDataAsset innocentPoolAsset;

        [SerializeField] public DialoguePoolDataAsset impostorPoolAsset;


        public string GetDataTableRowName => name;
    }
}