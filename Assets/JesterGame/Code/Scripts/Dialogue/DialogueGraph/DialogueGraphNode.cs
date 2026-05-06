using System;
using JesterGame.Code.Scripts.Dialogue.DialogueLines;
using Unity.GraphToolkit.Editor;
using UnrealToUnity.Code.Scripts.Core.DataTables;

namespace JesterGame.Code.Scripts.Dialogue.DialogueGraph
{
    [Serializable]
    public abstract class DialogueGraphNode : Node
    {
        protected const string EXEC_PORT_DEFAULT_NAME = "ExecutionPort";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            base.OnDefineOptions(context);

            context.AddOption<DataTableRowHandle>("Speaker")
                .WithDisplayName("Speaker")
                .Build();

            context.AddOption<string>("DialogueText")
                .WithDisplayName("Text")
                .WithDefaultValue("Text")
                .Build();
        }
    }

    [Serializable]
    public class StartDialogueNode : Node
    {
        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddOutputPort("ExecutionPort")
                .WithDisplayName("Start")
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }

    [Serializable]
    public class SimpleDialogueNode : DialogueGraphNode
    {
        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            base.OnDefineOptions(context);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
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

    [Serializable]
    public class AffectionDialogueNode : DialogueGraphNode
    {
        const string AFFECTION_NAME = "Affection";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            base.OnDefineOptions(context);
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort(EXEC_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            context.AddInputPort<int>(AFFECTION_NAME)
                .WithDisplayName("Affection Value")
                .WithDefaultValue(0)
                .Build();

            context.AddOutputPort(EXEC_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();
        }
    }

    [Serializable]
    public class OptionsDialogueNode : DialogueGraphNode
    {
        private const int MAX_CHOICES = 4;
        private const string OPTION_BASE_NAME = "Choice";
        private const string NUM_CHOICES_NAME = "NumberOfChoices";

        protected override void OnDefineOptions(IOptionDefinitionContext context)
        {
            base.OnDefineOptions(context);

            // An option for the number of choices availible to this dialogue node.
            // This will determine how many of the choice options are actually used.
            context.AddOption<int>(NUM_CHOICES_NAME)
                .WithDisplayName("Number of Choices")
                .WithDefaultValue(3)
                .Build();
        }

        protected override void OnDefinePorts(IPortDefinitionContext context)
        {
            context.AddInputPort(EXEC_PORT_DEFAULT_NAME)
                .WithDisplayName(string.Empty)
                .WithConnectorUI(PortConnectorUI.Arrowhead)
                .Build();

            var numChoices = GetNumChoices();
            for (var i = 0; i < numChoices; ++i)
            {
                context.AddInputPort<string>($"{OPTION_BASE_NAME}_input_{i + 1}")
                    .WithDisplayName($"Choice {i + 1}")
                    .WithDefaultValue($"Choice_{i + 1}")
                    .WithConnectorUI(PortConnectorUI.Circle)
                    .Build();

                context.AddOutputPort($"{OPTION_BASE_NAME}_{i + 1}")
                    .WithDisplayName($"Choice {i + 1}")
                    .WithConnectorUI(PortConnectorUI.Arrowhead)
                    .Build();
            }
        }

        private int GetNumChoices()
        {
            var numChoices = MAX_CHOICES;

            var option = GetNodeOptionByName(NUM_CHOICES_NAME);
            if (option != null && option.TryGetValue(out int newNumChoices))
                numChoices = newNumChoices;

            return (int)MathF.Max(MathF.Min(numChoices, MAX_CHOICES), 1);
        }
    }
}