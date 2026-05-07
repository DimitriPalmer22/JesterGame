using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace UnrealToUnity.Code.Scripts.Core.Cutscenes
{
    /// <summary>
    /// A component used to control cutscene code.
    /// Individual cutscenes should *probably* extend from this class.
    /// However, it is very possible to have some generic cutscene class that handles all cutscenes.
    /// This class uses an IEnumerator to handle the cutscene.
    /// </summary>
    /// <typeparam name="TCutsceneStruct">A struct type used to pass information to cutscenes. Usually only needs 1 per game.</typeparam>
    public abstract class CutsceneComponent<TCutsceneStruct> : MonoBehaviour
        where TCutsceneStruct : struct
    {
        [SerializeField] public UnityEvent<TCutsceneStruct> onCutsceneStarted;
        [SerializeField] public UnityEvent<TCutsceneStruct> onCutsceneEnded;

        [NonSerialized] private Coroutine _cutsceneCoroutine;

        /// <summary>
        /// A custom yield instruction that can be used to wait for the cutscene to finish.
        /// To be used externally.
        /// </summary>
        public WaitForCutsceneFinish WaitForCutsceneFinish { get; private set; } = new();

        public void RunCutscene(TCutsceneStruct cutsceneStruct)
        {
            StartCoroutine(CustomRunCutscene(cutsceneStruct));
        }

        public IEnumerator RunCutsceneEnumerator(TCutsceneStruct cutsceneStruct)
        {
            // Invoke the start event.
            onCutsceneStarted?.Invoke(cutsceneStruct);

            WaitForCutsceneFinish.StartCutscene();

            // Yield for the custom cutscene code.
            _cutsceneCoroutine = StartCoroutine(CustomRunCutscene(cutsceneStruct));
            yield return _cutsceneCoroutine;
            _cutsceneCoroutine = null;

            WaitForCutsceneFinish.Reset();

            // Invoke the end event.
            onCutsceneEnded?.Invoke(cutsceneStruct);
        }

        protected abstract IEnumerator CustomRunCutscene(TCutsceneStruct cutsceneStruct);
    }
}