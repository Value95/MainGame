using UnityEditor;
using UnityEngine;

[System.Serializable]
public class RotationClip : Clip
{
    [SerializeField]
    private Vector3 startRotation;
    [SerializeField]
    private Vector3 endRotation;
    
    public override void Initialize(UiAnimator pUiAnimator, ClipType clipType, float pStartTime, float pEndTime)
    {
        base.Initialize(pUiAnimator,clipType,pStartTime,pEndTime);
        clipName = "RotationClip";
    }
    
    public override void DrawInspector()
    {
        base.DrawClip();
        
        startRotation = EditorGUILayout.Vector3Field("StartRotation", startRotation);
        endRotation = EditorGUILayout.Vector3Field("EndRotation", endRotation);

    }

    public override void Play(GameObject pObj, float pPlayTime)
    {
        float lTotlaTime = endTime - startTime;
        float lPlayTime = pPlayTime - startTime;
        float lPercentage = (lPlayTime / lTotlaTime);

        Vector3 lCurRotation = CalculatePosition(startRotation, endRotation, animCurve.Evaluate(lPercentage));

        pObj.transform.rotation = Quaternion.Euler(lCurRotation);
    }
    
    Vector3 CalculatePosition(Vector3 start, Vector3 end, float percentage)
    {
        return Vector3.Lerp(start, end, percentage);
    }
    
    public override void Reset(GameObject pObj)
    {
        pObj.transform.rotation = Quaternion.Euler(startRotation);
    }
}
