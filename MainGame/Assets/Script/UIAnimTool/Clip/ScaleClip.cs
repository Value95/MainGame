#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ScaleClip : Clip
{
    [SerializeField]
    private Vector3 startScale;
    [SerializeField]
    private Vector3 endScale;

    
    public override void Initialize(UiAnimator pUiAnimator, ClipType clipType, float pStartTime, float pEndTime)
    {
        base.Initialize(pUiAnimator,clipType,pStartTime,pEndTime);
        clipName = "ScaleClip";
    }
    
    public override void DrawInspector()
    {
        base.DrawClip();
        
        startScale = EditorGUILayout.Vector3Field("StartScale", startScale);
        endScale = EditorGUILayout.Vector3Field("EndScale", endScale);

    }

    public override void Play(GameObject pObj, float pPlayTime)
    {
        float lTotlaTime = endTime - startTime;
        float lPlayTime = pPlayTime - startTime;
        float lPercentage = (lPlayTime / lTotlaTime);

        Vector3 lCurScale = CalculatePosition(startScale, endScale, animCurve.Evaluate(lPercentage));

        pObj.transform.localScale = lCurScale;
    }
    
    Vector3 CalculatePosition(Vector3 start, Vector3 end, float percentage)
    {
        return Vector3.Lerp(start, end, percentage);
    }
    
    public override void Reset(GameObject pObj)
    {
        pObj.transform.localScale = startScale;
    }
}
#endif