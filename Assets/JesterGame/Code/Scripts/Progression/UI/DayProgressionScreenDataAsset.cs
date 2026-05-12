using System.Collections;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.UserInterface;

namespace JesterGame.Code.Scripts.Progression.UI
{
    /// <summary>
    /// Just treat this as a black screen.
    /// </summary>
    [CreateAssetMenu(
        fileName = "DayProgressionScreenDataAsset",
        menuName = "JesterGame/UI/Day Progression Screen Data Asset")
    ]
    public class DayProgressionScreenDataAsset : UIDataAsset<DayProgressionBlackScreen>
    {
        public IEnumerator OpenScreen()
        {
            var screen = GetScreen();

            if (!screen)
                yield break;

            yield return screen.OpenScreen();
        }

        public IEnumerator CloseScreen()
        {
            var screen = GetScreen();

            if (!screen)
                yield break;

            yield return screen.CloseScreen();
        }
    }
}