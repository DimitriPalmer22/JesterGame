using System.Collections.Generic;

namespace UnrealToUnity.Code.Scripts.Core.Utility
{
    /// <summary>
    /// Contains a set of objects.
    /// Returns true if there are any objects in the set. False otherwise.
    /// </summary>
    public class BoolTokenManager
    {
        public delegate void TokenEventHandler(bool hasTokens);

        public event TokenEventHandler OnTokensChanged;

        private readonly HashSet<object> _tokens = new();

        /// <summary>
        /// Adds a token to the set.
        /// </summary>
        /// <param name="token">The token to add.</param>
        public void AddToken(object token)
        {
            _tokens.Add(token);
            OnTokensChanged?.Invoke(HasTokens);
        }

        /// <summary>
        /// Removes a token from the set.
        /// </summary>
        /// <param name="token">The token to remove.</param>
        public void RemoveToken(object token)
        {
            _tokens.Remove(token);
            OnTokensChanged?.Invoke(HasTokens);
        }

        /// <summary>
        /// Returns true if there are any tokens in the set. False otherwise.
        /// </summary>
        public bool HasTokens => _tokens.Count > 0;

        public object[] GetTokens()
        {
            var tokensArray = new object[_tokens.Count];
            _tokens.CopyTo(tokensArray);
            return tokensArray;
        }
    }
}