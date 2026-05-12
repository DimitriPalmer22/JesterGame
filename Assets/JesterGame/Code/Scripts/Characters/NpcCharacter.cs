using System;
using System.Collections;
using System.Linq;
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
                gameMode.GetCharacterInstance(npcDataHandle.RowName, out var characterInstance) &&
                characterInstance.characterType == CharacterType.Impostor
               )
            {
                foreach (var cRenderer in impostorMaterialRenderers)
                    cRenderer.material = impostorMaterial;
            }
        }

        private void Update()
        {
            animator.SetFloat(APCurrentVelocity, agent.velocity.magnitude);
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

            _speakingCoroutine = StartCoroutine(SpeakCoroutine(characterData, args));
        }

        private IEnumerator SpeakCoroutine(DialogueCharacter characterData, InteractEventArgs args)
        {
            // Deactivate the interaction helper component
            interactionHelperComponent.enabled = false;

            // Deactivate interactor component on the player character to prevent multiple interactions while the dialogue is open.
            args.interactor.enabled = false;

            // Stop the current behavior coroutine
            StopMainBehaviorCoroutine(false);
            agent.isStopped = true;
            var prevAcceleration = agent.acceleration;
            agent.acceleration = 10000;

            // Start a coroutine to look at the player. DON'T yield for it, just start if. End it after closing.
            var lookAtCoroutine = StartCoroutine(TurnTowards(args.interactor.transform, agent.angularSpeed));

            // Determine which lines to play. For now, just play the test data asset.
            // Wait for the dialogue panel to be complete.
            var currentDialogueGraph = DetermineCurrentDialogue(characterData);
            if (currentDialogueGraph != null)
            {
                yield return OpenDialoguePanel(currentDialogueGraph);

                // Mark the first interaction type for this room as complete
                if (
                    UtilLibrary.GetGameMode(out ImpostorGameMode gameMode) &&
                    gameMode.pawnToRoomMap.TryGetValue(this, out var currentRoom)
                )
                {
                    var characterInstance = gameMode.characterInstanceMap[npcDataHandle.RowName];
                    characterInstance.hasCompletedFirstInteractionPerRoom[currentRoom] = true;
                    gameMode.characterInstanceMap[npcDataHandle.RowName] = characterInstance;
                }
            }

            if (lookAtCoroutine != null)
                StopCoroutine(lookAtCoroutine);

            // Restart the current behavior coroutine.
            agent.isStopped = false;
            agent.acceleration = prevAcceleration;
            StartMainBehaviorCoroutine(false);

            // Re-activate the interaction helper component
            interactionHelperComponent.enabled = true;

            // Re-activate the interactor component on the player character
            args.interactor.enabled = true;

            _speakingCoroutine = null;
        }

        private IEnumerator OpenDialoguePanel(RuntimeDialogueGraph dialogueGraph)
        {
            if (dialogueScreenDataAsset)
                yield return StartCoroutine(
                    dialogueScreenDataAsset.RunDialogueScreen(npcDataHandle.RowName, dialogueGraph)
                );

            yield return null;
        }

        private IEnumerator TurnTowards(Transform lookAt, float turningRate)
        {
            // Smoothly rotate towards the target rotation
            while (true)
            {
                // Get the rotation from the direction
                var direction  = lookAt.position - transform.position;
                direction.y = 0;
                var targetRotation = Quaternion.LookRotation(direction, Vector3.up);

                var turnDelta = turningRate * Time.deltaTime;
                var newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnDelta);
                transform.rotation = newRotation;
                yield return null;
            }

            yield break;

            // Function to test if the angle is within an acceptable range.
            bool AngleWithinRange(Quaternion a, Quaternion b)
            {
                return false;
                // return Mathf.Abs(Quaternion.Angle(a, b)) <= 0.1f;
            }
        }

        public RuntimeDialogueGraph DetermineCurrentDialogue(DialogueCharacter characterData)
        {
            // Get the game mode to access the pawn to room map
            if (!UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
                return null;

            if (!gameMode.characterInstanceMap.TryGetValue(npcDataHandle.RowName, out var characterInstance))
                return null;

            // Check to see if the character is the impostor
            var bIsImpostor = characterInstance.characterType == CharacterType.Impostor;
            var currentPool = bIsImpostor
                ? characterData.impostorPoolAsset
                : characterData.innocentPoolAsset;

            if (!currentPool)
                return null;

            if (!gameMode.pawnToRoomMap.TryGetValue(this, out var currentRoom))
                return null;

            // Check to see if the character has already been interacted with while in this room.
            characterInstance.hasCompletedFirstInteractionPerRoom.TryGetValue(
                currentRoom,
                out var bCompletedRoomInteraction
            );

            // TODO: Remove the dialogue lines that have already been played!!!
            if (bCompletedRoomInteraction)
                return currentPool.Data.randomInteractions
                    .Where(n => n != null)
                    .GetRandom();

            return currentPool.Data.firstInteractionPerRoom[currentRoom];
        }

        public override void Die()
        {
            base.Die();

            // Disable the interaction helper
            interactionHelperComponent.enabled = false;

            var navAgent = GetComponent<NavMeshAgent>();
            agent.isStopped = true;
            agent.enabled = false;

            // stop the behavior
            StopMainBehaviorCoroutine(true);
        }
    }
}