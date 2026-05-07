using System;
using AYellowpaper;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.DataTables;
using UnrealToUnity.Code.Scripts.Core.Pawns;

namespace JesterGame.Code.Scripts.Characters
{
    public abstract class JesterGamePawn : Pawn
    {
        /// <summary>
        /// The row handle corresponding to the NPC data in the data table.
        /// </summary>
        [SerializeField] protected DataTableRowHandle npcDataHandle;

        [SerializeField] protected InterfaceReference<ICharacterBehavior> characterBehavior;

        private void Start()
        {
            if (characterBehavior.Value != null)
                StartCoroutine(characterBehavior.Value.OngoingCoroutine(this));
        }
    }
}