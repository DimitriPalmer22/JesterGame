using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.DataTables
{
    /// <summary>
    /// A class that tries to mimic the functionality of Unreal Engine's DataTable class,
    /// which is a way to store structured data in a table format.
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    public class DataTable<TStruct> : DataTableBase
        where TStruct : struct, IDataTableRow
    {
        /// <summary>
        /// The data within the table;
        /// </summary>
        [SerializeField] public TStruct[] rows;

        /// <summary>
        /// A map of all the rows in the table, using the row name as the key.
        /// </summary>
        private readonly Dictionary<string, TStruct> _dataMap = new();

        private bool _bConstructed = false;

        [Button]
        private void ConstructRowNameMap()
        {
            // Clear the map
            _dataMap.Clear();

            if (rows == null)
                return;

            // Add all the rows to the map, using the row name as the key.
            foreach (var row in rows)
            {
                if (!_dataMap.ContainsKey(row.GetDataTableRowName))
                    _dataMap.Add(row.GetDataTableRowName, row);
                else
                    Debug.LogError(
                        $"Duplicate row name \"{row.GetDataTableRowName}\" found `{name}`. This row will be ignored!");
            }

            // Set the constructed flag to true.
            _bConstructed = true;
        }

        public bool GetRow(string rowName, out TStruct result)
        {
            if (!_bConstructed)
                ConstructRowNameMap();

            return _dataMap.TryGetValue(rowName, out result);
        }
    }
}