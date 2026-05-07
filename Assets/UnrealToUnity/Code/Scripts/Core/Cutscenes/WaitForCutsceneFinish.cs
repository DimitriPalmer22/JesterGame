using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.Cutscenes
{
    public class WaitForCutsceneFinish : CustomYieldInstruction
    {
        private bool IsCutsceneActive { get; set; } = false;

        public override bool keepWaiting => IsCutsceneActive;

        public void StartCutscene() => IsCutsceneActive = true;

        public override void Reset()
        {
            base.Reset();
            IsCutsceneActive = false;
        }
    }
}