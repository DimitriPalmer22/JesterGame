using System.Collections;
using JesterGame.Code.Scripts.Core;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Cutscenes;

namespace JesterGame.Code.Scripts.Progression
{
    public class DayEndCutsceneComponent : CutsceneComponent<ProgressionEventArgs>
    {
        protected override IEnumerator CustomRunCutscene(ProgressionEventArgs cutsceneStruct)
        {
            Debug.Log($"Running day end cutscene for day {cutsceneStruct.currentDay - 1}.");
            yield return new WaitForSeconds(3f);
            Debug.Log($"Finished day end cutscene for day {cutsceneStruct.currentDay - 1}.");
        }
    }
}