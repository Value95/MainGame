
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ActiveClip : Clip
{
    [SerializeField]
    private bool isActive;
    
    public override void Initialize(UiAnimator pUiAnimator, ClipType clipType, float pStartTime, float pEndTime)
    {
        base.Initialize(pUiAnimator,clipType,pStartTime,pEndTime);
        clipName = "ActiveClip";
    }

    public override void DrawInspector()
    {
        startTime = EditorGUILayout.Slider("", startTime,0,uiAnimator.animTime);
        endTime = startTime + 0.1f;
        
        isActive = EditorGUILayout.Toggle("IsActive", isActive);
    }

    public override void Play(GameObject pObj, float pPlayTime)
    {
        pObj.SetActive(isActive);
    }
    
    public override void Reset(GameObject pObj)
    {
        pObj.SetActive(!isActive);
    }
}
