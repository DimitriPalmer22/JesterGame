using System;
using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.UserInterface
{
    /// <summary>
    /// A class to call functions on externally to open and interact with UI screens.
    /// </summary>
    public abstract class UIDataAsset<TScreenType> : ScriptableObject
        where TScreenType : UIScreen
    {
        /// <summary>
        /// The screen this data asset is associated with.
        /// This is used to open the screen and call functions on it.
        /// </summary>
        [SerializeField] protected TScreenType screenPrefab;

        /// <summary>
        /// A bool to keep track of if the screen prefab has been instantiated yet.
        /// This is used to prevent multiple instances of the screen from being created if the data asset is used multiple times.
        /// </summary>
        [NonSerialized] private bool _hasBeenInstantiated = false;

        /// <summary>
        /// The instance of the screen.
        /// </summary>
        [NonSerialized] private TScreenType _instantiatedScreen;

        protected void AssertValueSet()
        {
            Debug.Assert(screenPrefab != null, $"Screen reference is null in {name}. Please assign.");
        }

        protected TScreenType GetScreen()
        {
            AssertValueSet();

            // If the screen has already been instantiated, return the existing instance.
            if (_hasBeenInstantiated)
                return _instantiatedScreen;

            // Otherwise, instantiate the screen and return it.
            _instantiatedScreen = Instantiate(screenPrefab);
            _hasBeenInstantiated = true;
            return _instantiatedScreen;
        }
    }
}