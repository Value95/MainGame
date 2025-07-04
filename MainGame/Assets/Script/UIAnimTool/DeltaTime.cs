using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class EditorDeltaTime
{
    private static float _lastEditorUpdateTime;
    private static float _customDeltaTime;

    static EditorDeltaTime()
    {
        // 에디터 업데이트 이벤트에 메서드 등록
        EditorApplication.update += Update;
        _lastEditorUpdateTime = (float)EditorApplication.timeSinceStartup;
    }

    private static void Update()
    {
        // 에디터 모드에서의 델타 시간 계산
        float currentEditorTime = (float)EditorApplication.timeSinceStartup;
        _customDeltaTime = currentEditorTime - _lastEditorUpdateTime;
        _lastEditorUpdateTime = currentEditorTime;
    }

    public static float GetDeltaTime()
    {
        return Application.isPlaying ? Time.deltaTime : _customDeltaTime;
    }
}