using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnrealToUnity.Code.Scripts.Core.Utility.Interfaces;

namespace UnrealToUnity.Code.Scripts.Core.Cutscenes
{
    /// <summary>
    /// A component used to control cutscene code.
    /// Individual cutscenes should *probably* extend from this class.
    /// However, it is very possible to have some generic cutscene class that handles all cutscenes.
    /// This class uses an IEnumerator to handle the cutscene.
    /// </summary>
    /// <typeparam name="TCutsceneStruct">A struct type used to pass information to cutscenes. Usually only needs 1 per game.</typeparam>
    public abstract class CutsceneComponent<TCutsceneStruct> : CutsceneComponentBase, IOngoing<TCutsceneStruct>
        where TCutsceneStruct : struct
    {
        [SerializeField] public UnityEvent<TCutsceneStruct> onCutsceneStarted;
        [SerializeField] public UnityEvent<TCutsceneStruct> onCutsceneEnded;

        public void RunCutscene(TCutsceneStruct cutsceneStruct)
        {
            StartCoroutine(CustomRunCutscene(cutsceneStruct));
        }

        public IEnumerator OngoingCoroutine(TCutsceneStruct pawn)
        {
            // Invoke the start event.
            onCutsceneStarted?.Invoke(pawn);

            CutsceneFinishYield.StartCutscene();

            // Yield for the custom cutscene code.
            cutsceneCoroutine = StartCoroutine(CustomRunCutscene(pawn));
            yield return cutsceneCoroutine;
            cutsceneCoroutine = null;

            CutsceneFinishYield.Reset();

            // Invoke the end event.
            onCutsceneEnded?.Invoke(pawn);
        }

        protected abstract IEnumerator CustomRunCutscene(TCutsceneStruct cutsceneStruct);
    }
}