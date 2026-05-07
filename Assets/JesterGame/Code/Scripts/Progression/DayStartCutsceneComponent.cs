using System.Collections;
using JesterGame.Code.Scripts.Core;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Cutscenes;

namespace JesterGame.Code.Scripts.Progression
{
    public class DayStartCutsceneComponent : DayCutsceneComponentBase
    {
        protected override IEnumerator CustomRunCutscene(ProgressionEventArgs cutsceneStruct)
        {
            yield return null;
        }
    }
}