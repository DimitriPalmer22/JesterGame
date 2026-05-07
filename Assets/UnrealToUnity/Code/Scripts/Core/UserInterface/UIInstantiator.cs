using System;
using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.UserInterface
{
    /// <summary>
    /// Class that instantiates all the UIs given a list of UI Data Assets
    /// </summary>
    [CreateAssetMenu(fileName = "UIInstantiator", menuName = "UnrealToUnity/UI/UIInstantiator")]
    public class UIInstantiator : ScriptableObject
    {
        [SerializeField] public UIDataAssetBase[] uiDataAssets;

        public void InstantiateScreensAsync()
        {
            foreach (var uiDataAsset in uiDataAssets)
            {
                // If the screen has already been instantiated, skip it.
                // This prevents multiple instances of the same screen from being created if the data asset is used multiple times.
                if (!uiDataAsset || uiDataAsset.hasBeenInstantiated)
                    continue;

                uiDataAsset.InstantiateScreenAsync();
            }
        }
    }
}