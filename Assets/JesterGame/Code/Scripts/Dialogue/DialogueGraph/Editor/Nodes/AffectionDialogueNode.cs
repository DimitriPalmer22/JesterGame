using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

namespace JesterGame.Code.Scripts.Dialogue.DialogueGraph.Editor.Nodes
{
    [Serializable]
    internal class AffectionDialogueNode : DialogueGraphNode
    {
        public const string OPTION_AFFECTION = "Affection";

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort(EXEC_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddInputPort<int>(OPTION_AFFECTION)
                .WithDisplayName("Affection Value")
                .WithDefaultValue(0)
                .Build();

            context.AddOutputPort(EXEC_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}