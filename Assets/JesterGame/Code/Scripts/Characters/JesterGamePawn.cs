using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper;
using JesterGame.Code.Scripts.Characters.Behaviors;
using JesterGame.Code.Scripts.Characters.Behaviors.Brains;
using JesterGame.Code.Scripts.Core;
using JesterGame.Code.Scripts.Dialogue.Data;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
using UnrealToUnity.Code.Scripts.Core.DataTables;
using UnrealToUnity.Code.Scripts.Core.Pawns;
using UnrealToUnity.Code.Scripts.Core.Utility;
using UnrealToUnity.Code.Scripts.Core.Utility.Components;

namespace JesterGame.Code.Scripts.Characters
{
    public abstract class JesterGamePawn : Pawn
    {
        protected static readonly int APCurrentVelocity = Animator.StringToHash("currentVelocity");

        #region Serialized Fields

        /// <summary>
        /// The row handle corresponding to the NPC data in the data table.
        /// </summary>
        [SerializeField] protected DataTableRowHandle npcDataHandle;

        [SerializeField, Header("Character Behavior")]
        protected bool bDoCharacterBehavior;

        [SerializeField, ShowIf("bDoCharacterBehavior")]
        protected InterfaceReference<ICharacterBehavior> waitBehavior;

        [SerializeField, ReadOnly, ShowIf("bDoCharacterBehavior")]
        protected List<InterfaceReference<ICharacterBehavior>> behaviorQueue = new();

        [SerializeField] private CharacterBehaviorBrain innocentBrain;
        [SerializeField] private CharacterBehaviorBrain impostorBrain;

        [SerializeField] protected Animator animator;

        [SerializeField] protected AnimationHelperComponent animationHelper;

        [SerializeField] private UnityEvent<JesterGameEventArgs> onDeath;

        #endregion

        #region Private Fields

        private bool _isDead = false;

        private Coroutine _mainBehaviorCoroutine;
        private Coroutine _currentBehaviorCoroutine;
        private readonly ManualYield _activityYield = new ManualYield();

        #endregion

        public bool IsDead => _isDead;

        public AnimationHelperComponent AnimationHelper => animationHelper;

        protected override void CustomOnEnable()
        {
            base.CustomOnEnable();

            // Set the value in the map.
            if (UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
                gameMode.characterNameToPawnMap[npcDataHandle.RowName] = this;
        }

        protected override void CustomOnDisable()
        {
            base.CustomOnDisable();
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

                // var bHasCharacterData = TryGetCharacterData(out var characterData);
                // // Log the character starting the behavior
                // if (bHasCharacterData)
                //     Debug.Log($"{characterData.name} is starting behavior: {currentBehavior.Value.GetBehaviorName}");

                // Perform the current item & wait for it to finish.
                _currentBehaviorCoroutine = StartCoroutine(currentBehavior.Value.OngoingCoroutine(this));
                yield return _currentBehaviorCoroutine;

                // // Log the character has ended the behavior
                // if (bHasCharacterData)
                //     Debug.Log($"{characterData.name} has ended behavior: {currentBehavior.Value.GetBehaviorName}");

                // Use the correct brain to determine the next behavior(s) and enqueue them.
                if (behaviorQueue.Count <= 0)
                {
                    if (TryGetCurrentBrain(out var brain))
                    {
                        var nextBehaviors = brain.DetermineBehavior(this);
                        EnqueueBehaviors(nextBehaviors);
                    }

                    else
                        Debug.LogError($"Could not determine behaviors for NPC {name}");
                }
            }

            // Set the coroutine to null afterward.
            _mainBehaviorCoroutine = null;
        }

        public void StartMainBehaviorCoroutine(bool bClear)
        {
            StopMainBehaviorCoroutine(bClear);

            if (bDoCharacterBehavior)
                _mainBehaviorCoroutine = StartCoroutine(MainBehaviorCoroutine());
        }

        public void StopMainBehaviorCoroutine(bool bClear)
        {
            if (_currentBehaviorCoroutine != null)
                StopCoroutine(_currentBehaviorCoroutine);

            if (_mainBehaviorCoroutine != null)
                StopCoroutine(_mainBehaviorCoroutine);

            _currentBehaviorCoroutine = null;
            _mainBehaviorCoroutine = null;
        }

        private bool TryGetCurrentBrain(out CharacterBehaviorBrain brain)
        {
            brain = null;

            if (!TryGetCharacterData(out var characterData))
                return false;

            if (!UtilLibrary.GetGameMode(out ImpostorGameMode gameMode))
                return false;

            if (!gameMode.characterInstanceMap.TryGetValue(npcDataHandle.RowName, out var characterInstanceData))
                return false;

            brain = characterInstanceData.characterType switch
            {
                CharacterType.Normal => innocentBrain,
                CharacterType.Impostor => impostorBrain,
                CharacterType.Player => innocentBrain,
                _ => throw new ArgumentOutOfRangeException()
            };

            if (brain == null)
                return false;

            return true;
        }

        public PawnVfxHelper GetVfxHelper() => GetComponentInChildren<PawnVfxHelper>();

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

        public void OnAbilityAnimationFinished()
        {
            Debug.Log("Ability animation finished callback received.");
            _activityYield.Reset();
        }

        public virtual void Die()
        {
            if (IsDead)
                return;

            // StartCoroutine(AnimationHelper.PlayAnimationAndWait("Dead"));
            AnimationHelper.PlayAnimation("Dead");

            _isDead = true;

            var args = new JesterGameEventArgs
            {
                pawn = this,
                position = transform.position,
            };

            onDeath?.Invoke(args);
        }
    }
}