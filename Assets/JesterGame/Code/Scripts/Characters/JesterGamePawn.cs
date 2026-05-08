using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper;
using JesterGame.Code.Scripts.Characters.Behaviors;
using JesterGame.Code.Scripts.Dialogue.Data;
using NaughtyAttributes;
using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.DataTables;
using UnrealToUnity.Code.Scripts.Core.Pawns;

namespace JesterGame.Code.Scripts.Characters
{
    public abstract class JesterGamePawn : Pawn
    {
        #region Serialized Fields

        /// <summary>
        /// The row handle corresponding to the NPC data in the data table.
        /// </summary>
        [SerializeField] protected DataTableRowHandle npcDataHandle;

        [SerializeField, Header("Character Behavior")]
        protected bool bDoCharacterBehavior;

        [SerializeField, ShowIf("bDoCharacterBehavior")]
        protected InterfaceReference<ICharacterBehavior> waitBehavior;

        [SerializeField, ShowIf("bDoCharacterBehavior")]
        protected InterfaceReference<ICharacterBehavior>[] validBehaviors;

        [SerializeField, ReadOnly, ShowIf("bDoCharacterBehavior")]
        protected List<InterfaceReference<ICharacterBehavior>> behaviorQueue = new();

        #endregion

        #region Private Fields

        private Coroutine _mainBehaviorCoroutine;
        private Coroutine _currentBehaviorCoroutine;

        #endregion

        protected override void CustomOnEnable()
        {
            base.CustomOnEnable();

            if (_mainBehaviorCoroutine != null)
                StopCoroutine(_mainBehaviorCoroutine);

            if (bDoCharacterBehavior)
                _mainBehaviorCoroutine = StartCoroutine(MainBehaviorCoroutine());
        }

        protected override void CustomOnDisable()
        {
            base.CustomOnDisable();

            if (_mainBehaviorCoroutine != null)
                StopCoroutine(_mainBehaviorCoroutine);

            _mainBehaviorCoroutine = null;
        }

        public bool TryGetCharacterData(out DialogueCharacter characterData)
        {
            if (npcDataHandle.GetValue(out characterData))
                return true;

            Debug.LogError($"Failed to get character data for NPC {name} with handle {npcDataHandle}");
            return false;
        }

        private IEnumerator MainBehaviorCoroutine()
        {
            while (enabled && bDoCharacterBehavior)
            {
                // Try to pop something from the queue
                if (!DequeueBehavior(out var currentBehavior))
                {
                    // wait behavior if there are no valid actions.
                    if (waitBehavior != null && waitBehavior.Value != null)
                        currentBehavior = waitBehavior;
                    else
                    {
                        // Fallback: just wait and try again.
                        yield return new WaitForSecondsRealtime(0.1f);
                        continue;
                    }
                }

                var bHasCharacterData = TryGetCharacterData(out var characterData);

                // // Log the character starting the behavior
                // if (bHasCharacterData)
                //     Debug.Log($"{characterData.name} is starting behavior: {currentBehavior.Value.GetBehaviorName}");

                // Perform the current item & wait for it to finish.
                _currentBehaviorCoroutine = StartCoroutine(currentBehavior.Value.OngoingCoroutine(this));
                yield return _currentBehaviorCoroutine;

                // Log the character has ended the behavior
                if (bHasCharacterData)
                    Debug.Log($"{characterData.name} has ended behavior: {currentBehavior.Value.GetBehaviorName}");

                // TODO: Find a better way to decide behaviors
                if (behaviorQueue.Count <= 0 && validBehaviors != null && validBehaviors.Length > 0)
                {
                    // Get a random behavior from the valid behaviors
                    var randomIndex = UnityEngine.Random.Range(0, validBehaviors.Length);
                    behaviorQueue.Add(validBehaviors[randomIndex]);
                }
            }

            // Set the coroutine to null afterward.
            _mainBehaviorCoroutine = null;
        }

        #region Behavior Queue Functions

        public void EnqueueBehavior(InterfaceReference<ICharacterBehavior> behavior)
        {
            EnqueueBehavior(behavior.Value, behavior.UnderlyingValue);
        }

        public void EnqueueBehavior<TObject>(ICharacterBehavior behavior, TObject underlyingValue)
            where TObject : UnityEngine.Object
        {
            // Initialize the list if necessary.
            if (behaviorQueue == null)
                behaviorQueue = new List<InterfaceReference<ICharacterBehavior>>();

            // If either the behavior or underlying value are null, return
            if (behavior == null || underlyingValue == null)
            {
                Debug.LogWarning("Cannot enqueue behavior: behavior or underlying value is null.");
                return;
            }

            // Create the interface reference.
            var interfaceRef = new InterfaceReference<ICharacterBehavior>
            {
                Value = behavior,
                UnderlyingValue = underlyingValue
            };

            // Add the behavior to the end of the queue.
            behaviorQueue.Add(interfaceRef);
        }

        public void EnqueueBehaviors(IEnumerable<InterfaceReference<ICharacterBehavior>> behaviors)
        {
            foreach (var behaviorRef in behaviors)
                EnqueueBehavior(behaviorRef);
        }

        public bool DequeueBehavior(out InterfaceReference<ICharacterBehavior> behaviorRef)
        {
            behaviorRef = null;

            if (behaviorQueue == null || behaviorQueue.Count == 0)
                return false;

            behaviorRef = behaviorQueue[0];
            behaviorQueue.RemoveAt(0);

            return true;
        }

        public void ClearBehaviors() => behaviorQueue.Clear();

        #endregion
    }
}