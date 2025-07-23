#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PositionClip : Clip
{
    
    [SerializeField]
    public Vector3 startPoint;
    
    [SerializeField]
    public Vector3 targetPoint;

    public override void Initialize(UiAnimator pUiAnimator, ClipType clipType, float pStartTime, float pEndTime)
    {
        base.Initialize(pUiAnimator,clipType,pStartTime,pEndTime);
        clipName = "PositionClip";
    }

    public override void DrawInspector()
    {
        base.DrawClip();
        
        startPoint = EditorGUILayout.Vector3Field("startPoint", startPoint);
        targetPoint = EditorGUILayout.Vector3Field("TargetPoint", targetPoint);

    }

    public override void Play(GameObject pObj, float pPlayTime)
    {
        float lTotlaTime = endTime - startTime;
        float lPlayTime = pPlayTime - startTime;
        float lPercentage = (lPlayTime / lTotlaTime);

        Vector3 lTargetPoint = CalculatePosition(startPoint, targetPoint, animCurve.Evaluate(lPercentage));

        if (pObj == null)
        {
            return;
        }
        
        pObj.transform.position = lTargetPoint;
    }
    
    Vector3 CalculatePosition(Vector3 start, Vector3 end, float percentage)
    {
        return Vector3.Lerp(start, end, percentage);
    }
    
    public override void Reset(GameObject pObj)
    {
        if (pObj == null)
            return;
        
        pObj.transform.position = startPoint;
    }
}
#endif