using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.GameMode
{
    /// <summary>
    /// Monobehaviour class that represents the game mode.
    /// Supposed to mimic behavior of Unreal Engine's GameMode class, which is responsible for managing the rules and flow of the game.
    /// Since this class *needs* to attach to a GameObject, we can use it to manage the lifecycle of the game mode,
    /// and also to store any data that needs to persist across scenes.
    /// We can also abstractly define any components that *also* need to be on the game mode.
    /// </summary>
    public abstract class GameMode : MonoBehaviour
    {
    }
}