using System;
using System.Collections;
using JesterGame.Code.Scripts.Core;
using TMPro;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.UserInterface;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Progression.UI
{
    public class DayProgressionBlackScreen : UIScreen
    {
        private static readonly int AnimIntro = Animator.StringToHash("Intro");
        private static readonly int AnimOutro = Animator.StringToHash("Outro");
        private static readonly int AnimOutroAlt = Animator.StringToHash("OutroAlt");

        [SerializeField] private Animator animator;

        [SerializeField] private GameObject dayTextParent;
        [SerializeField] private TMP_Text dayText;

        [NonSerialized] private readonly ManualYield _introOutroYield = new();
        [NonSerialized] private bool bShowDayText = true;

        protected override void CustomInitialize()
        {
        }

        protected override void CustomStart()
        {
        }

        protected override IEnumerator OpenScreenCoroutine()
        {
            yield return animator.PlayAnimationAndWait(AnimIntro, _introOutroYield);
        }

        protected override IEnumerator CloseScreenCoroutine()
        {
            if (UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
                dayText.text = $"Day {gameMode.CurrentDayIndex + 1}";

            if (bShowDayText)
                yield return animator.PlayAnimationAndWait(AnimOutro, _introOutroYield);
            else
                yield return animator.PlayAnimationAndWait(AnimOutroAlt, _introOutroYield);
        }

        public void FinishAnimation()
        {
            _introOutroYield.Reset();
        }

        public void SetDayProgressionTextVisible(bool bVisible)
        {
            bShowDayText = bVisible;
            // dayTextParent.SetActive(bVisible);
        }
    }
}