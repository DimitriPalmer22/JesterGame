using System;
using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.UserInterface
{
    public class UIInstantiatorComponent : MonoBehaviour
    {
        [SerializeField] public UIInstantiator uiInstantiator;

        private void Awake()
        {
            uiInstantiator?.InstantiateScreensAsync();
        }
    }
}