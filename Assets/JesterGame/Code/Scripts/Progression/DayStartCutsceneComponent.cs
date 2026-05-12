using System.Collections;
using JesterGame.Code.Scripts.Core;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Progression
{
    public class DayStartCutsceneComponent : DayCutsceneComponentBase
    {
        protected override IEnumerator OnDayProgressionCutscene(ProgressionEventArgs cutsceneStruct)
        {
            // Disable input while fading
            var playerController = UtilLibrary.GetPlayerController();
            playerController?.AddInputBlocker(this);

            // // Fade to black
            // if (dayProgressionScreenDataAsset)
            // {
            //     yield return dayProgressionScreenDataAsset.OpenScreen();
            //     yield return new WaitForSecondsRealtime(screenFadeDelay);
            // }

            // Fade from black
            if (dayProgressionScreenDataAsset)
                yield return dayProgressionScreenDataAsset.CloseScreen();

            // Re-enable input
            playerController?.RemoveInputBlocker(this);
        }
    }
}