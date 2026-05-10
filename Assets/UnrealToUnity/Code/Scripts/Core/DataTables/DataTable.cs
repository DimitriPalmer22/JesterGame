using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
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
        [SerializeField, SerializedDictionary("Row Name", "Data")]
        private SerializedDictionary<string, TStruct> dataMap;


        public bool GetRow(string rowName, out TStruct result) => dataMap.TryGetValue(rowName, out result);

        public override bool HasRow(string rowName) => dataMap.ContainsKey(rowName);

        public override string[] GetAllRowNames() => dataMap.Keys.ToArray();

        public TStruct[] GetAllRowValues() => dataMap.Values.ToArray();

        public override DataTableRowHandle[] GetAllRowHandles()
        {
            return dataMap.Select(item => new DataTableRowHandle(this, item.Key)).ToArray();
        }
    }
}