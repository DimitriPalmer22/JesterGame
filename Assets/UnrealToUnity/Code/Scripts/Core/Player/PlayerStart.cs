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

        /// <summary>
        /// Priority of this player start.
        /// If there are multiple player starts in the level, the one with the highest priority will be used.
        /// </summary>
        [SerializeField] private int priority = 0;

        public Transform GetSpawnTransform()
        {
            if (spawnTransform)
                return spawnTransform;

            return gameObject.transform;
        }

        public int Priority => priority;
    }
}