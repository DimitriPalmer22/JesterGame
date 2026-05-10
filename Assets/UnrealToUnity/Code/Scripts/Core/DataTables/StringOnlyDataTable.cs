using System;
using UnityEditor;
using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.DataTables
{
    public class StringOnlyDataTable : DataTable<StringOnlyDataTable.StringOnlyTableStruct>
    {
        [Serializable]
        public struct StringOnlyTableStruct
        {
        }
    }
}