using UnityEngine;
using UnrealToUnity.Code.Scripts.Core.Utility.FloatTokenManager;

namespace UnrealToUnity.Code.Scripts.Core.Subsystems
{
    public class TimeScaleSubsystem : UnrealSubsystem
    {
        private readonly FloatTokenManager _floatTokenManager = new(FloatTokenManagerType.Multiply);
        private readonly SimpleFloatToken _gameSpeedToken = new(1);
        private readonly SimpleFloatToken _debugSpeedToken = new(1);

        protected override void CustomInitialize()
        {
            _floatTokenManager.ResetTokens();
            _floatTokenManager.OnValueChanged += OnTokenManagerValueChanged;

            // Add the game speed and debug tokens
            _floatTokenManager.AddToken(_gameSpeedToken);
            _floatTokenManager.AddToken(_debugSpeedToken);
        }

        private void OnTokenManagerValueChanged(FloatTokenManager sender, FloatTokenBase token, float value)
        {
            Time.timeScale = value;
        }

        protected override void CustomUpdate(float deltaTime)
        {
        }

        protected override void CustomCleanUp()
        {
        }

        public void SetGameSpeed(float speed) => _gameSpeedToken.SetTokenValue(speed);
        public void SetDebugSpeed(float speed) => _debugSpeedToken.SetTokenValue(speed);
    }
}