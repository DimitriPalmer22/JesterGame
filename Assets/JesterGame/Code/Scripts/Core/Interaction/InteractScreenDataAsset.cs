using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.UserInterface;

namespace JesterGame.Code.Scripts.Core.Interaction
{
    [CreateAssetMenu(fileName = "Interact Screen Data Asset", menuName = "JesterGame/UI/Interact Screen Data Asset")]
    public class InteractScreenDataAsset : UIDataAsset<InteractScreen>
    {
        public void ChangeSelectedInteractable(InteractEventArgs args)
        {
            // Assert that the value is set.
            AssertValueSet();

            var screen = GetScreen();
            screen?.OnSelectedInteractableChanged(args);
        }
    }
}