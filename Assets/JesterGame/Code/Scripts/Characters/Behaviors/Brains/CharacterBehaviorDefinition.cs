using System;
using AYellowpaper;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Utility.Interfaces;

namespace JesterGame.Code.Scripts.Characters.Behaviors.Brains
{
    [Serializable]
    public struct CharacterBehaviorDefinition : IWeightedSelection
    {
        [SerializeField] public string behaviorName;
        [SerializeField] public float weight;
        [SerializeField] public InterfaceReference<ICharacterBehavior>[] behaviors;

        public float Weight => weight;
    }
}