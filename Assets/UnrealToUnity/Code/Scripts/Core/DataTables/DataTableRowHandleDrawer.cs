#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.DataTables
{
    public abstract class AbstractDataTableRowHandleDrawer : PropertyDrawer
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
                // EditorGUI.indentLevel++;

                // Get the serialized properties
                var dataTableProperty = property.FindPropertyRelative("dataTable");
                var rowNameProperty = property.FindPropertyRelative("rowName");

                // Draw the data table reference field
                var dataTableRect = new Rect(
                    position.x,
                    position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 1,
                    position.width,
                    EditorGUIUtility.singleLineHeight
                );
                EditorGUI.PropertyField(dataTableRect, dataTableProperty, GUIContent.none);

                // Draw rowName dropdown
                var rowNameRect = new Rect(
                    position.x,
                    position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2,
                    position.width,
                    EditorGUIUtility.singleLineHeight
                );
                DrawRowNameDropdown(rowNameRect, dataTableProperty, rowNameProperty);

                // EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var dataTablePropHeight = EditorGUI.GetPropertyHeight(property, label, true);

            return dataTablePropHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private void DrawRowNameDropdown(Rect rect, SerializedProperty dataTableProperty,
            SerializedProperty rowNameProperty)
        {
            var dataTable = dataTableProperty.objectReferenceValue as DataTableBase;

            if (dataTable == null)
            {
                EditorGUI.TextField(rect, "Row Name", rowNameProperty.stringValue, GUIStyle.none);
                return;
            }

            // Get all available row names
            var rowNames = dataTable.GetAllRowNames();

            // Create a copy of rowNames, but with an empty string as the first element.
            var rowNamesWithEmpty = new string[rowNames.Length + 1];
            rowNamesWithEmpty[0] = "[---NO VALUE---]";
            for (var i = 0; i < rowNames.Length; i++)
                rowNamesWithEmpty[i + 1] = rowNames[i];

            if (rowNamesWithEmpty.Length == 0)
            {
                EditorGUI.TextField(rect, "Row Name", rowNameProperty.stringValue, GUIStyle.none);
                return;
            }

            // Get the selected index of the row name.
            // If the row name is out of bounds, set to the first row.
            var selectedIndex = System.Array.IndexOf(rowNamesWithEmpty, rowNameProperty.stringValue);
            if (selectedIndex < 0)
                selectedIndex = 0;

            var currentTextColor = selectedIndex > 0
                ? EditorStyles.label.normal.textColor
                : Color.red;

            // Draw dropdown
            EditorGUI.BeginChangeCheck();
            var style = new GUIStyle(EditorStyles.popup)
            {
                stretchWidth = true,
                contentOffset = new Vector2(0, 0)
            };
            style.hover.textColor = currentTextColor;
            style.normal.textColor = currentTextColor;
            style.active.textColor = currentTextColor;
            style.focused.textColor = currentTextColor;

            var noLabelRect = new Rect(
                rect.x - EditorGUIUtility.labelWidth,
                rect.y,
                rect.width + EditorGUIUtility.labelWidth,
                rect.height
            );
            var newIndex = EditorGUI.Popup(noLabelRect, null, selectedIndex, rowNamesWithEmpty, style);

            if (EditorGUI.EndChangeCheck())
            {
                rowNameProperty.stringValue = newIndex > 0
                    ? rowNamesWithEmpty[newIndex]
                    : string.Empty;
            }
        }
    }

    [CustomPropertyDrawer(typeof(DataTableRowHandle))]
    public class DataTableRowHandleDrawer : AbstractDataTableRowHandleDrawer
    {
    }

    [CustomPropertyDrawer(typeof(DataTableRowHandle<>))]
    public class GenericDataTableRowHandleDrawer : AbstractDataTableRowHandleDrawer
    {
    }
}

#endif