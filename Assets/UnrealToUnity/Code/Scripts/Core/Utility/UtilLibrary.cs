using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnrealToUnity.Code.Scripts.Core.Player;
using UnrealToUnity.Code.Scripts.Core.Subsystems;
using UnrealToUnity.Code.Scripts.Core.Utility.Interfaces;

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

        public static T GetRandom<T>(this IEnumerable<T> list)
        {
            var toArray = list.ToArray();

            if (toArray.Length == 0)
                return default;

            var randomIndex = Random.Range(0, toArray.Length);
            return toArray[randomIndex];
        }

        public static TWeightedType GetRandomWeightedOption<TWeightedType>(this IEnumerable<TWeightedType> options)
            where TWeightedType : IWeightedSelection
        {
            var optionsAsArray = options.Where(n => n.Weight > 0).ToArray();
            var totalWeight = optionsAsArray.Sum(option => option.Weight);

            var randomValue = Random.Range(0f, totalWeight);
            var cumulativeWeight = 0f;

            TWeightedType lastOption = default;
            foreach (var option in optionsAsArray)
            {
                lastOption = option;

                cumulativeWeight += option.Weight;
                if (randomValue <= cumulativeWeight)
                    return option;
            }

            // In case of rounding errors, return the last option
            return lastOption;
        }

        public static bool IsAtDestination(this NavMeshAgent agent)
        {
            return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
        }

        public static IEnumerator MoveToCoroutine(this NavMeshAgent navMeshAgent, Vector3 destination)
        {
            // Set the destination of the nav mesh agent to the position of this point of interest
            navMeshAgent.SetDestination(destination);

            // Wait until the nav mesh agent has reached the destination
            while (!navMeshAgent.IsAtDestination())
                yield return null;
        }
    }
}