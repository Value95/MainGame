#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[System.Serializable]
[CustomEditor(typeof(UiAnimator))]
public class TestDataEditor : UnityEditor.Editor
{
    [SerializeField]
    private UiAnimator _uiAnimator;
    private static UIAnimTimeLineWindow _uiAnimTimeLineWindow;
    private const string WindowOpenKey = "UIAnimTimeLineWindowOpen";

    private void OnEnable()
    {
        if (EditorPrefs.GetBool(WindowOpenKey, false))
        {
            _uiAnimTimeLineWindow = (UIAnimTimeLineWindow)EditorWindow.GetWindow(typeof(UIAnimTimeLineWindow));
            if (_uiAnimTimeLineWindow != null)
            {
                _uiAnimTimeLineWindow.CreateWindow(target as UiAnimator);
            }
        }
    }

    private void OnDisable()
    {
        if (_uiAnimTimeLineWindow != null)
        {
            _uiAnimTimeLineWindow.CloseWindow();
            EditorPrefs.SetBool(WindowOpenKey, false);
        }
        else
        {
            EditorPrefs.DeleteKey(WindowOpenKey);
        }
    }

    public override void OnInspectorGUI()
    {
        _uiAnimator = target as UiAnimator;
        
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if (_uiAnimTimeLineWindow == null && GUILayout.Button("UIAnimator Window"))
        {
            _uiAnimTimeLineWindow = (UIAnimTimeLineWindow)EditorWindow.GetWindow(typeof(UIAnimTimeLineWindow));
            
            _uiAnimTimeLineWindow.CreateWindow(target as UiAnimator);
            EditorPrefs.SetBool(WindowOpenKey, true); // 윈도우가 열렸음을 저장
        }

        if (_uiAnimTimeLineWindow != null)
        {
            _uiAnimator.DrawClipInfo();
            _DataUpdate();
            _uiAnimTimeLineWindow.Repaint();
        }
    }

    private void _DataUpdate()
    {
        if (_uiAnimTimeLineWindow != null)
            _uiAnimTimeLineWindow.DataRefresh();
    }
}
#endif