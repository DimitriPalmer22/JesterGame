using System;
using JesterGame.Code.Scripts.Characters;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Pawns;

namespace JesterGame.Code.Scripts.Core
{
    [Serializable]
    public struct JesterGameEventArgs
    {
        [SerializeField] public Controller controller;
        [SerializeField] public JesterGamePawn pawn;

        [SerializeField] public Vector3 position;
        [SerializeField] public float magnitude;
    }
}