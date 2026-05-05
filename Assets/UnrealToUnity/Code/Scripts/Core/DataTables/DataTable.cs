using System;
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
        [SerializeField, OnValueChanged("OnValueChanged")]
        private TStruct[] rows;

        /// <summary>
        /// A map of all the rows in the table, using the row name as the key.
        /// </summary>
        [NonSerialized] private readonly Dictionary<string, TStruct> _dataMap = new();

        [NonSerialized] private bool _bConstructed = false;

        public override void ValidateTable()
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

        [Button]
        public void InvalidateTable()
        {
            _bConstructed = false;
        }

        public bool GetRow(string rowName, out TStruct result)
        {
            if (!_bConstructed)
                ValidateTable();

            return _dataMap.TryGetValue(rowName, out result);
        }

        public override bool HasRow(string rowName)
        {
            if (!_bConstructed)
                ValidateTable();

            return _dataMap.ContainsKey(rowName);
        }

        public override string[] GetAllRowNames()
        {
            if (!_bConstructed)
                ValidateTable();

            var keys = new string[_dataMap.Keys.Count];
            _dataMap.Keys.CopyTo(keys, 0);
            return keys;
        }

        private void OnValueChanged()
        {
            ValidateTable();
            _bConstructed = false;
        }

        public TStruct[] GetAllRowValues()
        {
            var values = new TStruct[rows.Length];
            rows.CopyTo(values, 0);

            return values;
        }

        public override DataTableRowHandle[] GetAllRowHandles()
        {
            var handles = new DataTableRowHandle[rows.Length];
            for (var i = 0; i < rows.Length; i++)
                handles[i] = new DataTableRowHandle(this, rows[i].GetDataTableRowName);

            return handles;
        }
    }
}