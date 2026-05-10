using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.Utility
{
    public class ManualYield : CustomYieldInstruction
    {
        private bool IsYieldActive { get; set; } = false;

        public override bool keepWaiting => IsYieldActive;

        public void StartYield() => IsYieldActive = true;

        public override void Reset()
        {
            base.Reset();
            IsYieldActive = false;
        }
    }
}