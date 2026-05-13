using System.Collections;
using JesterGame.Code.Scripts.Core;

namespace JesterGame.Code.Scripts.Progression
{
    public class GameEndCutscene : DayCutsceneComponentBase
    {
        protected override IEnumerator OnDayProgressionCutscene(ProgressionEventArgs cutsceneStruct)
        {
            // Fade from black
            if (dayProgressionScreenDataAsset)
                yield return dayProgressionScreenDataAsset.CloseScreen();
        }
    }
}