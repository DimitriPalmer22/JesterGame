using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Player;
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

        public static PlayerController GetPlayerController(int index = 0)
        {
            if (!GetGameMode(out GameMode.GameMode gameMode))
                return null;

            return gameMode.GetPlayerController(index);
        }

        #region Vector Functions

        public static Vector3 NoYNormalized(this Vector3 vector)
        {
            vector.y = 0f;
            return vector.normalized;
        }

        #endregion

        public static bool IsValidIndex<T>(this T[] array, int index)
        {
            return index >= 0 && index < array.Length;
        }
    }
}