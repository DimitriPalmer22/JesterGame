using System.Collections;
using JesterGame.Code.Scripts.Core;
using JesterGame.Code.Scripts.Progression.UI;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Cutscenes;

namespace JesterGame.Code.Scripts.Progression
{
    public abstract class DayCutsceneComponentBase : CutsceneComponent<ProgressionEventArgs>
    {
        [SerializeField] public DayProgressionScreenDataAsset dayProgressionScreenDataAsset;

        protected override IEnumerator CustomRunCutscene(ProgressionEventArgs cutsceneStruct)
        {
            yield return OnDayProgressionCutscene(cutsceneStruct);
        }

        protected abstract IEnumerator OnDayProgressionCutscene(ProgressionEventArgs cutsceneStruct);
    }
}