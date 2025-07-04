using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Clip : ScriptableObject
{
    public enum ClipType
    {
        Position,
        Rotation,
        Scale,
        Active
    };

    [SerializeField] public UiAnimator uiAnimator;

    public ClipType eClipType;

    [SerializeField] public string clipName;

    public float startTime;
    public float endTime;

    [SerializeField] protected AnimationCurve animCurve;

    public Clip()
    {
        
    }
    
    public virtual void Initialize(UiAnimator pUiAnimator, ClipType clipType, float pStartTime, float pEndTime)
    {
        uiAnimator = pUiAnimator;
        eClipType = clipType;
        startTime = pStartTime;
        endTime = pEndTime;
        animCurve = new AnimationCurve();
    }

    protected void DrawClip()
    {
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        startTime = Mathf.Clamp(EditorGUILayout.FloatField("Start Time", startTime), 0, endTime);
        endTime = Mathf.Clamp(EditorGUILayout.FloatField("End Time", endTime), startTime, uiAnimator.animTime);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.MinMaxSlider(ref startTime, ref endTime, 0, uiAnimator.animTime);

        animCurve = EditorGUILayout.CurveField("Curve", animCurve);
    }

    public virtual void DrawInspector()
    {
    }

    public virtual void Play(GameObject pObj, float pPlayTime)
    {
    }

    public virtual void Reset(GameObject pObj)
    {
        
    }
}
