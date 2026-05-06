using System;
using System.Collections.Generic;
using System.Linq;
using JesterGame.Code.Scripts.Dialogue.DialogueGraph.Editor.Nodes;
using JesterGame.Code.Scripts.Dialogue.DialogueGraph.Runtime;
using Unity.GraphToolkit.Editor;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.DataTables;

namespace JesterGame.Code.Scripts.Dialogue.DialogueGraph.Editor
{
    [ScriptedImporter(1, DialogueGraph.ASSET_EXTENSION)]
    public class DialogueGraphImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            // Load the graph as a dialogue graph.
            var editorGraph = GraphDatabase.LoadGraphForImporter<DialogueGraph>(ctx.assetPath);

            // Create a runtime graph instance and associate it with the editor graph.
            var runtimeGraph = ScriptableObject.CreateInstance<RuntimeDialogueGraph>();

            // Create a node ID map to allow easy lookup.
            var nodeIDMap = new Dictionary<INode, string>();

            var editorNodes = editorGraph.GetNodes().ToArray();

            // Give each node its own ID
            foreach (var cNode in editorNodes)
                nodeIDMap[cNode] = Guid.NewGuid().ToString();

            // Get the starting node for the graph.
            var startNode = editorNodes.OfType<StartDialogueNode>().FirstOrDefault();
            if (startNode != null)
            {
                // Get the node associated with the first output port of the start dialogue node.
                var entryPort = startNode.GetOutputPort(0)?.FirstConnectedPort;
                if (entryPort != null)
                    runtimeGraph.entryNodeID = nodeIDMap[entryPort.GetNode()];
            }

            // Iterate through each of the nodes and process them
            foreach (var cNode in editorNodes)
            {
                // Skip invalid node types.
                if (cNode is StartDialogueNode)
                    continue;

                // TODO: Something.
                var runtimeNode = new RuntimeDialogueNode();

                if (cNode is DialogueGraphNode dialogueGraphNode)
                    ProcessDialogueNode(dialogueGraphNode, runtimeNode, nodeIDMap);
            }

            // Binds the scriptable object we just created to the graph and allows us to drag and drop
            // our dialogue graph into the inspector.
            ctx.AddObjectToAsset("RuntimeData", runtimeGraph);
            ctx.SetMainObject(runtimeGraph);
        }

        /// <summary>
        /// A function to convert an editor-based dialogue node into a runtime-based dialogue node.
        /// </summary>
        /// <param name="dialogueNode"></param>
        /// <param name="runtimeNode"></param>
        /// <param name="nodeIDMap"></param>
        private void ProcessDialogueNode(
            DialogueGraphNode dialogueNode, RuntimeDialogueNode runtimeNode,
            Dictionary<INode, string> nodeIDMap
        )
        {
            runtimeNode.nodeID = nodeIDMap[dialogueNode];

            runtimeNode.nextNodeIDs = new List<string>();
            foreach (var cNextNode in dialogueNode.GetNextNodes())
                runtimeNode.nextNodeIDs.Add(nodeIDMap[cNextNode]);

            // Set the speaker and text
            runtimeNode.speaker = GetOptionValue<DataTableRowHandle>(dialogueNode, DialogueGraphNode.OPTION_SPEAKER);
            runtimeNode.dialogueText = GetOptionValue<string>(dialogueNode, DialogueGraphNode.OPTION_TEXT);

            // Set the affection value if this is a dialogue node with affection options.
            runtimeNode.affectionValue = 0;
            if (dialogueNode is AffectionDialogueNode affectionNode)
                runtimeNode.affectionValue = GetInputValue<int>(affectionNode, AffectionDialogueNode.OPTION_AFFECTION);

            // Set the choice strings if this is an options dialogue node.
            runtimeNode.choiceStrings = new List<string>();
            if (dialogueNode is OptionsDialogueNode optionsDialogueNode)
                runtimeNode.choiceStrings = optionsDialogueNode.GetChoiceStrings();
        }

        private static T GetOptionValue<T>(Node node, string optionName)
        {
            if (node != null && node.GetNodeOptionByName(optionName).TryGetValue(out T value))
                return value;

            return default;
        }

        private static T GetInputValue<T>(Node node, string inputName)
        {
            if (node == null)
                return default;

            var inputPort = node.GetInputPortByName(inputName);
            if (inputPort == null)
                return default;

            // If the connected value is a variable, use that
            if (inputPort.IsConnected && inputPort.FirstConnectedPort.GetNode() is IVariableNode varNode)
            {
                varNode.Variable.TryGetDefaultValue(out T value);
                return value;
            }

            // Otherwise, use the value we typed in.
            inputPort.TryGetValue(out T inputValue);
            return inputValue;
        }
    }
}