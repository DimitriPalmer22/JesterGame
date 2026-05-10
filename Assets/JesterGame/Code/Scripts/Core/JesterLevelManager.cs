using System;
using JesterGame.Code.Scripts.Characters.Behaviors.PointsOfInterest;
using JesterGame.Code.Scripts.Rooms;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Levels;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Core
{
    public class JesterLevelManager : LevelManager
    {
        [SerializeField] private RoomDataAsset roomDataAsset;
        [SerializeField] private PointOfInterest[] pointsOfInterest;

        private void OnEnable()
        {
            // Add this to the room data map
            if (UtilLibrary.GetGameMode(out ImpostorGameMode gameMode) && roomDataAsset)
                gameMode.roomToLevelManagerMap.Add(roomDataAsset, this);
        }

        public PointOfInterest GetRandomPointOfInterest()
        {
            if (pointsOfInterest == null || pointsOfInterest.Length == 0)
                return null;

            var randomIndex = UnityEngine.Random.Range(0, pointsOfInterest.Length);
            return pointsOfInterest[randomIndex];
        }

        public PointOfInterest[] GetAllPointsOfInterest()
        {
            var copy = new PointOfInterest[pointsOfInterest.Length];
            Array.Copy(pointsOfInterest, copy, pointsOfInterest.Length);
            return copy;
        }

        public bool TryGetRoomDataAsset(out RoomDataAsset asset)
        {
            asset = roomDataAsset;
            return asset != null;
        }
    }
}