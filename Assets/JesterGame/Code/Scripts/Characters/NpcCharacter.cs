using System.Collections;
using JesterGame.Code.Scripts.Core.Interaction;
using JesterGame.Code.Scripts.Dialogue.Data;
using JesterGame.Code.Scripts.Dialogue.UI;
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

        /// <summary>
        /// Data asset for opening the dialogue screen.
        /// </summary>
        [SerializeField] private DialogueScreenDataAsset dialogueScreenDataAsset;

        [SerializeField] private DialogueDataAsset testDataAsset;

        private Coroutine _speakingCoroutine;

        protected override void Awake()
        {
            base.Awake();

            // Set the text of the interaction helper component
            if (interactionHelperComponent && npcDataHandle.GetValue(out DialogueCharacter characterData))
                interactionHelperComponent.SetInteractionText($"Speak with {characterData.name}");
        }

        public void StartSpeaking(InteractEventArgs args)
        {
            if (!TryGetCharacterData(out var characterData))
                return;

            // Stop the speaking coroutine
            if (_speakingCoroutine != null)
            {
                StopCoroutine(_speakingCoroutine);
                _speakingCoroutine = null;
            }

            // TODO: Determine which lines to play. For now, just play the test data asset.

            _speakingCoroutine = StartCoroutine(SpeakCoroutine(characterData, args));
        }

        private IEnumerator SpeakCoroutine(DialogueCharacter characterData, InteractEventArgs args)
        {
            // Deactivate the interaction helper component
            interactionHelperComponent.enabled = false;

            // Deactivate interactor component on the player character to prevent multiple interactions while the dialogue is open.
            args.interactor.enabled = false;

            // Wait for the dialogue panel to be complete.
            yield return OpenDialoguePanel();

            // Re-activate teh interaction helper component
            interactionHelperComponent.enabled = true;

            // Re-activate the interactor component on the player character
            args.interactor.enabled = true;

            _speakingCoroutine = null;
        }

        private bool TryGetCharacterData(out DialogueCharacter characterData)
        {
            if (npcDataHandle.GetValue(out characterData))
                return true;

            Debug.LogError($"Failed to get character data for NPC {name} with handle {npcDataHandle}");
            return false;
        }

        private IEnumerator OpenDialoguePanel()
        {
            if (dialogueScreenDataAsset)
                yield return StartCoroutine(dialogueScreenDataAsset.RunDialogueScreen(testDataAsset.dialogueLines));

            yield return null;
        }
    }
}