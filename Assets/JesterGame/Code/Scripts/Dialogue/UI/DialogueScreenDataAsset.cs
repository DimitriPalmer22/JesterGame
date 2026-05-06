using System.Collections;
using JesterGame.Code.Scripts.Dialogue.DialogueGraph.Runtime;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.UserInterface;

namespace JesterGame.Code.Scripts.Dialogue.UI
{
    [CreateAssetMenu(fileName = "Dialogue Screen Data Asset", menuName = "JesterGame/UI/Dialogue Screen Data Asset")]
    public class DialogueScreenDataAsset : UIDataAsset<DialogueScreen>
    {
        public IEnumerator RunDialogueScreen(RuntimeDialogueGraph dialogueGraph)
        {
            var screen = GetScreen();

            if (!screen)
                yield break;

            yield return screen.StartCoroutine(screen.RunDialogueCoroutine(dialogueGraph));
        }
    }
}