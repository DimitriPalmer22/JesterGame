using System;
using System.Linq;
using JesterGame.Code.Scripts.Dialogue.DialogueGraph.Editor.Nodes;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace JesterGame.Code.Scripts.Dialogue.DialogueGraph.Editor
{
    [Serializable, Graph(ASSET_EXTENSION)]
    public class DialogueGraph : Graph
    {
        public const string ASSET_EXTENSION = "dialogueGraph";

        [MenuItem("Assets/Create/JesterGame/Dialogue/Dialogue Graph", false)]
        static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<DialogueGraph>("DG_InteractionName");
        }

        public override void OnGraphChanged(GraphLogger graphLogger)
        {
            base.OnGraphChanged(graphLogger);

            // Use for error checking or validation.
            var allNodes = GetNodes().ToArray();

            // Make sure there is only one node of type StartDialogueNode.
            var startNodes = allNodes.OfType<Nodes.StartDialogueNode>().ToArray();
            if (startNodes.Length != 1)
                graphLogger.LogError(
                    $"Dialogue graph should have exactly one {nameof(StartDialogueNode)}. Found {startNodes.Count()}.");
        }
    }
}