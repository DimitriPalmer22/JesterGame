using System;
using System.Collections;
using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.Cutscenes
{
    public abstract class CutsceneComponentBase : MonoBehaviour
    {
        [NonSerialized] internal Coroutine cutsceneCoroutine;
        public Coroutine GetCutsceneCoroutine() => cutsceneCoroutine;

        /// <summary>
        /// A custom yield instruction that can be used to wait for the cutscene to finish.
        /// To be used externally.
        /// </summary>
        public WaitForCutsceneFinish CutsceneFinishYield { get; } = new();
    }
}