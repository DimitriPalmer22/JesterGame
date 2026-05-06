using System.Collections;
using TMPro;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.UserInterface;

namespace JesterGame.Code.Scripts.Core.Interaction
{
    public class InteractScreen : UIScreen
    {
        /// <summary>
        /// The text component that displays the interaction prompt.
        /// This should be set to the text component in the prefab that is used for this screen.
        /// </summary>
        [SerializeField] private TMP_Text interactText;

        protected override void CustomInitialize()
        {
            // Set the opacity to the starting value of the curve
            canvasGroup.alpha = opacityCurve.Evaluate(opacityCurve.keys[0].time);
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

        public void OnSelectedInteractableChanged(InteractEventArgs args)
        {
            // If the item being interacted with is NOT null, open the screen
            if (args.helper != null)
            {
                interactText.text = args.helper.InteractText;
                StartCoroutine(OpenScreen());
            }

            // Otherwise, if the item being interacted with is null, close the screen
            else
            {
                StartCoroutine(CloseScreen());
            }
        }
    }
}