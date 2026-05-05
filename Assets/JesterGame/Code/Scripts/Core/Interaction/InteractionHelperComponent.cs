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

        [SerializeField] public UnityEvent<InteractEventArgs> onInteract;
        [SerializeField] public UnityEvent<InteractEventArgs> onSelected;
        [SerializeField] public UnityEvent<InteractEventArgs> onDeselected;
        [SerializeField] public UnityEvent<InteractEventArgs> onSelectedTick;

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
            // Check if the interactor is within the interaction range.
            var distance = Vector3.Distance(transform.position, interactorComponent.transform.position);
            return distance <= interactionRange;
        }
    }
}