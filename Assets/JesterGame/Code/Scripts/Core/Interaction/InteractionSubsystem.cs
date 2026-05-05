using System.Collections.Generic;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core;
using UnrealToUnity.Code.Scripts.Core.Subsystems;

namespace JesterGame.Code.Scripts.Core.Interaction
{
    public class InteractionSubsystem : UnrealSubsystem
    {
        /// <summary>
        /// Store a set of all the interactable items in the world.
        /// This is updated by the InteractionHelperComponent when it is enabled/disabled.
        /// </summary>
        private readonly HashSet<InteractionHelperComponent> _interactableItems = new();

        protected override void CustomInitialize()
        {
            _interactableItems.Clear();
        }

        protected override void CustomUpdate(float deltaTime)
        {
        }

        protected override void CustomCleanUp()
        {
        }

        public void AddInteractionHelper(InteractionHelperComponent interactionHelperComponent)
        {
            _interactableItems.Add(interactionHelperComponent);
        }

        public void RemoveInteractionHelper(InteractionHelperComponent interactionHelperComponent)
        {
            _interactableItems.Remove(interactionHelperComponent);
        }

        public InteractionHelperComponent[] GetInteractionHelperComponents()
        {
            var components = new InteractionHelperComponent[_interactableItems.Count];
            _interactableItems.CopyTo(components);
            return components;
        }

        /// <summary>
        /// Add this subsystem to the GameBootstrapper so that it gets initialized.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void AddToBootstrapper()
        {
            var subSystem = new InteractionSubsystem();
            GameBootstrapper.RegisterSubsystem(subSystem);
        }
    }
}