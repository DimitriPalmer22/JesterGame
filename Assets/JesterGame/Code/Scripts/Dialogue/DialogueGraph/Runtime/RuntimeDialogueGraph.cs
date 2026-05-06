using System;
using System.Collections.Generic;
using UnityEngine;

namespace JesterGame.Code.Scripts.Dialogue.DialogueGraph.Runtime
{
    public class RuntimeDialogueGraph : ScriptableObject
    {
        [SerializeField] public string entryNodeID;
        [SerializeField] public List<RuntimeDialogueNode> allNodes = new();

        [NonSerialized] private bool _bMapConstructed = false;
        [NonSerialized] private readonly Dictionary<string, RuntimeDialogueNode> _nodeIDMap = new();

        public bool TryGetNodeByID(string nodeID, out RuntimeDialogueNode node)
        {
            if (!_bMapConstructed)
                ConstructNodeMap();

            return _nodeIDMap.TryGetValue(nodeID, out node);
        }

        private void ConstructNodeMap()
        {
            foreach (var node in allNodes)
                _nodeIDMap.Add(node.nodeID, node);

            _bMapConstructed = true;
        }
    }
}