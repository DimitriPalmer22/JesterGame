using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.DataTables;
using UnrealToUnity.Code.Scripts.Core.Pawns;

namespace JesterGame.Code.Scripts.Characters
{
    public class JesterGamePawn : Pawn
    {
        /// <summary>
        /// The row handle corresponding to the NPC data in the data table.
        /// </summary>
        [SerializeField] protected DataTableRowHandle npcDataHandle;
    }
}