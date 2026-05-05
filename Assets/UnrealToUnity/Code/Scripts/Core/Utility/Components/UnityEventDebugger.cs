using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.Utility.Components
{
    public class UnityEventDebugger : MonoBehaviour
    {
        public void LogMessage(string message)
        {
            Debug.Log(message);
        }

        public void LogWarning(string message)
        {
            Debug.LogWarning(message);
        }

        public void LogError(string message)
        {
            Debug.LogError(message);
        }
    }
}