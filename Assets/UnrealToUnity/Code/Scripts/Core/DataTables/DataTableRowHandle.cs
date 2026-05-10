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
    public struct DataTableRowHandle : IDataTableRowHandle, IEquatable<DataTableRowHandle>
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

        public static implicit operator DataTableRowHandle(DataTableRowHandle<DataTableBase> handle)
        {
            return new DataTableRowHandle(handle.dataTable, handle.rowName);
        }

        public static bool operator ==(DataTableRowHandle handle1, DataTableRowHandle handle2)
        {
            return Equals(handle1, handle2);
        }

        public static bool operator !=(DataTableRowHandle handle1, DataTableRowHandle handle2)
        {
            return !(handle1 == handle2);
        }

        public bool Equals(DataTableRowHandle other)
        {
            return Equals(dataTable, other.dataTable) && rowName == other.rowName;
        }

        public override bool Equals(object obj)
        {
            return obj is DataTableRowHandle other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(dataTable, rowName);
        }
    }

    [Serializable]
    public struct DataTableRowHandle<TTableType> : IDataTableRowHandle, IEquatable<DataTableRowHandle<TTableType>>
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

        public static bool operator ==(DataTableRowHandle<TTableType> handle1, DataTableRowHandle<TTableType> handle2)
        {
            return Equals(handle1, handle2);
        }

        public static bool operator !=(DataTableRowHandle<TTableType> handle1, DataTableRowHandle<TTableType> handle2)
        {
            return !(handle1 == handle2);
        }

        public bool Equals(DataTableRowHandle<TTableType> other)
        {
            return Equals(dataTable, other.dataTable) && rowName == other.rowName;
        }

        public override bool Equals(object obj)
        {
            return obj is DataTableRowHandle<TTableType> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(dataTable, rowName);
        }
    }
}