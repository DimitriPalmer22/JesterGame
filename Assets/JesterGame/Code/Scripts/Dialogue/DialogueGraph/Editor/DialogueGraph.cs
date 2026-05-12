using System;
using System.Linq;
using JesterGame.Code.Scripts.Dialogue.Data;
using JesterGame.Code.Scripts.Dialogue.DialogueGraph.Editor.Nodes;
using JesterGame.Code.Scripts.Dialogue.DialogueGraph.Runtime;
using JesterGame.Code.Scripts.Rooms;
using Unity.GraphToolkit.Editor;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Graph = Unity.GraphToolkit.Editor.Graph;
using Object = UnityEngine.Object;

namespace JesterGame.Code.Scripts.Dialogue.DialogueGraph.Editor
{
    [Serializable, Graph(ASSET_EXTENSION)]
    public class DialogueGraph : Graph
    {
        public const string ASSET_EXTENSION = "dialogueGraph";
        private const string GRAPH_PREFIX = "DG_";
        private const string TEMPLATE_GRAPH_NAME = GRAPH_PREFIX + "TEMPLATE";

        [MenuItem("Assets/Create/JesterGame/Dialogue/Dialogue Graph", false)]
        private static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<DialogueGraph>(GRAPH_PREFIX);
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

        [MenuItem("Assets/Create/JesterGame/Dialogue/Graphs from Dialogue Pool", false)]
        public static void CreateCorrespondingPoolGraphs()
        {
            AssetDatabase.Refresh();
            var rooms = Selection
                .GetFiltered(typeof(RoomDataAsset), SelectionMode.DeepAssets)
                .Cast<RoomDataAsset>()
                .ToArray();


            // Try to find and load the template graph.
            var templateGraphCandidates = AssetDatabase.FindAssets(TEMPLATE_GRAPH_NAME);
            if (templateGraphCandidates.Length == 0)
            {
                Debug.LogError("No template graph found!");
                return;
            }

            var templateGraphPath = AssetDatabase.GUIDToAssetPath(templateGraphCandidates[0]);

            foreach (var asset in Selection.GetFiltered(typeof(DialoguePoolDataAsset), SelectionMode.DeepAssets))
                CreatePoolGraph(asset, rooms, templateGraphPath);
        }

        private static void CreatePoolGraph(Object obj, RoomDataAsset[] rooms, string templateGraphPath)
        {
            Debug.Log("Creating corresponding dialogue graph...");
            var currentSelectedAsset = AssetDatabase.GetAssetPath(obj);

            // If the current selected asset is null, return
            if (string.IsNullOrEmpty(currentSelectedAsset))
            {
                Debug.LogError("No asset selected. Please select a dialogue pool data asset.");
                return;
            }

            // If the current selected asset is not of type DialoguePoolAsset
            var loadedAsset = AssetDatabase.LoadAssetAtPath<DialoguePoolDataAsset>(currentSelectedAsset);
            if (loadedAsset == null)
            {
                Debug.LogError("Selected asset is not a valid asset. Please select a dialogue pool data asset.");
                return;
            }

            // Ensure the data asset has each of the room assets assigned.
            foreach (var room in rooms)
            {
                if (room == null)
                    continue;

                if (loadedAsset.Data.firstInteractionPerRoom.ContainsKey(room))
                    continue;

                loadedAsset.Data.firstInteractionPerRoom[room] = null;
            }

            // Get the asset path of this scriptable object
            const string dialoguePoolPrefix = "DP_";
            const string dialogueGraphPrefix = GRAPH_PREFIX;

            var thisPath = AssetDatabase.GetAssetPath(loadedAsset);
            var thisAssetName = thisPath.Substring(thisPath.LastIndexOf('/') + 1);
            thisAssetName = thisAssetName.Substring(0, thisAssetName.LastIndexOf('.'));
            var baseAssetName = thisAssetName.Substring(dialoguePoolPrefix.Length);
            var baseFolder = thisPath.Substring(0, thisPath.LastIndexOf('/'));
            var folderPath = $"{baseFolder}";

            Debug.Log($"{folderPath} created! ({baseAssetName})");

            // Create graphs for each key in the room map.
            var keysCopy = loadedAsset.Data.firstInteractionPerRoom.Keys.ToArray();
            foreach (var item in keysCopy)
            {
                // If the item's key is null, continue
                if (item == null)
                    continue;

                // If the value is not null, continue
                if (loadedAsset.Data.firstInteractionPerRoom[item] != null)
                    continue;

                var graphName = $"{folderPath}/{dialogueGraphPrefix}{baseAssetName}_{item.roomName}.{ASSET_EXTENSION}";
                var createdGraph = CreateOrLoadGraph(graphName, templateGraphPath);
                var runtimeGraph = AssetDatabase.LoadAssetAtPath<RuntimeDialogueGraph>(graphName);

                loadedAsset.Data.firstInteractionPerRoom[item] = runtimeGraph;
            }

            // Create graphs for each place in the random list;
            for (var i = 0; i < loadedAsset.Data.randomInteractions.Length; i++)
            {
                // If the spot is already filled, continue
                if (loadedAsset.Data.randomInteractions[i] != null)
                    continue;

                var graphName = $"{folderPath}/{dialogueGraphPrefix}{baseAssetName}_Rand_{i+1}.{ASSET_EXTENSION}";
                var createdGraph = CreateOrLoadGraph(graphName, templateGraphPath);
                var runtimeGraph = AssetDatabase.LoadAssetAtPath<RuntimeDialogueGraph>(graphName);

                loadedAsset.Data.randomInteractions[i] = runtimeGraph;
            }

            // Mark the loaded asset as dirty, forcing you to save
            EditorUtility.SetDirty(loadedAsset);
        }

        private static DialogueGraph CreateOrLoadGraph(string path, string templateGraphPath)
        {
            var loadedAsset = GraphDatabase.LoadGraph<DialogueGraph>(path);

            if (loadedAsset != null)
                return loadedAsset;

            if (string.IsNullOrEmpty(templateGraphPath))
                return GraphDatabase.CreateGraph<DialogueGraph>(path);

            // Copy the template graph to the new path.
            var newGraph = AssetDatabase.CopyAsset(templateGraphPath, path);
            if (!newGraph)
            {
                Debug.LogError($"Failed to create graph at {path} from template {templateGraphPath}.");
                return null;
            }

            return GraphDatabase.LoadGraph<DialogueGraph>(path);
        }
    }
}