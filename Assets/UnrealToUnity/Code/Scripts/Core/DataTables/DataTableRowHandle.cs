using System;
using NaughtyAttributes;
using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.DataTables
{
    internal interface IDataTableRowHandle
    {
        public DataTableBase DataTable { get; }
        public string RowName { get; }

        public bool Validate();

        public bool GetValue<TType>(out TType value);
    }

    [Serializable]
    public struct DataTableRowHandle : IDataTableRowHandle
    {
        [SerializeField] public DataTableBase dataTable;
        [SerializeField] public string rowName;

        public DataTableRowHandle(DataTableBase dataTable, string rowName)
        {
            this.dataTable = dataTable;
            this.rowName = rowName;
        }

        public DataTableBase DataTable => dataTable;
        public string RowName => rowName;

        public bool Validate()
        {
            if (!dataTable)
                return false;

            return dataTable.HasRow(rowName);
        }

        public bool GetValue<TType>(out TType value)
        {
            value = default;

            // First, validate the handle to make sure the table and row are valid.
            if (!Validate())
                return false;

            // First, try to cast the data table to the correct type.
            if (dataTable is DataTable<TType> typedTable)
            {
                // If the cast was successful, try to get the row from the table.
                if (typedTable.GetRow(rowName, out value))
                    return true;

                // Otherwise, log an error that the row was not found.
                Debug.LogError($"Row \"{rowName}\" not found in table `{dataTable.name}`!");
            }
            else
                Debug.LogError($"Data table `{dataTable.name}` is not of type `{typeof(TType).Name}`!");

            return false;
        }
    }

    [Serializable]
    public struct DataTableRowHandle<TTableType> : IDataTableRowHandle
    {
        [SerializeField] public DataTable<TTableType> dataTable;
        [SerializeField] public string rowName;

        public DataTableRowHandle(DataTable<TTableType> dataTable, string rowName)
        {
            this.dataTable = dataTable;
            this.rowName = rowName;
        }

        public DataTableBase DataTable => dataTable;
        public string RowName => rowName;

        public bool Validate()
        {
            if (!dataTable)
                return false;
            return dataTable.HasRow(rowName);
        }

        public bool GetValue(out TTableType value) => dataTable.GetRow(rowName, out value);

        public bool GetValue<TType>(out TType value)
        {
            value = default;

            // First, validate the handle to make sure the table and row are valid.
            if (!Validate())
                return false;

            // First, try to cast the data table to the correct type.
            if (dataTable is DataTable<TType> typedTable)
            {
                // If the cast was successful, try to get the row from the table.
                if (typedTable.GetRow(rowName, out value))
                    return true;

                // Otherwise, log an error that the row was not found.
                Debug.LogError($"Row \"{rowName}\" not found in table `{dataTable.name}`!");
            }
            else
                Debug.LogError($"Data table `{dataTable.name}` is not of type `{typeof(TType).Name}`!");

            return false;
        }
    }
}