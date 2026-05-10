using System;

namespace UnrealToUnity.Code.Scripts.Core.Utility.FloatTokenManager
{
    [Serializable]
    public class SimpleFloatToken : FloatTokenBase
    {
        public SimpleFloatToken()
        {

        }

        public SimpleFloatToken(float value)
        {
            SetTokenValue(value);
        }

        public void SetTokenValue(float value) => SetValue(value);
    }
}