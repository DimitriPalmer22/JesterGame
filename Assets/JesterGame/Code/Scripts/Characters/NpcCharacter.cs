using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.AI;
using UnrealToUnity.Code.Scripts.Core.DataTables;
using UnrealToUnity.Code.Scripts.Core.Pawns;

namespace JesterGame.Code.Scripts.Characters
{
    [RequireComponent(typeof(AutoPossessor))]
    public class NpcCharacter : Pawn
    {
        /// <summary>
        /// The row handle corresponding to the NPC data in the data table.
        /// </summary>
        [SerializeField] private DataTableRowHandle npcDataHandle;
    }
}