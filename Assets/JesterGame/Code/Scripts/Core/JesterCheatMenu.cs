using NaughtyAttributes;
using UnrealToUnity.Code.Scripts.Core.CheatMenus;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Core
{
    public class JesterCheatMenu : CheatMenu
    {
        protected override string CheatMenuTypeName => "JesterCheatMenu";

        #region Progress Functions

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void IncrementProgress()
        {
            if (!UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
                return;

            // Increment the progress of the current level.
            gameMode.IncrementProgress();
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void DecrementProgress()
        {
            if (!UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
                return;

            // Increment the progress of the current level.
            gameMode.DecrementProgress();
        }

        #endregion
    }
}