using System;
using Unity.GraphToolkit.Editor;

namespace JesterGame.Code.Scripts.Dialogue.DialogueGraph.Editor.Nodes
{
    [Serializable]
    internal class StartDialogueNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            context.AddOutputPort("ExecutionPort")
                .WithDisplayName("Start")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }
}