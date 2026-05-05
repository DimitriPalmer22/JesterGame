using System;
using System.Collections.Generic;
using UnrealToUnity.Code.Scripts.Core.Subsystems;

namespace UnrealToUnity.Code.Scripts.Core.UserInterface
{
    /// <summary>
    /// Used for screens that are meant to be used across the entire game, such as the main menu, pause menu, etc.
    /// This is not meant for UI elements that are specific to a level or a character, such as health bars, ammo counters, etc.
    /// Those should be implemented as MonoBehaviours on the relevant GameObjects.
    /// </summary>
    public class UISubsystem : UnrealSubsystem
    {
        private readonly Dictionary<Type, UIScreen> _screenMap = new();

        protected override void CustomInitialize()
        {
            _screenMap.Clear();
        }

        protected override void CustomUpdate(float deltaTime)
        {
        }

        protected override void CustomCleanUp()
        {
            _screenMap.Clear();
        }

        public bool TryAddScreen(UIScreen screen)
        {
            if (screen == null)
                return false;

            var screenType = screen.GetType();
            if (_screenMap.ContainsKey(screenType))
                return false;

            _screenMap[screenType] = screen;
            return true;
        }

        public bool GetScreen<T>(out T screen) where T : UIScreen
        {
            if (_screenMap.TryGetValue(typeof(T), out var foundScreen) && foundScreen is T typedScreen)
            {
                screen = typedScreen;
                return true;
            }

            screen = null;
            return false;
        }
    }
}