using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.Utility.FloatTokenManager
{
    public delegate void FloatTokenEventHandler(FloatTokenManager sender, FloatTokenBase token, float value);

    [Serializable]
    public class FloatTokenManager
    {
        [SerializeField] private FloatTokenManagerType tokenManagerType = FloatTokenManagerType.Multiply;

        private readonly List<FloatTokenBase> _floatTokens = new();

        public event FloatTokenEventHandler OnValueChanged;

        public FloatTokenManager()
        {
        }

        public FloatTokenManager(FloatTokenManagerType tokenManagerType = FloatTokenManagerType.Multiply)
        {
            this.tokenManagerType = tokenManagerType;
        }

        public bool AddToken(FloatTokenBase token)
        {
            // Return if the token is null
            if (token == null)
                return false;

            if (_floatTokens.Contains(token))
                return false;

            token.currentTokenManager?.RemoveToken(token);

            _floatTokens.Add(token);
            token.OnValueChanged += BroadcastCurrentValueOnTokenValueChanged;
            token.currentTokenManager = this;

            var currentValue = GetValue();
            OnValueChanged?.Invoke(this, token, currentValue);
            return true;
        }

        public bool RemoveToken(FloatTokenBase token)
        {
            // Return if the token is null
            if (token == null)
                return false;

            if (!_floatTokens.Contains(token))
                return false;

            _floatTokens.Remove(token);
            token.OnValueChanged -= BroadcastCurrentValueOnTokenValueChanged;
            token.currentTokenManager = null;

            var currentValue = GetValue();
            OnValueChanged?.Invoke(this, token, currentValue);
            return true;
        }

        public void ResetTokens()
        {
            while (_floatTokens.Count > 0)
            {
                var token = _floatTokens[0];
                _floatTokens.RemoveAt(0);

                token.OnValueChanged -= BroadcastCurrentValueOnTokenValueChanged;
            }

            _floatTokens.Clear();

            var currentValue = GetValue();
            OnValueChanged?.Invoke(this, null, currentValue);
        }

        public float GetValue()
        {
            float currentValue = 1;
            if (tokenManagerType == FloatTokenManagerType.Add)
                currentValue = 0;

            foreach (var token in _floatTokens)
            {
                switch (tokenManagerType)
                {
                    case FloatTokenManagerType.Multiply:
                        currentValue *= token.Value;
                        break;

                    case FloatTokenManagerType.Add:
                        currentValue += token.Value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return currentValue;
        }

        private void BroadcastCurrentValueOnTokenValueChanged(FloatTokenManager sender, FloatTokenBase token, float _)
        {
            var currentValue = GetValue();
            OnValueChanged?.Invoke(sender, token, currentValue);
        }
    }
}