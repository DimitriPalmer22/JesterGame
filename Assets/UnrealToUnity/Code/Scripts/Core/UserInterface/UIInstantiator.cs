using System;
using System.Collections.Generic;
using JesterGame.Code.Scripts.Core;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Utility;
using UnrealToUnity.Code.Scripts.Core.Utility.Interfaces;

namespace UnrealToUnity.Code.Scripts.Core.UserInterface
{
    /// <summary>
    /// Class that instantiates all the UIs given a list of UI Data Assets
    /// </summary>
    [CreateAssetMenu(fileName = "UIInstantiator", menuName = "UnrealToUnity/UI/UIInstantiator")]
    public class UIInstantiator : ScriptableObject, IRunnable<JesterGameEventArgs>
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

                uiDataAsset.asyncInstantiationOp = uiDataAsset.InstantiateScreenAsync();
            }
        }

        public void Run(JesterGameEventArgs args)
        {
            InstantiateScreensAsync();
        }
    }
}