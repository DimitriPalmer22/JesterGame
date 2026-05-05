using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.DataTables
{
    public abstract class DataTableBase : ScriptableObject
    {
        public abstract bool HasRow(string rowName);

        public abstract string[] GetAllRowNames();

        public abstract void ValidateTable();

        public abstract DataTableRowHandle[] GetAllRowHandles();
    }
}