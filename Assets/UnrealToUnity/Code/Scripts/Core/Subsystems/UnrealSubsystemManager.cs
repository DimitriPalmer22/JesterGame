using System;
using System.Collections;
using System.Collections.Generic;

namespace UnrealToUnity.Code.Scripts.Core.Subsystems
{
    public class UnrealSubsystemManager : ICollection<UnrealSubsystem>
    {
        /// A list of all the active subsystems.
        private readonly HashSet<UnrealSubsystem> _subsystems = new();

        private readonly Dictionary<Type, UnrealSubsystem> _subsystemTypeMap = new();

        #region ICollection Implementation

        public int Count => _subsystems.Count;
        public bool IsReadOnly => false;

        public IEnumerator<UnrealSubsystem> GetEnumerator() => _subsystems.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(UnrealSubsystem item)
        {
            // If the subsystem is already in the list, we don't need to add it again.
            if (item == null || !_subsystems.Add(item) || _subsystemTypeMap.ContainsKey(item.GetType()))
                return;

            // Add the item to the dictionary as well, using its type as the key.
            _subsystemTypeMap[item.GetType()] = item;

            // Run the initialize function
            item.Initialize();
        }

        public void Clear()
        {
            // Run the cleanup function for each subsystem before clearing the list.
            foreach (var subsystem in _subsystems)
                subsystem.CleanUp();

            _subsystems.Clear();
            _subsystemTypeMap.Clear();
        }

        public bool Contains(UnrealSubsystem item)
        {
            return _subsystems.Contains(item);
        }

        public void CopyTo(UnrealSubsystem[] array, int arrayIndex)
        {
            _subsystems.CopyTo(array, arrayIndex);
        }

        public bool Remove(UnrealSubsystem item)
        {
            if (item == null)
                return false;

            var bWasRemoved = _subsystems.Remove(item);
            _subsystemTypeMap.Remove(item.GetType());

            // Run the cleanup function of the removed item
            if (bWasRemoved)
                item.CleanUp();

            return bWasRemoved;
        }

        #endregion

        #region Singleton Implementation

        public static UnrealSubsystemManager Instance { get; } = new UnrealSubsystemManager();

        #endregion

        public bool GetSubsystem<TValue>(out TValue subsystem) where TValue : UnrealSubsystem
        {
            var bIsFound = _subsystemTypeMap.TryGetValue(typeof(TValue), out var outValue);
            subsystem = bIsFound ? outValue as TValue : null;

            return bIsFound && subsystem != null;
        }
    }
}