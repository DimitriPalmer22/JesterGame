using System;
using System.Collections.Generic;
using Unity.GraphToolkit.Editor;

namespace JesterGame.Code.Scripts.Dialogue.DialogueGraph.Editor.Nodes
{
    [Serializable]
    internal class OptionsDialogueNode : DialogueGraphNode
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

        public List<string> GetChoiceStrings()
        {
            var list = new List<string>();

            var numChoices = GetNumChoices();
            for (var i = 0; i < numChoices; ++i)
            {
                var choice = GetNodeOptionByName($"{OPTION_BASE_NAME}_input_{i + 1}");
                if (choice == null)
                    continue;

                if (choice.TryGetValue(out string choiceString))
                    list.Add(choiceString);
            }

            return list;
        }
    }
}