using System;
using System.Collections.Generic;
using UnrealToUnity.Code.Scripts.Core.DataTables;

namespace JesterGame.Code.Scripts.Dialogue.DialogueGraph.Runtime
{
    [Serializable]
    public struct RuntimeDialogueNode
    {
        public string nodeID;
        public List<string> nextNodeIDs;

        public DataTableRowHandle speaker;
        public string dialogueText;

        public int affectionValue;

        public List<string> choiceStrings;
    }
}