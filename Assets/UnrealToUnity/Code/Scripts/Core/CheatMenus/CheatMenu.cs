using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Player;

namespace UnrealToUnity.Code.Scripts.Core.CheatMenus
{
    /// <summary>
    /// A monobehaviour class used to contain cheat functions that are useful during runtime.
    /// </summary>
    public abstract class CheatMenu : MonoBehaviour
    {
        /// <summary>
        /// The player controller this cheat menu is bound to.
        /// Any functions that affect specific controllers / pawns will use this controller.
        /// </summary>
        public PlayerController PlayerController { get; private set; }

        protected abstract string CheatMenuTypeName { get; }

        public void Initialize(PlayerController playerController)
        {
            this.PlayerController = playerController;

            // Change the name of this object.
            gameObject.name = $"{CheatMenuTypeName}_{playerController.gameObject.name}";
        }
    }
}