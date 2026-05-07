using System.Collections;
using System.Collections.Generic;
using JesterGame.Code.Scripts.Dialogue.Data;
using JesterGame.Code.Scripts.Dialogue.DialogueGraph.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnrealToUnity.Code.Scripts.Core.UserInterface;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Dialogue.UI
{
    public class DialogueScreen : UIScreen
    {
        #region Fields

        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private Image characterImage;

        [SerializeField] private VerticalLayoutGroup choicesContainer;
        [SerializeField] private DialogueScreenButton choiceButtonPrefab;

        [SerializeField] private float wordDelay = 0.25f;

        #endregion

        #region Private Fields

        private RuntimeDialogueGraph _currentDialogueGraph;
        private string _currentNodeID;
        private WaitForDialogueChoiceSelection _waitForChoiceSelection;

        #endregion


        protected override void CustomInitialize()
        {
        }

        protected override void CustomStart()
        {
        }

        protected override IEnumerator OpenScreenCoroutine()
        {
            DestroyChoices();

            // Get the end time based on the length of the curve.
            var beginTime = Time.unscaledTime;
            var endTime = beginTime + opacityCurve.keys[opacityCurve.length - 1].time;

            while (Time.unscaledTime < endTime)
            {
                var currentTime = Time.unscaledTime - beginTime;
                canvasGroup.alpha = opacityCurve.Evaluate(currentTime);
                yield return null;
            }

            canvasGroup.alpha = opacityCurve.Evaluate(opacityCurve.keys[opacityCurve.length - 1].time);
        }

        protected override IEnumerator CloseScreenCoroutine()
        {
            var beginTime = Time.unscaledTime;
            var endTime = beginTime + opacityCurve.keys[opacityCurve.length - 1].time;

            while (Time.unscaledTime < endTime)
            {
                var currentTime = Time.unscaledTime - beginTime;

                // Reverse the current time
                currentTime = opacityCurve.keys[opacityCurve.length - 1].time - currentTime;
                canvasGroup.alpha = opacityCurve.Evaluate(currentTime);
                yield return null;
            }

            canvasGroup.alpha = opacityCurve.Evaluate(opacityCurve.keys[0].time);
        }

        /// <summary>
        /// Set the dialogue lines to be displayed on the screen.
        /// This should be called before opening the screen.
        /// </summary>
        /// <param name="dialogueGraph">Dialogue graph asset for the interaction</param>
        private void SetDialogueInteraction(RuntimeDialogueGraph dialogueGraph)
        {
            // Set the current dialogue graph and current node
            _currentDialogueGraph = dialogueGraph;
            _currentNodeID = dialogueGraph.entryNodeID;

            // Initialize the text and image to be empty.
            dialogueText.text = string.Empty;
            nameText.text = string.Empty;
            characterImage.sprite = null;
        }

        public IEnumerator RunDialogueCoroutine(RuntimeDialogueGraph dialogueGraph)
        {
            SetDialogueInteraction(dialogueGraph);

            // Open the screen
            yield return StartCoroutine(OpenScreenCoroutine());

            // Disable player input while the dialogue is running.
            // Disable it specifically for the pawn so the player controller can still get input.
            var controlledPawn = UtilLibrary.GetPlayerController()?.ControlledPawn;
            controlledPawn?.AddInputBlocker(this);

            while (!string.IsNullOrEmpty(_currentNodeID) && _currentDialogueGraph)
            {
                // Get the runtime dialogue node struct from the current ID
                if (!_currentDialogueGraph.TryGetNodeByID(_currentNodeID, out var currentNode))
                {
                    Debug.LogError($"Dialogue node with ID {_currentNodeID} not found in dialogue graph!");
                    break;
                }

                // TODO: Animate or something.
                // Set the current text and speaker.
                if (currentNode.speaker.GetValue(out DialogueCharacter characterData))
                {
                    nameText.text = characterData.name;
                    characterImage.sprite = characterData.portrait;
                }
                else
                {
                    nameText.text = string.Empty;
                    characterImage.sprite = null;
                }

                // Display the text of the current line.
                yield return StartCoroutine(DisplayWordsInCurrentLine(currentNode));

                // If there are choices,
                // 2. wait for selection
                // 3. add the choice's nextLines back to the start of the dialogue lines.
                if (currentNode.choiceStrings.Count > 0)
                {
                    CreateChoices(currentNode);

                    // Wait for selection.
                    _waitForChoiceSelection = new WaitForDialogueChoiceSelection();
                    yield return _waitForChoiceSelection;

                    // Set the current node ID to the next node ID corresponding to the selected choice.
                    _currentNodeID = currentNode.nextNodeIDs[_waitForChoiceSelection.SelectionIndex];
                    _waitForChoiceSelection = null;

                    DestroyChoices();
                }

                // Otherwise, just go to the next node.
                else
                {
                    _currentNodeID = currentNode.nextNodeIDs?.Count > 0
                        ? currentNode.nextNodeIDs[0]
                        : string.Empty;

                    // TODO: Wait for input
                    yield return new WaitForSecondsRealtime(1f);
                }
            }

            // Re-enable player input while dialogue is running.
            controlledPawn?.RemoveInputBlocker(this);

            // Close the screen
            yield return StartCoroutine(CloseScreenCoroutine());

            yield return null;
        }

        private IEnumerator DisplayWordsInCurrentLine(RuntimeDialogueNode currentNode)
        {
            // Split the current line by spaces.
            var splitLine = currentNode.dialogueText.Split(' ');

            // Clear the current text.
            dialogueText.text = string.Empty;

            // Word by word, add the words to the current text.
            for (var index = 0; index < splitLine.Length; index++)
            {
                yield return new WaitForSecondsRealtime(wordDelay);

                // Add the current word
                var word = splitLine[index];
                dialogueText.text += $"{word}";

                // Add the space if not the last word.
                if (index < splitLine.Length - 1)
                    dialogueText.text += " ";
            }
        }

        private void DestroyChoices()
        {
            // Clear the children from the choices parent.
            // Create a copy array containing all the children.
            List<GameObject> currentChoices = new List<GameObject>();

            foreach (Transform child in choicesContainer.transform)
                currentChoices.Add(child.gameObject);

            choicesContainer.transform.DetachChildren();
            foreach (var currentChoice in currentChoices)
                Destroy(currentChoice);
        }

        private void CreateChoices(RuntimeDialogueNode currentNode)
        {
            // For each choice, instantiate a new button and set the text to the choice string.
            for (var index = 0; index < currentNode.choiceStrings.Count; index++)
            {
                var newButton = Instantiate(choiceButtonPrefab, choicesContainer.transform);
                newButton.BindToDialogueScreen(this, index);

                var choiceString = currentNode.choiceStrings[index];
                newButton.SetText(choiceString);
            }
        }

        public void SetCurrentChoiceIndex(int index)
        {
            _waitForChoiceSelection?.SetSelectionIndex(index);
        }
    }

    internal class WaitForDialogueChoiceSelection : CustomYieldInstruction
    {
        public int SelectionIndex { get; private set; } = -1;

        public override bool keepWaiting => SelectionIndex <= -1;

        public void SetSelectionIndex(int index)
        {
            SelectionIndex = index;
        }
    }
}