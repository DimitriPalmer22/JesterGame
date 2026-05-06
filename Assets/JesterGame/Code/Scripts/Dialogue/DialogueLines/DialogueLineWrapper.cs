using System;
using NaughtyAttributes;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.DataTables;

namespace JesterGame.Code.Scripts.Dialogue.DialogueLines
{
    /// <summary>
    /// Wrapper around dialogue line.
    /// Either represents a simple dialogue line or a dialogue choice.
    /// </summary>
    [Serializable]
    public struct DialogueLineWrapper
    {
        /// <summary>
        /// Row handle pointing to the speaker.
        /// </summary>
        [SerializeField] public DataTableRowHandle speaker;

        /// <summary>
        /// The text of the dialogue line.
        /// </summary>
        [SerializeField] public string text;

        [SerializeField, AllowNesting]
        public DialogueChoice[] choices;
    }
}