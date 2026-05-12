using System;
using System.Collections.Generic;
using JesterGame.Code.Scripts.Dialogue.Data;
using Unity.GraphToolkit.Editor;
using UnrealToUnity.Code.Scripts.Core.DataTables;

namespace JesterGame.Code.Scripts.Dialogue.DialogueGraph.Editor.Nodes
{
    [Serializable]
    public abstract class DialogueGraphNode : Node
    {
        protected const string EXEC_PORT_DEFAULT_NAME = "ExecutionPort";
        public const string OPTION_SPEAKER = "Speaker";
        public const string OPTION_TEXT = "DialogueText";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            base.OnDefineOptions(context);

            context.AddOption<DataTableRowHandle>(OPTION_SPEAKER)
                .WithDisplayName("Speaker")
                .Build();

            context.AddOption<string>(OPTION_TEXT)
                .WithDisplayName("Text")
                .WithDefaultValue("Text")
                .Build();
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            base.OnDefinePorts(context);

            // context.AddInputPort<DataTableRowHandle<DialogueCharacter>>($"{OPTION_SPEAKER}_Test")
            //     .WithDisplayName("Speaker")
            //     .Build();
            //
            // context.AddInputPort<string>($"{OPTION_TEXT}_Test")
            //     .WithDisplayName("Text")
            //     .Build();
        }

        public virtual List<DialogueGraphNode> GetNextNodes()
        {
            var list = new List<DialogueGraphNode>();

            foreach (var outputPort in GetOutputPorts())
            {
                if (!outputPort.IsConnected)
                    continue;

                if (outputPort.FirstConnectedPort == null)
                    continue;

                // Get the node connected to the output port only if it is a dialogue node
                if (outputPort.FirstConnectedPort.GetNode() is DialogueGraphNode connectedNode)
                    list.Add(connectedNode);
            }

            return list;
        }
    }
}