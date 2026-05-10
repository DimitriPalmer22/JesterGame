using System;

namespace UnrealToUnity.Code.Scripts.Core.Utility.FloatTokenManager
{
    [Serializable]
    public abstract class FloatTokenBase
    {
        internal FloatTokenManager currentTokenManager;
        private float _backingValue = 1f;

        public float Value => _backingValue;

        public event FloatTokenEventHandler OnValueChanged;

        protected void SetValue(float value)
        {
            _backingValue = value;
            OnValueChanged?.Invoke(currentTokenManager, this, value);
        }
    }
}