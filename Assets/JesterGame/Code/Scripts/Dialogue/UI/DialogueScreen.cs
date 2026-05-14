using System.Collections;
using System.Collections.Generic;
using JesterGame.Code.Scripts.Core;
using JesterGame.Code.Scripts.Dialogue.Data;
using JesterGame.Code.Scripts.Dialogue.DialogueGraph.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnrealToUnity.Code.Scripts.Core.UserInterface;
using UnrealToUnity.Code.Scripts.Core.Utility;
using UnrealToUnity.Code.Scripts.Core.Utility.Components;

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

        [SerializeField] private Button nextLineButton;
        [SerializeField] private Button skipTextButton;

        [SerializeField] private float wordDelay = 0.25f;

        [SerializeField] private AnimationHelperComponent animationHelperComponent;

        [SerializeField] private UnityEvent onTextUpdated;

        #endregion

        #region Private Fields

        private RuntimeDialogueGraph _currentDialogueGraph;
        private string _currentNodeID;

        private WaitForNextLineInput _waitForNextLineInput;
        private WaitForDialogueChoiceSelection _waitForChoiceSelection;

        private bool _bIsSkipping;

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

            // Disable the next line button
            SetNextLineSkipTextVisibility(false, false);

            yield return animationHelperComponent.PlayAnimationAndWait("Intro");

            // enable the next line button
            SetNextLineSkipTextVisibility(true, true);
        }

        protected override IEnumerator CloseScreenCoroutine()
        {
            // Disable the next line button
            SetNextLineSkipTextVisibility(false, false);

            yield return animationHelperComponent.PlayAnimationAndWait("Outro");

            // enable the next line button
            SetNextLineSkipTextVisibility(true, true);
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

        /// <summary>
        ///
        /// </summary>
        /// <param name="currentCharacter"></param>
        /// <param name="dialogueGraph"></param>
        /// <returns></returns>
        public IEnumerator RunDialogueCoroutine(string currentCharacter, RuntimeDialogueGraph dialogueGraph)
        {
            SetDialogueInteraction(dialogueGraph);

            UtilLibrary.GetGameMode(out ImpostorGameMode gameMode);

            if (_currentDialogueGraph.TryGetNodeByID(_currentNodeID, out var cNode))
            {
                if (cNode.speaker.GetValue(out DialogueCharacter characterData))
                {
                    nameText.text = characterData.name;
                    characterImage.sprite = characterData.portrait;
                    characterImage.enabled = (characterData.portrait != null);
                }
            }
            else
            {
                yield break;
            }

            // Open the screen
            yield return OpenScreenCoroutine();

            // Disable player input while the dialogue is running.
            // Disable it specifically for the pawn so the player controller can still get input.
            var controlledPawn = UtilLibrary.GetPlayerController()?.ControlledPawn;
            controlledPawn?.AddInputBlocker(this);

            // Keep looping until we reach an empty node.
            while (!string.IsNullOrEmpty(_currentNodeID) && _currentDialogueGraph)
            {
                // Get the runtime dialogue node struct from the current ID
                if (!_currentDialogueGraph.TryGetNodeByID(_currentNodeID, out var currentNode))
                {
                    Debug.LogError($"Dialogue node with ID {_currentNodeID} not found in dialogue graph!");
                    break;
                }

                // Modify the affection value if there is a change.
                if (gameMode && currentNode.affectionValue != 0)
                    gameMode.ModifyCharacterAffection(currentCharacter, currentNode.affectionValue);

                // currentNode.affectionValue

                // Animate or something.
                // Set the current text and speaker.
                if (currentNode.speaker.GetValue(out DialogueCharacter characterData))
                {
                    nameText.text = characterData.name;
                    characterImage.sprite = characterData.portrait;
                    characterImage.enabled = (characterData.portrait != null);
                }
                else
                {
                    nameText.text = string.Empty;
                    characterImage.sprite = null;
                }

                // Disable the next line button if the line has more words than the threshold.
                const int skipThreshold = 5;
                if (currentNode.dialogueText.Split(' ').Length >= skipThreshold)
                {
                    SetNextLineSkipTextVisibility(false, true);
                    EventSystem.current.SetSelectedGameObject(skipTextButton.gameObject);
                }
                else
                    SetNextLineSkipTextVisibility(false, false);

                // Display the text of the current line.
                yield return StartCoroutine(DisplayWordsInCurrentLine(currentNode, characterData));

                // If there are choices,
                // 2. wait for selection
                // 3. add the choice's nextLines back to the start of the dialogue lines.
                if (currentNode.choiceStrings.Count > 0)
                {
                    SetNextLineSkipTextVisibility(false, false);

                    CreateChoices(currentNode);

                    // Wait for selection.
                    _waitForChoiceSelection = new WaitForDialogueChoiceSelection();
                    yield return _waitForChoiceSelection;

                    // Set the current node ID to the next node ID corresponding to the selected choice.
                    _currentNodeID = currentNode.nextNodeIDs[_waitForChoiceSelection.SelectionIndex];
                    _waitForChoiceSelection = null;

                    // Re-enable the next line button
                    SetNextLineSkipTextVisibility(true, false);
                    EventSystem.current.SetSelectedGameObject(nextLineButton.gameObject);

                    DestroyChoices();
                }

                // Otherwise, just go to the next node.
                else
                {
                    // Re-enable the next line button
                    SetNextLineSkipTextVisibility(true, false);
                    EventSystem.current.SetSelectedGameObject(nextLineButton.gameObject);

                    _currentNodeID = currentNode.nextNodeIDs?.Count > 0
                        ? currentNode.nextNodeIDs[0]
                        : string.Empty;

                    // Wait for input
                    _waitForNextLineInput = new WaitForNextLineInput();
                    yield return _waitForNextLineInput;
                    _waitForNextLineInput = null;
                }
            }

            // Re-enable player input while dialogue is running.
            controlledPawn?.RemoveInputBlocker(this);

            // Close the screen
            yield return CloseScreenCoroutine();

            yield return null;
        }

        private IEnumerator DisplayWordsInCurrentLine(RuntimeDialogueNode currentNode, DialogueCharacter characterData)
        {
            // Split the current line by spaces.
            var splitLine = currentNode.dialogueText.Split(' ');

            // Clear the current text.
            dialogueText.text = string.Empty;

            // Word by word, add the words to the current text.
            for (var index = 0; index < splitLine.Length; index++)
            {
                if (!_bIsSkipping)
                    yield return new WaitForSecondsRealtime(wordDelay);

                // Add the current word
                var word = splitLine[index];

                var cWord = $"{word}";

                // Force italicize if necessary.
                if (characterData.bForceItalics)
                    cWord = $"<i>{cWord}</i>";

                dialogueText.text += cWord;

                // Add the space if not the last word.
                if (index < splitLine.Length - 1)
                    dialogueText.text += " ";

                // Call the text updated event.
                if (!_bIsSkipping)
                    onTextUpdated?.Invoke();
            }

            if (_bIsSkipping)
                onTextUpdated?.Invoke();

            _bIsSkipping = false;
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
            DialogueScreenButton[] choiceButtons = new DialogueScreenButton[currentNode.choiceStrings.Count];

            // For each choice, instantiate a new button and set the text to the choice string.
            for (var index = 0; index < currentNode.choiceStrings.Count; index++)
            {
                var newButton = Instantiate(choiceButtonPrefab, choicesContainer.transform);
                choiceButtons[index] = newButton;

                newButton.BindToDialogueScreen(this, index);

                var choiceString = currentNode.choiceStrings[index];
                newButton.SetText(choiceString);

                // Focus on the top-most button
                if (index == 0)
                    EventSystem.current.SetSelectedGameObject(newButton.gameObject);
            }

            for (var index = 0; index < choiceButtons.Length; index++)
            {
                var navigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                };

                if (index > 0)
                    navigation.selectOnUp = choiceButtons[index - 1].button;
                if (index < currentNode.choiceStrings.Count - 1)
                    navigation.selectOnDown = choiceButtons[index + 1].button;

                // Set up the navigation for the buttons.
                choiceButtons[index].button.navigation = navigation;
            }
        }

        public void SetCurrentChoiceIndex(int index)
        {
            _waitForChoiceSelection?.SetSelectionIndex(index);
        }

        public void OnNextLineInput()
        {
            _waitForNextLineInput?.ReceiveInput();
        }

        public void OnSkipTextInput()
        {
            _bIsSkipping = true;
        }

        private void SetNextLineSkipTextVisibility(bool bNextLine, bool bSkipText)
        {
            nextLineButton.gameObject.SetActive(bNextLine);
            skipTextButton.gameObject.SetActive(bSkipText);
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

    internal class WaitForNextLineInput : CustomYieldInstruction
    {
        private bool _inputReceived;

        public override bool keepWaiting => !_inputReceived;

        public void ReceiveInput() => _inputReceived = true;
    }
}