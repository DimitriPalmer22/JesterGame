using System.Collections;
using JesterGame.Code.Scripts.Core;
using JesterGame.Code.Scripts.Dialogue.DialogueGraph.Runtime;
using JesterGame.Code.Scripts.Dialogue.UI;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.DataTables;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Progression
{
    public class StartGameCutsceneComponent : DayCutsceneComponentBase
    {
        [SerializeField]
        private DataTableRowHandle playerRowHandle;

        [SerializeField]
        private DialogueScreenDataAsset dialogueScreenDataAsset;

        [SerializeField]
        private RuntimeDialogueGraph startDialogue;

        protected override IEnumerator OnDayProgressionCutscene(ProgressionEventArgs cutsceneStruct)
        {
            if (!UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
                yield break;

            yield return dialogueScreenDataAsset.RunDialogueScreen(playerRowHandle.RowName, startDialogue);
        }
    }
}