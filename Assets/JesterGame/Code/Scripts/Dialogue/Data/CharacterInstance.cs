using System;
using AYellowpaper.SerializedCollections;
using JesterGame.Code.Scripts.Rooms;
using NaughtyAttributes;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.DataTables;

namespace JesterGame.Code.Scripts.Dialogue.Data
{
    [Serializable]
    public struct CharacterInstance
    {
        /// <summary>
        /// A reference to the dialogue character this instance handles.
        /// </summary>
        [SerializeField, Required] public DataTableRowHandle characterAsset;

        /// <summary>
        /// The type of the character, whether it's an impostor or not.
        /// </summary>
        [SerializeField] public CharacterType characterType;

        public const int MAX_AFFECTION = 100;
        public const int MIN_AFFECTION = 100;

        [SerializeField] public int currentAffection;

        [SerializedDictionary("Room Data Asset", "Has Completed First Interaction"), ReadOnly]
        public readonly SerializedDictionary<RoomDataAsset, bool> hasCompletedFirstInteractionPerRoom;

        public CharacterInstance(DataTableRowHandle characterAsset, CharacterType characterType)
        {
            this.characterAsset = characterAsset;
            this.characterType = characterType;
            currentAffection = 0;
            hasCompletedFirstInteractionPerRoom = new SerializedDictionary<RoomDataAsset, bool>();
        }
    }
}