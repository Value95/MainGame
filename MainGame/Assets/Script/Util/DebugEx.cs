public static class DebugEx
{
    public static void Log(object message)
    {
#if UNITY_EDITOR
        UnityEngine.Debug.Log($"[LOG] {message}");
#endif
    }

    public static void LogError(object message)
    {
        UnityEngine.Debug.LogError($"[ERROR] {message}");
    }

    public static void LogWarning(object message)
    {
        UnityEngine.Debug.LogWarning($"[WARN] {message}");
    }
}
