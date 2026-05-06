using System;
using JesterGame.Code.Scripts.Dialogue.Data;
using UnityEngine;

namespace JesterGame.Code.Scripts.Dialogue.DialogueLines
{
    [Serializable]
    public struct DialogueChoice
    {
        /// <summary>
        /// The text of the dialogue choice.
        /// </summary>
        [SerializeField] public string text;

        /// <summary>
        /// The affection value associated with this choice, which can affect character relationships or story outcomes.
        /// </summary>
        [SerializeField] public int affectionValue;

        /// <summary>
        /// A reference to the next set of lines to play as a result of this choice.
        /// Normally, there *would* be an array here to handle this,
        /// but Unity's inspector really does not like nested arrays.
        /// </summary>
        [SerializeField] public DialogueDataAsset nextLines;
    }
}