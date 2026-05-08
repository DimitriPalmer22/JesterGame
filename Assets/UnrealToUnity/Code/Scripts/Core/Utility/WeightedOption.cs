using System;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Utility.Interfaces;

namespace UnrealToUnity.Code.Scripts.Core.Utility
{
    [Serializable]
    public struct WeightedOption<TType> : IWeightedSelection
    {
        [SerializeField] public TType value;
        [SerializeField] public float weight;

        public WeightedOption(TType value, float weight = 1f)
        {
            this.value = value;
            this.weight = weight;
        }

        public WeightedOption(float weight) : this(default, weight)
        {
        }

        public static implicit operator TType(WeightedOption<TType> option) => option.value;

        public static implicit operator WeightedOption<TType>(TType value) => new(value);

        public float Weight => weight;
    }
}