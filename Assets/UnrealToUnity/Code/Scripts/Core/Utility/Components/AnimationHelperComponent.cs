using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.Utility.Components
{
    public class AnimationHelperComponent : MonoBehaviour
    {
        private static readonly Dictionary<string, int> StaticStringHashes = new();

        [NonSerialized] private readonly ManualYield _manualYield = new();

        [SerializeField, SerializedDictionary("Code Name", "State Name In Animator")]
        private SerializedDictionary<string, string> animationStateNames;

        [SerializeField] private Animator animator;

        private void Awake()
        {
            var keysCopy = animationStateNames.Keys.ToArray();

            // Create the hash values for the animation states
            foreach (var key in keysCopy)
                AddAnimationHash(key, animationStateNames[key]);
        }

        public IEnumerator PlayAnimationAndWait(string codeName)
        {
            yield return animator.PlayAnimationAndWait(GetHashFromCodeName(codeName), _manualYield);
        }

        public void PlayAnimation(string codeName)
        {
            animator.Play(GetHashFromCodeName(codeName), -1, 0);
        }

        private int GetHashFromCodeName(string codeName)
        {
            if (!animationStateNames.TryGetValue(codeName, out var stateName))
            {
                Debug.LogError($"Animation state name for '{codeName}' not found.");
                return -1;
            }

            return StaticStringHashes[stateName];
        }

        public void AddAnimationHash(string codeName, string nameInAnimator)
        {
            // Add the values to the dictionary
            animationStateNames[codeName] = nameInAnimator;

            // Add the hash values to the static dictionary
            if (!StaticStringHashes.ContainsKey(nameInAnimator))
                StaticStringHashes.Add(nameInAnimator, Animator.StringToHash(nameInAnimator));
        }

        public void FinishAnimation()
        {
            _manualYield.Reset();
            Debug.Log("Finished animation");
        }
    }
}