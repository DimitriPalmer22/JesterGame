using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Core.Interaction
{
    /// <summary>
    /// A component used to detect which items nearby are able to be inteacted with.
    /// </summary>
    public class InteractorComponent : MonoBehaviour
    {
        /// <summary>
        /// Store the currently selected interaction helper.
        /// </summary>
        private InteractionHelperComponent _currentHelper;

        [SerializeField, BoxGroup("Events")] public UnityEvent<InteractEventArgs> onFocusedInteractableChanged;
        [SerializeField, BoxGroup("Events")] public UnityEvent<InteractEventArgs> onInteracted;

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
                if (closestHelper != null && distance > closestDistance)
                    continue;

                closestHelper = helper;
                closestDistance = distance;
            }

            return closestHelper;
        }

        private void Interact(InteractionHelperComponent helper)
        {
            if (helper == null)
                return;

            // Invoke the onInteract event of the helper with this interactor as the argument.
            var args = new InteractEventArgs(this, helper);
            helper.onInteract.Invoke(args);

            // Also invoke the onInteracted event of this interactor with the helper as the argument.
            onInteracted.Invoke(args);
        }

        [Button(enabledMode: EButtonEnableMode.Playmode)]
        public void InteractWithSelected() => Interact(_currentHelper);

        public void InteractInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
                InteractWithSelected();
        }
    }
}