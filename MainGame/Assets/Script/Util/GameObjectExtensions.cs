using UnityEngine;

public static class GameObjectExtensions
{
    /// <summary>
    /// GameObject가 null이 아닐 때만 SetActive를 안전하게 수행합니다.
    /// </summary>
    public static void SafeSetActive(this GameObject go, bool isActive)
    {
        if (!go)
        {
            //Debug.Log($"GameObject {go.name} is null");
            return;
        }

        if (go.activeSelf != isActive)
            go.SetActive(isActive);
    }
}