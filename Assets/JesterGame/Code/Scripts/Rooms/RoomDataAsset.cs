using UnityEngine;
using UnityEngine.Events;
using UnrealToUnity.Code.Scripts.Core.DataTables;

namespace JesterGame.Code.Scripts.Rooms
{
    /// <summary>
    /// A scriptable object used only to hold references to different rooms.
    /// </summary>
    [CreateAssetMenu(fileName = "Room_", menuName = "JesterGame/RoomDataAsset")]
    public class RoomDataAsset : ScriptableObject
    {
        [SerializeField] public string roomName;

        [SerializeField] public UnityEvent<RoomEventArgs> onRoomEntered;
    }
}