using UnityEditor;
using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.DataTables
{
    [CustomPropertyDrawer(typeof(DataTableRowHandle))]
    public class DataTableRowHandleDrawer : PropertyDrawer
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
                EditorGUI.PropertyField(dataTableRect, dataTableProperty);

                // Draw rowName dropdown
                var rowNameRect = new Rect(
                    position.x,
                    position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2,
                    position.width,
                    EditorGUIUtility.singleLineHeight
                );
                DrawRowNameDropdown(rowNameRect, dataTableProperty, rowNameProperty);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var dataTablePropHeight = EditorGUI.GetPropertyHeight(property, label, true);

            return dataTablePropHeight + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        private void DrawRowNameDropdown(Rect rect, SerializedProperty dataTableProperty,
            SerializedProperty rowNameProperty)
        {
            DataTableBase dataTable = dataTableProperty.objectReferenceValue as DataTableBase;

            if (dataTable == null)
            {
                EditorGUI.TextField(rect, "Row Name", rowNameProperty.stringValue);
                return;
            }

            // Validate the table first to make sure the data is up to date.
            // This will log any errors with the table if there are any.
            dataTable.ValidateTable();

            // Get all available row names
            var rowNames = dataTable.GetAllRowNames();

            if (rowNames.Length == 0)
            {
                EditorGUI.TextField(rect, "Row Name", rowNameProperty.stringValue);
                return;
            }

            // Get the selected index of the row name.
            // If the row name is out of bounds, set to the first row.
            int selectedIndex = System.Array.IndexOf(rowNames, rowNameProperty.stringValue);
            if (selectedIndex < 0)
                selectedIndex = 0;

            // Draw dropdown
            EditorGUI.BeginChangeCheck();
            int newIndex = EditorGUI.Popup(rect, "Row Name", selectedIndex, rowNames);

            if (EditorGUI.EndChangeCheck())
            {
                rowNameProperty.stringValue = rowNames[newIndex];
            }
        }
    }
}