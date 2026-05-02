using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Subsystems;

namespace UnrealToUnity.Code.Scripts.Core
{
    public static class GameBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnGameStart()
        {
            Debug.Log("Game is starting, no GameObject needed!");

            // Create a list of subsystems to initialize on startup.
            var startupSubsystems = new UnrealSubsystem[]
            {
            };
            foreach (var startupSubsystem in startupSubsystems)
                UnrealSubsystemManager.Instance.Add(startupSubsystem);
        }
    }
}