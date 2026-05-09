using System;
using AYellowpaper.SerializedCollections;
using JesterGame.Code.Scripts.Dialogue.DialogueGraph.Runtime;
using JesterGame.Code.Scripts.Rooms;
using UnityEngine;

namespace JesterGame.Code.Scripts.Dialogue.Data
{
    [Serializable]
    public struct DialoguePool
    {
        /// <summary>
        /// The *first* dialogue interaction per room.
        /// </summary>
        [SerializedDictionary("Room Data Asset", "Dialogue Graph")]
        public SerializedDictionary<RoomDataAsset, RuntimeDialogueGraph> firstInteractionPerRoom;

        /// <summary>
        /// A list of dialogue interactions that can be randomly selected from when the player interacts with a room.
        /// These interactions will be used if the player has already completed the first interaction for that room.
        /// </summary>
        [SerializeField] public RuntimeDialogueGraph[] randomInteractions;
    }
}