using System;
using NaughtyAttributes;
using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.DataTables
{
    [Serializable]
    public struct DataTableRowHandle
    {
        /// <summary>
        /// A hard reference to the table to select a row from.
        /// </summary>
        [SerializeField] public DataTableBase dataTable;

        /// <summary>
        /// The name of the row to select.
        /// </summary>
        [SerializeField] public string rowName;

        public DataTableRowHandle(DataTableBase dataTable, string rowName)
        {
            this.dataTable = dataTable;
            this.rowName = rowName;
        }

        [Button]
        public bool Validate()
        {
            if (dataTable == null)
                return false;

            return dataTable.HasRow(rowName);
        }

        public bool GetValue<TStruct>(out TStruct value)
            where TStruct : struct, IDataTableRow
        {
            value = default;

            // First, validate the handle to make sure the table and row are valid.
            if (!Validate())
                return false;

            // First, try to cast the data table to the correct type.
            if (dataTable is DataTable<TStruct> typedTable)
            {
                // If the cast was successful, try to get the row from the table.
                if (typedTable.GetRow(rowName, out value))
                    return true;

                // Otherwise, log an error that the row was not found.
                Debug.LogError($"Row \"{rowName}\" not found in table `{dataTable.name}`!");
            }
            else
                Debug.LogError($"Data table `{dataTable.name}` is not of type `{typeof(TStruct).Name}`!");

            return false;
        }
    }
}