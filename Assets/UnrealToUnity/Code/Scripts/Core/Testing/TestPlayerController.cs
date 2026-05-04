using UnityEngine;
using UnityEngine.InputSystem;
using UnrealToUnity.Code.Scripts.Core.Player;

namespace UnrealToUnity.Code.Scripts.Core.Testing
{
    public class TestPlayerController : PlayerController
    {
        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.phase != InputActionPhase.Started)
                return;

            Debug.Log("Pause button pressed");
        }
    }
}