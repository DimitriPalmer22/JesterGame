using System;
using JesterGame.Code.Scripts.Characters;
using UnityEngine;
using UnityEngine.Events;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Core.Proximity
{
    public class ProximityActivationComponent : MonoBehaviour
    {
        [SerializeField] private float range = 5f;
        [SerializeField] private bool bActivateOnce = true;

        [SerializeField] private UnityEvent<JesterGameEventArgs> onActivate;

        private void Update()
        {
            if (!enabled)
                return;

            var playerController = UtilLibrary.GetPlayerController(0);

            if (playerController == null || playerController.ControlledPawn == null)
                return;

            // Check distance to player pawn
            var distance = Vector3.Distance(playerController.ControlledPawn.transform.position, transform.position);

            if (distance < range)
            {
                var args = new JesterGameEventArgs
                {
                    controller = playerController,
                    pawn = playerController.ControlledPawn as JesterGamePawn,
                    position = transform.position,
                    magnitude = distance
                };

                Activate(args);
            }
        }

        private void Activate(JesterGameEventArgs args)
        {
            onActivate?.Invoke(args);

            if (bActivateOnce)
                enabled = false;
        }

        private void OnDrawGizmos()
        {
            if (!enabled)
                return;

            // Draw a wire sphere to visualize the activation range
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}