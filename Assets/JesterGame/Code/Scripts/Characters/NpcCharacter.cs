using System.Collections;
using JesterGame.Code.Scripts.Core.Interaction;
using JesterGame.Code.Scripts.Dialogue.Data;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.AI;
using UnrealToUnity.Code.Scripts.Core.DataTables;
using UnrealToUnity.Code.Scripts.Core.Pawns;

namespace JesterGame.Code.Scripts.Characters
{
    [RequireComponent(typeof(AutoPossessor))]
    public class NpcCharacter : Pawn
    {
        /// <summary>
        /// The row handle corresponding to the NPC data in the data table.
        /// </summary>
        [SerializeField] private DataTableRowHandle npcDataHandle;

        /// <summary>
        ///
        /// </summary>
        [SerializeField] private InteractionHelperComponent interactionHelperComponent;

        private Coroutine _speakingCoroutine;

        protected override void Awake()
        {
            base.Awake();

            // Set the text of the interaction helper component
            if (interactionHelperComponent && npcDataHandle.GetValue(out DialogueCharacter characterData))
                interactionHelperComponent.SetInteractionText($"Speak with {characterData.name}");
        }

        public void StartSpeaking()
        {
            if (!TryGetCharacterData(out var characterData))
                return;
            _speakingCoroutine = StartCoroutine(SpeakCoroutine(characterData));
        }

        private IEnumerator SpeakCoroutine(DialogueCharacter characterData)
        {
            Debug.Log($"{characterData.name} is starting to speak!");
            yield return null;
            Debug.Log($"{name} has finished speaking");

            _speakingCoroutine = null;
        }

        private bool TryGetCharacterData(out DialogueCharacter characterData)
        {
            if (npcDataHandle.GetValue(out characterData))
                return true;

            Debug.LogError($"Failed to get character data for NPC {name} with handle {npcDataHandle}");
            return false;
        }
    }
}