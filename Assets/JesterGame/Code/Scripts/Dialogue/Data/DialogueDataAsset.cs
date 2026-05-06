using JesterGame.Code.Scripts.Dialogue.DialogueLines;
using NaughtyAttributes;
using UnityEngine;

namespace JesterGame.Code.Scripts.Dialogue.Data
{
    /// <summary>
    /// ScriptableObject that holds dialogue data, which can be used to create dialogue sequences in the game.
    /// </summary>
    [CreateAssetMenu(fileName = "New Dialogue Data", menuName = "JesterGame/Dialogue/Dialogue Data")]
    public class DialogueDataAsset : ScriptableObject
    {
        /// <summary>
        /// The lines found within this dialogue.
        /// </summary>
        [SerializeField, AllowNesting] public DialogueLineWrapper[] dialogueLines;
    }
}