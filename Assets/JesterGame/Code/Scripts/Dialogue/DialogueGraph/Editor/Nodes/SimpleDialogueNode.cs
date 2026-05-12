using System;
using Unity.GraphToolkit.Editor;

namespace JesterGame.Code.Scripts.Dialogue.DialogueGraph.Editor.Nodes
{
    [Serializable]
    internal class SimpleDialogueNode : DialogueGraphNode
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddInputPort(EXEC_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddOutputPort(EXEC_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}