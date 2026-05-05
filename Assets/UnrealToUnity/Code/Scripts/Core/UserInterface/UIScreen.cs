using System;
using System.Collections;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Subsystems;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace UnrealToUnity.Code.Scripts.Core.UserInterface
{
    public abstract class UIScreen : MonoBehaviour
    {
        /// <summary>
        /// Whether to only allow one instance of this screen at a time by adding
        /// it to the UI subsystem.
        /// </summary>
        protected virtual bool AddToSubsystem => true;

        public bool IsOpen { get; private set; } = false;

        private Coroutine _openScreenCoroutine;
        private Coroutine _closeScreenCoroutine;

        private void Awake()
        {
            if (AddToSubsystem && UtilLibrary.GetSubsystem(out UISubsystem uiSubsystem))
            {
                // If there is no existing screen of this type, add this.
                if (uiSubsystem.TryAddScreen(this))
                {
                }

                // otherwise, destroy this instance since there is already one in the subsystem.
                else
                {
                    Debug.LogWarning($"A `{GetType().Name}` screen already exists in the UI subsystem. Destroying.");
                    Destroy(gameObject);
                    return;
                }
            }

            // Initialize
            CustomInitialize();
        }

        protected abstract void CustomInitialize();

        private void Start()
        {
            // Start
            CustomStart();
        }

        protected abstract void CustomStart();

        public IEnumerator OpenScreen()
        {
            // Stop the close screen coroutine
            if (_closeScreenCoroutine != null)
            {
                StopCoroutine(_closeScreenCoroutine);
                _closeScreenCoroutine = null;
            }

            _openScreenCoroutine = StartCoroutine(OpenScreenCoroutine());
            yield return _openScreenCoroutine;

            IsOpen = true;
            _openScreenCoroutine = null;
        }

        /// <summary>
        /// A coroutine / animation for opening the screen.
        /// This is called by the OpenScreen function, which can be called by external code to open the screen.
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerator OpenScreenCoroutine();

        public IEnumerator CloseScreen()
        {
            // Stop the open screen coroutine
            if (_openScreenCoroutine != null)
            {
                StopCoroutine(_openScreenCoroutine);
                _openScreenCoroutine = null;
            }

            _closeScreenCoroutine = StartCoroutine(CloseScreenCoroutine());
            yield return _closeScreenCoroutine;

            IsOpen = false;
            _closeScreenCoroutine = null;
        }

        protected abstract IEnumerator CloseScreenCoroutine();
    }
}