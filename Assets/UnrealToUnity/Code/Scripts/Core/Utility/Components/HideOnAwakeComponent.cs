using System;
using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.Utility.Components
{
    public class HideOnAwakeComponent : MonoBehaviour
    {
        private void Awake()
        {
            var renderers = GetComponentsInChildren<Renderer>(true);
            foreach (var cRenderer in renderers)
                cRenderer.enabled = false;
        }
    }
}