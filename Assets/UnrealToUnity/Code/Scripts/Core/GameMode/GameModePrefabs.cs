using System;
using UnrealToUnity.Code.Scripts.Core.Pawns;
using UnrealToUnity.Code.Scripts.Core.Player;

namespace UnrealToUnity.Code.Scripts.Core.GameMode
{
    [Serializable]
    public struct GameModePrefabs
    {
        /// <summary>
        /// The number of player controllers to instantiate.
        /// </summary>
        public int numberOfPlayers;

        /// <summary>
        /// The player controller prefab to use for this game mode.
        /// </summary>
        public PlayerController playerControllerPrefab;

        /// The pawn prefab to use for this game mode.
        public Pawn pawnPrefab;
    }
}