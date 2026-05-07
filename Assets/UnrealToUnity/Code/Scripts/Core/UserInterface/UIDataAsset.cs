using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnrealToUnity.Code.Scripts.Core.UserInterface
{
    public abstract class UIDataAssetBase : ScriptableObject
    {
        /// <summary>
        /// A bool to keep track of if the screen prefab has been instantiated yet.
        /// This is used to prevent multiple instances of the screen from being created if the data asset is used multiple times.
        /// </summary>
        [NonSerialized] protected internal bool hasBeenInstantiated = false;

        [NonSerialized] protected readonly CancellationTokenSource instantiationCancelSource = new();

        public abstract void InstantiateScreenAsync();
    }

    /// <summary>
    /// A class to call functions on externally to open and interact with UI screens.
    /// </summary>
    public abstract class UIDataAsset<TScreenType> : UIDataAssetBase
        where TScreenType : UIScreen
    {
        /// <summary>
        /// The screen this data asset is associated with.
        /// This is used to open the screen and call functions on it.
        /// </summary>
        [SerializeField] protected TScreenType screenPrefab;

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

            // instantiate the screen and return it.
            if (!hasBeenInstantiated)
            {
                // Cancel any ongoing async instantiation, since we're doing a synchronous instantiation now.
                instantiationCancelSource.Cancel();
                SetInstantiatedScreen(Instantiate(screenPrefab));
            }

            // If the instantiated screen is inactive for any reason (probably due to instantiation), activate it.
            if (_instantiatedScreen && !_instantiatedScreen.gameObject.activeSelf)
                _instantiatedScreen.gameObject.SetActive(true);

            return _instantiatedScreen;
        }

        public override void InstantiateScreenAsync()
        {
            var instantiateParameters = new InstantiateParameters()
            {
                originalImmutable = screenPrefab,
                parent = null,
                worldSpace = true,
                scene = SceneManager.GetActiveScene()
            };
            // var asyncOp = InstantiateAsync(screenPrefab, instantiateParameters, instantiationCancelSource.Token);
            var asyncOp = InstantiateAsync(screenPrefab);
            asyncOp.completed += OnAsyncInstantiationComplete;
        }

        private void OnAsyncInstantiationComplete(AsyncOperation operation)
        {
            if (operation is not AsyncInstantiateOperation<TScreenType> castOperation)
                return;

            SetInstantiatedScreen(castOperation.Result[0]);

            operation.completed -= OnAsyncInstantiationComplete;

            Debug.Log($"Instantiated screen {_instantiatedScreen} in {name} ({hasBeenInstantiated}).");
        }

        private void SetInstantiatedScreen(TScreenType screen)
        {
            _instantiatedScreen = screen;
            _instantiatedScreen.gameObject.SetActive(false);
            hasBeenInstantiated = _instantiatedScreen != null;
        }
    }
}