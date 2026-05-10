using System.Collections.Generic;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Subsystems;
using UnrealToUnity.Code.Scripts.Core.UserInterface;

namespace UnrealToUnity.Code.Scripts.Core
{
    public static class GameBootstrapper
    {
        private static readonly HashSet<UnrealSubsystem> ExtraRegisteredSubsystems = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnGameStart()
        {
            // Create a list of subsystems to initialize on startup.
            var startupSubsystems = new UnrealSubsystem[]
            {
                new GameInstanceSubsystem(),
                new UISubsystem(),
                new TimeScaleSubsystem()
            };

            foreach (var startupSubsystem in startupSubsystems)
                UnrealSubsystemManager.Instance.Add(startupSubsystem);

            foreach (var startupSubsystem in ExtraRegisteredSubsystems)
                UnrealSubsystemManager.Instance.Add(startupSubsystem);

            // Debug.Log($"Initialized subsystem: {GetType().Name}");

            var initializationString = "Initialized Subsystems:";
            foreach (var subsystem in UnrealSubsystemManager.Instance)
                initializationString += $"\n- {subsystem.GetType().Name}";
            Debug.Log(initializationString);
        }

        public static void RegisterSubsystem(UnrealSubsystem subsystem)
        {
            ExtraRegisteredSubsystems.Add(subsystem);
        }
    }
}