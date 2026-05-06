using System;
using Unity.GraphToolkit.Editor;
using UnityEditor;

namespace JesterGame.Code.Scripts.Dialogue.DialogueGraph
{
    [Serializable, Graph(ASSET_EXTENSION)]
    public class DialogueGraph : Graph
    {
        private const string ASSET_EXTENSION = "dialogueGraph";

        [MenuItem("Assets/Create/JesterGame/Dialogue/Dialogue Graph", false)]
        static void CreateAssetFile()
        {
            GraphDatabase.PromptInProjectBrowserToCreateNewAsset<DialogueGraph>("DG_InteractionName");
        }

        public override void OnGraphChanged(GraphLogger graphLogger)
        {
            base.OnGraphChanged(graphLogger);

            // Use for error checking or validation.
        }
    }
}