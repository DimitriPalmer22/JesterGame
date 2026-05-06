using System;
using UnityEditor;
using UnityEngine;

namespace JesterGame.Code.Scripts.Dialogue.DialogueLines
{
    [Serializable]
    public struct DialogueChoiceSimple
    {
        /// <summary>
        /// The text of the dialogue choice.
        /// </summary>
        [SerializeField] public string text;

        /// <summary>
        /// The affection value associated with this choice, which can affect character relationships or story outcomes.
        /// </summary>
        [SerializeField] public int affectionValue;

        public DialogueChoiceSimple(string text, int affectionValue)
        {
            this.text = text;
            this.affectionValue = affectionValue;
        }

        public static DialogueChoiceSimple Empty => new DialogueChoiceSimple(string.Empty, 0);
    }

    [CustomPropertyDrawer(typeof(DialogueChoiceSimple))]
    public class DialogueChoiceSimpleDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw the foldout
            property.isExpanded = EditorGUI.Foldout(
                new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                property.isExpanded,
                label
            );

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                var affectionValueProperty = property.FindPropertyRelative("affectionValue");
                var textProperty = property.FindPropertyRelative("text");

                var textRect = new Rect(
                    position.x,
                    position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 1,
                    position.width,
                    EditorGUIUtility.singleLineHeight
                );
                var affectionRect = new Rect(
                    position.x,
                    position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2,
                    position.width,
                    EditorGUIUtility.singleLineHeight
                );

                EditorGUI.PropertyField(textRect, textProperty);
                EditorGUI.PropertyField(affectionRect, affectionValueProperty);

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.standardVerticalSpacing;
        }
    }
}