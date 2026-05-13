using System.Collections;
using JesterGame.Code.Scripts.Core;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Progression
{
    public class GameEndCutscene : DayCutsceneComponentBase
    {
        protected override IEnumerator OnDayProgressionCutscene(ProgressionEventArgs cutsceneStruct)
        {
            // Disable the player's input
            var playerController = UtilLibrary.GetPlayerController(0);
            playerController?.AddInputBlocker(this);

            // Fade from black
            if (dayProgressionScreenDataAsset)
                yield return dayProgressionScreenDataAsset.CloseScreen();

            // TODO: Impostor stuff?
            
        }
    }
}