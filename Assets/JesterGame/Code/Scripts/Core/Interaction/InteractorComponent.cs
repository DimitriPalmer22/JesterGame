using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Core.Interaction
{
    /// <summary>
    /// A component used to detect which items nearby are able to be interacted with.
    /// </summary>
    public class InteractorComponent : MonoBehaviour
    {
        /// <summary>
        /// Store the currently selected interaction helper.
        /// </summary>
        private InteractionHelperComponent _currentHelper;

        [SerializeField, Foldout("Events")] public UnityEvent<InteractEventArgs> onFocusedInteractableChanged;
        [SerializeField, Foldout("Events")] public UnityEvent<InteractEventArgs> onInteracted;

        [NonSerialized] private bool bQuitting;

        private void OnDisable()
        {
            if (bQuitting || gameObject == null || this == null)
                return;

            var args = new InteractEventArgs(this, null);
            args.previousHelper = _currentHelper;

            if (_currentHelper != null)
            {
                _currentHelper.onDeselected.Invoke(args);
                _currentHelper = null;
            }

            onFocusedInteractableChanged.Invoke(args);
        }

        private void Update()
        {
            // If the current helper is NOT the current helper, invoke events
            var newHelper = FindBestHelper();
            if (newHelper != _currentHelper)
            {
                var args = new InteractEventArgs(this, newHelper);
                args.previousHelper = _currentHelper;

                if (_currentHelper != null)
                    _currentHelper.onDeselected.Invoke(args);

                if (newHelper != null)
                    newHelper.onSelected.Invoke(args);

                _currentHelper = newHelper;

                // Invoke the onFocusedInteractableChanged event of this interactor with the new helper as the argument.
                onFocusedInteractableChanged.Invoke(args);
            }

            // Tick the onSelected event of the current helper if it exists.
            _currentHelper?.onSelectedTick.Invoke(new InteractEventArgs(this, _currentHelper));
        }

        private InteractionHelperComponent FindBestHelper()
        {
            // Cannot find any helpers if the component is not enabled.
            if (!enabled)
                return null;

            if (!UtilLibrary.GetSubsystem(out InteractionSubsystem interactionSubsystem))
                return null;

            var interactionHelpers = interactionSubsystem.GetInteractionHelperComponents();

            // Find the closest candidate for interaction
            InteractionHelperComponent closestHelper = null;
            var closestDistance = float.MaxValue;
            foreach (var helper in interactionHelpers)
            {
                var distance = Vector3.Distance(transform.position, helper.transform.position);

                // If we can't interact with the helper, then skip it.
                var canInteract = helper.CanInteract(this);
                if (!canInteract)
                    continue;

                // If not a better candidate than the current closest helper, then skip it.
                if (closestHelper && distance > closestDistance)
                    continue;

                closestHelper = helper;
                closestDistance = distance;
            }

            return closestHelper;
        }

        private void Interact(InteractionHelperComponent helper)
        {
            // Ignore if not enabled
            if (!enabled)
                return;

            if (helper)
                helper.Interact(this);
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void InteractWithSelected() => Interact(_currentHelper);

        public void InteractInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
                InteractWithSelected();
        }

        private void OnDrawGizmos()
        {
            if (_currentHelper)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, _currentHelper.transform.position);
                Gizmos.DrawWireSphere(_currentHelper.transform.position, _currentHelper.InteractionRange);
            }
        }

        private void OnApplicationQuit()
        {
            bQuitting = true;
        }
    }
}