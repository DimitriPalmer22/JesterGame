using System;
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
    }
}