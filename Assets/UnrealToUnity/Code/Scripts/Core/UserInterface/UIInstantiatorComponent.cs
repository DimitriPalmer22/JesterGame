using System;
using JesterGame.Code.Scripts.Core;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Utility.Interfaces;

namespace UnrealToUnity.Code.Scripts.Core.UserInterface
{
    public class UIInstantiatorComponent : MonoBehaviour, IRunnable<JesterGameEventArgs>
    {
        [SerializeField] public UIInstantiator uiInstantiator;

        private void Awake()
        {
            Run(default);
        }

        public void Run(JesterGameEventArgs args)
        {
            uiInstantiator?.InstantiateScreensAsync();
        }
    }
}