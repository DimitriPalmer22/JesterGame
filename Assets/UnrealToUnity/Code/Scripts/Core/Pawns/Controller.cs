using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.Pawns
{
    public abstract class Controller : Actor
    {
        #region Fields

        /// <summary>
        /// The pawn this controller is currently controlling.
        /// </summary>
        private Pawn _controlledPawn;

        /// <summary>
        /// Should this controller mimic the pawn's position?
        /// </summary>
        [SerializeField] public bool bFollowPawn = true;

        #endregion

        #region Functions

        public void Update()
        {
            // If the controller is set to follow the pawn, then set the position accordingly.
            if (bFollowPawn)
                gameObject.transform.position = _controlledPawn.transform.position;
        }

        /// <summary>
        /// A function to take control of a pawn.
        /// </summary>
        public void Possess(Pawn pawn)
        {
            // If the currently controlled pawn is not null, unpossess it first.
            if (_controlledPawn)
                UnPossess();

            // If the new pawn is null, return
            if (!pawn)
                return;

            // Set the references in both this controller and the pawn.
            _controlledPawn = pawn;
            _controlledPawn.owningController = this;

            // If following the pawn, immediately set the position
            if (bFollowPawn)
                gameObject.transform.position = _controlledPawn.transform.position;

            // Log that this controller is possessing the pawn
            Debug.Log($"{name} is now possessing {pawn.name}");
        }

        public void UnPossess()
        {
            // Return if there is no currently controlled pawn.
            if (!_controlledPawn)
                return;

            var oldPawn = _controlledPawn;

            // Clear the references in both the controller and the pawn
            _controlledPawn.owningController = null;
            _controlledPawn = null;

            // Log that this controller is unpossessing the pawn
            Debug.Log($"{name} has unpossessed {oldPawn.name}");
        }

        #endregion
    }
}