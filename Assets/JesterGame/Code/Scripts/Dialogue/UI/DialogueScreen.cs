using System.Collections;
using System.Collections.Generic;
using JesterGame.Code.Scripts.Dialogue.Data;
using JesterGame.Code.Scripts.Dialogue.DialogueLines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnrealToUnity.Code.Scripts.Core.UserInterface;

namespace JesterGame.Code.Scripts.Dialogue.UI
{
    public class DialogueScreen : UIScreen
    {
        #region Fields

        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private Image characterImage;

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
        public void SetDialogueInteraction(DialogueLineWrapper[] dialogueLines)
        {
            // Replace the existing lines with the new lines.
            _dialogueLines.Clear();
            foreach (var line in dialogueLines)
                _dialogueLines.Add(line);

            if (_dialogueLines.Count > 0)
            {
                dialogueText.text = _dialogueLines[0].text;

                if (_dialogueLines[0].speaker.GetValue(out DialogueCharacter characterData))
                {
                    nameText.text = characterData.name;
                    characterImage.sprite = characterData.portrait;
                }
            }
        }
    }
}