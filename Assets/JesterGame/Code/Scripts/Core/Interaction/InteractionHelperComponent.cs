using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Core.Interaction
{
    /// <summary>
    /// A component used to represent an item that can be interacted with.
    /// This component itself doesn't have much logic for being interacted with.
    /// Instead, other scripts should extend from this component's public API.
    /// </summary>
    public class InteractionHelperComponent : MonoBehaviour
    {
        [SerializeField] private float interactionRange = 2f;
        [SerializeField] private bool bDisableAfterInteract = true;
        [SerializeField] private string interactText = "Interact";

        [SerializeField] public UnityEvent<InteractEventArgs> onInteract;
        [SerializeField] public UnityEvent<InteractEventArgs> onSelected;
        [SerializeField] public UnityEvent<InteractEventArgs> onDeselected;
        [SerializeField] public UnityEvent<InteractEventArgs> onSelectedTick;

        public string InteractText => interactText;

        private void OnEnable()
        {
            // Add this interaction helper to the InteractionSubsystem so that it can be detected by InteractorComponents.
            if (UtilLibrary.GetSubsystem(out InteractionSubsystem interactionSubsystem))
                interactionSubsystem.AddInteractionHelper(this);
        }

        private void OnDisable()
        {
            if (UtilLibrary.GetSubsystem(out InteractionSubsystem interactionSubsystem))
                interactionSubsystem.RemoveInteractionHelper(this);
        }

        private void OnDrawGizmosSelected()
        {
            // Draw a sphere indicating the interactionRange
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, interactionRange);
        }

        public bool CanInteract(InteractorComponent interactorComponent)
        {
            // If this is not enabled, return false.
            if (!enabled)
                return false;

            // If the interactor component is null, return false.
            if (interactorComponent == null)
                return false;

            // Check if the interactor is within the interaction range.
            var distance = Vector3.Distance(transform.position, interactorComponent.transform.position);
            return distance <= interactionRange;
        }

        public void Interact(InteractorComponent interactorComponent)
        {
            if (this == null || interactorComponent == null)
                return;

            // Invoke the onInteract event of this with this interactor as the argument.
            var args = new InteractEventArgs(interactorComponent, this);
            onInteract.Invoke(args);

            // Also invoke the onInteracted event of this interactor with the helper as the argument.
            interactorComponent.onInteracted.Invoke(args);

            if (bDisableAfterInteract)
                enabled = false;
        }

    }
}