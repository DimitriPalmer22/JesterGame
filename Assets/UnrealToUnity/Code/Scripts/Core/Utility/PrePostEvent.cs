using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnrealToUnity.Code.Scripts.Core.Utility
{
    [Serializable]
    public struct PrePostEvent<TArgs> where TArgs : struct
    {
        [SerializeField] public UnityEvent<TArgs> preEvent;
        [SerializeField] public UnityEvent<TArgs> onEvent;
        [SerializeField] public UnityEvent<TArgs> postEvent;
    }
}