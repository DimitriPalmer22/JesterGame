using System;
using System.Linq;
using JesterGame.Code.Scripts.Dialogue.Data;
using JesterGame.Code.Scripts.Dialogue.DialogueGraph.Editor.Nodes;
using JesterGame.Code.Scripts.Dialogue.DialogueGraph.Runtime;
using JesterGame.Code.Scripts.Rooms;
using Unity.GraphToolkit.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JesterGame.Code.Scripts.Dialogue.DialogueGraph.Editor
{
    [Serializable, Graph(ASSET_EXTENSION)]
    public class DialogueGraph : Graph
    {
        public const string ASSET_EXTENSION = "dialogueGraph";

        [MenuItem("Assets/Create/JesterGame/Dialogue/Dialogue Graph", false)]
        static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<DialogueGraph>("DG_");
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

            foreach (var asset in Selection.GetFiltered(typeof(DialoguePoolDataAsset), SelectionMode.DeepAssets))
                CreatePoolGraph(asset, rooms);
        }

        private static void CreatePoolGraph(Object obj, RoomDataAsset[] rooms)
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
            const string dialogueGraphPrefix = "DG_";

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
                var createdGraph = CreateOrLoadGraph(graphName);
                var runtimeGraph = AssetDatabase.LoadAssetAtPath<RuntimeDialogueGraph>(graphName);

                loadedAsset.Data.firstInteractionPerRoom[item] = runtimeGraph;
            }

            // Create graphs for each place in the random list;
            for (var i = 0; i < loadedAsset.Data.randomInteractions.Length; i++)
            {
                // If the spot is already filled, continue
                if (loadedAsset.Data.randomInteractions[i] != null)
                    continue;

                var graphName = $"{folderPath}/{dialogueGraphPrefix}{baseAssetName}_Rand_{i}.{ASSET_EXTENSION}";
                var createdGraph = CreateOrLoadGraph(graphName);
                var runtimeGraph = AssetDatabase.LoadAssetAtPath<RuntimeDialogueGraph>(graphName);

                loadedAsset.Data.randomInteractions[i] = runtimeGraph;
            }
        }

        private static DialogueGraph CreateOrLoadGraph(string path)
        {
            var loadedAsset = GraphDatabase.LoadGraph<DialogueGraph>(path);

            if (loadedAsset != null)
                return loadedAsset;

            return GraphDatabase.CreateGraph<DialogueGraph>(path);
        }
    }
}