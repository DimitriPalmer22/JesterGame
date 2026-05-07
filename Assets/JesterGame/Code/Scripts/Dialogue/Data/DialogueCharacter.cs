using System;
using NaughtyAttributes;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.DataTables;

namespace JesterGame.Code.Scripts.Dialogue.Data
{
    [Serializable]
    public struct DialogueCharacter : IDataTableRow
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


        public string GetDataTableRowName => name;
    }
}