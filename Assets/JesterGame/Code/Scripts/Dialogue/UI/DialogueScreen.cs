using System;
using System.Collections;
using System.Collections.Generic;
using JesterGame.Code.Scripts.Dialogue.Data;
using JesterGame.Code.Scripts.Dialogue.DialogueLines;
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

        [SerializeField] private float wordDelay = 0.25f;

        #endregion

        #region Private Fields

        private readonly List<DialogueLineWrapper> _dialogueLines = new();

        #endregion


        protected override void CustomInitialize()
        {
        }

        protected override void CustomStart()
        {
        }

        protected override IEnumerator OpenScreenCoroutine()
        {
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
        /// <param name="dialogueLines"></param>
        private void SetDialogueInteraction(DialogueLineWrapper[] dialogueLines)
        {
            // Replace the existing lines with the new lines.
            _dialogueLines.Clear();
            foreach (var line in dialogueLines)
                _dialogueLines.Add(line);

            // Initialize the text and image to be empty.
            dialogueText.text = string.Empty;
            nameText.text = string.Empty;
            characterImage.sprite = null;
        }

        public IEnumerator RunDialogueCoroutine(DialogueLineWrapper[] dialogueLines)
        {
            SetDialogueInteraction(dialogueLines);

            // Open the screen
            yield return StartCoroutine(OpenScreenCoroutine());

            // Disable player input while the dialogue is running.
            // Disable it specifically for the pawn so the player controller can still get input.
            var controlledPawn = UtilLibrary.GetPlayerController()?.ControlledPawn;
            controlledPawn?.AddInputBlocker(this);

            // Play through the dialogue lines, waiting a few seconds after each line is done before playing the next line.
            // foreach (var dialogueLine in _dialogueLines)
            // {
            //     yield return StartCoroutine(DisplayWordsInCurrentLine(dialogueLine));
            //     yield return new WaitForSecondsRealtime(3);
            // }

            while (_dialogueLines.Count > 0)
            {
                // Remove the first item from the list.
                var currentLine = _dialogueLines[0];
                _dialogueLines.RemoveAt(0);

                // TODO: Animate or something.
                // Set the current text and speaker.
                if (currentLine.speaker.GetValue(out DialogueCharacter characterData))
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
                yield return StartCoroutine(DisplayWordsInCurrentLine(currentLine));

                // If there are choices,
                // 2. wait for selection
                // 3. add the choice's nextLines back to the start of the dialogue lines.
                if (currentLine.choices?.Length > 0)
                {
                    const int selectionIndex = 0;
                    var currentChoice = currentLine.choices[selectionIndex];
                    _dialogueLines.InsertRange(
                        0,
                        currentChoice.nextLines?.dialogueLines ??
                        Array.Empty<DialogueLineWrapper>()
                    );

                    Debug.LogWarning(
                        $"Choices not implemented yet! Automatically selecting the first choice: \"{currentChoice.text}\""
                    );
                }

                // Wait
                yield return new WaitForSecondsRealtime(1f);
            }

            // Re-enable player input while dialogue is running.
            controlledPawn?.RemoveInputBlocker(this);

            // Close the screen
            yield return StartCoroutine(CloseScreenCoroutine());

            yield return null;
        }

        private IEnumerator DisplayWordsInCurrentLine(DialogueLineWrapper currentLine)
        {
            // Split the current line by spaces.
            var splitLine = currentLine.text.Split(' ');

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
    }
}