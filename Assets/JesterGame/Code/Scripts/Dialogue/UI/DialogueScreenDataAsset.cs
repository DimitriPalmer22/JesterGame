using JesterGame.Code.Scripts.Dialogue.DialogueLines;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.UserInterface;

namespace JesterGame.Code.Scripts.Dialogue.UI
{
    [CreateAssetMenu(fileName = "Dialogue Screen Data Asset", menuName = "JesterGame/UI/Dialogue Screen Data Asset")]
    public class DialogueScreenDataAsset : UIDataAsset<DialogueScreen>
    {
        public void Test(DialogueLineWrapper[] dialogueLines)
        {
            var screen = GetScreen();

            if (!screen)
                return;

            screen.SetDialogueInteraction(dialogueLines);
            screen.StartCoroutine(screen.OpenScreen());
        }
    }
}