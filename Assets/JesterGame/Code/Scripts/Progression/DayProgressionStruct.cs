using System;
using JesterGame.Code.Scripts.Core;
using UnityEngine;
using UnityEngine.Events;
using UnrealToUnity.Code.Scripts.Core.Cutscenes;
using UnrealToUnity.Code.Scripts.Core.Utility;

namespace JesterGame.Code.Scripts.Progression
{
    [Serializable]
    public struct DayProgressionStruct
    {
        /// <summary>
        /// How many times do we interact with people before the game progresses to the next day.
        /// </summary>
        [SerializeField, Delayed] public int numProgressionsInDay;

        /// <summary>
        /// The minimum affection needed for the impostor to NOT kill someone.
        /// Anything lower than this, and the kill a random NPC.
        /// </summary>
        [SerializeField, Delayed] public int minimumImpostorAffection;

        [SerializeField] public PrePostEvent<ProgressionEventArgs> dayStartEvents;
        [SerializeField] public PrePostEvent<ProgressionEventArgs> dayEndEvents;

        [SerializeField] public CutsceneComponent<ProgressionEventArgs> dayStartCutscene;
        [SerializeField] public CutsceneComponent<ProgressionEventArgs> dayEndCutscene;
    }
}