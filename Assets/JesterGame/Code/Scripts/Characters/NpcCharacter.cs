using System;
using System.Collections;
using JesterGame.Code.Scripts.Core;
using JesterGame.Code.Scripts.Core.Interaction;
using JesterGame.Code.Scripts.Dialogue.Data;
using JesterGame.Code.Scripts.Dialogue.DialogueGraph.Runtime;
using JesterGame.Code.Scripts.Dialogue.UI;
using UnityEngine;
using UnityEngine.AI;
using UnrealToUnity.Code.Scripts.Core.AI;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Characters
{
    [RequireComponent(typeof(AutoPossessor))]
    public class NpcCharacter : JesterGamePawn
    {
        /// <summary>
        ///
        /// </summary>
        [SerializeField] private InteractionHelperComponent interactionHelperComponent;

        /// <summary>
        /// Data asset for opening the dialogue screen.
        /// </summary>
        [SerializeField] private DialogueScreenDataAsset dialogueScreenDataAsset;

        [SerializeField] private RuntimeDialogueGraph testDialogueGraph;

        [SerializeField] private NavMeshAgent agent;

        [SerializeField] private Renderer[] impostorMaterialRenderers;
        [SerializeField] public Material impostorMaterial;

        private Coroutine _speakingCoroutine;

        protected override void Awake()
        {
            base.Awake();

            // Set the text of the interaction helper component
            if (interactionHelperComponent && npcDataHandle.GetValue(out DialogueCharacter characterData))
                interactionHelperComponent.SetInteractionText($"Speak with {characterData.name}");
        }

        private void Start()
        {
            // If this is the impostor, apply the impostor material to the renderer
            if (UtilLibrary.GetGameMode(out ImpostorGameMode gameMode) &&
                gameMode.GetCharacterInstance(npcDataHandle.rowName, out var characterInstance) &&
                characterInstance.characterType == CharacterType.Impostor
               )
            {
                foreach (var cRenderer in impostorMaterialRenderers)
                    cRenderer.material = impostorMaterial;
            }
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

            // Stop the current behavior coroutine
            StopMainBehaviorCoroutine();
            agent.isStopped = true;
            var prevAcceleration = agent.acceleration;
            agent.acceleration = 10000;

            // TODO: Start a coroutine to look at the player. DON'T yield for it, just start if. End it after closing.

            // Wait for the dialogue panel to be complete.
            yield return OpenDialoguePanel();

            // Restart the current behavior coroutine.
            agent.isStopped = false;
            agent.acceleration = prevAcceleration;
            StartMainBehaviorCoroutine();

            // Re-activate the interaction helper component
            interactionHelperComponent.enabled = true;

            // Re-activate the interactor component on the player character
            args.interactor.enabled = true;

            _speakingCoroutine = null;
        }

        private IEnumerator OpenDialoguePanel()
        {
            if (dialogueScreenDataAsset)
                yield return StartCoroutine(
                    dialogueScreenDataAsset.RunDialogueScreen(npcDataHandle.rowName, testDialogueGraph)
                );

            yield return null;
        }
    }
}