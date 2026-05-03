using UnrealToUnity.Code.Scripts.Core.Subsystems;

namespace UnrealToUnity.Code.Scripts.Core.Utility
{
    /// <summary>
    /// A static library of useful functions.
    /// </summary>
    public static class UtilLibrary
    {
        public static bool GetSubsystem<TSubsystem>(out TSubsystem subsystem) where TSubsystem : UnrealSubsystem
        {
            return UnrealSubsystemManager.Instance.GetSubsystem(out subsystem);
        }

        public static bool GetGameMode<TGameMode>(out TGameMode gameMode) where TGameMode : GameMode.GameMode
        {
            gameMode = null;

            // Try to get the game instance subsystem.
            if (!UnrealSubsystemManager.Instance.GetSubsystem(out GameInstanceSubsystem subsystem))
                return false;

            // cast the game mode to the correct type
            gameMode = subsystem.CurrentGameMode as TGameMode;

            return gameMode != null;
        }
    }
}