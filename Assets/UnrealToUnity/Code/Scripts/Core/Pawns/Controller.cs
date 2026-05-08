using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.Pawns
{
    public abstract class Controller : Actor
    {
        #region Fields

        /// <summary>
        /// The pawn this controller is currently controlling.
        /// </summary>
        public Pawn ControlledPawn { get; private set; }

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
                gameObject.transform.position = ControlledPawn.transform.position;
        }

        /// <summary>
        /// A function to take control of a pawn.
        /// </summary>
        public void Possess(Pawn pawn)
        {
            // If the currently controlled pawn is not null, unpossess it first.
            if (ControlledPawn)
                UnPossess();

            // If the new pawn is null, return
            if (!pawn)
                return;

            // Set the references in both this controller and the pawn.
            ControlledPawn = pawn;
            ControlledPawn.owningController = this;

            // If following the pawn, immediately set the position
            if (bFollowPawn)
            {
                gameObject.transform.position = ControlledPawn.transform.position;
                transform.parent = ControlledPawn.transform;
            }

            CustomPossess(pawn);

            // // Log that this controller is possessing the pawn
            // Debug.Log($"{name} is now possessing {pawn.name}");
        }

        protected abstract void CustomPossess(Pawn pawn);

        public void UnPossess()
        {
            // Return if there is no currently controlled pawn.
            if (!ControlledPawn)
                return;

            var oldPawn = ControlledPawn;

            // Clear the references in both the controller and the pawn
            ControlledPawn.owningController = null;
            ControlledPawn = null;

            if (transform.parent == oldPawn.transform)
                transform.parent = null;

            CustomUnPossess(oldPawn);

            // Log that this controller is unpossessing the pawn
            Debug.Log($"{name} has unpossessed {oldPawn.name}");
        }

        protected abstract void CustomUnPossess(Pawn pawn);

        #endregion
    }
}