using UnityEngine;
using UnityEngine.Serialization;

namespace UnrealToUnity.Code.Scripts.Core.Player
{
    /// <summary>
    /// A position in the world where the player spawns from.
    /// </summary>
    public class PlayerStart : Actor
    {
        /// <summary>
        /// The transform that the player will spawn at when they enter the game.
        /// Uses the position and rotation of the transform to determine where the player spawns and which direction they face.
        /// @note If this is NOT set, then it uses the gameobject's transform.
        /// </summary>
        [SerializeField] private Transform spawnTransform;

        public Transform GetSpawnTransform()
        {
            if (spawnTransform)
                return spawnTransform;

            return gameObject.transform;
        }
    }
}