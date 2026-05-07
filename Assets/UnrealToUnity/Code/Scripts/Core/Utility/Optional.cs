using System;
using NaughtyAttributes;
using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.Utility
{
    /// <summary>
    /// An optional wrapper for structs.
    /// Allows us to more easily check if a value has been set.
    /// </summary>
    /// <typeparam name="TStruct"></typeparam>
    [Serializable]
    public struct Optional<TStruct> where TStruct : struct
    {
        public delegate bool TryGetDelegate(out TStruct outValue);

        [SerializeField] private bool bHasValue;
        [SerializeField, ShowIf("bHasValue")] private TStruct value;

        public Optional(TStruct value)
        {
            this.value = value;
            bHasValue = true;
        }

        private Optional(bool bHasValue, TStruct value)
        {
            this.bHasValue = bHasValue;
            this.value = value;
        }

        public static Optional<TStruct> Empty => new(false, default);

        public bool TryGetValue(out TStruct outValue)
        {
            if (bHasValue)
            {
                outValue = value;
                return true;
            }

            outValue = default;
            return false;
        }

        /// <summary>
        /// Returns the value without first checking if it has been set.
        /// Use with caution.
        /// </summary>
        public TStruct GetValueUnsafe => value;

        public void FromTryGet(TryGetDelegate tryGetDelegate)
        {
            bHasValue = tryGetDelegate(out value);
        }

        public void Reset()
        {
            bHasValue = false;
            value = default;
        }

        public static implicit operator Optional<TStruct>(TStruct value)
        {
            return new Optional<TStruct>
            {
                bHasValue = true,
                value = value
            };
        }

        public static implicit operator bool(Optional<TStruct> optional)
        {
            return optional.bHasValue;
        }

        public override string ToString()
        {
            if (!bHasValue)
                return "[NO VALUE]";

            return value.ToString();
        }
    }
}