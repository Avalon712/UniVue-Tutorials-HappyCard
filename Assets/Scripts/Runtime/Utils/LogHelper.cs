using System.Diagnostics;

namespace HayypCard.Utils
{
    public static class LogHelper
    {
        [Conditional("UNITY_EDITOR")]
        public static void Info(string message)
        {
            UnityEngine.Debug.Log(message);
        }

        [Conditional("UNITY_EDITOR")]
        public static void Warn(string message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        [Conditional("UNITY_EDITOR")]
        public static void Error(string message)
        {
            UnityEngine.Debug.LogError(message);
        }
    }
}
