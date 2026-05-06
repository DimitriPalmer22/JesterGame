using System.Collections.Generic;
using UnityEngine;

namespace JesterGame.Code.Scripts.Dialogue.DialogueGraph.Runtime
{
    public class RuntimeDialogueGraph : ScriptableObject
    {
        public string entryNodeID;
        public readonly List<RuntimeDialogueNode> allNodes = new();
    }
}